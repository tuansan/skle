using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class ChatModel : BaseNopModel
    {
        public string Id { get; set; }
        public string content { get; set; }
        public string type { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public DateTime times { get; set; }
    }
}
