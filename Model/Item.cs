using System.Text;
using System.Text.Json.Serialization;
using System.Reflection;

namespace AsyncRestApi.Model
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
                string itemData = data.ToPropertyString();

                stringBuilder.AppendLine(itemData); 
            }

            return stringBuilder.ToString();
		}

        public IEnumerable<PropertyInfo> GetNonNullProperties()
        {
            Type itemType = typeof(Item);

            List<PropertyInfo> nonNullProperties = [];

            IEnumerable<PropertyInfo> properties = itemType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
				object? value = property.GetValue(this);

                if (value is not null)
                {
                    nonNullProperties.Add(property);
                }
            }

            if (data is not null)
            {
                IEnumerable<PropertyInfo> dataProperties = data.GetNonNullProperties();
                
                foreach (PropertyInfo property in dataProperties)
                {
					nonNullProperties.Add(property);
				}
            }

            return [.. nonNullProperties];
        }
	}
}
