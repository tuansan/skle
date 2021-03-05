using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class MemberSearchModel : BaseSearchModel
    {
        public MemberSearchModel()
        {
            ListStatus = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Fields.KeySearch")]
        public string KeySearch { get; set; }

        [NopResourceDisplayName("Admin.Fields.PhoneNumber")]
        public string PhoneNumber { get; set; }

        [NopResourceDisplayName("Admin.Fields.StatusId")]
        public int StatusId { get; set; }

        public IList<SelectListItem> ListStatus { get; set; }
    }
}