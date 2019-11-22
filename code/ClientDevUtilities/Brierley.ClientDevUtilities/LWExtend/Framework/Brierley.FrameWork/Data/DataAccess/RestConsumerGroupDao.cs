using Brierley.FrameWork.Data.DomainModel;
using PetaPoco;

namespace Brierley.FrameWork.Data.DataAccess
{
    /// <summary>
    /// DAO class for RestConsumerGroup, for exposing inherited Create, Update, Delete.
    /// </summary>
    public class RestConsumerGroupDao : RestDaoBase<RestConsumerGroup>
    {
        public RestConsumerGroupDao(Database database, ServiceConfig config)
            : base(database, config)
        {
        }
    }
}
