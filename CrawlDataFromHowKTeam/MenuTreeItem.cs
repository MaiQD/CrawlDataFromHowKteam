using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlDataFromHowKTeam
{
	public class MenuTreeItem
	{
		public string Name { get; set; }
		public string URL { get; set; }
		public ObservableCollection<MenuTreeItem> items { get; set; }
		public MenuTreeItem()
		{
			this.items = new ObservableCollection<MenuTreeItem>();
		}
	}
}
