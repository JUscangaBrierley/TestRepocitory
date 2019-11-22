using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brierley.ClientDevUtilities.Data
{
    public static class PetaPocoExtensions
    {
        public static IDbConnection GetConnection(this PetaPoco.Database database)
        {
            if (database.Connection == null || database.Connection.State == ConnectionState.Closed || database.Connection.State == ConnectionState.Broken)
            {
                database.OpenSharedConnection();
            }

            return database.Connection;
        }

        public static IDbConnection GetConnection(this LWGateway.PetaPocoGateway.IDatabase database)
        {
            if (database.Connection == null || database.Connection.State == ConnectionState.Closed || database.Connection.State == ConnectionState.Broken)
            {
                database.OpenSharedConnection();
            }

            return database.Connection;
        }
    }
}
