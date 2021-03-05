using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public class GroupModel : BaseNopEntityModel
    {
        public Guid GroupGuid { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string CoverUrl { get; set; }
        public int CountMember { get; set; }
        public int CountPost { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool isApproval { get; set; }

        public int StatusGroupRequestId { get; set; }
        public int CountPostNew { get; set; }
        public int PostId { get; set; }
    }
}
