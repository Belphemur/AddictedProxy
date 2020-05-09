using System.Collections.Generic;

namespace AddictedProxy.Model
{
    public class Episode
    {

        public int Id { get; set; }
        public int Season { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public Subtitle[] Subtitles { get; set; }
    }
}