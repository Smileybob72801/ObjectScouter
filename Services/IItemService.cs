using ObjectScouter.Model;

namespace ObjectScouter.Services
{
	internal interface IItemService
	{
		IEnumerable<Item>? Items { get; set; }

		string?[] GetValuesOfAllMatchingProperties(string targetName);
	}
}