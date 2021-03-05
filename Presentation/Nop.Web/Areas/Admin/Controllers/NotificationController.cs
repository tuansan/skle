using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class NotificationController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly INotificationService _NotificationService;
        private readonly IGroupService _GroupService;
        private readonly IMemberService _MemberService;
        private readonly IPictureService _PictureService;

        #endregion Fields

        #region Ctor

        public NotificationController(IPermissionService permissionService, INotificationService notificationService, IGroupService groupService, IMemberService memberService, IPictureService pictureService)
        {
            _permissionService = permissionService;
            _NotificationService = notificationService;
            _GroupService = groupService;
            _MemberService = memberService;
            _PictureService = pictureService;
        }


        public IActionResult Index()
        {
            return RedirectToAction(nameof(ListNotification));
        }

        #endregion Ctor

        #region Utilities

        private NotificationModel PrepareNotification(NotificationModel model = null, MyNotification entity = null)
        {
            //Action<NotificationLocalizedModel, int> localizedModelConfiguration = null;
            if (model == null)
                model = new NotificationModel();

            if (entity != null)
            {
                model = entity.ToModel<NotificationModel>();

                //define localized model configuration action
                //localizedModelConfiguration = (locale, languageId) =>
                //{
                //    locale.Name = _localizationService.GetLocalized(entity, entity => entity.Name, languageId, false, false);
                //};
                switch (entity.TypeId)
                {
                    case (int)ENTypeNotification.member:
                        model.TargetMembers = entity.Targets.Split(",").Select(s => int.Parse(s)).ToList();
                        break;
                    case (int)ENTypeNotification.group:
                        model.TargetGroups = entity.Targets.Split(",").Select(s => int.Parse(s)).ToList();
                        break;
                }
            }

            model.AvailableGroups = _GroupService.GetAllGroup().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();

            model.AvailableMembers = _MemberService.GetAllMember().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();

            model.AvailableTypes.Add(new SelectListItem("Gửi tất cả", ((int)ENTypeNotification.all).ToString()));
            model.AvailableTypes.Add(new SelectListItem("Gửi cá nhân", ((int)ENTypeNotification.member).ToString()));
            model.AvailableTypes.Add(new SelectListItem("Gửi nhóm", ((int)ENTypeNotification.group).ToString()));

            return model;
        }

        #endregion Utilities

        #region Methos

        #region Notification

        public virtual IActionResult ListNotification()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = new NotificationSearchModel();

            model.SetGridPageSize();

            return View(model);
        }

        public virtual IActionResult CreateNotification(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = PrepareNotification();
            if(id > 0)
            {
                model.TypeId = (int)ENTypeNotification.member;
                model.TargetMembers.Add(id);
            }
            return View(model);
        }

        public virtual IActionResult EditNotification(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var entity = _NotificationService.GetNotificationById(id);
            if (entity == null)
                return RedirectToAction("ListNotification");

            var model = PrepareNotification(null, entity);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListNotification(NotificationSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var entitys = new PagedList<MyNotification>(_NotificationService.GetAllNotification(KeySearch: searchModel.KeySearch).ToList(), searchModel.Page - 1, searchModel.PageSize);

            var model = new NotificationListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var _model = entity.ToModel<NotificationModel>();
                    _model.TypeText = entity.TypeId == 0 ? "Tất cả" : entity.TypeId == 1 ? "Gửi cá nhân - #" + entity.Targets : "Gửi nhóm - #" + entity.Targets;
                    return _model;
                });
            });

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult UpdateInlineNotification(NotificationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product Download with the specified id
            var entity = _NotificationService.GetNotificationById(model.Id)
                ?? throw new ArgumentException("No product Download found with the specified id");

            _NotificationService.Update(entity);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult DeleteNotificationSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    _NotificationService.DeleteNotification(item);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateNotification(NotificationModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity<MyNotification>();

                switch (model.TypeId)
                {
                    case (int)ENTypeNotification.member:
                        entity.Targets = String.Join(",", model.TargetMembers);
                        foreach (var item in model.TargetMembers)
                        {
                            var member = _MemberService.GetMemberById(item);
                            _NotificationService.Insert(new NotiMobi()
                            {
                                BranchId = model.TypeId,
                                TypeId = (int)ENTypeNotiMobi.he_thong,
                                Content = model.Content,
                                MemberId = item,
                                Title = model.Title,
                                //PictureUrl = _PictureService.GetPictureUrl(member.AvatarId),
                            });
                        }
                        break;
                    case (int)ENTypeNotification.group:
                        entity.Targets = String.Join(",", model.TargetGroups);
                        foreach (var item in model.TargetGroups)
                        {
                            var gr = _GroupService.GetGroupById(item);
                            _NotificationService.Insert(new NotiMobi()
                            {
                                BranchId = model.TypeId,
                                TypeId = (int)ENTypeNotiMobi.he_thong,
                                Content = model.Content,
                                MemberId = item,
                                Title = model.Title,
                                //PictureUrl = _PictureService.GetPictureUrl(gr.AvatarId),
                            });
                        }
                        break;
                    case (int)ENTypeNotification.all:
                        _NotificationService.Insert(new NotiMobi()
                        {
                            BranchId = model.TypeId,
                            TypeId = (int)ENTypeNotiMobi.he_thong,
                            Content = model.Content,
                            Title = model.Title,
                        });
                        break;
                }
                _NotificationService.Insert(entity);


                if (!continueEditing)
                    return RedirectToAction("ListNotification");

                return RedirectToAction("EditNotification", new { id = entity.Id });
            }
            model = PrepareNotification(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditNotification(NotificationModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = _NotificationService.GetNotificationById(model.Id);
                if (entity == null)
                    return RedirectToAction("ListNotification");

                entity = model.ToEntity<MyNotification>();

                switch (model.TypeId)
                {
                    case (int)ENTypeNotification.member:
                        entity.Targets = String.Join(",", model.TargetMembers);
                        break;
                    case (int)ENTypeNotification.group:
                        entity.Targets = String.Join(",", model.TargetGroups);
                        break;
                }

                _NotificationService.Update(entity);

                //if (sp.AvatarId > 0 && sp.AvatarId != model.AvatarId) DeletePicture((int)sp.AvatarId);

                if (!continueEditing)
                    return RedirectToAction("ListNotification");

                return RedirectToAction("EditNotification", new { id = entity.Id });
            }

            model = PrepareNotification(model);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteNotification(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            _NotificationService.DeleteNotification(id);

            return RedirectToAction("ListNotification");
        }

        #endregion Notification

        #endregion Methos
    }
}