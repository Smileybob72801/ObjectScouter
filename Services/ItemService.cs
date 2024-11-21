using ObjectScouter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectScouter.Services
{
	internal class ItemService : IItemService
	{
		public IEnumerable<Item>? Items { get; set; }

		const string ItemsNullMessage = "Items collection is null";
		public string?[] GetValuesOfAllMatchingProperties(string targetName)
		{

			if (Items is null)
			{
				throw new InvalidOperationException(ItemsNullMessage);
			}

			return Items.SelectMany(item => item.GetNonNullProperties())
				.Where(property => string.Equals(targetName, property.Key, StringComparison.OrdinalIgnoreCase))
				.Select(property => property.Value?.ToString())
				.ToArray();
		}
	}
}
