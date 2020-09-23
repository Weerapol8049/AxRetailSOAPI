using AxRetailSOAPI.QueryService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AxRetailSOAPI.Classes
{
    public class ExecuteStaticQuery
    {
        public static DataSet Get(string tableQuery)
        {
            QueryServiceClient client = new QueryServiceClient();
            DataSet dataSet = new DataSet();
            Paging paging = null;
            try
            {
                client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                dataSet = client.ExecuteStaticQuery(tableQuery, ref paging);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return dataSet;
        }
    }
}