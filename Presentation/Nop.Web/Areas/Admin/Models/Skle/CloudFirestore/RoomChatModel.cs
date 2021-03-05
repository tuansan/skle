using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public partial class RoomChatModel : BaseNopModel
    {
        public string chatId { get; set; }
        public string lastChat { get; set; }
        public DateTime times { get; set; }
        public DateTime timeCreate { get; set; }
        public string name { get; set; }
        public string avatarUrl { get; set; }
    }
}
