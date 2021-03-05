using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class MemberLogSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Fields.Content")]
        public string KeySearch { get; set; }
        [NopResourceDisplayName("Admin.Fields.MemberId")]
        public int MemberId { get; set; }

        public IList<SelectListItem> ListMembers { get; set; }
    }
}