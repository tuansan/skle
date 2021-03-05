using Microsoft.AspNetCore.Mvc;
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
    public class ProvinceController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly IProvinceService _ProvinceService;

        #endregion Fields

        #region Ctor

        public ProvinceController(IPermissionService permissionService,
            IProvinceService ProvinceService)
        {
            _permissionService = permissionService;
            _ProvinceService = ProvinceService;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(ListProvince));
        }

        #endregion Ctor

        #region Utilities

        private ProvinceModel PrepareProvince(ProvinceModel model = null, Province entity = null)
        {
            //Action<ProvinceLocalizedModel, int> localizedModelConfiguration = null;
            if (model == null)
                model = new ProvinceModel();

            if (entity != null)
            {
                model = entity.ToModel<ProvinceModel>();

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

        #region Province

        public virtual IActionResult ListProvince()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = new ProvinceSearchModel();

            model.SetGridPageSize();

            return View(model);
        }

        public virtual IActionResult CreateProvince()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var model = PrepareProvince();

            return View(model);
        }

        public virtual IActionResult EditProvince(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a manufacturer with the specified id
            var entity = _ProvinceService.GetProvinceById(id);
            if (entity == null)
                return RedirectToAction("ListProvince");

            var model = PrepareProvince(null, entity);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListProvince(ProvinceSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var entitys = new PagedList<Province>(_ProvinceService.GetAllProvince(KeySearch: searchModel.KeySearch).ToList(), searchModel.Page - 1, searchModel.PageSize);

            var model = new ProvinceListModel().PrepareToGrid(searchModel, entitys, () =>
            {
                return entitys.Select(entity =>
                {
                    var _model = entity.ToModel<ProvinceModel>();
                    return _model;
                });
            });

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult UpdateInlineProvince(ProvinceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            //try to get a product Download with the specified id
            var entity = _ProvinceService.GetProvinceById(model.Id)
                ?? throw new ArgumentException("No product Download found with the specified id");

            _ProvinceService.Update(entity);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult DeleteProvinceSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    _ProvinceService.DeleteProvince(item);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult CreateProvince(ProvinceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = model.ToEntity<Province>();

                _ProvinceService.Insert(entity);

                if (!continueEditing)
                    return RedirectToAction("ListProvince");

                return RedirectToAction("EditProvince", new { id = entity.Id });
            }
            model = PrepareProvince(model);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult EditProvince(ProvinceModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var entity = _ProvinceService.GetProvinceById(model.Id);
                if (entity == null)
                    return RedirectToAction("ListProvince");

                entity = model.ToEntity<Province>();

                _ProvinceService.Update(entity);

                //if (sp.AvatarId > 0 && sp.AvatarId != model.AvatarId) DeletePicture((int)sp.AvatarId);

                if (!continueEditing)
                    return RedirectToAction("ListProvince");

                return RedirectToAction("EditProvince", new { id = entity.Id });
            }

            model = PrepareProvince(model);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult DeleteProvince(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            _ProvinceService.DeleteProvince(id);

            return RedirectToAction("ListProvince");
        }

        #endregion Province

        #endregion Methos
    }
}