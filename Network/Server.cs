using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeRaider.Network
{
    public static class Server
    {
        // this class will just provide Domain data for the database


        public static string Domain = "http://192.168.1.51:8000/";

        // General
        public const string Ping = "ping";
        public const string All = "all";       // gets all the series
        public const string Random = "random"; // gives random shows
        public const string Search = "search";
        public const string Watch = "watch";


        // Users
        public const string Users = "users/";
        public const string Create = "create";


        public const string Data = "data/";
        public const string AddSeries = "add_series";
        public const string RemoveSeries = "remove_series";

        public const string AddCompletedEpisode = "add_completed_episode";
        public const string RemoveCompletedEpisode = "remove_completed_episode";

        public const string AddBookmarkedEpisode = "add_bookmarked_episode";
        public const string RemoveBookmarkedEpisode = "remove_bookmarked_episode";



        // Catogries
        public const string Completed = "Completed";
        public const string PlanToWatch = "PlanToWatch";
        public const string Watching = "Watching";
        public const string WatchAgain = "WatchAgain";
        public const string Bookmarked = "Bookmarked";


    }
}
