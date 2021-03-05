using System;

namespace Nop.Core.Domain.Skle
{
    public partial class PostLike : BaseEntity
    {
        public int PostId { get; set; }
        public int MemberId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool isLike { get; set; }
    }
}