using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public class GetPostFileModel
    {
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public int Id { get; set; }
    }
}
