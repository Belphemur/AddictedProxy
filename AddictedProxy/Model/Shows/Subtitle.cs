using System.ComponentModel.DataAnnotations.Schema;

namespace AddictedProxy.Model.Shows
{
    public class Subtitle
    {
        public int Id { get; set; }
        public int EpisodeId { get; set; }

        [ForeignKey(nameof(EpisodeId))]
        public virtual Episode Episode { get; set; }

        public string Version { get; set; }
        public bool Completed { get; set; }
        public bool HearingImpaired { get; set; }
        public bool Corrected { get; set; }
        public bool HD { get; set; }
        public Uri DownloadUri { get; set; }
        public string Language { get; set; }
    }
}