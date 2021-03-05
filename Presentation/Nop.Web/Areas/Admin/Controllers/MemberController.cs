using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Skle;
using Nop.Services.Customers;
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
    public class MemberController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IMemberService _MemberService;
        private readonly IPictureService _PictureService;
        private readonly ICustomerService _CustomerService;
        private readonly IProvinceService _ProvinceService;
        private readonly IWorkContext _workContext;

        #endregion Fields

        #region Ctor

        public MemberController(IPermissionService permissionService,
            IMemberService MemberService,
            IPictureService PictureService,
            ICustomerService CustomerService,
            IWorkContext workContext,
            IProvinceService ProvinceService)
        {
            _permissionService = permissionService;
            _MemberService = MemberService;
            _PictureService = PictureService;
            _CustomerService = CustomerService;
            _workContext = workContext;
            _ProvinceService = ProvinceService;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(ListMember));
        }

        #endregion Ctor

        #region Utilities

        private MemberModel PrepareMember(MemberModel model = null, Member entity = null)
        {
            //Action<MemberLocalizedModel, int> localizedModelConfiguration = null;
            if (model == null)
                model = new MemberModel();

            if (entity != null)
            {
                model = entity.ToModel<MemberModel>();

                //define localized model configuration action
                //localizedModelConfiguration = (locale, languageId) =>
                //{
                //    locale.Name = _localizationService.GetLocalized(entity, entity => entity.Name, languageId, false, false);
                //};
            }

            model.AvailableGenders.Add(new SelectListItem("Nam", "0"));
            model.AvailableGenders.Add(new SelectListItem("Nữ", "1"));

            model.AvailableStatus.Add(new SelectListItem("Hoạt động", (int)ENStatusMember.Active + ""));
            model.AvailableStatus.Add(new SelectListItem("Khoá", (int)ENStatusMember.Blocked + ""));

            model.AvailableProvinces = _ProvinceService.GetAllProvince().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();

            model.Fields = _MemberService.GetAllMemberField(entity.Id).Select(s => s.FieldId).ToList();

            model.AvailablFields = _MemberService.GetAllField().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();

            return model;
        }

        #endregion Utilities

        #region Methos

        #region Member

        public virtual IActionResult ListMember()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = new MemberSearchModel();

            model.SetGridPageSize();
            model.ListStatus.Add(new SelectListItem("Tất cả", (int)ENStatusMember.All + "", true));
            model.ListStatus.Add(new SelectListItem("Hoạt động", (int)ENStatusMember.Active + ""));
            model.ListStatus.Add(new SelectListItem("Đã khoá", (int)ENStatusMember.Blocked + ""));

            return View(model);
        }

        public virtual IActionResult CreateMember()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = PrepareMember();

            return View(model);
        }

        public virtual IActionResult EditMember(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var entity = _MemberService.GetMemberById(id);
            if (entity == null)
                return RedirectToAction("ListMember");

            var model = PrepareMember(null, entity);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListMember(MemberSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var members = _MemberService.GetAllMember(KeySearch: searchModel.KeySearch, searchModel.StatusId).ToList();
            if (!string.IsNullOrEmpty(searchModel.PhoneNumber))
            {
                members = members.Where(s => _CustomerService.GetCustomerById(s.CustomerId).PhoneNumber.Contains(searchModel.PhoneNumber)).ToList();
            }
            var entitys = new PagedList<Member>(members, searchModel.Page - 1, searchModel.PageSize);

            var model = new MemberListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var _model = entity.ToModel<MemberModel>();
                    _model.AvatarUrl = _PictureService.GetPictureUrl(entity.AvatarId, 100, defaultPictureType: Core.Domain.Media.PictureType.Avatar);
                    _model.Active = entity.StatusId == (int)ENStatusMember.Active;
                    _model.PhoneNumber = _CustomerService.GetCustomerById(entity.CustomerId).PhoneNumber;
                    return _model;
                });
            });

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult UpdateInlineMember(MemberModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product Download with the specified id
            var entity = _MemberService.GetMemberById(model.Id)
                ?? throw new ArgumentException("No product Download found with the specified id");
            if (entity.Deleted == model.Active)
            {
                if (entity.Deleted)
                {
                    entity.Deleted = false;
                    _MemberService.Update(entity, "Undeleted by: " + _workContext.CurrentCustomer.Id);
                }
                else
                {
                    _MemberService.DeleteMember(entity.Id, "Deleted by: " + _workContext.CurrentCustomer.Id);
                }
            }

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult DeleteMemberSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    _MemberService.DeleteMember(item, "Deleted by: " + _workContext.CurrentCustomer.Id);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateMember(MemberModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity<Member>();

                _MemberService.Insert(entity);

                if (!continueEditing)
                    return RedirectToAction("ListMember");

                return RedirectToAction("EditMember", new { id = entity.Id });
            }
            model = PrepareMember(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditMember(MemberModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var member = _MemberService.GetMemberById(model.Id);
                if (member == null)
                    return RedirectToAction("ListMember");

                var entity = model.ToEntity<Member>();
                entity.CountFriend = member.CountFriend;
                entity.CountGroup = member.CountGroup;
                entity.CountPost = member.CountPost;
                entity.CreatedAt = member.CreatedAt;
                entity.CustomerId = member.CustomerId;
                entity.FirebaseId = member.FirebaseId;
                _MemberService.Update(entity, "Admin update: " + _workContext.CurrentCustomer.Id);

                if (member.AvatarId > 0 && member.AvatarId != model.AvatarId) _PictureService.DeletePicture(_PictureService.GetPictureById(member.AvatarId));
                if (member.CoveId > 0 && member.CoveId != model.CoveId) _PictureService.DeletePicture(_PictureService.GetPictureById(member.CoveId));

                var memberfield = _MemberService.GetAllMemberField(entity.Id).ToList();
                foreach (var item in model.Fields)
                {
                    if (memberfield.Count(s => s.FieldId == item) == 0)
                        _MemberService.Insert(new MemberField() { FieldId = item, MemberId = entity.Id });
                }
                foreach (var item in memberfield)
                {
                    if (model.Fields.Count(s => s == item.FieldId) == 0)
                    {
                        _MemberService.Delete(item);
                    }
                }
                if (!continueEditing)
                    return RedirectToAction("ListMember");

                return RedirectToAction("EditMember", new { id = entity.Id });
            }

            model = PrepareMember(model);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteMember(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            _MemberService.DeleteMember(id, "Deleted by: " + _workContext.CurrentCustomer.Id);

            return RedirectToAction("ListMember");
        }

        #endregion Member
        #region Member log


        public virtual IActionResult ListMemberLog(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = new MemberLogSearchModel();

            model.SetGridPageSize();

            model.MemberId = id;

            model.ListMembers = _MemberService.GetAllMember().Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(_CustomerService.GetCustomerById(s.CustomerId).PhoneNumber + " - " + s.Id, s.Id + "")).ToList();

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListMemberLog(MemberLogSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var memberLogs = _MemberService.GetAllMemberLog(searchModel.MemberId, searchModel.KeySearch).OrderByDescending(s => s.CreatedAt).ToList();

            var entitys = new PagedList<MemberLog>(memberLogs, searchModel.Page - 1, searchModel.PageSize);

            var model = new MemberLogListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var _model = entity.ToModel<MemberLogModel>();
                    _model.member = _MemberService.GetMemberById(entity.MemberId).ToModel<MemberModel>();
                    return _model;
                });
            });

            return Json(model);
        }

        #endregion
        #endregion Methos
    }
}