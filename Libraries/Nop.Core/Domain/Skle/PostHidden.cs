using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Skle
{
    public partial class PostHidden : BaseEntity
    {
        public int FormId { get; set; }
        public int ToId { get; set; }
        public bool isAll { get; set; }
        public int TargetId { get; set; }
    }
}
