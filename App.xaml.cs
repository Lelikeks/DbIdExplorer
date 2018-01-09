using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace DbIdExplorer
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

			FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
				new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag))); 
			
			base.OnStartup(e);
		}
	}
}
