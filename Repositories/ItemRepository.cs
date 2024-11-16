namespace ObjectScouter.Repositories
{
	internal interface IItemRepository
	{
		void AddId(string id);
		string GetIdAtIndex(int index);
		IEnumerable<string> GetIds();
		void RemoveId(string id);
	}

	internal class ItemRepository : IItemRepository
	{
		private readonly List<string> _ids;

		public ItemRepository()
		{
			_ids = [];
		}

		public void AddId(string id)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw new ArgumentException("ID cannot be null or empty.", nameof(id));
			}

			_ids.Add(id);

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

		public void RemoveId(string id)
		{
			if (_ids.Remove(id))
			{
				Console.WriteLine($"ID {id} removed.");
			}
			else
			{
				Console.WriteLine($"ID {id} not found.");
			}
		}
	}
}
