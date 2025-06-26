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
using System.Threading.Tasks;

namespace AnimeRaider.Network
{
    public static class Requester
    {


        public static HttpClient client = new HttpClient();

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
            try
            {
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



    }
}
