﻿using AxRetailSOAPI.AxRetailSO;
using AxRetailSOAPI.Classes;
using AxRetailSOAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AxRetailSOAPI.Controllers
{
    public class RetailSOController : ApiController
    {

        //[HttpGet]
        //[Route("api/retailso/loaddata/{name}/{type}")]
        //public IHttpActionResult LoadData(string name, string type)
        //{
        //    string field = "";
        //    string storeName = "";
        //    List<RetailSO> soList = new List<RetailSO>();
        //    List<Stored> storeList = new List<Stored>();
        //    try
        //    {
        //        DataTable tableStore = ExecuteStaticQuery.Get("STMSalesStore").Tables["STMSalesStore"];

        //        switch (Convert.ToInt32(type))
        //        {
        //            case 0:
        //                field = "Admin"; 
        //                break;
        //            case 1:
        //                field = "SALES";
        //                break;
        //            case 2:
        //                field = "SALESMANAGER";
        //                break;
        //            case 3:
        //                field = "AREAMANAGER";
        //                break;
        //            case 4:
        //                field = "KEYACMANAGER";
        //                break;
        //        }



        //        if (field == "Admin")
        //        {
        //            storeList = (from a in tableStore.AsEnumerable()
        //                         orderby a.Field<string>("StmStoreId")
        //                         select new Stored
        //                         {
        //                             StoreId = a.Field<string>("StmStoreId"),
        //                             StoreName = a.Field<string>("StmStoreName")
        //                         }).ToList();
        //        }
        //        else
        //        {
        //            storeList = (from a in tableStore.AsEnumerable()
        //                         orderby a.Field<string>("StmStoreId")
        //                         where a.Field<string>(field) == name
        //                         select new Stored
        //                         {
        //                             StoreId = a.Field<string>("StmStoreId"),
        //                             StoreName = a.Field<string>("StmStoreName")
        //                         }).ToList();
        //        }

        //        string sql = string.Format(@"SELECT [STMSTOREID]
        //                                           ,[STMSTORENAME]
        //                                      FROM [dbo].[STMSALESSTORE]
        //                                      WHERE '{0}' = '{1}'", field, name);


        //        string storeId = "";
        //        foreach (var item in storeList)
        //        {
        //            storeId += item.StoreId + ",";
        //        }
        //        if (storeId != "")
        //            storeId = storeId.Remove(storeId.Length - 1, 1);

        //        DataTable dtData = QueryData.Find(
        //            "STMSalesDaily",
        //            "StmSalesSoDaily",
        //            "StmStoreId",
        //            storeId
        //            ).Tables["STMSalesDaily"];
        //        int i = 1;
        //        foreach (DataRow row in dtData.AsEnumerable().OrderByDescending(x => x.Field<DateTime>("SalesDate")))
        //        {
        //            DataSet dsimage = QueryData.Find(
        //                      "STMSalesImage",
        //                      "StmSalesImage",
        //                      "RefRecId",
        //                      row["RecId"].ToString()
        //                      );

        //            DataSet ds = QueryData.Find(
        //                   "STMSalesSODailyLine",
        //                   "StmSalesSoDailyLine",
        //                   "SalesOrderDaily",
        //                   row["RecId"].ToString()
        //                   );

        //            string[] sName = (from a in storeList.AsEnumerable() where a.StoreId == row["StmStoreId"].ToString() select a.StoreName).ToArray();

        //            soList.Add(new RetailSO
        //            {
        //                No = i++,
        //                RecId = row["RecId"].ToString(),
        //                StoreId = row["StmStoreId"].ToString(),
        //                StoreName = sName[0].ToString(),
        //                Pool = row["SalesPoolId"].ToString(),
        //                Qty = Convert.ToDouble(row["SalesQty"]),
        //                Amount = Convert.ToDouble(row["SalesAmount"]),
        //                Date = Convert.ToDateTime(row["SalesDate"]),
        //                DueDate = Convert.ToDateTime(row["DueDate"]),
        //                ConfirmDate = Convert.ToDateTime(row["ConfirmDate"]),
        //                PurchId = row["PurchId"].ToString(),
        //                SalesId = row["SalesId"].ToString(),
        //                CustName = row["SalesName"].ToString(),
        //                LineCount = ds.Tables["StmSalesSoDailyLine"].Rows.Count,
        //                ImageCount = dsimage.Tables["STMSalesImage"].Rows.Count
        //            });

        //            string sNamer = sName[0].ToString();

        //        }

        //        return Json(soList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message);
        //    }
        //}
        string ConnectionString = @"Data Source=AXSQL2; 
                                Initial Catalog=AX63_STM_Live; 
                                Persist Security Info=True; 
                                User ID=stmm;
                                Password=stmm@48624; 
                                Pooling=false;
                                Application Name=UpdateMRPTrack;";
        [HttpGet]
        [Route("api/retailso/loaddata/{name}/{type}")]
        public IHttpActionResult LoadData(string name, string type)
        {
            string field = "";
            string where = "";
            List<RetailSO> soList = new List<RetailSO>();
            List<Stored> storeList = new List<Stored>();
            try
            {
                switch (Convert.ToInt32(type))
                {
                    case 0:
                        field = "Admin";
                        break;
                    case 1:
                        field = "SALES";
                        break;
                    case 2:
                        field = "SALESMANAGER";
                        break;
                    case 3:
                        field = "AREAMANAGER";
                        break;
                    case 4:
                        field = "KEYACMANAGER";
                        break;
                }

                if (field != "Admin")
                     where = string.Format("WHERE {0} = '{1}'", field, name);

                string sql = string.Format(@"SELECT[SALESAMOUNT]
                                                  ,[SALESDATE]
                                                  ,[SALESPOOLID]
                                                  ,[SALESQTY]
                                                  ,so.[STMSTOREID]
                                                  ,store.STMSTORENAME
                                                  ,so.[RECID]
                                                  ,[CONFIRMDATE]
                                                  ,[DUEDATE]
                                                  ,[PURCHID]
                                                  ,[SALESID]
                                                  ,[SALESNAME]
	                                              ,(SELECT COUNT(RECID) FROM dbo.STMSALESIMAGE WHERE REFRECID = so.RECID) IMAGECOUNT
	                                              ,(SELECT COUNT(RECID) FROM dbo.STMSALESSODAILYLINE WHERE SALESORDERDAILY = so.RECID) LINECOUNT
                                              FROM[dbo].[STMSALESSODAILY] so
                                              LEFT JOIN[dbo].[STMSALESSTORE] store
                                              ON store.STMSTOREID = so.STMSTOREID
                                              {0}
                                              ORDER BY so.SALESDATE DESC", where);

                SqlConnection con = new SqlConnection(ConnectionString);
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);

                int i = 1;
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        soList.Add(new RetailSO
                        {
                            No = i++,
                            RecId = reader["RECID"].ToString(),
                            StoreId = reader["STMSTOREID"].ToString(),
                            StoreName = reader["STMSTORENAME"].ToString(),
                            Pool = reader["SALESPOOLID"].ToString(),
                            Qty = Convert.ToDouble(reader["SALESQTY"]),
                            Amount = Convert.ToDouble(reader["SALESAMOUNT"]),
                            Date = Convert.ToDateTime(reader["SALESDATE"]),
                            DueDate = Convert.ToDateTime(reader["DUEDATE"]),
                            ConfirmDate = Convert.ToDateTime(reader["CONFIRMDATE"]),
                            PurchId = reader["PURCHID"].ToString(),
                            SalesId = reader["SALESID"].ToString(),
                            CustName = reader["SALESNAME"].ToString(),
                            LineCount = Convert.ToInt32(reader["LINECOUNT"]),
                            ImageCount = Convert.ToInt32(reader["IMAGECOUNT"])
                        });
                    }
                    //Connection.Close();
                }

                return Json(soList);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }


        [Route("api/retailso/pool")]
        [HttpGet]
        public IHttpActionResult GetPool()
        {
            List<Pool> list = new List<Pool>();

            string sql = string.Format(@"SELECT [SALESPOOLID],[NAME] FROM [dbo].[SALESPOOL]");

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(sql, con);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Pool
                    {
                        PoolId = reader["SALESPOOLID"].ToString(),
                        Name = reader["NAME"].ToString()
                    });
                }
                //Connection.Close();
            }

            return Json(list);
        }

        [Route("api/retailso/stored/{name}/{type}")]
        [HttpGet]
        public IHttpActionResult GetStored(string name, string type)
        {
            string field = "", where = "";
            List<Stored> storeList = new List<Stored>();
 
            switch (Convert.ToInt32(type))
            {
                case 0:
                    field = "Admin";
                    break;
                case 1:
                    field = "SALES";
                    break;
                case 2:
                    field = "SALESMANAGER";
                    break;
                case 3:
                    field = "AREAMANAGER";
                    break;
                case 4:
                    field = "KEYACMANAGER";
                    break;
            }

            if (field != "Admin")
                where = string.Format("WHERE {0} = '{1}'", field, name);


            string sql = string.Format(@"SELECT [STMSTOREID]
                                              ,[STMSTORENAME]
                                          FROM [dbo].[STMSALESSTORE]
                                          {0}", where);
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            SqlCommand cmd = new SqlCommand(sql, con);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    storeList.Add(new Stored
                    {
                        StoreId = reader["STMSTOREID"].ToString(),
                        StoreName = reader["STMSTORENAME"].ToString()
                    });
                }
                //Connection.Close();
            }
            
            return Json(storeList);
        }

        [Route("api/retailso/create")]
        [HttpPost]
        public void Post(RetailSO parmSo)
        {
 
            #region Create sales daily 
            EntityKey[] entitykey = new EntityKey[1];

            var callContext = new CallContext()
            {
                MessageId = Guid.NewGuid().ToString(),
                LogonAsUser = string.Format(@"{0}\{1}", UserAccount.Domain, UserAccount.Username),
                Language = "en-us"
            };

            string duedate = parmSo.DueDate.ToShortDateString();
            string confirmdate = parmSo.ConfirmDate.ToShortDateString();
            string orderDate = parmSo.Date.ToShortDateString();

            AxdEntity_StmSalesSoDaily order = new AxdEntity_StmSalesSoDaily();
            if (parmSo.DueDate != DateTime.MinValue)
            {
                order.DueDate = DateTime.ParseExact(duedate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                order.DueDateSpecified = true;
            }
            if (parmSo.ConfirmDate != DateTime.MinValue)
            {
                order.ConfirmDate = DateTime.ParseExact(confirmdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                order.ConfirmDateSpecified = true;
            }
            if (parmSo.Date != DateTime.MinValue)
            {
                order.SalesDate = DateTime.ParseExact(orderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                order.SalesDateSpecified = true;
            }

            //order.SalesDate = DateTime.Now;
            //order.SalesDateSpecified = true;
            order.StmStoreId = parmSo.StoreId;
            order.SalesId = parmSo.SalesId;
            order.SalesName = parmSo.CustName;
            order.PurchId = parmSo.PurchId;

            order.SalesPoolId = parmSo.Pool;
            order.SalesQty = Convert.ToDecimal(parmSo.Qty);
            order.SalesQtySpecified = true;
            order.SalesAmount = Convert.ToDecimal(parmSo.Amount);
            order.SalesAmountSpecified = true;

            var orderList = new AxdSalesDaily()
            {
                StmSalesSoDaily = new AxdEntity_StmSalesSoDaily[1] { order }
            };

            using (var client = new SalesDailyServiceClient())
            {
                client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                entitykey = client.create(callContext, orderList);
            }

            long refRecId = Convert.ToInt64(entitykey[0].KeyData[0].Value);
            
            #region Create Log
            RetailSOLog.Create(refRecId, parmSo.CreateBy);
            #endregion

            #endregion
        }

        [Route("api/retailso/edit")]
        [HttpPost]
        public void Put(RetailSO parmSo)
        {
            /// response JSON
            /// 
            /// {
            ///    "RecId": "5637147579",
            ///    "Pool": "WARDROBE"
            /// }

            EntityKey[] entitykey = new EntityKey[1];
            AxdSalesDaily axdSoDaily = new AxdSalesDaily();
            try
            {
                var callContext = new CallContext()
                {
                    MessageId = Guid.NewGuid().ToString(),
                    LogonAsUser = string.Format(@"{0}\{1}", UserAccount.Domain, UserAccount.Username),
                    Language = "en-us"
                };

                using (var client = new SalesDailyServiceClient())
                {
                    client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                    client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                    client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                    axdSoDaily = client.read(callContext, Critera.read(parmSo.RecId));
                }

                string duedate = parmSo.DueDate.ToShortDateString();
                string confirmdate = parmSo.ConfirmDate.ToShortDateString();
                string orderDate = parmSo.Date.ToShortDateString();

                using (var client = new SalesDailyServiceClient())
                {
                    AxdEntity_StmSalesSoDaily soDailyEntity = axdSoDaily.StmSalesSoDaily[0];
                    var axdSODaily2 = new AxdSalesDaily()
                    {
                        ClearNilFieldsOnUpdate = axdSoDaily.ClearNilFieldsOnUpdate,
                        ClearNilFieldsOnUpdateSpecified = true,
                        DocPurpose = axdSoDaily.DocPurpose,
                        DocPurposeSpecified = true,
                        SenderId = axdSoDaily.SenderId

                    };


                    var soDailyEntityNew = new AxdEntity_StmSalesSoDaily();
                    soDailyEntityNew._DocumentHash = soDailyEntity._DocumentHash; ///for update method
                    soDailyEntityNew.RecId = Convert.ToInt64(parmSo.RecId);
                    soDailyEntityNew.RecIdSpecified = true;
                    if (duedate != "01/01/1900" && duedate != "01/01/0544")//(soDaily.DueDate != DateTime.MinValue)
                    {
                        soDailyEntityNew.DueDate = DateTime.ParseExact(duedate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        soDailyEntityNew.DueDateSpecified = true;
                    }

                    if (parmSo.ConfirmDate != DateTime.MinValue)
                    {
                        soDailyEntityNew.ConfirmDate = DateTime.ParseExact(confirmdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        soDailyEntityNew.ConfirmDateSpecified = true;
                    }

                    if (orderDate != "01/01/1900" && orderDate != "01/01/0544") //(soDaily.Date != DateTime.MinValue)
                    {
                        soDailyEntityNew.SalesDate = DateTime.ParseExact(orderDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        soDailyEntityNew.SalesDate = soDailyEntity.SalesDate;
                    }

                    soDailyEntityNew.SalesDateSpecified = true;
                    soDailyEntityNew.SalesPoolId = parmSo.Pool;// string.IsNullOrEmpty(soDaily.Pool) ? soDailyEntity.SalesPoolId : soDaily.Pool;
                    soDailyEntityNew.PurchId = parmSo.PurchId;
                    soDailyEntityNew.SalesId = parmSo.SalesId;
                    soDailyEntityNew.SalesName = parmSo.CustName;
                    soDailyEntityNew.SalesQty = Convert.ToDecimal(parmSo.Qty);// string.IsNullOrEmpty(soDaily.Qty.ToString()) ? Convert.ToDecimal(soDailyEntity.SalesQty) : Convert.ToDecimal(soDaily.Qty);
                    soDailyEntityNew.SalesQtySpecified = true;
                    soDailyEntityNew.StmStoreId = parmSo.StoreId;//string.IsNullOrEmpty(soDaily.StoreId) ? soDailyEntity.StmStoreId : soDaily.StoreId;
                    soDailyEntityNew.SalesAmount = Convert.ToDecimal(parmSo.Amount);// Convert.ToDecimal(soDaily.Amount) == soDailyEntity.SalesAmount ? Convert.ToDecimal(soDailyEntity.SalesAmount) : Convert.ToDecimal(soDaily.Amount);
                    soDailyEntityNew.SalesAmountSpecified = true;

                    axdSODaily2.StmSalesSoDaily = new AxdEntity_StmSalesSoDaily[1] { soDailyEntityNew };

                    client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                    client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                    client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                    client.update(callContext, Critera.read(parmSo.RecId), axdSODaily2);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Route("api/retailso/delete/{recid}")]
        [HttpPost]
        public void Delete(string recid)
        {
            var callContext = new CallContext()
            {
                MessageId = Guid.NewGuid().ToString(),
                LogonAsUser = string.Format(@"{0}\{1}", UserAccount.Domain, UserAccount.Username),
                Language = "en-us"
            };

            using (SalesDailyServiceClient client = new SalesDailyServiceClient())
            {
                client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                client.delete(callContext, Critera.read(recid));
            }

            #region Delete lines

            DataTable dt = QueryData.Find(
                   "STMSalesSODailyLine",
                   "StmSalesSoDailyLine",
                   "SalesOrderDaily",
                   recid
                   ).Tables["StmSalesSoDailyLine"];

            var callContextline = new AxRetailSOLine.CallContext()
            {
                MessageId = Guid.NewGuid().ToString(),
                LogonAsUser = string.Format(@"{0}\{1}", UserAccount.Domain, UserAccount.Username),
                Language = "en-us"
            };

            foreach (DataRow row in dt.Rows)
            {
                AxRetailSOLine.KeyField keyField = new AxRetailSOLine.KeyField() { Field = "RecId", Value = row["RecId"].ToString() };
                AxRetailSOLine.EntityKey entityKey = new AxRetailSOLine.EntityKey();

                entityKey.KeyData = new AxRetailSOLine.KeyField[1] { keyField };

                AxRetailSOLine.EntityKey[] entityKeys = new AxRetailSOLine.EntityKey[1] { entityKey };

                using (AxRetailSOLine.STMSODailyLineServiceClient client = new AxRetailSOLine.STMSODailyLineServiceClient())
                {
                    client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                    client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                    client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                    client.delete(callContextline, entityKeys);
                }
            }

            #endregion

            #region Delete Image

            DataTable dtImage = QueryData.Find(
                        "STMSalesImage",
                        "StmSalesImage",
                        "RefRecId",
                        recid
                        ).Tables[0];

            var callContextimage = new AxRetailSOImages.CallContext()
            {
                MessageId = Guid.NewGuid().ToString(),
                LogonAsUser = string.Format(@"{0}\{1}", UserAccount.Domain, UserAccount.Username),
                Language = "en-us"
            };

            foreach (DataRow row in dtImage.Rows)
            {
                AxRetailSOImages.KeyField keyFieldimage = new AxRetailSOImages.KeyField() { Field = "RecId", Value = row["RecId"].ToString() };
                AxRetailSOImages.EntityKey entityKeyimage = new AxRetailSOImages.EntityKey();

                entityKeyimage.KeyData = new AxRetailSOImages.KeyField[1] { keyFieldimage };

                AxRetailSOImages.EntityKey[] entityKeysimage = new AxRetailSOImages.EntityKey[1] { entityKeyimage };

                using (AxRetailSOImages.STM_SalesImageServiceClient client = new AxRetailSOImages.STM_SalesImageServiceClient())
                {
                    client.ClientCredentials.Windows.ClientCredential.Domain = UserAccount.Domain;
                    client.ClientCredentials.Windows.ClientCredential.UserName = UserAccount.Username;
                    client.ClientCredentials.Windows.ClientCredential.Password = UserAccount.Password;
                    client.delete(callContextimage, entityKeysimage);
                }

                #region Delete file image in server
                string path = string.Format(@"{0}{1}", row["Image"].ToString(), row["Name"].ToString());
                File.Delete(path);
                #endregion
            }

            #endregion
        }

    }
}
