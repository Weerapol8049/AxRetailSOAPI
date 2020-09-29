using AxRetailSOAPI.AxRetailSOLine;
using AxRetailSOAPI.Classes;
using AxRetailSOAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AxRetailSOAPI.Controllers
{
    public class RetailSOLineController : ApiController
    {
        string ConnectionString = @"Data Source=AXSQL2; 
                                Initial Catalog=AX63_STM_Live; 
                                Persist Security Info=True; 
                                User ID=stmm;
                                Password=stmm@48624; 
                                Pooling=false;
                                Application Name=UpdateMRPTrack;";
        [HttpGet]
        [Route("api/retailsoline/loaddata/{recid}")]
        public IHttpActionResult LoadData(string recid)
        {
            try
            {
                List<RetailSOLine> list = new List<RetailSOLine>();
                string sql = string.Format(@"SELECT [RECID]
                                                  ,[MODEL]
                                                  ,[SALESAMOUNT]
                                                  ,[SALESDATE]
                                                  ,[SALESQTY]
                                                  ,[SINK]
                                                  ,[SERIES]
                                                  ,[STMSTOREID]
                                                  ,[SALESORDERDAILY]
                                              FROM [dbo].[STMSALESSODAILYLINE]
                                              WHERE SALESORDERDAILY = '{0}'", recid);
                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new RetailSOLine
                        {
                            RecId = reader["RECID"].ToString(),
                            Amount = Convert.ToDouble(reader["SALESAMOUNT"]),
                            Date = Convert.ToDateTime(reader["SALESDATE"]),
                            Model = reader["MODEL"].ToString(),
                            Qty = Convert.ToDouble(reader["SALESQTY"]),
                            Series = reader["SERIES"].ToString(),
                            Sink = reader["SINK"].ToString(),
                            Top = reader["STMSTOREID"].ToString()
                        });
                    }
                    //Connection.Close();
                }
               
                return Json(list);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [Route("api/retailsoline/series/{pool}")]
        [HttpGet]
        public IHttpActionResult GetSeries(string pool)
        {
            List<Series> list = new List<Series>();
            string sql = string.Format(@"SELECT DISTINCT [SERIES]
                                          FROM [dbo].[STMPRODUCTSERIES]
                                          WHERE SALESPOOLID = '{0}'
                                          ORDER BY SERIES", pool);

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(sql, con);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Series
                    {
                        ProdSeries = reader["SERIES"].ToString()
                    });
                }
               // Connection.Close();
            }
           
            return Json(list);
        }

        [Route("api/retailsoline/model/{series}")]
        [HttpGet]
        public IHttpActionResult GetModel(string series)
        {

            List<Series> list = new List<Series>();
            string sql = string.Format(@"SELECT DISTINCT [MODEL]
                                          FROM [dbo].[STMPRODUCTSERIES]
                                          WHERE SERIES = '{0}'
                                          ORDER BY MODEL", series);

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(sql, con);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Series
                    {
                        Model = reader["MODEL"].ToString()
                    });
                }
                //Connection.Close();
            }

            return Json(list);
        }

        [Route("api/retailsoline/create")]
        [HttpPost]
        public void CreateLine(RetailSOLine soDaily)
        {
            EntityKey[] entitykey = new EntityKey[1];
            if (!string.IsNullOrEmpty(soDaily.Series))
            {
                string orderDate = soDaily.Date.ToShortDateString();

                var callContextLine = new CallContext()
                {
                    MessageId = Guid.NewGuid().ToString(),
                    LogonAsUser = string.Format(@"{0}\{1}", UserAccount.Domain, UserAccount.Username),
                    Language = "en-us"
                };

                var lineEntity = new AxdEntity_StmSalesSoDailyLine();

                lineEntity.Series = soDaily.Series;
                lineEntity.Model = soDaily.Model;
                lineEntity.Sink = soDaily.Sink;

                if (soDaily.Date != DateTime.MinValue)
                {
                    lineEntity.SalesDate = DateTime.ParseExact(orderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    lineEntity.SalesDateSpecified = true;
                }

                //lineEntity.SalesDate = orderDate;
                //lineEntity.SalesDateSpecified = true;
                lineEntity.StmStoreId = soDaily.Top;//ใช้เก็บข้อมูล Top แทน
                //lineEntity.SalesPoolId = soDaily.Pool;
                lineEntity.SalesQty = Convert.ToDecimal(soDaily.Qty);
                lineEntity.SalesQtySpecified = true;
                lineEntity.SalesAmount = Convert.ToDecimal(soDaily.Amount);
                lineEntity.SalesAmountSpecified = true;
                lineEntity.SalesOrderDaily = Convert.ToInt64(soDaily.RecIdHeader);
                lineEntity.SalesOrderDailySpecified = true;

                var soLine = new AxdSTMSODailyLine()
                {
                    StmSalesSoDailyLine = new AxdEntity_StmSalesSoDailyLine[1] { lineEntity }
                };

                using (var client = new STMSODailyLineServiceClient())
                {
                    client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                    client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                    client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                    entitykey = client.create(callContextLine, soLine);
                }

                long refRecId = Convert.ToInt64(entitykey[0].KeyData[0].Value);
                RetailSOLog.Create(refRecId, soDaily.CreateBy);
            }
        }

        [Route("api/retailsoline/edit")]
        [HttpPost]
        public void EditLine(RetailSOLine soDaily)
        {
            EntityKey[] entitykey = new EntityKey[1];
            AxdSTMSODailyLine axdSoLine = new AxdSTMSODailyLine();
            var callContext = new CallContext()
            {
                MessageId = Guid.NewGuid().ToString(),
                LogonAsUser = string.Format(@"{0}\{1}", UserAccount.Domain, UserAccount.Username),
                Language = "en-us"
            };

            KeyField keyField = new KeyField() { Field = "RecId", Value = soDaily.RecId };
            EntityKey entityKey = new EntityKey();

            entityKey.KeyData = new KeyField[1] { keyField };

            EntityKey[] entityKeys = new EntityKey[1] { entityKey };

            using (var client = new STMSODailyLineServiceClient())
            {
                client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                axdSoLine = client.read(callContext, entityKeys);
            }

            string orderDate = soDaily.Date.ToShortDateString();

            using (var client = new STMSODailyLineServiceClient())
            {
                AxdEntity_StmSalesSoDailyLine soLineEntity = axdSoLine.StmSalesSoDailyLine[0];
                var axdsoline2 = new AxdSTMSODailyLine()
                {
                    ClearNilFieldsOnUpdate = axdSoLine.ClearNilFieldsOnUpdate,
                    ClearNilFieldsOnUpdateSpecified = true,
                    DocPurpose = axdSoLine.DocPurpose,
                    DocPurposeSpecified = true,
                    SenderId = axdSoLine.SenderId

                };

                var solineEntityNew = new AxdEntity_StmSalesSoDailyLine();
                solineEntityNew._DocumentHash = soLineEntity._DocumentHash; ///for update method
                solineEntityNew.RecId = Convert.ToInt64(soDaily.RecId);
                solineEntityNew.RecIdSpecified = true;

                if (soDaily.Date != DateTime.MinValue)
                {
                    solineEntityNew.SalesDate = DateTime.ParseExact(orderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else
                {
                    solineEntityNew.SalesDate = soLineEntity.SalesDate;
                }

                solineEntityNew.SalesDateSpecified = true;
                //solineEntityNew.SalesPoolId = soDaily.Pool;
                solineEntityNew.Series = soDaily.Series;
                solineEntityNew.Model = soDaily.Model;
                solineEntityNew.Sink = soDaily.Sink;
                solineEntityNew.SalesQty = Convert.ToDecimal(soDaily.Qty);
                solineEntityNew.SalesQtySpecified = true;
                solineEntityNew.StmStoreId = soDaily.Top;
                solineEntityNew.SalesAmount = Convert.ToDecimal(soDaily.Amount);
                solineEntityNew.SalesAmountSpecified = true;
                solineEntityNew.SalesOrderDaily = Convert.ToInt64(soDaily.RecIdHeader);
                solineEntityNew.SalesOrderDailySpecified = true;
                axdsoline2.StmSalesSoDailyLine = new AxdEntity_StmSalesSoDailyLine[1] { solineEntityNew };

                client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                client.update(callContext, entityKeys, axdsoline2);
            }

        }

        [Route("api/retailsoline/delete/{recid}")]
        [HttpPost]
        public void DeleteLine(string recid)
        {
            //{
            //    "RecId": "5637147579"
            //}

            var callContext = new CallContext()
            {
                MessageId = Guid.NewGuid().ToString(),
                LogonAsUser = string.Format(@"{0}\{1}", UserAccount.Domain, UserAccount.Username),
                Language = "en-us"
            };

            KeyField keyField = new KeyField() { Field = "RecId", Value = recid };
            EntityKey entityKey = new EntityKey();

            entityKey.KeyData = new KeyField[1] { keyField };

            EntityKey[] entityKeys = new EntityKey[1] { entityKey };

            using (STMSODailyLineServiceClient client = new STMSODailyLineServiceClient())
            {
                client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                client.delete(callContext, entityKeys);
            }
        }

    }
}
