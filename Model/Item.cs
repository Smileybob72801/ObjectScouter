using System.Text;
using System.Text.Json.Serialization;

namespace NumberVerifier.Model
{
    public class Item
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
                string itemData = data.GetNonNullProperties();

                stringBuilder.AppendLine(itemData); 
            }

            return stringBuilder.ToString();
		}
	}
}
