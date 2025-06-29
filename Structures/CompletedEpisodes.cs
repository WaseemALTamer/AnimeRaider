using System.Text.Json.Serialization;
using System.Collections.Generic;



namespace AnimeRaider.Structures
{
    public class CompletedEpisodes{
        [JsonPropertyName("Name")]
        public string? Name { get; set; }


        [JsonPropertyName("Episodes")]
        public List<Episode>? Episodes { get; set; }
    }
}
