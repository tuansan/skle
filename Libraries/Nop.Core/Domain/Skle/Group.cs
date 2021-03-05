using System;

namespace Nop.Core.Domain.Skle
{
    public partial class Group : BaseEntity
    {
        public Guid GroupGuid { get; set; }
        public string Name { get; set; }
        public int AvatarId { get; set; }
        public int CoveId { get; set; }
        public int CountMember { get; set; }
        public int CountPost { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool isApproval { get; set; }
    }
}