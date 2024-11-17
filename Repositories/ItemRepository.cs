namespace ObjectScouter.Repositories
{
	internal interface IItemRepository
	{
		Task AddId(string id);
		string GetIdAtIndex(int index);
		IEnumerable<string> GetIds();
		Task LoadFromFileAsync();
		Task RemoveId(string id);
		Task SaveToFileAsync();
	}

	internal class ItemRepository : IItemRepository
	{
		private readonly string _filePath = "ids.txt";
		private readonly List<string> _ids;

		public ItemRepository()
		{
			_ids = [];
		}

		public async Task AddId(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("ID cannot be null or empty.", nameof(id));
			}

			_ids.Add(id);

			await SaveToFileAsync();

			Console.WriteLine($"ID {id} added to repository.");
		}

		public IEnumerable<string> GetIds()
		{
			return _ids.AsReadOnly();
		}

		public string GetIdAtIndex(int index)
		{
			return _ids[index];
		}

		public async Task RemoveId(string id)
		{
			if (_ids.Remove(id))
			{
				Console.WriteLine($"ID {id} removed.");
				await SaveToFileAsync();
			}
			else
			{
				Console.WriteLine($"ID {id} not found.");
			}
		}

		public async Task SaveToFileAsync()
		{
			await File.WriteAllLinesAsync(_filePath, _ids);
		}

		public async Task LoadFromFileAsync()
		{
			if (File.Exists(_filePath))
			{
				string[] lines = await File.ReadAllLinesAsync(_filePath);
				_ids.AddRange(lines);
			}
		}
	}
}
