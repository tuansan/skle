using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Api.Models
{
    public class UpdatePictureModel
    {
        public byte[] PictureBinary { get; set; }
        public string MimeType { get; set; }
        public bool isUpAvartar { get; set; }

    }
}
