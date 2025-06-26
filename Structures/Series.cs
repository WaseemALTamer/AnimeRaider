using System.Text.Json.Serialization;
using System.Collections.Generic;






namespace AnimeRaider.Structures
{
    public class Series{

        [JsonPropertyName("Name")]
        public string? Name { get; set; }


        [JsonPropertyName("Cover")]
        public string? Cover { get; set; }


        [JsonPropertyName("Episodes")]
        public List<Episode>?  Episodes { get; set; }


    }
}