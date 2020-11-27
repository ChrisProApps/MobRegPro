using System;
using Xamarin.Forms;
using System.Globalization;
using Xamarin.Forms.Xaml;
using MobRegPro.Resources;
using System.Resources;
using System.Reflection;

namespace MobRegPro
{
	[ContentProperty("Text")]
	public class TranslateExtension : IMarkupExtension
	{
		public string Text { get; set; }

		private CultureInfo ci;
		public TranslateExtension()
		{
			ci = new CultureInfo(App.appSettings.languageID);
		}

		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Text == null) return "";
			ResourceManager resManager = new ResourceManager("MobRegPro.Resources.AppResources",
															 typeof(TranslateExtension).GetTypeInfo().Assembly);
			var translation = resManager.GetString(Text, ci);
			if (translation == null) return Text;
			else return translation;
		}
	}
}
