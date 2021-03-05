using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public partial class GroupModel : BaseNopEntityModel
    {
        public GroupModel()
        {
            GroupGuid = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        public Guid GroupGuid { get; set; }

        [NopResourceDisplayName("Admin.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Fields.AvatarId")]
        [UIHint("Picture")]
        public int AvatarId { get; set; }

        [NopResourceDisplayName("Admin.Fields.CoveId")]
        [UIHint("Picture")]
        public int CoveId { get; set; }

        public int CountMember { get; set; }
        public int CountPost { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }

        [NopResourceDisplayName("Admin.Fields.isApproval")]
        public bool isApproval { get; set; }
        public int CountRequest { get; set; }
        public string PictureUrl { get; set; }
    }
}