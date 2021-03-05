using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public partial class ReportModel : BaseNopEntityModel
    {
        public int FormId { get; set; }
        public int TargetId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool isNew { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Name { get; set; }
    }
}
