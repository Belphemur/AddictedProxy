using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Model.Shows
{
    [Index(nameof(TvShowId), nameof(Season), nameof(Number), IsUnique = true)]
    public class Episode : IDiscoverableObject
    {
        public int Id { get; set; }

        public int TvShowId { get; set; }

        [ForeignKey(nameof(TvShowId))]
        public virtual TvShow TvShow { get; set; }

        public int Season { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public IList<Subtitle> Subtitles { get; set; }
        
        /// <summary>
        /// When was the episode discovered
        /// </summary>
        public DateTime Discovered { get; set; }
    }
}