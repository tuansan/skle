using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public partial class GroupMemberSearchModel : BaseSearchModel
    {
        public GroupMemberSearchModel()
        {
            Members = new List<SelectListItem>();
        }
        public int GroupId { get; set; }
        [NopResourceDisplayName("Admin.Fields.MemberIds")]
        public IList<int> MemberIds { get; set; }
        public IList<SelectListItem> Members { get; set; }
    }
}
