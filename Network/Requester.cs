using AnimeRaider.Structures;
using Avalonia.Media.Imaging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AnimeRaider.Network
{
    public static class Requester
    {


        public static HttpClient client = new HttpClient();

        public static async Task<bool> GetPing(){
            try{
                string url = Server.Domain + Server.Ping;
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                using var response = await client.GetAsync(url, cts.Token);
                return response.IsSuccessStatusCode;
            }
            catch{
                return false;
            }
        }


        public static async Task<List<Series>?> GetAllSeries(){
            string url = Server.Domain + Server.All;
            try{
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions{
                    PropertyNameCaseInsensitive = true
                };

                List<Series>? seriesList = JsonSerializer.Deserialize<List<Series>>(responseBody, options);
                return seriesList;
            }
            catch (HttpRequestException e){
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e){
                Console.WriteLine($"JSON parse error: {e.Message}");
            }
            return null;
        }


        public static async Task<List<Series>?> GetRandomSeries(int number = 12)
        {
            string url = Server.Domain + Server.Random + "?" + "&number=" + number;
            try{
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                List<Series>? seriesList = JsonSerializer.Deserialize<List<Series>>(responseBody, options);
                return seriesList;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parse error: {e.Message}");
            }
            return null;
        }


        public static async Task<List<Series>?> GetSearchSeries(string query)
        {
            string url = Server.Domain + Server.Search + "?" + "&query=" + query;
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions{
                    PropertyNameCaseInsensitive = true
                };

                List<Series>? seriesList = JsonSerializer.Deserialize<List<Series>>(responseBody, options);
                return seriesList;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parse error: {e.Message}");
            }
            return null;
        }


        public static async Task<SKBitmap?> GetImage(string path){
            try{
                string url = Server.Domain + path;

                using var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync();

                // Decode the stream into an SKBitmap (Skia image)
                return SKBitmap.Decode(stream);
            }
            catch (HttpRequestException e){
                Console.WriteLine($"HTTP error: {e.Message}");
            }
            catch (Exception e){
                Console.WriteLine($"Unexpected error: {e.Message}");
            }

            return null;
        }


        public static async Task<UserData?> GetUserData(string username, string password){
            string url = Server.Domain + Server.Users + Server.Data + Server.All +  "?" + "&username=" + username + "&password=" + password;
            try{
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions{
                    PropertyNameCaseInsensitive = true
                };

                UserData? data = JsonSerializer.Deserialize<UserData?>(responseBody, options);
                return data;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parse error: {e.Message}");
            }
            return null;
        }


        public static async Task<bool> AddCompletedEpisode(string? username, string? password, string? series_name, Episode? episode){

            if (username == null ||
                password == null ||
                series_name == null ||
                episode == null) return false;


            string serialized_episode = JsonSerializer.Serialize(episode);

            string url = Server.Domain + Server.Users + Server.Data + Server.AddCompletedEpisode +
                         $"?username={username}" +
                         $"&password={password}" +
                         $"&series={series_name}" +
                         $"&episode={serialized_episode}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                // Try to parse it as a JSON bool
                if (bool.TryParse(content, out bool result)){
                    return result;
                }

                if (content.Trim() == "\"true\"")
                    return true;


                if (content.Trim() == "\"false\"")
                    return false;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parse error: {e.Message}");
            }
            return false;
        }


        public static async Task<bool> RemoveCompletedEpisode(string? username, string? password, string? series_name, Episode? episode)
        {

            if (username == null ||
                password == null ||
                series_name == null ||
                episode == null) return false;


            string serialized_episode = JsonSerializer.Serialize(episode);

            string url = Server.Domain + Server.Users + Server.Data + Server.RemoveCompletedEpisode +
                         $"?username={username}" +
                         $"&password={password}" +
                         $"&series={series_name}" +
                         $"&episode={serialized_episode}";


            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                // Try to parse it as a JSON bool
                if (bool.TryParse(content, out bool result))
                {
                    return result;
                }

                if (content.Trim() == "\"true\"")
                    return true;


                if (content.Trim() == "\"false\"")
                    return false;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parse error: {e.Message}");
            }
            return false;
        }





        public static async Task<bool> AddBookmarkEpisode(string? username, string? password, string? series_name, Episode? episode)
        {

            if (username == null ||
                password == null ||
                series_name == null ||
                episode == null) return false;


            string serialized_episode = JsonSerializer.Serialize(episode);

            string url = Server.Domain + Server.Users + Server.Data + Server.AddBookmarkedEpisode +
                         $"?username={username}" +
                         $"&password={password}" +
                         $"&series={series_name}" +
                         $"&episode={serialized_episode}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                // Try to parse it as a JSON bool
                if (bool.TryParse(content, out bool result))
                {
                    return result;
                }

                if (content.Trim() == "\"true\"")
                    return true;


                if (content.Trim() == "\"false\"")
                    return false;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parse error: {e.Message}");
            }
            return false;
        }


        public static async Task<bool> RemoveBookmarkEpisode(string? username, string? password, string? series_name, Episode? episode)
        {

            if (username == null ||
                password == null ||
                series_name == null ||
                episode == null) return false;


            string serialized_episode = JsonSerializer.Serialize(episode);

            string url = Server.Domain + Server.Users + Server.Data + Server.RemoveBookmarkedEpisode +
                         $"?username={username}" +
                         $"&password={password}" +
                         $"&series={series_name}" +
                         $"&episode={serialized_episode}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                // Try to parse it as a JSON bool
                if (bool.TryParse(content, out bool result))
                {
                    return result;
                }

                if (content.Trim() == "\"true\"")
                    return true;


                if (content.Trim() == "\"false\"")
                    return false;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parse error: {e.Message}");
            }
            return false;
        }



        public static async Task<bool> AddSeriesToCategory(string? username, string? password,string? category, Series? series)
        {

            // for category check the server.cs for the constants and pick the catogroy such as [complete, plantowatch, watching, ...]

            if (username == null ||
                password == null ||
                series == null) return false;


            string serialized_series = JsonSerializer.Serialize(series);

            string url = Server.Domain + Server.Users + Server.Data + Server.AddSeries +
                         $"?username={username}" +
                         $"&password={password}" +
                         $"&category={category}" +
                         $"&series={serialized_series}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                // Try to parse it as a JSON bool
                if (bool.TryParse(content, out bool result))
                {
                    return result;
                }

                if (content.Trim() == "\"true\"")
                    return true;


                if (content.Trim() == "\"false\"")
                    return false;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parse error: {e.Message}");
            }
            return false;
        }



        public static async Task<bool> RemoveSeriesToCategory(string? username, string? password, string? category, Series? series)
        {

            // for category check the server.cs for the constants and pick the catogroy such as [complete, plantowatch, watching, ...]

            if (username == null ||
                password == null ||
                series == null) return false;


            string serialized_series = JsonSerializer.Serialize(series);

            string url = Server.Domain + Server.Users + Server.Data + Server.RemoveSeries +
                         $"?username={username}" +
                         $"&password={password}" +
                         $"&category={category}" +
                         $"&series={serialized_series}";

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                // Try to parse it as a JSON bool
                if (bool.TryParse(content, out bool result))
                {
                    return result;
                }

                if (content.Trim() == "\"true\"")
                    return true;


                if (content.Trim() == "\"false\"")
                    return false;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON parse error: {e.Message}");
            }
            return false;
        }

    }
}
