using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using MobRegPro.iOS;

using CoreGraphics;
using UIKit;
using System.Drawing;
using Foundation;

[assembly: Dependency(typeof(FileSystem_iOS))]
namespace MobRegPro.iOS
{
	public class FileSystem_iOS : IFileSystem
	{
		PAResult lastResult = null;


		public string GetLocalFolder()
		{
			string path = string.Empty;
			lastResult = new PAResult ();
			try{
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				path = Path.Combine (documentsPath, "..", "Library"); // Library folder
			}
			catch(Exception ex) {
				lastResult.status = ex.Message;
				lastResult.statusCode = 1;
			}
			return path;
		}

		public string GetDataPath(string fileName)
		{
			
			string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
			string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
			var path = Path.Combine (libraryPath, fileName);
			return path;
		}

		public PAResult ExistsFile(string fileName)
		{
			lastResult = new PAResult ();
			if (!File.Exists (fileName)) {
				lastResult.statusCode = 1;
				lastResult.status = $"File : {fileName} not found";
			}
			return lastResult;
		}

		public PAResult DeleteFile (string fileName)
		{
			lastResult = new PAResult ();
			try {
				File.Delete(fileName);
			}
			catch(Exception ex) {
				lastResult.status = ex.Message;
				lastResult.statusCode = 1;
			}
			return lastResult;
		}

		public PAResult DeleteFilesWithExtention (string fileName, string extention)
		{
			lastResult = new PAResult ();
			try {
				string dir = Path.GetDirectoryName(fileName);
				var files = Directory.EnumerateFiles(dir);
				foreach(string f in files)
				{
					if (f.Contains(extention))
						File.Delete(f);
				}
			}
			catch(Exception ex) {
				lastResult.status = ex.Message;
				lastResult.statusCode = 1;
			}
			return lastResult;
		}



		public Stream CreateFileStream(string pathName)
		{
			PAResult lastResult = new PAResult ();
			FileStream fs = null;
			try {
				//FileStream fs = File.Create (pathName);
				fs = new FileStream (pathName, FileMode.Create);
				lastResult.result = fs;
			} catch (Exception ex) {
				lastResult.statusCode = 1;
				lastResult.status = ex.Message;
			}
			return fs;
		}

		public Stream OpenFileStream(string pathName)
		{
			lastResult = new PAResult ();
			FileStream fs = null;

			try {
				//FileStream fs = File.Create (pathName);
				fs = new FileStream (pathName, FileMode.Open);
				lastResult.result = fs;
			} catch (Exception ex) {
				lastResult.statusCode = 1;
				lastResult.status = ex.Message;
			}
			return fs;
		}

		public PAResult GetLastResult()
		{
			return lastResult;
		}

		async public Task<PAResult> SaveStreamAsync(string pathName, Stream stream)
		{
			lastResult = new PAResult ();

			try {
				//FileStream fs = File.Create (pathName);
				FileStream fs = new FileStream (pathName, FileMode.Create);
				await stream.CopyToAsync(fs);
				await fs.FlushAsync ();
				fs.Close ();
			} catch (Exception ex) {
				lastResult.statusCode = 1;
				lastResult.status = ex.Message;
			}
			return lastResult;

		}


		// Image functions
		//
		public byte[] ResizeImage(byte[] imageData, float width, float height, float scale, float quality)
		{
			// Load the bitmap
			UIImage originalImage = new UIImage(NSData.FromArray(imageData));

			//UIImage originalImage =	UIImage.FromByteArray(imageData);
			//
			var Hoehe = originalImage.Size.Height;
			var Breite = originalImage.Size.Width;

			// Check if scaling requested
			//
			if (height == 0.0 || width == 0.0) {
				height = (float)Hoehe / scale;
				width = (float)Breite / scale;
			}


			//
			nfloat ZielHoehe = 0;
			nfloat ZielBreite = 0;
			//

			if (Hoehe > Breite) // Höhe (71 für Avatar) ist Master
			{
				ZielHoehe = height;
				nfloat teiler = Hoehe / height;
				ZielBreite = Breite / teiler;
			}
			else // Breite (61 for Avatar) ist Master
			{
				ZielBreite = width;
				nfloat teiler = Breite / width;
				ZielHoehe = Hoehe / teiler;
			}
			//
			width = (float)ZielBreite;
			height = (float)ZielHoehe;
			//
			UIGraphics.BeginImageContext(new SizeF(width, height));
			originalImage.Draw(new RectangleF(0, 0, width, height));
			var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			//
			var bytesImagen = resizedImage.AsJPEG(quality).ToArray(); //quality : 100 is max
			resizedImage.Dispose();
			return bytesImagen;
		}
	
	}
}

