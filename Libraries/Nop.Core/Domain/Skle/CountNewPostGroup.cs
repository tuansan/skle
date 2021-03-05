using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Skle
{
    public partial class CountNewPostGroup : BaseEntity
    {
        public int MemberId { get; set; }
        public int GroupId { get; set; }
        public int CountPost { get; set; }
    }
}
