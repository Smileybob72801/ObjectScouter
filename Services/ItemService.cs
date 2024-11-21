using ObjectScouter.Model;
using ObjectScouter.UserInteraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectScouter.Services
{
	internal class ItemService(IUserInteraction userInteraction) : IItemService
	{
		public IEnumerable<Item>? Items { get; set; }

		private readonly IUserInteraction _userInteraction = userInteraction;

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

		public void FindPropertiesByValue(string target)
		{
			if (Items is null)
			{
				throw new InvalidOperationException(ItemsNullMessage);
			}

			foreach (Item item in Items)
			{
				IEnumerable<KeyValuePair<string, object>> properties = item.GetNonNullProperties();

				foreach (KeyValuePair<string, object> property in properties)
				{
					if (string.Equals(target, property.Value.ToString(), StringComparison.OrdinalIgnoreCase))
					{
						_userInteraction.DisplayText(
							$"{item.Name} has matching {property.Key}: {property.Value}{Environment.NewLine}");
					}
				}
			}
		}
	}
}
