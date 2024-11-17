using System.Text;
using System.Text.Json.Serialization;
using System.Reflection;

namespace ObjectScouter.Model
{
    public class Item
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("data")]
        public Dictionary<string, object> Data { get; set; } = [];
        //public Data? data { get; set; }

		public override string ToString()
		{
            StringBuilder stringBuilder = new();

			stringBuilder.AppendLine($"Id: {Id}");
			stringBuilder.AppendLine($"Name: {Name}");

            if (Data is not null && Data.Count > 0)
            {
				foreach (KeyValuePair<string, object> kvp in Data)
                {
                    if (kvp.Value is not null)
                    {
						stringBuilder.AppendLine($"{kvp.Key}: {kvp.Value}");
					}
                }
            }

            return stringBuilder.ToString();
		}

        public IEnumerable<KeyValuePair<string, object>> GetNonNullProperties()
        {
            if (Data is null)
            {
                yield break;
            }

            foreach (KeyValuePair<string, object> kvp in Data)
            {
                if (kvp.Value is not null)
                {
                    yield return kvp;
                }
            }
        }
	}
}
