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

		public void ListStrings(IEnumerable<string> strings)
		{
			DisplayText($"Searchable properties: ");

			foreach (string s in strings)
			{
				DisplayText(s);
			}

			DisplayText("");
        }

		public void WaitForAnyInput()
		{
			Console.ReadKey();
		}
		
		public bool GetYesOrNo(string prompt, string invalidResponse)
		{

			while (true)
			{
				DisplayText(prompt);

                string input = Console.ReadKey().KeyChar.ToString();

				if (string.Equals(input, "y", StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
				else if (string.Equals(input, "n", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				else
				{
					DisplayText(invalidResponse);
				}
			}
			
		}
	}
}
