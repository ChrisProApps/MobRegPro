using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.IO;
using Windows.Storage;
using MobRegPro.Helpers;
using Xamarin.Forms;
using MobRegPro.WinPhone;

[assembly: Dependency(typeof(FileSystem_WinPhone))]
namespace MobRegPro.WinPhone
{
	public class FileSystem_WinPhone : IFileSystem
	{
		public byte[] ResizeImageWinPhone(byte[] imageData, float width, float height, float scale, float quality)
		{
			byte[] resizedData;


			using (MemoryStream streamIn = new MemoryStream(imageData))
			{
				WriteableBitmap bitmap = PictureDecoder.DecodeJpeg(streamIn, (int)width, (int)height);
				//
				float ZielHoehe = 0;
				float ZielBreite = 0;
				//
				float Hoehe = bitmap.PixelHeight;
				float Breite = bitmap.PixelWidth;

				//
				// Change to scale 1/8
				//
				if (height == 0.0 || width == 0.0) {
					height = (float)Hoehe / scale;
					width = (float)Breite / scale;
				}



				//
				if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
				{
					ZielHoehe = height;
					float teiler = Hoehe / height;
					ZielBreite = Breite / teiler;
				}
				else // Breite (61 für Avatar) ist Master
				{
					ZielBreite = width;
					float teiler = Breite / width;
					ZielHoehe = Hoehe / teiler;
				}
				//                
				using (MemoryStream streamOut = new MemoryStream())
				{
					bitmap.SaveJpeg(streamOut, (int)ZielBreite, (int)ZielHoehe, 0, 50); //50 => 100 is max quality
					resizedData = streamOut.ToArray();
				}
			}
			return resizedData;
		}

	}
