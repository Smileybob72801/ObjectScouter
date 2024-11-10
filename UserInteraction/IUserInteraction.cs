using AsyncRestApi.Model;

namespace AsyncRestApi.UserInteraction
{
    internal interface IUserInteraction
    {
        public void DisplayText(string text);
		string GetValidString();
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
	}
}
