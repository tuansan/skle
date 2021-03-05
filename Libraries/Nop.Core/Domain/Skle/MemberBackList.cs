using System;

namespace Nop.Core.Domain.Skle
{
    public partial class MemberBackList : BaseEntity
    {
        public int FromId { get; set; }
        public int ToId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}