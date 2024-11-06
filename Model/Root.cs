using System.Text;
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

		public override string ToString()
		{
            StringBuilder stringBuilder = new();

			stringBuilder.AppendLine($"Id: {id}");
			stringBuilder.AppendLine($"Name: {name}");

            if (data is not null)
            {
                string rootData = data.GetNonNullProperties();

                stringBuilder.AppendLine(rootData); 
            }

            return stringBuilder.ToString();
		}
	}
}
