using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Skle
{
    public partial class Report : BaseEntity
    {
        public int FormId { get; set; }
        public int TargetId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool isNew { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
