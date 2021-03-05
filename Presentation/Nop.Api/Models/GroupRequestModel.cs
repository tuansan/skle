using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public class GroupRequestModel
    {
        public int GroupId { get; set; }
        public int MemberId { get; set; }
    }
}
