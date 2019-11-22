using System.Collections.Generic;
using Brierley.FrameWork.CampaignManagement.DomainModel;
using Brierley.FrameWork.Data;
using System;


namespace Brierley.FrameWork.CampaignManagement
{
	public class StepCollection : List<Step>
	{
        private long _campaignID;
		
		public StepCollection(long CampaignID)
        {
            _campaignID = CampaignID;
            using (CampaignManager manager = LWDataServiceUtil.CampaignManagerInstance())
            {
                List<Step> steps = manager.GetStepsByCampaignID(CampaignID);
                foreach (Step step in steps)
                {
                    this.Add(step);
                }
            }
        }
		
        public long Add(string StepName, string StepDescription, StepType StepType)
        {
			Step step = new Step();
			step.CampaignId = _campaignID;
			step.StepType = StepType;
			step.UIName = StepName;
			step.UIDescription = StepDescription;
			using(var mgr = LWDataServiceUtil.CampaignManagerInstance())
			{
				mgr.CreateStep(step);
			}
			this.Add(step);
			return step.Id;
        }
		
		public void Update(int StepID, string StepName, string StepDescription, int UIPositionX, int UIPositionY, int LastRecordCount)
		{
			Step step = this[StepID];
			if (step != null)
			{
				step.UIName = StepName;
				step.UIDescription = StepDescription;
				step.UIPositionX = UIPositionX;
				step.UIPositionY = UIPositionY;
				step.UILastRecordCount = LastRecordCount;
				using (var mgr = LWDataServiceUtil.CampaignManagerInstance())
				{
					mgr.UpdateStep(step);
				}
			}
		}
		
		public new bool Remove(Step Item)
		{
			using (var mgr = LWDataServiceUtil.CampaignManagerInstance())
			{
				mgr.DeleteStep(Item.Id, true, false);
			}
			return base.Remove(Item);
		}
		
		public bool Remove(int StepID)
		{
			Step step = this[StepID];
			if (step != null)
			{
				return Remove(step);
			}
			else
			{
				return false;
			}
		}
		
		public Step this[long StepID]
        {
            get
            {
				if(this.Count > 0)
				{
					foreach(Step step in this)
					{
						if(step.Id == StepID)
						{
							return step;
						}
					}
				}
				return null;
            }
        }
	}
}
