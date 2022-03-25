namespace AddictedProxy.Model.Shows
{
    internal class SubtitleRow
    {
        public int EpisodeId { get; set; }
        public int Season { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Scene { get; set; }
        public bool Completed { get; set; }
        public bool HearingImpaired { get; set; }
        public bool Corrected { get; set; }
        public bool HD { get; set; }
        public Uri DownloadUri { get; set; }

        public int Version { get; set; }
    }
}