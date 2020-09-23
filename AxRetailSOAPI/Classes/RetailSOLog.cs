using AxRetailSOAPI.AxRetailSOLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AxRetailSOAPI.Classes
{
    public class RetailSOLog
    {
        public static void Create(long refRecId, string createBy)
        {
            var callContextLog = new CallContext()
            {
                MessageId = Guid.NewGuid().ToString(),
                LogonAsUser = string.Format(@"{0}\{1}", UserAccount.Domain, UserAccount.Username),
                Language = "en-us"
            };

            var log = new AxdEntity_StmSalesSoLog()
            {
                RefRecId = refRecId,
                RefRecIdSpecified = true,
                CreateBy = createBy
            };

            //var logList = new AxdAxdSTMSalesSolog() //UAT
            var logList = new AxdAxdSTMSalesSoLog()   //LIVE
            {
                StmSalesSoLog = new AxdEntity_StmSalesSoLog[1] { log }
            };

            //using (AxdSTMSalesSologServiceClient client = new AxdSTMSalesSologServiceClient()) //UAT
            using (AxdSTMSalesSoLogServiceClient client = new AxdSTMSalesSoLogServiceClient()) //LIVE
            {
                client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                client.create(callContextLog, logList);
            }
        }
    }
}