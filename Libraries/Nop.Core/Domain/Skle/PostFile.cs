using System;

namespace Nop.Core.Domain.Skle
{
    public partial class PostFile : BaseEntity
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
    }
}