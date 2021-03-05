using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public class AddPostModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public IFormFileCollection Files { get; set; }
        public string Ids { get; set; }
    }
}
