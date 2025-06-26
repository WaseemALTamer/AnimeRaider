using System.Text.Json.Serialization;




namespace AnimeRaider.Structures
{
    public class Episode
    {
        [JsonPropertyName("Name")]
        public string? Name { get; set; }


        [JsonPropertyName("Duration")]
        public double? Duration { get; set; }


        [JsonPropertyName("Thumbnail")]
        public string? Thumbnail { get; set; }

    }
}