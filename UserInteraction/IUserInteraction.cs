﻿using ObjectScouter.Model;
using System.Reflection;

namespace ObjectScouter.UserInteraction
{
	internal interface IUserInteraction
    {
        void DisplayText(string text);
		string GetValidString(string prompt = "");
		void ListStrings(string?[] strings);
		void ListItems(IEnumerable<Item> items);
		void WaitForAnyInput();
		bool GetYesOrNo(string prompt, string invalidResponse);
	}
}
