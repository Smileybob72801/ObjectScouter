using ObjectScouter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectScouter.Services
{
	internal class ItemService(List<Item> items) : IItemService
	{
		private readonly List<Item> _items = items;

		public string?[] GetValuesOfAllMatchingProperties(string targetName)
		{
			return _items.SelectMany(item => item.GetNonNullProperties())
				.Where(property => string.Equals(targetName, property.Key, StringComparison.OrdinalIgnoreCase))
				.Select(property => property.Value?.ToString())
				.ToArray();
		}
	}
}
