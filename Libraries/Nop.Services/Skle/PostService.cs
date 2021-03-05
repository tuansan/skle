using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Skle;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Media;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Nop.Services.Skle
{
    public partial class PostService : IPostService
    {
        #region Fields

        private const string ServerApiUrl = "ServerApiUrl";

        private readonly IRepository<Post> _PostRepository;
        private readonly IRepository<PostLog> _PostLogRepository;
        private readonly IRepository<PostFile> _PostFileRepository;
        private readonly IRepository<PostLike> _PostLikeRepository;
        private readonly IRepository<PostSpam> _PostSpamRepository;
        private readonly IRepository<PostHidden> _PostHiddenRepository;
        private readonly IRepository<Picture> _PictureRepository;
        private readonly IRepository<Member> _MemberRepository;
        private readonly IRepository<Customer> _CustomerRepository;
        private readonly IRepository<Report> _ReportRepository;
        private readonly IPictureService _pictureService;
        private readonly IDownloadService _downloadService;
        private readonly INopFileProvider _fileProvider;
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public PostService(IRepository<Post> postRepository, IRepository<PostLog> postLogRepository, IRepository<PostFile> postFileRepository, IRepository<PostLike> postLikeRepository, IRepository<PostSpam> postSpamRepository, IRepository<PostHidden> postHiddenRepository, IRepository<Picture> pictureRepository, IRepository<Member> memberRepository, IRepository<Customer> customerRepository, IRepository<Report> reportRepository, IPictureService pictureService, IDownloadService downloadService, INopFileProvider fileProvider, IWorkContext workContext)
        {
            _PostRepository = postRepository;
            _PostLogRepository = postLogRepository;
            _PostFileRepository = postFileRepository;
            _PostLikeRepository = postLikeRepository;
            _PostSpamRepository = postSpamRepository;
            _PostHiddenRepository = postHiddenRepository;
            _PictureRepository = pictureRepository;
            _MemberRepository = memberRepository;
            _CustomerRepository = customerRepository;
            _ReportRepository = reportRepository;
            _pictureService = pictureService;
            _downloadService = downloadService;
            _fileProvider = fileProvider;
            _workContext = workContext;
        }

        #endregion Ctor

        #region Methods

        #region common

        private static string GetQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetProperties()
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }

        private IRestResponse Call(Method method, string path, object callParams = null, bool authen = true)
        {
            string requestUriString = string.Format("{0}/{1}", "https://api.skle.vn/api", path);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var request = new RestRequest(method);

            if (callParams != null)
            {
                if (method == Method.GET)
                    requestUriString += string.Format("?{0}", GetQueryString(callParams));
                else
                    request.AddJsonBody(callParams);
            }
            if (authen)
                request.AddHeader("authorization", string.Format("Bearer {0}", GetApiToken()));

            var client = new RestClient(requestUriString);

            IRestResponse response = client.Execute(request);

            return response;
        }

        public IRestResponse Get(string path, object data = null, bool authen = true)
        {
            return Call(Method.GET, path, data, authen);
        }

        public IRestResponse Post(string path, object data, bool authen = true)
        {
            return Call(Method.POST, path, data, authen);
        }

        public IRestResponse Put(string path, object data)
        {
            return Call(Method.PUT, path, data);
        }

        public IRestResponse Delete(string path)
        {
            return Call(Method.DELETE, path);
        }

        private string GetApiToken()
        {
            var response = Post("Authen", new
            {
                ClientId = "WebApi",
                Key = "e91d9edccab414ae8adc39ae9877085fa60f2489cda6917d023cfc968a27234d"
            }, false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                dynamic model = JsonConvert.DeserializeObject(response.Content);
                string apiToken = ((Newtonsoft.Json.Linq.JContainer)((Newtonsoft.Json.Linq.JContainer)model["objectInfo"]).Last).Last.ToString();
                //apiToken = (string)JsonConvert.DeserializeObject([0]["objectInfo"]);
                return apiToken;
            }
            return "";
        }

        #endregion common

        #region Post

        public IPagedList<Post> GetAllPostPagedList(int GroupId = 0, int MemberId = 0, string KeySearch = null, int PageIndex = 0, int PageSize = int.MaxValue, int statusId = (int)ENStatusDeleted.Active, int CountSpam = 0, int sortById = (int)ENSortByPost.time, bool getOnlyTotalCount = false, int currentMemberId = 0)
        {
            var query = from t1 in _PostRepository.Table
                        join t2 in _MemberRepository.Table on t1.MemberId equals t2.Id
                        join t3 in _CustomerRepository.Table on t2.CustomerId equals t3.Id
                        where !t2.Deleted && t3.Active && !t3.Deleted
                        select t1;
            if (currentMemberId > 0)
            {
                query = query.Where(s => !_PostHiddenRepository.Table.Any(z => z.FormId == currentMemberId && z.ToId == s.MemberId && (z.isAll || (z.TargetId == s.Id))));
            }
            if (statusId > 0)
            {
                bool deleted = statusId == (int)ENStatusDeleted.Deleted;
                query = query.Where(s => s.Deleted == deleted);
            }
            if (GroupId > 0)
            {
                query = query.Where(s => s.GroupId == GroupId);
            }
            if (MemberId > 0)
            {
                query = query.Where(s => s.MemberId == MemberId);
            }
            if (!string.IsNullOrEmpty(KeySearch))
            {
                query = query.Where(s => s.Content.Contains(KeySearch));
            }
            if (CountSpam > 0)
            {
                query = query.Where(s => s.CountSpam > CountSpam - 1);
            }
            switch (sortById)
            {
                case (int)ENSortByPost.time:
                    query = query.OrderByDescending(s => s.CreatedAt);
                    break;

                case (int)ENSortByPost.like:
                    query = query.OrderByDescending(s => s.CountLike);
                    break;

                case (int)ENSortByPost.spam:
                    query = query.OrderByDescending(s => s.CountSpam);
                    break;
            }
            return new PagedList<Post>(query, PageIndex, PageSize, getOnlyTotalCount);
        }

        public virtual IPagedList<PicturePost> GetAllPicturePostByMemberId(int MemberId, int PageIndex = 0, int PageSize = 20)
        {
            var query = from t1 in _PostRepository.Table
                        join t2 in _PostFileRepository.Table on t1.Id equals t2.PostId
                        join t3 in _PictureRepository.Table on t2.PictureId equals t3.Id
                        where !t1.Deleted && t2.MemberId == MemberId
                        select new PicturePost
                        {
                            CreatedAt = t1.CreatedAt,
                            PictureId = t3.Id
                        };

            return new PagedList<PicturePost>(query.OrderByDescending(s => s.CreatedAt), PageIndex, PageSize);
        }

        public Post GetPostById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(Post));
            return _PostRepository.GetById(id);
        }

        public bool Insert(Post item, string content = "Insert")
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Post));
            _PostRepository.Insert(item);

            Insert(new PostLog()
            {
                After = JsonConvert.SerializeObject(item),
                Content = content,
                CreatedAt = DateTime.Now,
                PostId = item.Id,
                StatusId = (int)ENStatusLog.Insert
            });
            return true;
        }

        public bool Update(Post item, string content = "Update")
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Post));
            _PostRepository.Update(item);

            Insert(new PostLog()
            {
                After = JsonConvert.SerializeObject(item),
                Content = content,
                CreatedAt = DateTime.Now,
                PostId = item.Id,
                StatusId = (int)ENStatusLog.Update
            });
            return true;
        }

        public bool Delete(Post item, string content = "Delete")
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Post));
            item.Deleted = true;
            _PostRepository.Update(item);

            Insert(new PostLog()
            {
                After = JsonConvert.SerializeObject(item),
                Content = content,
                CreatedAt = DateTime.Now,
                PostId = item.Id,
                StatusId = (int)ENStatusLog.Delete
            });
            return true;
        }

        #endregion Post

        #region PostLog

        public IList<PostLog> GetAllPostLog(int PostId = 0)
        {
            var query = _PostLogRepository.Table;
            if (PostId > 0)
            {
                query = query.Where(s => s.PostId == PostId);
            }
            return query.ToList();
        }

        public PostLog GetPostLogById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(PostLog));
            return _PostLogRepository.GetById(id);
        }

        public bool Insert(PostLog item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(PostLog));
            _PostLogRepository.Insert(item);

            return true;
        }

        public bool Update(PostLog item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(PostLog));
            _PostLogRepository.Update(item);

            return true;
        }

        public bool DeletePostLog(int id)
        {
            var item = _PostLogRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(PostLog));

            _PostLogRepository.Delete(item);

            return true;
        }

        #endregion PostLog

        #region PostFile

        public IList<PostFile> GetAllPostFile(int PostId = 0, int MemberId = 0)
        {
            var query = _PostFileRepository.Table;
            if (PostId > 0)
            {
                query = query.Where(s => s.PostId == PostId);
            }
            if (MemberId > 0)
            {
                query = query.Where(s => s.MemberId == MemberId);
            }
            return query.ToList();
        }

        public PostFile GetPostFileById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(PostFile));
            return _PostFileRepository.GetById(id);
        }

        public bool Insert(PostFile item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(PostFile));
            _PostFileRepository.Insert(item);

            return true;
        }

        public bool Update(PostFile item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(PostFile));
            _PostFileRepository.Update(item);

            return true;
        }

        public bool DeletePostFile(int id)
        {
            var item = _PostFileRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(PostFile));

            var pictureId = item.PictureId;
            var downloadId = item.DownloadId;
            var videoUrl = item.VideoUrl;

            if (!string.IsNullOrEmpty(videoUrl))
            {
                var contentType = item.MimeType.Split("/").First();
                switch (contentType)
                {
                    case "video":
                        try
                        {
                            Post("Post/DeleteFileByPath", new { Fileurl = item.VideoUrl.Replace("/video/upload/", "/video/upload/thumbs/").Replace(item.VideoUrl.Split(".").LastOrDefault(), "jpg") });
                        }
                        catch { }
                        try
                        {
                            Post("Post/DeleteFileByPath", new { Fileurl = videoUrl });
                        }
                        catch { }
                        break;

                    case "audio":
                        try
                        {
                            Post("Post/DeleteFileByPath", new { Fileurl = videoUrl });
                        }
                        catch { }
                        break;
                }
            }

            _PostFileRepository.Delete(item);

            //try to get a picture with the specified id
            if (pictureId > 0)
            {
                var picture = _pictureService.GetPictureById(pictureId)
                    ?? throw new ArgumentException("No picture found with the specified id");

                _pictureService.DeletePicture(picture);
            }
            if (downloadId > 0)
            {
                var download = _downloadService.GetDownloadById(downloadId)
                    ?? throw new ArgumentException("No download found with the specified id");

                _downloadService.DeleteDownload(download);
            }

            return true;
        }

        #endregion PostFile

        #region PostLike

        public IList<PostLike> GetAllPostLike(int PostId = 0)
        {
            var query = _PostLikeRepository.Table;
            if (PostId > 0)
            {
                query = query.Where(s => s.PostId == PostId);
            }
            return query.ToList();
        }

        public PostLike GetPostLikeById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(PostLike));
            return _PostLikeRepository.GetById(id);
        }

        public bool Insert(PostLike item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(PostLike));
            _PostLikeRepository.Insert(item);

            return true;
        }

        public bool Update(PostLike item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(PostLike));
            _PostLikeRepository.Update(item);

            return true;
        }

        public bool DeletePostLike(int id)
        {
            var item = _PostLikeRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(PostLike));

            item.isLike = false;
            _PostLikeRepository.Update(item);

            return true;
        }

        #endregion PostLike

        #region PostSpam

        public IList<PostSpam> GetAllPostSpam(int PostId = 0)
        {
            var query = _PostSpamRepository.Table;
            if (PostId > 0)
            {
                query = query.Where(s => s.PostId == PostId);
            }
            return query.ToList();
        }

        public PostSpam GetPostSpamById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(PostSpam));
            return _PostSpamRepository.GetById(id);
        }

        public bool Insert(PostSpam item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(PostSpam));
            _PostSpamRepository.Insert(item);

            return true;
        }

        public bool Update(PostSpam item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(PostSpam));
            _PostSpamRepository.Update(item);

            return true;
        }

        public bool DeletePostSpam(int id)
        {
            var item = _PostSpamRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(PostSpam));

            _PostSpamRepository.Delete(item);

            return true;
        }

        public bool CheckPostHidden(int formId, int toId, int targetId = 0)
        {
            return _PostHiddenRepository.Table.Any(s => s.FormId == formId && s.ToId == toId && (s.isAll || (s.TargetId == targetId)));
        }

        public PostHidden GetPostHidden(int formId, int toId, int targetId = 0)
        {
            return _PostHiddenRepository.Table.FirstOrDefault(s => s.FormId == formId && s.ToId == toId && (s.isAll || (s.TargetId == targetId)));
        }

        public bool Insert(PostHidden item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(PostHidden));
            item.isAll = item.TargetId == 0;
            _PostHiddenRepository.Insert(item);

            return true;
        }

        public bool Delete(PostHidden item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(PostHidden));

            _PostHiddenRepository.Delete(item);
            return true;
        }

        public IPagedList<Report> GetAllReportPagedList(bool isNew = true, int PageIndex = 0, int PageSize = int.MaxValue)
        {
            var query = _ReportRepository.Table.Where(s => s.isNew == isNew);
            return new PagedList<Report>(query, PageIndex, PageSize);
        }

        public Report GetReportById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(Report));
            return _ReportRepository.GetById(id);
        }

        public void Insert(Report item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Report));
            _ReportRepository.Insert(item);
        }

        public void Update(Report item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Report));
            _ReportRepository.Update(item);
        }

        public void Delete(Report item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Report));
            _ReportRepository.Delete(item);
        }

        #endregion PostSpam

        #endregion Methods
    }
}