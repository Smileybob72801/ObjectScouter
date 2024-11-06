using System.Text.Json.Serialization;

namespace NumberVerifier.Model
{
    public class Root
    {
        [JsonPropertyName("id")]
        public string? id { get; set; }

        [JsonPropertyName("name")]
        public string? name { get; set; }

        [JsonPropertyName("data")]
        public Data? data { get; set; }
    }


}
