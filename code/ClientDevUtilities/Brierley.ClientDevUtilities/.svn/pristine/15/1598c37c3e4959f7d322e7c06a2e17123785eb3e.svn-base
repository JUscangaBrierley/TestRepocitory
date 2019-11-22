//-----------------------------------------------------------------------
//(C) 2008 Brierley & Partners.  All Rights Reserved
//THIS IS UNPUBLISHED PROPRIETARY SOURCE CODE OF Brierley & Partners.
//-----------------------------------------------------------------------

using System;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Exceptions;
using Brierley.FrameWork.Common.Logging;

namespace Brierley.FrameWork.Data
{
	public class LWQueryBatchInfo
	{
        private const string _className = "LWQueryBatchInfo";
        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_FRAMEWORK);

		public int StartIndex{get;set;}

		public int BatchSize{get;set;}

		public LWQueryBatchInfo()
		{
			StartIndex = 0;
			BatchSize = -1;
		}

		public LWQueryBatchInfo(int startIndex, int batchSize)
		{
			StartIndex = startIndex;
			BatchSize = batchSize;
		}

        public void Validate()
        {
            if (!IsValid(this))
            {
                throw new LWException("Invalid batch parameters specified.") { ErrorCode = 3227 };
            }
        }

        public static bool IsValid(LWQueryBatchInfo batchInfo)
        {
            return (batchInfo == null || batchInfo.StartIndex < 0 || batchInfo.BatchSize <= 0) ? false : true;
        }

        public static LWQueryBatchInfo GetValidBatch(int? startIndex, int? batchSize, bool enforceValidBatch)
        {
            string methodName = "GetValidBatch";

            LWQueryBatchInfo batchInfo = null;
            if (startIndex != null && batchSize != null)
            {
                batchInfo = new LWQueryBatchInfo() { StartIndex = startIndex.Value, BatchSize = batchSize.Value };
                if (enforceValidBatch)
                {
                    batchInfo.Validate();
                }
                else if (!LWQueryBatchInfo.IsValid(batchInfo))
                {
                    batchInfo = null;
                    _logger.Debug(_className, methodName, "Invalid batch parameters.  Ignoring batch.");
                }
            }

            return batchInfo;
        }

        public static long[] GetIds(long[] keys, int? startIndex, int? batchSize, bool enforceValidBatch)
        {
            string methodName = "GetIds";

            LWQueryBatchInfo batchInfo = GetValidBatch(startIndex, batchSize, enforceValidBatch);            
            int howMany = keys.Length;
            int available = keys.Length;
            if (LWQueryBatchInfo.IsValid(batchInfo))
            {
                available = keys.Length - batchInfo.StartIndex;
                howMany = available > batchInfo.BatchSize ? batchInfo.BatchSize : available;
            }
            if (available <= 0)
            {
                string msg = string.Format("Unable to get records starting at {0}.  There are only {1} records available.", startIndex.Value, keys.Length);
                _logger.Error(_className, methodName, msg);
                throw new LWException(msg) { ErrorCode = 3390 };
            }
            if (howMany > available)
            {
                howMany = available;
            }
            long[] ids = new long[howMany];
            int sidx = startIndex != null ? startIndex.Value : 0;
            Array.Copy(keys, sidx, ids, 0, howMany);

            return ids;
        }

        public static T[] GetBatchValues<T>(T[] values, int? startIndex, int? batchSize, bool enforceValidBatch)
        {
            string methodName = "GetBatchValues";

            LWQueryBatchInfo batchInfo = GetValidBatch(startIndex, batchSize, enforceValidBatch);
            int howMany = values.Length;
            int available = values.Length;
            if (LWQueryBatchInfo.IsValid(batchInfo))
            {
                available = values.Length - batchInfo.StartIndex;
                howMany = available > batchInfo.BatchSize ? batchInfo.BatchSize : available;
            }
            if (available <= 0)
            {
                string msg = string.Format("Unable to get records starting at {0}.  There are only {1} records available.", startIndex.Value, values.Length);
                _logger.Error(_className, methodName, msg);
                throw new LWException(msg) { ErrorCode = 3390 };
            }
            if (howMany > available)
            {
                howMany = available;
            }
            T[] ids = new T[howMany];
            int sidx = startIndex != null ? startIndex.Value : 0;
            Array.Copy(values, sidx, ids, 0, howMany);

            return ids;
        }
    }
}
