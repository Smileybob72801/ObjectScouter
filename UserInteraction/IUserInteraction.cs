using AsyncRestApi.Model;
using System.Reflection;

namespace AsyncRestApi.UserInteraction
{
    internal interface IUserInteraction
    {
        void DisplayText(string text);
		string GetValidString();
		void ListProperties(IEnumerable<PropertyInfo> properties);
		void PrintObjects(IEnumerable<Item> items);
	}

	internal class UserInteractionConsole : IUserInteraction
	{
		public void DisplayText(string text)
		{
            Console.WriteLine(text);
        }

        public string GetValidString()
        {
            string? result;

            do
            {
                result = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(result));

            return result;
        }

		public void PrintObjects(IEnumerable<Item> items)
		{
			foreach (Item item in items)
			{
				Console.WriteLine(item);
			}
		}

		public void ListProperties(IEnumerable<PropertyInfo> properties)
		{
			Console.WriteLine("Searchable properties: ");

			foreach (PropertyInfo property in properties)
			{
				Console.WriteLine(property.Name);
			}

            Console.WriteLine();
        }
	}
}
