using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.ClientDevUtilities.MessageQueue;
using Brierley.FrameWork.Common;
using Brierley.FrameWork.Common.Logging;
using Brierley.LoyaltyWare.DataAcquisition.Core.Output;

namespace Brierley.ClientDevUtilities.OutputProvider
{
    public class MoveMessagesOutputProvider : IDAPOutputProvider
    {
        private const string CLASS_NAME = "MoveMessagesOutputProvider";

        private static LWLogger _logger = LWLoggerManager.GetLogger(LWConstants.LW_LWINTEGRATION_SERVICE);

        private long _jobNumber;
        private string _sourceEndpointUri;
        private bool _sourceIsPublic;
        private string _destinationEndpointUri;
        private bool _destinationIsPublic;

        public void Initialize(System.Collections.Specialized.NameValueCollection globals, System.Collections.Specialized.NameValueCollection args, long jobId, Brierley.LoyaltyWare.DataAcquisition.Core.DAPDirectives config, System.Collections.Specialized.NameValueCollection parameters, Brierley.LoyaltyWare.DataAcquisition.Core.PerformanceCounters.DAPPerformanceCounterUtil performUtil)
        {
            const string METHOD_NAME = "Initialize";

            _jobNumber = Convert.ToInt64(globals["JobNumber"]);

            _sourceEndpointUri = parameters["SourceEndpointUri"];
            _sourceIsPublic = Convert.ToBoolean(parameters["SourceIsPublic"]);
            _destinationEndpointUri = parameters["DestinationEndpointUri"];
            _destinationIsPublic = Convert.ToBoolean(parameters["DestinationIsPublic"]);

            _logger.Trace(CLASS_NAME, METHOD_NAME, "Output provider initialized.");
        }

        public void ProcessMessageBatch(List<string> messageBatch)
        {
            const string METHOD_NAME = "ProcessMessageBatch";

            _logger.Trace(CLASS_NAME, METHOD_NAME, "Message batch started.");

            MessageQueueUtility.MoveMessagesToDestinationQueue(_sourceEndpointUri, _sourceIsPublic, _destinationEndpointUri, _destinationIsPublic);
        }

        public int Shutdown()
        {
            const string METHOD_NAME = "Shutdown";

            _logger.Trace(CLASS_NAME, METHOD_NAME, "Output provider shutdown.");

            return 0;
        }

        public void Dispose()
        {

        }
    }
}
