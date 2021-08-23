using System;
using Microsoft.Maui;
using Recipes.ViewModels;

namespace Recipes
{
	public class MyRecipesAdapter : IVirtualListViewAdapter
	{
		private ItemsViewModel _itemsViewModel;

		public MyRecipesAdapter(ItemsViewModel itemsViewModel)
		{
			_itemsViewModel = itemsViewModel;
		}

		public int Sections => _itemsViewModel.Items == null ? 0 : 1;

		public object Item(int sectionIndex, int itemIndex)
		{
			var item = _itemsViewModel.Items[itemIndex];
			return item ?? null;
		}

		public int ItemsForSection(int sectionIndex)
		{
			return _itemsViewModel.Items.Count;
		}

		public object Section(int sectionIndex)
		{
			throw new NotImplementedException();
		}
	}
}
