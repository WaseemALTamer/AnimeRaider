using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AnimeRaider.Structures{

    public class UserData{
        [JsonPropertyName("Completed")]
        public List<Series>? Completed { get; set; }


        [JsonPropertyName("PlanToWatch")]
        public List<Series>? PlanToWatchver { get; set; }


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