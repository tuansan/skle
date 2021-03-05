using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class ChatSearchModel : BaseSearchModel
    {
        public string chatId { get; set; }
        public string mem1 { get; set; }
        public string mem2 { get; set; }
    }
}
