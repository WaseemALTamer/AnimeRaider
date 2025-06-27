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


        public static string Ping = "ping";

        public static string All = "all"; // this is a path that gets all the series

        public static string Random = "random"; // this will give you random shows

        public static string Search = "search";

        public static string Watch = "watch";


    }
}
