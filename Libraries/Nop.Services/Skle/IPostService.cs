using Nop.Core;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Skle;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Skle
{
    public partial interface IPostService
    {
        IPagedList<Post> GetAllPostPagedList(int GroupId = 0, int MemberId = 0, string KeySearch = null, int PageIndex = 0, int PageSize = int.MaxValue, int statusId = (int)ENStatusDeleted.Active, int CountSpam = 0, int sortById = (int)ENSortByPost.time, bool getOnlyTotalCount = false, int currentMemberId = 0);

        IPagedList<PicturePost> GetAllPicturePostByMemberId(int MemberId, int PageIndex = 0, int PageSize = 20);

        Post GetPostById(int id);

        bool Insert(Post item, string content = "Insert");

        bool Update(Post item, string content = "Update");

        bool Delete(Post item, string content = "Delete");

        IList<PostLog> GetAllPostLog(int PostId = 0);

        PostLog GetPostLogById(int id);

        bool Insert(PostLog item);

        bool Update(PostLog item);

        bool DeletePostLog(int id);

        IList<PostFile> GetAllPostFile(int PostId = 0, int MemberId = 0);

        PostFile GetPostFileById(int id);

        bool Insert(PostFile item);

        bool Update(PostFile item);

        bool DeletePostFile(int id);

        IList<PostLike> GetAllPostLike(int PostId = 0);

        PostLike GetPostLikeById(int id);

        bool Insert(PostLike item);

        bool Update(PostLike item);

        bool DeletePostLike(int id);

        IList<PostSpam> GetAllPostSpam(int PostId = 0);

        PostSpam GetPostSpamById(int id);

        bool Insert(PostSpam item);

        bool Update(PostSpam item);

        bool DeletePostSpam(int id);

        bool CheckPostHidden(int formId, int toId, int targetId = 0);
        PostHidden GetPostHidden(int formId, int toId, int targetId = 0);
        bool Insert(PostHidden item);
        bool Delete(PostHidden item);

        IRestResponse Get(string path, object data = null, bool authen = true);

        IRestResponse Post(string path, object data, bool authen = true);

        IPagedList<Report> GetAllReportPagedList(bool isNew = true, int PageIndex = 0, int PageSize = int.MaxValue);
        Report GetReportById(int id);
        void Insert(Report item);
        void Update(Report item);
        void Delete(Report item);
    }
}