using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Data.DomainModel;

namespace Brierley.FrameWork.JobScheduler
{
    public interface IExchangeRateProvider
    {
        IList<ExchangeRate> FetchExchangeRates();
    }
}
