using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Model.Shows
{
    [Index(nameof(DownloadUri), IsUnique = true)]
    public class Subtitle : IDiscoverableObject
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
        
        /// <summary>
        /// When was the subtitle discovered
        /// </summary>
        public DateTime Discovered { get; set; }
    }
}