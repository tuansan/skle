using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public partial class ReportModel
    {
        public int TargetId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
