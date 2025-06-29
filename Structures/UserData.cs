using System.Text.Json.Serialization;
using System.Collections.Generic;



namespace AnimeRaider.Structures{

    public class UserData{
        [JsonPropertyName("Completed")]
        public List<Series>? Completed { get; set; }


        [JsonPropertyName("PlanToWatch")]
        public List<Series>? PlanToWatch { get; set; }


        [JsonPropertyName("Watching")]
        public List<Series>? Watching { get; set; }


        [JsonPropertyName("WatchAgain")]
        public List<Series>? WatchAgain { get; set; }


        [JsonPropertyName("Bookmarked")]
        public List<Series>? Bookmarked { get; set; }


        [JsonPropertyName("BookmarkedEpisodes")]
        public List<Bookmark>? BookmarkedEpisodes { get; set; }


        [JsonPropertyName("CompletedEpisodes")]
        public List<CompletedEpisodes>? CompletedEpisodes { get; set; }

    }
}