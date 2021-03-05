using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Skle
{
    public partial class MemberMessage: BaseEntity
    {
        public int MessageId { get; set; }
        public int MemberId { get; set; }
    }
}
