using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Skle
{
    public partial class NotiMobiClick : BaseEntity
    {
        public int MemberId { get; set; }
        public int NotificationId { get; set; }
    }
}
