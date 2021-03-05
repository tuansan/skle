using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class RoomChatSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Fields.KeySearch")]
        public int MemberId { get; set; }
        public IList<SelectListItem> Members { get; set; }
    }
}
