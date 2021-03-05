using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public class RoomChat
    {
        public string chatId { get; set; }
        public string chatWith { get; set; }
        public bool isDelete { get; set; }
        public string lastChat { get; set; }
        public double timeCreate { get; set; }
        public double timestamp { get; set; }
    }
}
