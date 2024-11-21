using ObjectScouter.Helpers;
using ObjectScouter.Model;
using ObjectScouter.Repositories;
using ObjectScouter.UserInteraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectScouter.Services
{
	internal class ItemService(IUserInteraction userInteraction,
		IApiService apiService) : IItemService
	{
		public IEnumerable<Item>? Items { get; set; }

		public HashSet<string>? PropertyNames { get; set; }
		public string RequestUri { get => requestUri; }

		private readonly IUserInteraction _userInteraction = userInteraction;

		private readonly IApiService _apiService = apiService;

		private readonly string requestUri = "/objects";

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
				.Distinct(new StringComparerIgnoreCase())
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

		public HashSet<string> GetAllProperties()
		{
			HashSet<string> result = new(new StringComparerIgnoreCase());

			if (Items is null)
			{
				throw new InvalidOperationException(ItemsNullMessage);
			}

			foreach (Item item in Items)
			{
				IEnumerable<KeyValuePair<string, object>> properties =
					item.GetNonNullProperties();

				foreach (var property in properties)
				{
					result.Add(property.Key);
				}
			}

			return result;
		}

		public async Task<IEnumerable<Item>> GetAllObjects()
		{
			// Just to simulate background work
			//await Task.Delay(4000);

			var result = await _apiService.ReadAsync<IEnumerable<Item>>(RequestUri);

			return result;
		}
	}
}
