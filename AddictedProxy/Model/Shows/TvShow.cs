using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Model.Shows
{
    public class TvShow
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual IList<Episode> Episodes { get; set; }
        public virtual IList<Season> Seasons { get; set; }
        public DateTime LastUpdated { get; set; }
        
        /// <summary>
        /// When was the last time we refreshed the seasons of the show
        /// </summary>
        public DateTime? LastSeasonRefreshed { get; set; }
    }
    
}