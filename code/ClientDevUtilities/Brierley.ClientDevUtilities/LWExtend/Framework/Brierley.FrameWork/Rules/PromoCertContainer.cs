using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Brierley.FrameWork;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.FrameWork.Data;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.Rules
{
    public class PromoCertBucketInfo
    {
        public ContentObjType ObjectType;
        public string TypeCode;
        public int BucketSize;
    }

    class PromoCertBucket
    {
        public object LockObject = new object();
        public List<PromotionCertificate> Bucket = new List<PromotionCertificate>();
        public PromoCertBucketInfo Info;
    }

    public class PromoCertContainer : IDisposable
    {
        #region Fields
        private const string _className = "PromoCertContainer";
        private LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

        private bool _disposed = false;

        private Dictionary<string, PromoCertBucket> certBucketMap = new Dictionary<string, PromoCertBucket>();
        #endregion

        #region Private Methods
        private string GetBucketKey(ContentObjType objectType, string typeCode)
        {
            return objectType.ToString() + "::" + typeCode;
        }

        private void ReleaseUnusedCertificates()
        {
			using (var svc = LWDataServiceUtil.ContentServiceInstance())
			{
				foreach (PromoCertBucket bucket in certBucketMap.Values)
				{
					lock (bucket.LockObject)
					{
						if (bucket.Bucket.Count > 0)
						{
							// reclaim all in one go.
							var certsToReclaim = (from x in bucket.Bucket select x.CertNmbr);
							svc.ReclaimCertificates(certsToReclaim.ToArray<string>());
						}
					}
				}
			}
        }

        private void ReplenishCerts(PromoCertBucket certBucket)
        {
			using (var svc = LWDataServiceUtil.ContentServiceInstance())
			{
				List<PromotionCertificate> certs = svc.RetrieveAvailablePromoCertificate(certBucket.Info.ObjectType, certBucket.Info.TypeCode, DateTime.Now, DateTime.Now, certBucket.Info.BucketSize);
				if (certs != null)
				{
					foreach (PromotionCertificate cert in certs)
					{
						certBucket.Bucket.Add(cert);
					}
				}
			}
        }
        #endregion

        #region Construction/INitialization
        public PromoCertContainer(PromoCertBucketInfo[] bucketInfos)
        {
            if (bucketInfos != null && bucketInfos.Length > 0)
            {
                foreach (PromoCertBucketInfo bucketInfo in bucketInfos)
                {
                    string key = GetBucketKey(bucketInfo.ObjectType, bucketInfo.TypeCode);
                    if (!certBucketMap.ContainsKey(key))
                    {
                        certBucketMap.Add(key, new PromoCertBucket() { Info = bucketInfo });
                    }
                }
            }
        }
        #endregion

        #region Public Methods
        public string GetNextAvailableCert(ContentObjType objectType, string typeCode)
        {
            string certNmbr = string.Empty;
            string key = GetBucketKey(objectType, typeCode);
            PromoCertBucket certBucket = certBucketMap[key];
            if (certBucket != null)
            {
                lock (certBucket.LockObject)
                {
                    if (certBucket.Bucket.Count == 0)
                    {
                        ReplenishCerts(certBucket);
                    }
                    if (certBucket.Bucket.Count > 0)
                    {
                        certNmbr = certBucket.Bucket[0].CertNmbr;
                        certBucket.Bucket.RemoveAt(0);
                    }
                }
            }
            return certNmbr;
        }
        #endregion

        #region IDIsposable
        public void Dispose()
        {
            string methodName = "Dispose";
            _logger.Debug(_className, methodName, "Disposing DAP.");
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    ReleaseUnusedCertificates();
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
