using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class ReportSearchModel : BaseSearchModel
    {
        public ReportSearchModel()
        {
            ListStatus = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Fields.isNew")]
        public bool isNew { get; set; }
        public IList<SelectListItem> ListStatus { get; set; }
    }
}
