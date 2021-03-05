using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Skle;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Skle;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Skle;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class PostController : BaseAdminController
    {
        #region Reports

        private readonly IPermissionService _permissionService;
        private readonly IPostService _PostService;
        private readonly IMemberService _MemberService;
        private readonly IGroupService _GroupService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        private readonly IDownloadService _downloadService;

        #endregion Reports

        #region Ctor

        public PostController(IPermissionService permissionService,
            IPostService PostService,
            IMemberService MemberService,
            IGroupService GroupService,
            IWorkContext workContext,
            IPictureService pictureService,
            IDownloadService downloadService
            )
        {
            _permissionService = permissionService;
            _PostService = PostService;
            _MemberService = MemberService;
            _GroupService = GroupService;
            _workContext = workContext;
            _pictureService = pictureService;
            _downloadService = downloadService;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(ListPost));
        }

        #endregion Ctor

        #region Utilities

        private PostModel PreparePost(PostModel model = null, Post entity = null)
        {
            //Action<PostLocalizedModel, int> localizedModelConfiguration = null;
            if (model == null)
                model = new PostModel();

            if (entity != null)
            {
                model = entity.ToModel<PostModel>();

                //define localized model configuration action
                //localizedModelConfiguration = (locale, languageId) =>
                //{
                //    locale.Name = _localizationService.GetLocalized(entity, entity => entity.Name, languageId, false, false);
                //};
            }

            //model.GiaiDaus = _GiaiDauService.GetAll().Select(s => new SelectListItem { Text = s.TenGiaiDau, Value = s.Id.ToString() }).ToList();

            //model.DoiBongs = _DoiBongService.GetAll().Select(s => new SelectListItem { Text = s.TenDoiBong, Value = s.Id.ToString() }).ToList();

            return model;
        }

        private void deletePost(int id)
        {
            var post = _PostService.GetPostById(id);
            _PostService.Delete(post);
            var group = _GroupService.GetGroupById(post.GroupId);
            group.CountPost--;
            _GroupService.Update(group);
        }

        #endregion Utilities

        #region Methos

        #region Post

        public virtual IActionResult ListPost(int GroupId = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = new PostSearchModel();

            model.SetGridPageSize();

            model.ListStatus.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Tất cả", (int)ENStatusDeleted.All + "", true));
            model.ListStatus.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Hoạt động", (int)ENStatusDeleted.Active + ""));
            model.ListStatus.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Đã xoá", (int)ENStatusDeleted.Deleted + ""));

            model.Members = _MemberService.GetAllMember(StatusId: 0).Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(s.Name + " - " + s.Id, s.Id.ToString())).ToList();
            model.Members.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("", "0", true));

            model.Groups = _GroupService.GetAllGroup().Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(s.Name, s.Id.ToString())).ToList();
            model.Groups.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("", "0", true));

            model.SortBys.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Thời gian", (int)ENSortByPost.time + "", true));
            model.SortBys.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Lượt like", (int)ENSortByPost.like + ""));
            model.SortBys.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Lượt spam", (int)ENSortByPost.spam + ""));

            model.GroupId = GroupId;

            return View(model);
        }

        public virtual IActionResult CreatePost()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = PreparePost();

            return View(model);
        }

        public virtual IActionResult EditPost(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var entity = _PostService.GetPostById(id);
            if (entity == null)
                return RedirectToAction("ListPost");

            var model = PreparePost(null, entity);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListPost(PostSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var entitys = _PostService.GetAllPostPagedList(GroupId: searchModel.GroupId, MemberId: searchModel.MemberId, KeySearch: searchModel.KeySearch, searchModel.Page - 1, searchModel.PageSize, searchModel.StatusId, searchModel.CountSpam, searchModel.SortById);

            var model = new PostListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var _model = entity.ToModel<PostModel>();
                    if (_model.Content?.Length > 40)
                        _model.Content = _model.Content.Substring(0, 40) + "...";
                    return _model;
                });
            });

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult UpdateInlinePost(PostModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product Download with the specified id
            var entity = _PostService.GetPostById(model.Id)
                ?? throw new ArgumentException("No product Download found with the specified id");

            _PostService.Update(entity, "Admin " + _workContext.CurrentCustomer.Id);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult DeletePostSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    deletePost(item);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditPost(PostModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = _PostService.GetPostById(model.Id);
                if (entity == null)
                    return RedirectToAction("ListPost");
                var group = _GroupService.GetGroupById(entity.GroupId);
                string str = string.Empty;
                if (entity.Deleted == true && model.Deleted == false)
                {
                    str = " undelete post: " + entity.Id;
                    group.CountPost++;
                    _GroupService.Update(group);
                }
                if (entity.Deleted == false && model.Deleted == true)
                {
                    str = " delete post " + entity.Id;
                    group.CountPost--;
                    _GroupService.Update(group);
                }
                entity.Content = model.Content;
                entity.Deleted = model.Deleted;

                _PostService.Update(entity, "Admin " + _workContext.CurrentCustomer.Id + str);

                //if (sp.AvatarId > 0 && sp.AvatarId != model.AvatarId) DeletePicture((int)sp.AvatarId);

                if (!continueEditing)
                    return RedirectToAction("ListPost");

                return RedirectToAction("EditPost", new { id = entity.Id });
            }

            model = PreparePost(model);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeletePost(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            deletePost(id);

            return RedirectToAction("ListPost");
        }

        #endregion Post

        #region post file

        [HttpPost]
        public virtual IActionResult PostFileList(PostFileSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var entitys = new PagedList<PostFile>(_PostService.GetAllPostFile(searchModel.PostId).ToList(), searchModel.Page - 1, searchModel.PageSize);

            var model = new PostFileListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var _model = entity.ToModel<PostFileModel>();
                    _model.PictureUrl = _pictureService.GetPictureUrl(entity.PictureId, showDefaultPicture: false);
                    if (!string.IsNullOrEmpty(entity.VideoUrl))
                        _model.VideoUrl = "https://api.skle.vn/" + entity.VideoUrl;
                    if (entity.DownloadId > 0)
                    {
                        var download = _downloadService.GetDownloadById(entity.DownloadId);
                        _model.DownloadUrl = Url.Action("DownloadFile", "Download", new { downloadGuid = download.DownloadGuid });
                        _model.Downloadname = download.Filename + download.Extension;
                    }
                    return _model;
                });
            });

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult PostFileDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            _PostService.DeletePostFile(id);

            return new NullJsonResult();
        }

        #endregion post file

        #region Repost

        public virtual IActionResult ListReport()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = new ReportSearchModel();

            model.SetGridPageSize();


            model.isNew = true;
            model.ListStatus.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Mới", true.ToString()));
            model.ListStatus.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem("Đã xử lý", false.ToString()));
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListReport(ReportSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var entitys = _PostService.GetAllReportPagedList(searchModel.isNew, searchModel.Page - 1, searchModel.PageSize);

            var model = new ReportListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var _model = entity.ToModel<ReportModel>();
                    _model.Name = _MemberService.GetMemberById(entity.FormId).Name;
                    return _model;
                });
            });

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult UpdateInlineReport(ReportModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product Download with the specified id
            var entity = _PostService.GetReportById(model.Id)
                ?? throw new ArgumentException("No product Download found with the specified id");
            entity.isNew = model.isNew;
            _PostService.Update(entity);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult DeleteReportSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                foreach (var id in selectedIds)
                {
                    _PostService.Delete(_PostService.GetReportById(id));
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult DeleteReport(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            _PostService.Delete(_PostService.GetReportById(id));

            return RedirectToAction("ListReport");
        }

        #endregion

        #endregion Methos
    }
}