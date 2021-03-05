using System;

namespace Nop.Core.Domain.Skle
{
    public partial class Post : BaseEntity
    {
        public Guid PostGuid { get; set; }
        public string Content { get; set; }
        public int MemberId { get; set; }
        public int CountLike { get; set; }
        public int CountSpam { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Deleted { get; set; }
        public int GroupId { get; set; }
    }
}