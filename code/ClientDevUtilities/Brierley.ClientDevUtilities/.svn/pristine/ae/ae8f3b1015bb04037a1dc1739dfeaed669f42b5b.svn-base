using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Cloud
{
	public class AwsStorageClient : ICloudStorageClient
	{
		private const string _className = "AwsStorageClient";
		private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);
		private string _bucketName;
		private RegionEndpoint _endpoint;
		private AmazonS3Client _amazonS3Client;
		

		internal AwsStorageClient()
		{
		}

		#region ICloudStorageClient implementation
		public void Initialize()
		{
			string bucketName = StringUtils.FriendlyString(LWConfigurationUtil.GetConfigurationValue("CloudBucketName"), LWConfigurationUtil.GetCurrentEnvironmentContext().Organization);
			string endpointName = StringUtils.FriendlyString(LWConfigurationUtil.GetConfigurationValue("CloudEndpointName"), "USEast1");
			Initialize(bucketName, endpointName);
		}

		public void Initialize(string bucketName, string endpointName)
		{
			const string methodName = "Initialize";
			_bucketName = bucketName;
			_endpoint = ResolveEndpoint(endpointName);
			if (_endpoint == null)
			{
				_endpoint = RegionEndpoint.USEast1;
			}

			string accessKey = LWConfigurationUtil.GetConfigurationValue("AwsStorageAccessKey");
			string secretKey = LWConfigurationUtil.GetConfigurationValue("AwsStorageSecretKey");
			BasicAWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);

			_logger.Debug(_className, methodName, string.Format("Initializing Amazon S3 Client with bucket '{0}' on endpoint '{1}'", _bucketName, _endpoint));
			_amazonS3Client = new AmazonS3Client(credentials, _endpoint);
			
		}

		public CloudObjectCollection ListObjects(string path)
		{
			string relativePath = StringUtils.FriendlyString(path);
			ListObjectsRequest req = new ListObjectsRequest();
			req.BucketName = _bucketName;
			req.Delimiter = "/";
			if (!string.IsNullOrEmpty(path))
			{
				if (!relativePath.EndsWith("/"))
				{
					relativePath += "/";
				}
				req.Prefix = relativePath;
			}

			ListObjectsResponse resp = _amazonS3Client.ListObjects(req);

			CloudObjectCollection result = new CloudObjectCollection();
			if (resp.S3Objects.Count > 0)
			{
				foreach (var cloudObject in resp.S3Objects)
				{
					if (!string.IsNullOrEmpty(path) && cloudObject.Key == relativePath)
					{
						continue;
					}
					string fileName = cloudObject.Key;
					if (fileName.StartsWith(relativePath))
					{
						fileName = fileName.Substring(relativePath.Length);
					}
                    CloudFile cloudFile = new CloudFile()
                    {
                        Name = fileName,
                        Path = relativePath,
                        URL = GetUrl() + _bucketName + "/" + relativePath + fileName,
                        Size = cloudObject.Size
                    };
					result.Add(cloudFile);
				}
			}
			if (resp.CommonPrefixes.Count > 0)
			{
				foreach (var prefix in resp.CommonPrefixes)
				{
					string folderName = prefix;
					if (prefix.EndsWith("/"))
					{
						folderName = folderName.Substring(0, folderName.Length - 1);
					}
					if (folderName.StartsWith(relativePath))
					{
						folderName = folderName.Substring(relativePath.Length);
					}
                    CloudDirectory cloudDirectory = new CloudDirectory()
                    {
                        Name = folderName,
                        Path = relativePath,
                        URL = GetUrl() + _bucketName + "/" + relativePath + folderName,
                        Size = -1
                    };
					result.Add(cloudDirectory);
				}
			}
			return result;
		}

		public string CreateFolder(string folderPath)
		{
			if (!folderPath.EndsWith("/"))
			{
				folderPath += "/";
			}

			PutObjectRequest req = new PutObjectRequest();
			req.BucketName = _bucketName;
			req.Key = folderPath;
			req.ContentBody = string.Empty;
			req.CannedACL = S3CannedACL.PublicRead;

			PutObjectResponse resp = _amazonS3Client.PutObject(req);

			return resp.ETag;
		}

		public string UploadFile(string cloudFilePath, string localFilePath, string contentType)
		{
			PutObjectRequest req = new PutObjectRequest();
			req.BucketName = _bucketName;
			req.ContentType = contentType;
			req.FilePath = localFilePath;
			req.Key = cloudFilePath;
			req.CannedACL = S3CannedACL.PublicRead;

			PutObjectResponse resp = _amazonS3Client.PutObject(req);
			return resp.ETag;
		}

		public string RenameFile(string oldFilePath, string newFilePath)
		{
			if (oldFilePath == newFilePath)
			{
				throw new Exception("Cannot rename file to itself");
			}

			string result = DoRenameFile(oldFilePath, newFilePath);

			DeleteFile(oldFilePath);

			return result;
		}

		public string RenameFolder(string oldFolderPath, string newFolderPath)
		{
			if (oldFolderPath == newFolderPath)
			{
				throw new Exception("Cannot rename folder to itself");
			}

			ListObjectsRequest req = new ListObjectsRequest();
			req.BucketName = _bucketName;
			req.Delimiter = "/";
			req.Prefix = oldFolderPath + "/";

			ListObjectsResponse resp = _amazonS3Client.ListObjects(req);
			string result = null;
			if (resp.S3Objects.Count > 0)
			{
				// rename the folder - this deletes the old folder only if it is empty
				result = RenameFile(oldFolderPath + "/", newFolderPath + "/");

				// rename the objects below the folder
				foreach (var file in resp.S3Objects)
				{
					if (file.Key != (oldFolderPath + "/"))
					{
						string newKey = newFolderPath + file.Key.Substring(oldFolderPath.Length);
						RenameFile(file.Key, newKey);
					}
				}
			}

			// recursively rename subfolders
			if (resp.CommonPrefixes.Count > 0)
			{
				foreach (var commonPrefix in resp.CommonPrefixes)
				{
					string oldSubDir = commonPrefix.Substring(0, commonPrefix.Length - 1);
					string newSubDir = newFolderPath + oldSubDir.Substring(oldFolderPath.Length);
					RenameFolder(oldSubDir, newSubDir);
				}
			}
			return result;
		}

		public void DeleteFile(string filePath)
		{
			DeleteObjectRequest req = new DeleteObjectRequest();
			req.BucketName = _bucketName;
			req.Key = filePath;

			DeleteObjectResponse resp = _amazonS3Client.DeleteObject(req);
		}

		public void DeleteFolder(string folderPath)
		{
			if (!folderPath.EndsWith("/"))
			{
				folderPath += "/";
			}

			ListObjectsRequest req = new ListObjectsRequest();
			req.BucketName = _bucketName;
			req.Delimiter = "/";
			req.Prefix = folderPath;

			ListObjectsResponse resp = _amazonS3Client.ListObjects(req);
			if (resp.S3Objects.Count > 0)
			{
				// delete objects
				foreach (var file in resp.S3Objects)
				{
					DeleteFile(file.Key);
				}
			}

			// recursively delete subfolders
			if (resp.CommonPrefixes.Count > 0)
			{
				foreach (var commonPrefix in resp.CommonPrefixes)
				{
					DeleteFolder(commonPrefix);
				}
			}

			// delete the folder
			DeleteFile(folderPath);
		}

        public Stream GetFile(string path)
        {
            string url = GetUrl() + _bucketName + "/" + path;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                return resp.GetResponseStream();
            }
            catch
            {
                // request failed
                return null;
            }            
        }
		#endregion

		private string DoRenameFile(string oldFilePath, string newFilePath)
		{
			CopyObjectRequest req = new CopyObjectRequest();
			req.SourceBucket = _bucketName;
			req.DestinationBucket = _bucketName;
			req.SourceKey = oldFilePath;
			req.DestinationKey = newFilePath;

			// throws exception if error
			CopyObjectResponse resp = _amazonS3Client.CopyObject(req);

			return resp.ETag;
		}

		private RegionEndpoint ResolveEndpoint(string endpointName)
		{
			RegionEndpoint result = null;

            var fields = typeof(RegionEndpoint).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            result = (from a in fields where a.Name == endpointName select a.GetValue(null)).FirstOrDefault() as RegionEndpoint;

            if (result == null)
                result = RegionEndpoint.GetBySystemName(endpointName);

			return result;
		}

        private string GetUrl()
        {
            return string.Format("https://s3{0}.amazonaws.com/", _endpoint != RegionEndpoint.USEast1 ? "-" + _endpoint.SystemName : string.Empty);
        }
	}
}
