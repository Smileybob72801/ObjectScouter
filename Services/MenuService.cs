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
	internal class MenuService(
		IUserInteraction userInteraction,
		IApiService apiService,
		IItemRepository itemRepository,
		IItemService itemService) : IMenuService
	{
		private readonly IUserInteraction _userInteraction = userInteraction;
		private readonly IApiService _apiService = apiService;
		private readonly IItemRepository _itemRepository = itemRepository;
		private readonly IItemService _itemService = itemService;

		public async Task HandleCreateItemAsync()
		{
			Item itemToAdd = new()
			{
				Name = _userInteraction.GetValidString(
				$"Enter the name for the new item:{Environment.NewLine}")
			};

			bool finishedAddingProperties;
			do
			{
				string nameOfPropertyToAdd = _userInteraction.GetValidString(
					$"Enter new property name:{Environment.NewLine}");

				string valueOfPropertyToAdd = _userInteraction.GetValidString(
					$"Enter value for {nameOfPropertyToAdd}:{Environment.NewLine}");

				itemToAdd.Data[nameOfPropertyToAdd] = valueOfPropertyToAdd;

				finishedAddingProperties = _userInteraction.GetYesOrNo(
					$"Finished adding properties? Y or N?{Environment.NewLine}",
					"Invalid response.");
			}
			while (!finishedAddingProperties);			

			Task? updateData = Task.Run(async () =>
			{
				await _apiService.PostAsync(_itemService.RequestUri, itemToAdd);
				_itemService.Items = await _itemService.GetAllObjects();
				_itemService.PropertyNames = _itemService.GetAllProperties();
			});
		}

		public async Task HandleSearchAsync()
		{
			if (_itemService.PropertyNames is not null)
			{
				_userInteraction.ListStrings([.. _itemService.PropertyNames]);

				string targetName = _userInteraction.GetValidString(
					$"Enter a property to search for:{Environment.NewLine}");

				string?[] validPropertiesValues = _itemService.GetValuesOfAllMatchingProperties(targetName);

				_userInteraction.ListStrings(validPropertiesValues);

				string targetValue = _userInteraction.GetValidString(
					$"Enter a value to search all {targetName} properties for:{Environment.NewLine}");

				_itemService.FindPropertiesByValue(targetValue);
			}
			else
			{
				_userInteraction.DisplayText(
					$"No properties found to search for.{Environment.NewLine}");
			}
		}
		public async Task HandleDeleteItemAsync()
		{
			string userInput;
			string cancelCommand = "cancel";

			bool complete;
			do
			{
				userInput = _userInteraction.GetValidString(
				$"Enter ID of object to delete, or type 'Cancel':{Environment.NewLine}");

				if (string.Equals(userInput, cancelCommand, StringComparison.OrdinalIgnoreCase))
				{
					complete = true;
				}
				else
				{
					complete = await _itemRepository.RemoveId(userInput);

					if (!complete)
					{
						_userInteraction.DisplayText(
							$"Item ID not found. Please try again.{Environment.NewLine}");
					}
					else if (complete)
					{
						_userInteraction.DisplayText(
							$"ID {userInput} removed.{Environment.NewLine}");
					}
				}
			}
			while (!complete);

			Task? deleteTask = Task.Run(async () =>
			{
				_itemService.Items = await _itemService.GetAllObjects();
				_itemService.PropertyNames = _itemService.GetAllProperties();
			});
		}

		public async Task HandleListItemsAsync()
		{
			if (_itemService.Items is not null)
			{
				_userInteraction.ListItems(_itemService.Items);
			}
			else
			{
				_userInteraction.DisplayText($"No items to display.{Environment.NewLine}");
			}
		}

		public async Task HandleExitAsync()
		{
			_userInteraction.DisplayText($"Press any key to close application...");
			_userInteraction.WaitForAnyInput();
		}
	}
}
