using System;
using Xamarin.Forms;
using System.Globalization;
using Xamarin.Forms.Xaml;

namespace MobRegPro
{
	[ContentProperty("Source")]
	public class ImageResourceExtention : IMarkupExtension
	{
		public string Source { get; set;}
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Source == null)
				return null;
			var imageSource = ImageSource.FromResource (Source);
			return imageSource;
		}
	}

//	public class ImageResourceConvertor : TypeConverter
//	{
//		
//		public override bool CanConvertFrom (Type sourceType)
//		{
//			return sourceType == typeof(string);
//		}
//
////		public object ConvertFrom (object o)
////		{
////			var imageSource = ImageSource.FromResource ((string)o);
////			return imageSource;
////		}
//
//		public override object ConvertFrom (CultureInfo culture, object value)
//		{
//			var imageSource = ImageSource.FromResource ((string)value);
//			return imageSource;
//		}
//
//	}



}

