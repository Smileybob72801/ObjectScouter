using ObjectScouter.Model;

namespace ObjectScouter.Services
{
	internal interface IItemService
	{
		IEnumerable<Item>? Items { get; set; }
		HashSet<string>? PropertyNames { get; set; }
		string RequestUri { get; }

		void FindPropertiesByValue(string target);
		Task<IEnumerable<Item>> GetAllObjects();
		HashSet<string> GetAllProperties();
		string?[] GetValuesOfAllMatchingProperties(string targetName);
	}
}