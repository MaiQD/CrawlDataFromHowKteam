using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Windows;

namespace CrawlDataFromHowKTeam
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ObservableCollection<MenuTreeItem> TreeItems;
		private HttpClient httpClient;
		private HttpClientHandler HttpClientHandler;
		private CookieContainer cookieContainer = new CookieContainer();

		public MainWindow()
		{
			InitializeComponent();
			InitHttpClient();
			TreeItems = new ObservableCollection<MenuTreeItem>();
			MenuTreeItem item1 = new MenuTreeItem()
			{
				Name = "Item1",
				URL = "https://www.howkteam.com/Course/Lap-trinh-WPF-co-ban/TreeView-Binding-trong-Lap-trinh-WPF-1354",
				items = new ObservableCollection<MenuTreeItem>()
				{
					new MenuTreeItem() { Name = "sub item1", URL="sadsda"},
					new MenuTreeItem() { Name = "sub item1", URL="sadsda"},
					new MenuTreeItem() { Name = "sub item1", URL="sadsda"},
					new MenuTreeItem() { Name = "sub item1", URL="sadsda"}
				}
			};
			MenuTreeItem item2 = new MenuTreeItem()
			{
				Name = "Item2",
				URL = "https://www.howkteam.com/Course/Lap-trinh-WPF-co-ban/TreeView-Binding-trong-Lap-trinh-WPF-1354",
				items = new ObservableCollection<MenuTreeItem>()
				{
					new MenuTreeItem() { Name = "sub item1", URL="sadsda"},
					new MenuTreeItem() { Name = "sub item1", URL="sadsda"},
					new MenuTreeItem() { Name = "sub item1", URL="sadsda"},
					new MenuTreeItem() { Name = "sub item1", URL="sadsda"}
				}
			};

			AddItemIntoTreeViewItem(TreeItems, item1);
			AddItemIntoTreeViewItem(TreeItems, item2);

			TreeMain.ItemsSource = TreeItems;
		}

		#region fucntion

		private void InitHttpClient()
		{
			this.HttpClientHandler = new HttpClientHandler
			{
				CookieContainer = cookieContainer,
				ClientCertificateOptions = ClientCertificateOption.Automatic,
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
				AllowAutoRedirect = true,
				UseDefaultCredentials = false
			};
			this.httpClient = new HttpClient(this.HttpClientHandler);
			this.httpClient.DefaultRequestHeaders.TryAddWithoutValidation("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36");

			/*
             * Header:
             * - Origin
             * - Host
             * - Referer
             * - :scheme
             * - accept
             * - Accept-Encoding
             * - Accept-Language
             * - User-Argent
             */
			this.httpClient.BaseAddress = new Uri("https://www.howkteam.com/");
		}

		private void AddItemIntoTreeViewItem(ObservableCollection<MenuTreeItem> root, MenuTreeItem node)
		{
			root.Add(node);
		}

		private string CrawlDataFromUrl(string url)
		{
			var html = string.Empty;
			html = httpClient.GetStringAsync(url).Result;
			return html;
		}

		#endregion fucntion

		private void Button_Click(object sender, RoutedEventArgs e)
		{
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			webBrowserMenu.Text = CrawlDataFromUrl("Course/Lap-trinh-WPF-co-ban/TreeView-Binding-trong-Lap-trinh-WPF-1354");
			//TreeMain.Items
		}
	}
}