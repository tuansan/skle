using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Skle
{
    public partial class NotiNew : BaseEntity
    {
        public int MemberId { get; set; }
        public DateTime ReadTime { get; set; }
    }
}
