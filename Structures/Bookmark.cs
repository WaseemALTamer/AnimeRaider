using System.Text.Json.Serialization;


namespace AnimeRaider.Structures
{
    public class Bookmark
    {
        [JsonPropertyName("Name")]
        public string? Name { get; set; }


        [JsonPropertyName("Episode")]
        public Episode? Episode { get; set; }
    }
}
