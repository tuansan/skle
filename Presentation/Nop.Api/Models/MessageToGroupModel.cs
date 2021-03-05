using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public partial class MessageToGroupModel 
    {
        public int GroupId { get; set; }
        public string Content { get; set; }
    }
}
