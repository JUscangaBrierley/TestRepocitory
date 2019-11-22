using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Brierley.FrameWork.Cloud
{
	public interface ICloudStorageClient
	{
		void Initialize();
		void Initialize(string bucketName, string endpointName);

		CloudObjectCollection ListObjects(string path);

		string CreateFolder(string folderPath);

		string UploadFile(string cloudFilePath, string localFilePath, string contentType);

		string RenameFile(string oldFilePath, string newFilePath);

		string RenameFolder(string oldFolderPath, string newFolderPath);

		void DeleteFile(string filePath);

		void DeleteFolder(string folderPath);

        Stream GetFile(string path);
	}
}
