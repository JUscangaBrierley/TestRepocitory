using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.CampaignManagement;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
	public class StepIODao : DaoBase<StepIO>
	{
		public StepIODao(Database database, ServiceConfig config)
			: base(database, config)
		{
		}

		public StepIO Create(long inputStepId, long outputStepId)
		{
			StepIO io = new StepIO(inputStepId, outputStepId);
			Create(io);
			return io;
		}

		public StepIO Create(long inputStepId, long outputStepId, MergeType mergeType, int mergeOrder)
		{
			StepIO io = new StepIO(inputStepId, outputStepId);
			io.MergeType = mergeType;
			io.MergeOrder = mergeOrder;
			Create(io);
			return io;
		}

		public StepIO Retrieve(long inputStepId, long outputStepId)
		{
			return Database.SingleOrDefault<StepIO>("select * from LW_CLStepIOXref where InputStepId = @0 and OutputStepId = @1", inputStepId, outputStepId);
		}

		public List<StepIO> RetrieveInputs(long outputStepId)
		{
			return Database.Fetch<StepIO>("select * from LW_CLStepIOXref where OutputStepId = @0", outputStepId);
		}

		public List<StepIO> RetrieveOutputs(long inputStepId)
		{
			return Database.Fetch<StepIO>("select * from LW_CLStepIOXref where InputStepId = @0", inputStepId);
		}

		public void Delete(long inputStepId, long outputStepId)
		{
			Database.Execute("delete from LW_CLStepIOXref where InputStepId = @0 and OutputStepId = @1", inputStepId, outputStepId);
		}

        public override void Update(StepIO t)
        {
            Database.Execute("update LW_CLStepIOXref set MergeOrder = @0, MergeTypeId = @1 where InputStepId = @2 and OutputStepId = @3", t.MergeOrder, t.MergeType, t.InputStepId, t.OutputStepId);
        }
	}
}
