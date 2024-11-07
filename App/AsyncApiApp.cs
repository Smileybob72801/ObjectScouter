using AsyncRestApi.Model;
using AsyncRestApi.Services;
using System.Reflection;

namespace AsyncRestApi.App
{
    internal class AsyncApiApp(IApiReaderService apiReaderService)
    {
        const string ApiBaseAddress = "https://api.restful-api.dev/";
        const string RequestUri = "/objects";

        private readonly IApiReaderService _apiReaderService = apiReaderService;

        public void Run()
        {
            IEnumerable<Item> items = GetAllObjects(_apiReaderService).Result;

            PrintObjects(items);

            FindProperty(items);
        }

        private void PrintObjects(IEnumerable<Item> objects)
        {
            foreach (Item item in objects)
            {
                Console.WriteLine(item);
            }
        }

        private async Task<IEnumerable<Item>> GetAllObjects(IApiReaderService apiReaderService)
        {
            var result = await apiReaderService.ReadAsync(ApiBaseAddress, RequestUri);

            return result;
        }

        private void FindProperty(IEnumerable<Item> items)
        {
            foreach (Item item in items)
            {
				IEnumerable<PropertyInfo> properties = item.GetNonNullProperties();

                foreach (PropertyInfo property in properties)
                {
                    if (property.Name.Equals("description", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Found a matching property: {property.Name}");
                    }
                }
            }
        }
    }
}
