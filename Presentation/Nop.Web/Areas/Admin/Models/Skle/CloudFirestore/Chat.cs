using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class Chat
    {
        public string Id { get; set; }
        public dynamic content { get; set; }
        public string idFrom { get; set; }
        public string idTo { get; set; }
        public bool isread { get; set; }
        public double timestamp { get; set; }
        public string type { get; set; }
    }
}
