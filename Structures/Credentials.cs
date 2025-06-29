using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AnimeRaider.Structures
{
    public class Credentials{
        [JsonPropertyName("Domain")]
        public string? Domain { get; set; }

        [JsonPropertyName("Username")]
        public string? Username { get; set; }

        [JsonPropertyName("Password")]
        public string? Password { get; set; }

    }
}
