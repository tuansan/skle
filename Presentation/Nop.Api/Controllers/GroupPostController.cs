using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Api.Factories;
using Nop.Api.Infrastructure.Mapper.Extensions;
using Nop.Api.Models;
using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Skle;
using Nop.Core.Infrastructure;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Skle;
using Plugin.Media.Abstractions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nop.Api.Controllers
{
    public class GroupPostController : BaseRouteController
    {
        #region Fields

        private readonly IPostService _postService;
        private readonly ICustomerService _customerService;
        private readonly IGroupService _groupService;
        private readonly IPictureService _pictureService;
        private readonly IDownloadService _downloadService;
        private readonly IMemberService _memberService;
        private readonly INopFileProvider _fileProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ICommonFactory _commonFactory;
        private readonly INotificationService _notificationService;

        #endregion Fields

        #region Ctor

        public GroupPostController(IPostService postService, ICustomerService customerService, IGroupService groupService, IPictureService pictureService, IDownloadService downloadService, IMemberService memberService, INopFileProvider fileProvider, IHttpContextAccessor httpContextAccessor, ILocalizationService localizationService, ICommonFactory commonFactory, INotificationService notificationService)
        {
            _postService = postService;
            _customerService = customerService;
            _groupService = groupService;
            _pictureService = pictureService;
            _downloadService = downloadService;
            _memberService = memberService;
            _fileProvider = fileProvider;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _commonFactory = commonFactory;
            _notificationService = notificationService;
        }

        #endregion Ctor
        #region common

        private void InsertNewFile(IFormFileCollection Files, Post post)
        {
            foreach (var item in Files)
            {
                var Binary = _downloadService.GetDownloadBits(item);
                var fileName = item.FileName.Replace("." + item.FileName.Split(".").LastOrDefault(), "");
                var fileExtension = _fileProvider.GetFileExtension(item.FileName);
                string contentType = item.ContentType;

                var postfile = new PostFile()
                {
                    MemberId = currentMemberId,
                    PostId = post.Id,
                    MimeType = contentType,
                    CreatedAt = DateTime.Now
                };
                switch (contentType.Split("/").First())
                {
                    case "image":
                        var picture = _pictureService.InsertPicture(Binary, contentType, item.FileName, validateBinary: true);
                        postfile.PictureId = picture.Id;
                        break;

                    case "video":
                        using (var stream = new MemoryStream(Binary))
                        {
                            var thumbsDirectoryPath = _fileProvider.GetAbsolutePath(@"video\upload");
                            string filename = Guid.NewGuid().ToString();
                            var path = filename + GetExtVideo(contentType.Split("/").Last());
                            var saveFolder = _fileProvider.Combine(thumbsDirectoryPath, path);

                            System.IO.File.WriteAllBytes(saveFolder, Binary);

                            postfile.VideoUrl = "/video/upload/" + path;

                            try
                            {
                                var thumbnailImagePath = _fileProvider.Combine(thumbsDirectoryPath, "/thumbs/" + filename + ".jpg");

                                GetThumbnail(saveFolder, thumbnailImagePath);
                            }
                            catch
                            {
                                //do notthing
                            }
                        }
                        break;
                    case "audio":
                        var thumbsDirectoryPath2 = _fileProvider.GetAbsolutePath(@"audio\upload");

                        var path2 = Guid.NewGuid().ToString() + GetExtAudio(contentType);
                        var saveFolder2 = _fileProvider.Combine(thumbsDirectoryPath2, path2);

                        System.IO.File.WriteAllBytes(saveFolder2, Binary);

                        postfile.VideoUrl = "/audio/upload/" + path2;
                        break;
                    default:
                        var download = new Download()
                        {
                            Extension = fileExtension,
                            ContentType = contentType,
                            DownloadBinary = Binary,
                            DownloadGuid = Guid.NewGuid(),
                            IsNew = true,
                            Filename = fileName
                        };
                        _downloadService.InsertDownload(download);
                        postfile.DownloadId = download.Id;
                        break;
                }
                _postService.Insert(postfile);
            }
        }

        public static void GetThumbnail(string video, string thumbnail, string size = "640x480")
        {
            var cmd = "ffmpeg  -itsoffset -1  -i " + '"' + video + '"' + " -vcodec mjpeg -vframes 1 -an -f rawvideo -s " + size + " " + '"' + thumbnail + '"';
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C " + cmd
            };

            var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();
        }


        private static string GetExtVideo(string type)
        {
            switch (type)
            {
                case "x-flv":
                    return ".flv";
                case "x-mpegURL":
                    return ".m3u8";
                case "MP2T":
                    return ".ts";
                case "3gpp":
                    return ".3gp";
                case "3gpp2":
                    return ".3g2";
                case "quicktime":
                    return ".3gp";
                case "x-msvideo":
                    return ".avi";
                case "x-ms-wmv":
                    return ".wmv";
                case "mpeg":
                    return ".mpeg";
                case "ogg":
                    return ".ogv";
                case "webm":
                    return ".webm";
                default:
                    return ".mp4";
            }
        }

        private static string GetExtAudio(string type)
        {
            switch (type)
            {
                case "aac":
                    return ".aac";
                case "midi":
                    return ".mid";
                case "x-midi":
                    return ".midi";
                case "ogg":
                    return ".oga";
                case "opus":
                    return ".opus";
                case "wav":
                    return ".wav";
                case "webm":
                    return ".weba";
                case "3gpp":
                    return ".3gp";
                case "3gpp2":
                    return ".3g2";
                default:
                    return ".mp3";
            }
        }
        private PostModel EntityPostToModel(Post entity, bool isFull = false)
        {
            var model = entity.ToModel<PostModel>();
            var listFile = _postService.GetAllPostFile(entity.Id);
            foreach (var item in listFile)
            {
                var file = new GetPostFileModel()
                {
                    MimeType = item.MimeType,
                    Id = item.Id
                };
                switch (item.MimeType.Split("/").First())
                {
                    case "image":
                        if (isFull)
                            file.Url = _pictureService.GetPictureUrl(item.PictureId);
                        else
                            file.Url = _pictureService.GetPictureUrl(item.PictureId, 640);
                        break;

                    case "video":
                        if (isFull)
                            file.Url = item.VideoUrl;
                        else
                            file.Url = item.VideoUrl.Replace("/video/upload/", "/video/upload/thumbs/").Replace(item.VideoUrl.Split(".").LastOrDefault(), "jpg");
                        break;
                    case "audio":
                        file.Url = item.VideoUrl;
                        break;
                    default:
                        var download = _downloadService.GetDownloadById(item.DownloadId);
                        file.FileName = download.Filename + download.Extension;
                        file.Url = Url.ActionLink("GetFileUpload", "Home", new { downloadId = download.DownloadGuid });
                        break;

                }
                model.ListFile.Add(file);
            }
            var mem = _memberService.GetMemberById(entity.MemberId);
            model.AvatarUrl = _pictureService.GetPictureUrl(mem.AvatarId, defaultPictureType: Core.Domain.Media.PictureType.Avatar);
            model.Name = mem.Name;
            model.isLike = _postService.GetAllPostLike(entity.Id).Any(s => s.MemberId == currentMemberId);
            model.isSpam = _postService.GetAllPostSpam(entity.Id).Any(s => s.MemberId == currentMemberId);
            return model;
        }
        #endregion
        [HttpGet]
        public IActionResult Get(string KeySearch = null, int GroupId = 0, int MemberId = 0, int PageIndex = 0, int PageSize = int.MaxValue)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var list = _postService.GetAllPostPagedList(GroupId, MemberId, KeySearch, PageIndex, PageSize, currentMemberId: currentMemberId);
            if (GroupId > 0)
                _groupService.UpdateCountNewPostGroup(currentMemberId, GroupId);
            var model = list.Select(s => EntityPostToModel(s));
            return Ok(MessageReturn.Success("Ok", model));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var entity = _postService.GetPostById(id);
            return Ok(MessageReturn.Success("Ok", EntityPostToModel(entity, true)));
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [Consumes("multipart/form-data")]
        public IActionResult Post([FromForm] AddPostModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();
            try
            {
                if (model.Files == null)
                    model.Files = _httpContextAccessor.HttpContext.Request.Form?.Files;
            }
            catch { }
            if (string.IsNullOrEmpty(model.Content) && model.Files.Count == 0)
                return Ok(MessageReturn.Error(_localizationService.GetResource("AddPost.Null")));
            if (model.Id == 0)
                return BadRequest();
            var gm = _groupService.GetGroupMemberByGroupIdAndMemberId(currentMemberId, model.Id);
            if (gm == null || gm.StatusId != (int)ENStatusGroupRequest.confirm)
                return Ok(MessageReturn.Error(_localizationService.GetResource("admin.khongcoquyen.dangbai")));
            var post = new Post
            {
                MemberId = currentMemberId,
                Content = model.Content,
                CreatedAt = DateTime.Now,
                PostGuid = Guid.NewGuid(),
                GroupId = model.Id
            };
            if (_postService.Insert(post, "Content: " + model.Content))
            {
                var group = _groupService.GetGroupById(model.Id);
                group.CountPost++;
                _groupService.Update(group);

                if (model.Files?.Count > 0)
                    InsertNewFile(model.Files, post);

                var currentMember = _memberService.GetMemberById(currentMemberId);
                _notificationService.Insert(new NotiMobi()
                {
                    BranchId = (int)ENTypeNotification.group,
                    Content = _localizationService.GetResource("noti.baidang.moi") + " " + group.Name,
                    GroupId = group.Id,
                    currentMemberId = currentMemberId,
                    //PictureUrl = _pictureService.GetPictureUrl(currentMember.AvatarId, defaultPictureType: Core.Domain.Media.PictureType.Avatar),
                    Name = currentMember.Name,
                    PostId = post.Id,
                    TypeId = (int)ENTypeNotiMobi.bai_dang
                });

            };
            return Ok(MessageReturn.Success());
        }


        [HttpPut]
        [DisableRequestSizeLimit]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [Consumes("multipart/form-data")]
        public IActionResult Put([FromForm] AddPostModel model)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var post = _postService.GetPostById(model.Id);

            post.Content = model.Content;

            _postService.Update(post, "Đổi content thành: " + post.Content);
            var listFile = _postService.GetAllPostFile(post.Id);
            foreach (var item in listFile)
            {
                if (string.IsNullOrEmpty(model.Ids) || model.Ids.Split(",").Select(s => int.Parse(s)).Count(s => s == item.Id) == 0)
                    _postService.DeletePostFile(item.Id);
            }

            try
            {
                if (model.Files == null)
                    model.Files = _httpContextAccessor.HttpContext.Request.Form?.Files;
            }
            catch { }

            if (model.Files?.Count > 0)
                InsertNewFile(model.Files, post);

            return Ok(MessageReturn.Success());
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_commonFactory.CheckCustommer(currentMemberId))
                return Unauthorized();

            var post = _postService.GetPostById(id);
            if (post.MemberId != currentMemberId)
                return BadRequest();

            post.Deleted = true;

            var group = _groupService.GetGroupById(post.GroupId);

            group.CountPost--;

            _groupService.Update(group);

            _postService.Delete(post);


            return Ok(MessageReturn.Success());
        }
    }
}