using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class PostLogSearchModel : BaseSearchModel
    {
        [NopResourceDisplayName("Admin.Fields.KeySearch")]
        public string KeySearch { get; set; }
    }
}