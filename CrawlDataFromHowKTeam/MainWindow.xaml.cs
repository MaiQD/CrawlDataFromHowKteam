using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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

		private void Crawl(string url)
		{
			string html = CrawlDataFromUrl(url);
			var list_Course = Regex.Matches(html, @"(?<=Học nhanh)([\s\S]*?)(?=(\s)*<div class=""text-warning font-size-sm"">)");
			foreach (var course in list_Course)
			{
				string strCourse = course.ToString();
				string CourseName = Regex.Match(strCourse, @"(?<=>)(.*?)(?=</h4>)").Value;
				string CouseUrl = Regex.Match(strCourse, @"(?<=href="")(.*?)(?="">)").Value;

				var item = new MenuTreeItem
				{
					Name = CourseName,
					URL = CouseUrl
				};
				AddItemIntoTreeViewItem(TreeItems, item);
			}
		}
		#endregion fucntion

		private void Button_Click(object sender, RoutedEventArgs e)
		{
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Crawl("learn");
			//TreeMain.Items
		}

		
	}
}