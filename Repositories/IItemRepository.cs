namespace ObjectScouter.Repositories
{
	internal interface IItemRepository
	{
		Task AddId(string id);
		string GetIdAtIndex(int index);
		IEnumerable<string> GetIds();
		Task LoadFromFileAsync();
		Task<bool> RemoveId(string id);
		Task SaveToFileAsync();
	}
}
