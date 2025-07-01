using AnimeRaider.Structures;
using AnimeRaider.Setting;
using System.Reflection;
using System.Text.Json;
using System.IO;
using System;

namespace AnimeRaider
{
    public static class AppData
    {
        public static string CredentialsPath = $"AppData/SavedUser.json";
        public static Credentials? Credentials = LoadSavedUserData();




        public static Credentials LoadSavedUserData(){
            Credentials credentialsInstance = new Credentials();

            // If file doesn't exist, create it from current static Config
            if (!File.Exists(CredentialsPath)){
                SaveConfig(credentialsInstance);
                return credentialsInstance;
            }

            try{
                string json = File.ReadAllText(CredentialsPath);
                credentialsInstance = JsonSerializer.Deserialize<Credentials>(json)!;

                if (credentialsInstance == null)
                    throw new Exception("Deserialized object was null.");

            }
            catch{
                SaveConfig(credentialsInstance);
            }

            return credentialsInstance;
        }

        public static void SaveConfig(Credentials credentials){
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(credentials, options);
            File.WriteAllText(CredentialsPath, json);
        }


    }
}
