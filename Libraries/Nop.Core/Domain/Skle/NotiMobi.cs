using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Skle
{
    public partial class NotiMobi : BaseEntity
    {
        public string PictureUrl { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Sent { get; set; }
        public int BranchId { get; set; }
        public int TypeId { get; set; }
        public int GroupId { get; set; }
        public int currentMemberId { get; set; }
        public int MemberId { get; set; }
        public int PostId { get; set; }
        public int MessageId { get; set; }
        public int SentTries { get; set; }
    }
}
