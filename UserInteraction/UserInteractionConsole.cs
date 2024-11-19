using ObjectScouter.Model;
using System.Reflection;

namespace ObjectScouter.UserInteraction
{
	internal class UserInteractionConsole : IUserInteraction
	{
		public void DisplayText(string text)
		{
            Console.WriteLine(text);
        }

        public string GetValidString(string prompt = "")
        {
            string? result;

            do
            {
				if (!string.IsNullOrEmpty(prompt))
				{
					DisplayText(prompt);
				}

                result = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(result));

			DisplayText("");

			return result;
        }

		public void ListItems(IEnumerable<Item> items)
		{
			foreach (Item item in items)
			{
				DisplayText(item.ToString());
			}
		}

		public void ListProperties(IEnumerable<string> properties)
		{
			DisplayText($"Searchable properties: ");

			foreach (string property in properties)
			{
				DisplayText(property);
			}

			DisplayText("");
        }

		public void WaitForAnyInput()
		{
			Console.ReadKey();
		}
	}
}
