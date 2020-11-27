using System;
using System.IO;
using System.Threading.Tasks;

namespace MobRegPro
{
	public interface IFileSystem
	{
		//File functions
		string GetLocalFolder ();
		string GetDataPath(string fileName);
		Task<PAResult> SaveStreamAsync(string pathName, Stream stream);
		Stream CreateFileStream (string pathName);
		Stream OpenFileStream (string pathName);
		PAResult DeleteFile (string fileName);
		PAResult DeleteFilesWithExtention (string fileName, string extention);
		PAResult ExistsFile (string fileName);
		PAResult GetLastResult();

		//Image Functions
		byte[] ResizeImage(byte[] imageData, float width, float height, float scale, float quality);

	}
}

