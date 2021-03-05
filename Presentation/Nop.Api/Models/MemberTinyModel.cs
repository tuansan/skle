using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public class MemberTinyModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AvatarUrl { get; set; }

        public string CoveUrl { get; set; }

        public string Province { get; set; }

        public IEnumerable<string> Fields { get; set; }
    }
}
