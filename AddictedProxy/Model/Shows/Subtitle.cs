using System;

namespace AddictedProxy.Model
{
    public class Subtitle
    {
        public string Version { get; set; }
        public bool Completed { get; set; }
        public bool HearingImpaired { get; set; }
        public bool Corrected { get; set; }
        public bool HD { get; set; }
        public Uri DownloadUri { get; set; }
        public string Language { get; set; }
    }
}