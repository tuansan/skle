using System;

namespace Nop.Core.Domain.Skle
{
    public partial class MemberLog : BaseEntity
    {
        public int MemberId { get; set; }
        public string Content { get; set; }
        public string After { get; set; }
        public DateTime CreatedAt { get; set; }
        public int StatusId { get; set; }
    }
}