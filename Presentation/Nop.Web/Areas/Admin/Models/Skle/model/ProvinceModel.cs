using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class ProvinceModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Fields.Active")]
        public bool Active { get; set; }
    }
}