namespace ObjectScouter.Services
{
	internal interface IItemService
	{
		string?[] GetValuesOfAllMatchingProperties(string targetName);
	}
}