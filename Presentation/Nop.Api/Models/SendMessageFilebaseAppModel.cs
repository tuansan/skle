using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public partial class SendMessageFilebaseAppModel
    {
        public SendMessageFilebaseAppModel()
        {
            Ids = new List<string>();
        }
        public string Id { get; set; }
        public IList<string> Ids { get; set; }
        public string Data { get; set; }
    }
}
