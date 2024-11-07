using System.Text;
using System.Text.Json.Serialization;
using System.Reflection;

namespace AsyncRestApi.Model
{
    public class Data
    {
        [JsonPropertyName("color")]
        public string? color { get; set; }

        [JsonPropertyName("capacity")]
        public string? capacity { get; set; }

        [JsonPropertyName("capacity GB")]
        public int? capacityGB { get; set; }

        [JsonPropertyName("price")]
        public double? price { get; set; }

        [JsonPropertyName("generation")]
        public string? generation { get; set; }

        [JsonPropertyName("year")]
        public int? year { get; set; }

        [JsonPropertyName("CPU model")]
        public string? CPUmodel { get; set; }

        [JsonPropertyName("Hard disk size")]
        public string? Harddisksize { get; set; }

        [JsonPropertyName("Strap Colour")]
        public string? StrapColour { get; set; }

        [JsonPropertyName("Case Size")]
        public string? CaseSize { get; set; }

        [JsonPropertyName("Color")]
        public string? Color { get; set; }

        [JsonPropertyName("Description")]
        public string? Description { get; set; }

        [JsonPropertyName("Capacity")]
        public string? Capacity { get; set; }

        [JsonPropertyName("Screen size")]
        public double? Screensize { get; set; }

        [JsonPropertyName("Generation")]
        public string? Generation { get; set; }

        [JsonPropertyName("Price")]
        public string? Price { get; set; }

        public string ToPropertyString()
        {
            StringBuilder stringBuilder = new();

			IEnumerable<PropertyInfo> properties = GetNonNullProperties();

            foreach (PropertyInfo property in properties)
            {
				object? value = property.GetValue(this);

				stringBuilder.AppendLine($"{property.Name}: {value}");
			}

            return stringBuilder.ToString();
        }

        public IEnumerable<PropertyInfo> GetNonNullProperties()
        {
			Type dataType = typeof(Data);

			PropertyInfo[] properties = dataType.GetProperties();

            List<PropertyInfo> nonNullProperties = [];

			foreach (PropertyInfo property in properties)
			{
				object? value = property.GetValue(this);

				if (value is not null)
				{
					nonNullProperties.Add(property);
				}
			}

			return nonNullProperties;
		}
    }


}
