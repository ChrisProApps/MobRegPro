using System;
using System.IO;
using MobRegPro.Droid;
using Xamarin.Forms;
using System.Threading.Tasks;
using Android.Graphics;

[assembly: Dependency(typeof(FileSystem_Android))]
namespace MobRegPro.Droid
{
	public class FileSystem_Android : IFileSystem
	{
		PAResult lastResult;

		public string GetLocalFolder()
		{
			string documentsPath = string.Empty;
			lastResult = new PAResult ();
			try{
				documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal); // Documents folder
			}
			catch(Exception ex) {
				lastResult.status = ex.Message;
				lastResult.statusCode = 1;
			}
			return documentsPath;
		}


		public string GetDataPath(string fileName)
		{
			string documentsPath = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal); // Documents folder
			var path = System.IO.Path.Combine (documentsPath, fileName);
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
				string dir = System.IO.Path.GetDirectoryName(fileName);
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
			lastResult = new PAResult ();
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
				fs.Dispose();
			} catch (Exception ex) {
				lastResult.statusCode = 1;
				lastResult.status = ex.Message;
			}
			return lastResult;

		}

		// Image
		//
		public byte[] ResizeImage(byte[] imageData, float width, float height, float scale, float quality)
		{
			// Load the bitmap 
			Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
			//
			float ZielHoehe = 0;
			float ZielBreite = 0;
			//
			var Hoehe = originalImage.Height;
			var Breite = originalImage.Width;

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
			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)ZielBreite, (int)ZielHoehe, false);
			// 
			using (MemoryStream ms = new MemoryStream())
			{
				resizedImage.Compress(Bitmap.CompressFormat.Jpeg, (int)quality, ms); //2nd parameter is quality 110
				return ms.ToArray();
			}
		}

				
	}
}

