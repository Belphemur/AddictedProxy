using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Model.Shows
{
    [Index(nameof(TvShowId), nameof(Season), nameof(Number), IsUnique = true)]
    public class Episode
    {
        public int Id { get; set; }

        public int TvShowId { get; set; }

        [ForeignKey(nameof(TvShowId))]
        public virtual TvShow TvShow { get; set; }

        public int Season { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public Subtitle[] Subtitles { get; set; }
    }
}