namespace AddictedProxy.Model.Shows
{
    public class TvShow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public virtual List<Episode> Episodes { get; set; }
    }
}