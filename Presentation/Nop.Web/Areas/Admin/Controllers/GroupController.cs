using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Skle;
using Nop.Services.Customers;
using Nop.Services.Localization;
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
    public class GroupController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IGroupService _GroupService;
        private readonly IPictureService _pictureSrvice;
        private readonly IMemberService _memberSrvice;
        private readonly ICustomerService _customerSrvice;
        private readonly IWorkContext _workContext;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;

        #endregion Fields

        #region Ctor

        public GroupController(IPermissionService permissionService, IGroupService groupService, IPictureService pictureSrvice, IMemberService memberSrvice, ICustomerService customerSrvice, IWorkContext workContext, INotificationService notificationService, ILocalizationService localizationService, IPictureService pictureService)
        {
            _permissionService = permissionService;
            _GroupService = groupService;
            _pictureSrvice = pictureSrvice;
            _memberSrvice = memberSrvice;
            _customerSrvice = customerSrvice;
            _workContext = workContext;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _pictureService = pictureService;
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(ListGroup));
        }

        #endregion Ctor

        #region Utilities

        private GroupModel PrepareGroup(GroupModel model = null, Group entity = null)
        {
            //Action<GroupLocalizedModel, int> localizedModelConfiguration = null;
            if (model == null)
                model = new GroupModel();

            if (entity != null)
            {
                model = entity.ToModel<GroupModel>();

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

        #endregion Utilities

        #region Methos

        #region Group

        public virtual IActionResult ListGroup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = new GroupSearchModel();

            model.SetGridPageSize();

            return View(model);
        }

        public virtual IActionResult CreateGroup()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = PrepareGroup();

            return View(model);
        }

        public virtual IActionResult EditGroup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var entity = _GroupService.GetGroupById(id);
            if (entity == null)
                return RedirectToAction("ListGroup");

            var model = PrepareGroup(null, entity);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListGroup(GroupSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var entitys = new PagedList<Group>(_GroupService.GetAllGroup(KeySearch: searchModel.KeySearch).ToList(), searchModel.Page - 1, searchModel.PageSize);

            var model = new GroupListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var _model = entity.ToModel<GroupModel>();
                    _model.PictureUrl = _pictureSrvice.GetPictureUrl(entity.AvatarId, 100);
                    _model.CountRequest = _GroupService.GetAllGroupMember(GroupId: entity.Id).Count(s => s.StatusId == (int)ENStatusGroupRequest.wait_confirm);
                    return _model;
                });
            });

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult UpdateInlineGroup(GroupModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product Download with the specified id
            var entity = _GroupService.GetGroupById(model.Id)
                ?? throw new ArgumentException("No product Download found with the specified id");

            _GroupService.Update(entity);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult DeleteGroupSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    _GroupService.DeleteGroup(item, "Admin: " + _workContext.CurrentCustomer.Id + "- delete");
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateGroup(GroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity<Group>();

                _GroupService.Insert(entity, "Admin: " + _workContext.CurrentCustomer.Id);

                if (!continueEditing)
                    return RedirectToAction("ListGroup");

                return RedirectToAction("EditGroup", new { id = entity.Id });
            }
            model = PrepareGroup(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditGroup(GroupModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = _GroupService.GetGroupById(model.Id);
                if (entity == null)
                    return RedirectToAction("ListGroup");

                entity.Name = model.Name;
                entity.AvatarId = model.AvatarId;
                entity.CoveId = model.CoveId;
                entity.isApproval = model.isApproval;

                _GroupService.Update(entity, "Admin: " + _workContext.CurrentCustomer.Id);

                //if (sp.AvatarId > 0 && sp.AvatarId != model.AvatarId) DeletePicture((int)sp.AvatarId);

                if (!continueEditing)
                    return RedirectToAction("ListGroup");

                return RedirectToAction("EditGroup", new { id = entity.Id });
            }

            model = PrepareGroup(model);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteGroup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            _GroupService.DeleteGroup(id);

            return RedirectToAction("ListGroup");
        }

        #endregion Group

        #region manage member

        #endregion

        #region GroupMemberRequest

        public virtual IActionResult GroupMember(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = new GroupMemberSearchModel();
            model.SetGridPageSize();
            model.GroupId = id;
            model.Members = _memberSrvice.GetAllMemberNotInGroup(id).Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList();
            return View(model);
        }
        [HttpPost]
        public virtual IActionResult GroupMemberAdd(GroupMemberSearchModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var group = _GroupService.GetGroupById(model.GroupId);
            if (group == null)
                return Content("Có gì đó sai sai");
            foreach (var item in model.MemberIds)
            {
                var gm = _GroupService.GetGroupMemberByGroupIdAndMemberId(item, model.GroupId);
                if (gm != null && gm.StatusId == (int)ENStatusGroupRequest.confirm)
                    continue;
                if(gm != null)
                {
                    gm.StatusId = (int)ENStatusGroupRequest.confirm;
                    gm.CreatedAt = DateTime.Now;
                    _GroupService.Update(gm);

                    _notificationService.Insert(new NotiMobi()
                    {
                        BranchId = (int)ENTypeNotification.member,
                        Content = _localizationService.GetResource("member.ConfirmGroupMemberRequest") + " " + group.Name,
                        GroupId = group.Id,
                        //PictureUrl = _pictureService.GetPictureUrl(group.AvatarId, 100, showDefaultPicture: false),
                        Name = _localizationService.GetResource("noti.admin"),
                        MemberId = gm.MemberId,
                        TypeId = (int)ENTypeNotiMobi.xacnhan_vaonhom
                    });
                }
                if(gm == null)
                {
                    gm = new GroupMember { CreatedAt = DateTime.Now, GroupId = model.GroupId, MemberId = item,StatusId = (int)ENStatusGroupRequest.confirm };
                    _GroupService.Insert(gm);

                    _notificationService.Insert(new NotiMobi()
                    {
                        BranchId = (int)ENTypeNotification.member,
                        Content = _localizationService.GetResource("member.AddGroupMember") + " " + group.Name,
                        GroupId = group.Id,
                        //PictureUrl = _pictureService.GetPictureUrl(group.AvatarId, 100, showDefaultPicture: false),
                        Name = _localizationService.GetResource("noti.admin"),
                        MemberId = gm.MemberId,
                        TypeId = (int)ENTypeNotiMobi.xacnhan_vaonhom
                    });
                }
                group.CountMember++;
                _GroupService.Update(group, _workContext.CurrentCustomer.Id + " Confirm");
            }
            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ListGroupMember(GroupMemberSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var entitys = new PagedList<GroupMember>(_GroupService.GetAllGroupMember(GroupId: searchModel.GroupId).Where(s => s.StatusId  != (int)ENStatusGroupRequest.cancel).OrderBy(s => s.StatusId).ToList(), searchModel.Page - 1, searchModel.PageSize);

            var model = new GroupMemberListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var _model = new GroupMemberModel();
                    _model.Id = entity.Id;
                    _model.CreatedAt = entity.CreatedAt;
                    var member = _memberSrvice.GetMemberById(entity.MemberId);
                    var custommer = _customerSrvice.GetCustomerById(member.CustomerId);
                    _model.Name = member.Name;
                    _model.Phone = custommer.PhoneNumber;
                    _model.AvatarUrl = _pictureSrvice.GetPictureUrl(member.AvatarId, 100, defaultPictureType: Core.Domain.Media.PictureType.Avatar);
                    _model.StatusId = entity.StatusId;
                    _model.MemberId = entity.MemberId;
                    return _model;
                });
            });

            return Json(model);
        }


        [HttpPost]
        public virtual IActionResult DeleteGroupMemberRequest(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var grmb = _GroupService.GetGroupMemberById(id) ?? throw new ArgumentException("No Group Member found with the Group Member id");
            
            _GroupService.DeleteGroupMember(grmb.Id);
            var group = _GroupService.GetGroupById(grmb.GroupId);
            group.CountMember--;
            _GroupService.Update(group, "Thành viên: " + grmb.MemberId +" đã bị admin: " + _workContext.CurrentCustomer.Id + " trục xuất");

            return Json(new { Result = true });
        }
        

        [HttpPost]
        public virtual IActionResult ConfirmGroupMemberRequest(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var grmb = _GroupService.GetGroupMemberById(id) ?? throw new ArgumentException("No Group Member found with the Group Member id");
            
            grmb.StatusId = (int)ENStatusGroupRequest.confirm;

            _GroupService.Update(grmb);
            var group = _GroupService.GetGroupById(grmb.GroupId);
            group.CountMember++;
            _GroupService.Update(group, _workContext.CurrentCustomer.Id + " Confirm");

            _notificationService.Insert(new NotiMobi()
            {
                BranchId = (int)ENTypeNotification.member,
                Content = _localizationService.GetResource("member.ConfirmGroupMemberRequest") + " " + group.Name,
                GroupId = group.Id,
                //PictureUrl = _pictureService.GetPictureUrl(group.AvatarId, 100, showDefaultPicture: false),
                Name = _localizationService.GetResource("noti.admin"),
                MemberId = grmb.MemberId,
                TypeId = (int)ENTypeNotiMobi.xacnhan_vaonhom
            });

            return Json(new { Result = true });
        }
        
        

        [HttpPost]
        public virtual IActionResult CancelGroupMemberRequest(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var grmb = _GroupService.GetGroupMemberById(id) ?? throw new ArgumentException("No Group Member found with the Group Member id");
            
            grmb.StatusId = (int)ENStatusGroupRequest.none;

            _GroupService.Update(grmb);
            return Json(new { Result = true });
        }

        #endregion
        #endregion Methos
    }
}