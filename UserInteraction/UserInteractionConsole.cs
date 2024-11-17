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

            return result;
        }

		public void PrintObjects(IEnumerable<Item> items)
		{
			foreach (Item item in items)
			{
				DisplayText(item.ToString());
			}
		}

		public void ListProperties(IEnumerable<string> properties)
		{
			DisplayText($"{Environment.NewLine}Searchable properties: ");

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
