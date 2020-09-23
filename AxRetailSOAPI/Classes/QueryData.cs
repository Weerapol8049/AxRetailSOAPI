using AxRetailSOAPI.QueryService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AxRetailSOAPI.Classes
{
    public class QueryData
    {
        public static DataSet Find(string queryName, string tableName, string fieldName, string value, string fieldName2 = "", string value2 = "")
        {
            DataSet ds = new DataSet();
            QueryServiceClient client = new QueryServiceClient();

            //Set up paging so that 1000 records are retrieved at a time

            Paging paging = new ValueBasedPaging() { RecordLimit = 1000 };
            QueryMetadata query;
            QueryDataSourceMetadata customerDataSource;
            QueryDataRangeMetadata range, range2;

            query = new QueryMetadata();
            // Set the properties of the query.
            //query.QueryType = QueryType.Join;
            query.DataSources = new QueryDataSourceMetadata[1];
            // Set the properties of the Customers data source.
            customerDataSource = new QueryDataSourceMetadata();
            customerDataSource.Name = queryName;
            customerDataSource.Enabled = true;
            customerDataSource.Table = tableName;
            //Add the data source to the query.
            query.DataSources[0] = customerDataSource;
            // Setting DynamicFieldList property to false so I can specify only a few fields
            customerDataSource.DynamicFieldList = true;

            range = new QueryDataRangeMetadata();
            range.TableName = tableName;
            range.FieldName = fieldName;
            range.Value = value;
            range.Enabled = true;

            range2 = new QueryDataRangeMetadata();
            range2.TableName = tableName;
            range2.FieldName = fieldName2;
            range2.Value = value2;
            range2.Enabled = true;

            customerDataSource.Ranges = new QueryDataRangeMetadata[fieldName2 == "" ? 1 : 2];
            customerDataSource.Ranges[0] = range;
            if (fieldName2 != "")
                customerDataSource.Ranges[1] = range2;

            client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
            client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
            client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
            
            ds = client.ExecuteQuery(query, ref paging);
            return ds;
        }
    }
}