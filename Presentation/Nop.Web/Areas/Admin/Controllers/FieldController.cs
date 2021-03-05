using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Skle;
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
    public partial class FieldController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IMemberService _FieldService;

        #endregion Fields

        #region Ctor

        public FieldController(IPermissionService permissionService,
            IMemberService FieldService)
        {
            _permissionService = permissionService;
            _FieldService = FieldService;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(ListField));
        }

        #endregion Ctor

        #region Utilities

        private FieldModel PrepareField(FieldModel model = null, Field entity = null)
        {
            //Action<FieldLocalizedModel, int> localizedModelConfiguration = null;
            if (model == null)
                model = new FieldModel();

            if (entity != null)
            {
                model = entity.ToModel<FieldModel>();

                //define localized model configuration action
                //localizedModelConfiguration = (locale, languageId) =>
                //{
                //    locale.Name = _localizationService.GetLocalized(entity, entity => entity.Name, languageId, false, false);
                //};
            }

            //model.GiaiDaus = _GiaiDauService.GetAll().Select(s => new SelectListItem { Text = s.TenGiaiDau, Value = s.Id.ToString() }).ToList();

            //model.DoiBongs = _DoiBongService.GetAll().Select(s => new SelectListItem { Text = s.TenDoiBong, Value = s.Id.ToString() }).ToList();
            model.AvailableStatus.Add(new SelectListItem("Hiển thị", ((int)ENStatusField.show).ToString()));
            model.AvailableStatus.Add(new SelectListItem("Ẩn", ((int)ENStatusField.hide).ToString()));
            return model;
        }

        #endregion Utilities

        #region Methos

        #region Field

        public virtual IActionResult ListField()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = new FieldSearchModel();

            model.SetGridPageSize();

            return View(model);
        }

        public virtual IActionResult CreateField()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = PrepareField();

            return View(model);
        }

        public virtual IActionResult EditField(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var entity = _FieldService.GetFieldById(id);
            if (entity == null)
                return RedirectToAction("ListField");

            var model = PrepareField(null, entity);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListField(FieldSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var entitys = new PagedList<Field>(_FieldService.GetAllField(KeySearch: searchModel.KeySearch).ToList(), searchModel.Page - 1, searchModel.PageSize);

            var model = new FieldListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var _model = entity.ToModel<FieldModel>();
                    _model.Status = entity.StatusId == 1;
                    return _model;
                });
            });

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult UpdateInlineField(FieldModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product Download with the specified id
            var entity = _FieldService.GetFieldById(model.Id)
                ?? throw new ArgumentException("No product Download found with the specified id");

            _FieldService.Update(entity);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult DeleteFieldSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    _FieldService.DeleteField(item);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateField(FieldModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity<Field>();

                _FieldService.Insert(entity);

                if (!continueEditing)
                    return RedirectToAction("ListField");

                return RedirectToAction("EditField", new { id = entity.Id });
            }
            model = PrepareField(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditField(FieldModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = _FieldService.GetFieldById(model.Id);
                if (entity == null)
                    return RedirectToAction("ListField");

                entity = model.ToEntity<Field>();

                _FieldService.Update(entity);

                //if (sp.AvatarId > 0 && sp.AvatarId != model.AvatarId) DeletePicture((int)sp.AvatarId);

                if (!continueEditing)
                    return RedirectToAction("ListField");

                return RedirectToAction("EditField", new { id = entity.Id });
            }

            model = PrepareField(model);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteField(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            _FieldService.DeleteField(id);

            return RedirectToAction("ListField");
        }

        #endregion Field

        #endregion Methos
    }
}