using System;
using System.Collections.Generic;
using AnimeRaider.Setting;
using AnimeRaider.Structures;




namespace AnimeRaider.SharedData
{
    public static class Data{

        public static string Username = "";
        public static string Password = "";



        public static List<Series>? AllSeries;


        public static List<Series>? RandomSeries;


        public static List<Series>? SearchedSeries;


        public static UserData? UserData;






        // now we will have function speicifcally for the User data to interact with it


        public static bool IsEpisodeInCompleted(string? series_name, string? episode_name) {
            if (series_name == null || episode_name == null) return false;
            if (UserData == null || UserData.CompletedEpisodes == null)
                return false;
            foreach (var completedSeries in UserData.CompletedEpisodes)
            {
                if (completedSeries?.Name == null) continue;
                if (completedSeries.Name != series_name) continue;

                if (completedSeries.Episodes == null) continue;
                foreach (var episode in completedSeries.Episodes){
                    if (episode?.Name == null) continue;
                    if (episode.Name == episode_name)
                        return true;
                }
            }

            return false;
        }


        public static bool IsEpisodeBookmarked(string? series_name, string? episode_name){
            if (UserData == null || UserData.BookmarkedEpisodes == null)
                return false;


            foreach (var bookedmarkedEpisodes in UserData.BookmarkedEpisodes){
                if (bookedmarkedEpisodes?.Name == null) continue;
                if (bookedmarkedEpisodes.Name != series_name) continue;

                if (bookedmarkedEpisodes.Episode == null) continue;

                if (bookedmarkedEpisodes.Episode.Name == episode_name) 
                    return true;
            }

            return false;
        }

        
        public static bool SetEpisodeComplete(string? series_name, Episode? episode) {
            // this function will intract with the data locally only


            if (series_name == null || episode == null) return false;
            if (UserData == null || UserData.CompletedEpisodes == null)
                return false;

            for (int i = 0; i < UserData.CompletedEpisodes.Count; i++){
                var _series = UserData.CompletedEpisodes[i];
                if (_series.Name == series_name)
                {
                    if (_series.Episodes == null) continue; // this needs to be removed later

                    _series.Episodes.Add(episode);

                    return true;
                }
            }

            UserData.CompletedEpisodes.Add(new CompletedEpisodes{
                Name = series_name,
                Episodes = new System.Collections.Generic.List<Episode> { episode }
            });


            return true;
        }


        public static bool SetEpisodeBookmarked(string? series_name, Episode? episode){
            // this function will intract with the data locally only

            if (series_name == null || episode == null) return false;
            if (UserData == null || UserData.BookmarkedEpisodes == null)
                return false;

            foreach (var bookedmarkedEpisodes in UserData.BookmarkedEpisodes)
            {
                if (bookedmarkedEpisodes?.Name == null) continue;
                if (bookedmarkedEpisodes.Name != series_name) continue;

                if (bookedmarkedEpisodes.Episode == null) continue;

                bookedmarkedEpisodes.Episode = episode;

            }

            UserData.BookmarkedEpisodes.Add(new Bookmark
            {
                Name = series_name,
                Episode = episode
            });


            return true;
        }


        public static bool RemoveEpisodeComplete(string? series_name, Episode? episode){
            if (series_name == null || episode == null) return false;
            if (UserData == null || UserData.CompletedEpisodes == null)
                return false;

            for (int i = 0; i < UserData.CompletedEpisodes.Count; i++){
                var _series = UserData.CompletedEpisodes[i];
                if (_series.Name == series_name){
                    if (_series.Episodes == null) continue;

                    // Try to remove matching episode by name
                    var removed = _series.Episodes.RemoveAll(e => e.Name == episode.Name) > 0;

                    // If no episodes left, remove the series entry entirely
                    if (_series.Episodes.Count == 0){
                        UserData.CompletedEpisodes.RemoveAt(i);
                    }

                    return removed;
                }
            }

            return false;
        }


        public static bool RemoveEpisodeBookmarked(string? series_name, Episode? episode){
            if (series_name == null || episode == null) return false;
            if (UserData == null || UserData.BookmarkedEpisodes == null)
                return false;

            var removed = UserData.BookmarkedEpisodes.RemoveAll(b =>
                b != null &&
                b.Name == series_name &&
                b.Episode != null &&
                b.Episode.Name == episode.Name
            ) > 0;

            return removed;
        }


    }
}
