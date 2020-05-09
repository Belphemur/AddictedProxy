using System.IO;

namespace AddictedProxy.Model
{
    public class DownloadSubtitleResult
    {
        public string Filename { get; set; }
        public Stream Stream { get; set; }
        public string Mediatype { get; set; }
    }
}