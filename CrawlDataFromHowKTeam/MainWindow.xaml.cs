using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
			TreeMain.Dispatcher.Invoke(new Action(() =>
			{
				root.Add(node);
			}));
			
		}

		private string CrawlDataFromUrl(string url)
		{
			var html = string.Empty;
			html = WebUtility.HtmlDecode( httpClient.GetStringAsync(url).Result);
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
		#region silent web browser
		public static void SetSilent(WebBrowser browser, bool silent)
		{
			if (browser == null)
				throw new ArgumentNullException("browser");

			// get an IWebBrowser2 from the document
			IOleServiceProvider sp = browser.Document as IOleServiceProvider;
			if (sp != null)
			{
				Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
				Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

				object webBrowser;
				sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
				if (webBrowser != null)
				{
					webBrowser.GetType().InvokeMember("Silent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser, new object[] { silent });
				}
			}
		}

		[ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IOleServiceProvider
		{
			[PreserveSig]
			int QueryService([In] ref Guid guidService, [In] ref Guid riid, [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
		}
		#endregion
		private void Button_Click(object sender, RoutedEventArgs e)
		{
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Task task = new Task(() => { Crawl("learn"); });
			task.Start();
		}

		

		private void webBrowserMenu_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
		{
			SetSilent(webBrowserMenu, true);
		}

		private void btnNode_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var btn = sender as TextBlock;
			webBrowserMenu.Navigate(baseUrl + btn.Tag.ToString());
		}
	}
}