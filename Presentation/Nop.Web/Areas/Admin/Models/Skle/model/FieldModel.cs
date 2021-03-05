using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class FieldModel : BaseNopEntityModel
    {
        public FieldModel()
        {
            AvailableStatus = new List<SelectListItem>();
            StatusId = 1;
        }

        [NopResourceDisplayName("Admin.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Fields.StatusId")]
        public int StatusId { get; set; }

        public bool Deleted { get; set; }

        public bool Status { get; set; }
        public IList<SelectListItem> AvailableStatus { get; set; }
    }
}