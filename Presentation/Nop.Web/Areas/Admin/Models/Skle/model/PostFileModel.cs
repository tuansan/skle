using Nop.Web.Framework.Models;
using System;

namespace Nop.Web.Areas.Admin.Models.Skle
{
    public partial class PostFileModel : BaseNopEntityModel
    {
        public int MemberId { get; set; }
        public int PostId { get; set; }
        public int FileType { get; set; }
        public int PictureId { get; set; }
        public int DownloadId { get; set; }
        public string MimeType { get; set; }
        public string Extension { get; set; }
        public DateTime CreatedAt { get; set; }
        public string VideoUrl { get; set; }

        public string Downloadname { get; set; }
        public string DownloadUrl { get; set; }
        public string PictureUrl { get; set; }
    }
}
