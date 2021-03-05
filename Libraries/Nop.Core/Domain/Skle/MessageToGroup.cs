using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Skle
{
    public partial class MessageToGroup : BaseEntity
    {
        public int MemberId { get; set; }
        public int GroupId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
