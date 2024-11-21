using ObjectScouter.Model;

namespace ObjectScouter.Services
{
	internal interface IItemService
	{
		IEnumerable<Item>? Items { get; set; }

		void FindPropertiesByValue(string target);
		string?[] GetValuesOfAllMatchingProperties(string targetName);
	}
}