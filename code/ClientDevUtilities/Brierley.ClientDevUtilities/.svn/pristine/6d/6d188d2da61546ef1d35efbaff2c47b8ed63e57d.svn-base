using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Config;

namespace Brierley.FrameWork.Cloud
{
	public enum CloudProviderEnum { AmazonAws, MicrosoftAzure };

	public class CloudManager
	{
		public static ICloudStorageClient CloudStorageClient()
		{
			bool enableCloudStorage = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("EnableCloudStorage"), false);
			ICloudStorageClient result = null;
			if (enableCloudStorage)
			{
				string cloudProviderString = StringUtils.FriendlyString(LWConfigurationUtil.GetConfigurationValue("CloudProvider"), "AmazonAws");
				CloudProviderEnum cloudProvider = (CloudProviderEnum)Enum.Parse(typeof(CloudProviderEnum), cloudProviderString);
				result = CloudStorageClient(cloudProvider);
			}
			return result;
		}

		public static ICloudStorageClient CloudStorageClient(CloudProviderEnum cloudProvider)
		{
			bool enableCloudStorage = StringUtils.FriendlyBool(LWConfigurationUtil.GetConfigurationValue("EnableCloudStorage"), false);
			ICloudStorageClient result = null;
			if (enableCloudStorage)
			{
				switch (cloudProvider)
				{
					case CloudProviderEnum.AmazonAws:
						result = new AwsStorageClient();
						break;

					case CloudProviderEnum.MicrosoftAzure:
						throw new Exception("Microsoft Azure storage functionality has not yet been implemented");
				}
			}
			return result;
		}
	}
}
