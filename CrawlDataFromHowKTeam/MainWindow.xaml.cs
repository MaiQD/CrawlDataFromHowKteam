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
		private string baseUrl = "https://www.howkteam.com/";

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
			this.httpClient.BaseAddress = new Uri(baseUrl);
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

				//thêm mới các node con
				CrawlListVideo(CouseUrl, item);
			}
		}

		private void CrawlListVideo(string url, MenuTreeItem prarentNode)
		{
			//lấy ra html course con
			var htmlCourse = CrawlDataFromUrl(url);
			//lấy ra link đưa đến trang danh sách các phần bài học
			var urlPart = Regex.Match(htmlCourse, @"(?<=<div class=""asyncPartial"" data-url="")(.*?)(?="")").Value;
			//html của danh sách phần học
			var htmlListCourse = CrawlDataFromUrl(urlPart);

			var listPart = Regex.Matches(htmlListCourse, @"list-group-item([\s\S]*?)</a>");
			foreach (var part in listPart)
			{
				var strPart = part.ToString();
				string partName = Regex.Match(strPart, @"(?<=</span>)([\s\S]*?)(?=(<))").Value.Trim();
				string partUrl = Regex.Match(strPart, @"(?<=href="")([\s\S]*?)(?="")").Value;
				var item = new MenuTreeItem
				{
					Name = partName,
					URL = partUrl
				};
				AddItemIntoTreeViewItem(prarentNode.items, item);
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

		private void btnNode_Click(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			webBrowserMenu.Navigate(baseUrl+ btn.Tag.ToString());
		}
	}
}