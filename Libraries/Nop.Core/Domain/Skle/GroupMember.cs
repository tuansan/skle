using System;

namespace Nop.Core.Domain.Skle
{
    public partial class GroupMember : BaseEntity
    {
        public int MemberId { get; set; }
        public int GroupId { get; set; }
        public int StatusId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}