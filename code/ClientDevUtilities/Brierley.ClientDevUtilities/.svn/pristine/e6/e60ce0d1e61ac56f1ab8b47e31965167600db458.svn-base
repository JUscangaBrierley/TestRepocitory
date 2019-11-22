using System.Collections.Generic;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.Data;

namespace Brierley.FrameWork.CampaignManagement
{
	public enum StepCollectionType
	{
		Input,
		Output
	}

	public class StepIOCollection : List<long>
	{
		private long _stepID = -1;
		private StepCollectionType _ioType;


		public StepIOCollection(long StepID, StepCollectionType IOType)
		{
			_stepID = StepID;
			_ioType = IOType;

            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                if (_ioType == StepCollectionType.Input)
                {
                    List<StepIO> io = manager.StepIODao.RetrieveInputs(_stepID);
                    foreach (StepIO input in io)
                    {
                        this.Add(input.InputStepId, input.MergeType, input.MergeOrder, false);
                    }
                }
                else
                {
                    List<StepIO> io = manager.StepIODao.RetrieveOutputs(_stepID);
                    foreach (StepIO output in io)
                    {
                        this.Add(output.OutputStepId, output.MergeType, output.MergeOrder, false);
                    }
                }
            }
		}


		public new void Add(long StepID)
		{
			Add(StepID, null, null, true);
		}


		public void Add(long StepID, MergeType MergeType, int MergeOrder)
		{
			Add(StepID, MergeType, MergeOrder, true);
		}


		public void Add(long StepID, MergeType? MergeType, int? MergeOrder, bool SaveToDatabase)
		{
			long inputStepID;
			long outputStepID;

			if (_ioType == StepCollectionType.Input)
			{
				inputStepID = StepID;
				outputStepID = _stepID;
			}
			else
			{
				inputStepID = _stepID;
				outputStepID = StepID;
			}

			if (SaveToDatabase)
			{
                using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                {
                    if (MergeType != null && MergeOrder != null)
                    {
                        manager.StepIODao.Create(inputStepID, outputStepID, (MergeType)MergeType, (int)MergeOrder);
                    }
                    else
                    {
                        manager.StepIODao.Create(inputStepID, outputStepID);
                    }
                }
			}
			base.Add(StepID);
		}


		public new bool Remove(long StepID)
		{
			long inputStepID;
			long outputStepID;

			if (_ioType == StepCollectionType.Input)
			{
				inputStepID = StepID;
				outputStepID = _stepID;
			}
			else
			{
				inputStepID = _stepID;
				outputStepID = StepID;
			}
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
                manager.StepIODao.Delete(inputStepID, outputStepID);
			return base.Remove(StepID);
		}


		public new void Clear()
		{
			while (this.Count > 0)
			{
				this.Remove(this[0]);
			}
		}

	}
}
