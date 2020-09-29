using AxRetailSOAPI.Classes;
using AxRetailSOAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AxRetailSOAPI.Controllers
{
    public class LoginController : ApiController
    {
        //[HttpGet]
        //[Route("api/retailso/login/{username}/{password}")]
        //public IHttpActionResult Login(string username, string password)
        //{
        //    try
        //    {
        //        DataTable tableUser = ExecuteStaticQuery.Get("STMSalesUser").Tables["StmSalesUser"];
        //        var getUser = (from a in tableUser.AsEnumerable()
        //                       where a.Field<string>("StmUserName") == username &&
        //                             a.Field<string>("StmPassword") == password
        //                       select new User {
        //                           Username = a.Field<string>("StmUserName"),
        //                           Password = a.Field<string>("StmPassword"),
        //                           Name = a.Field<string>("StmName"),
        //                           Type = a.Field<byte>("StmSalesStoreType").ToString()
        //                       }).ToList();

        //        if (username == "admin" && password == "_admin123")
        //        {
        //            return Json("ADMIN");
        //        }
        //        else if (getUser.Count == 0)
        //        {
        //            return Json("NOK");
        //        }
        //        else
        //            return Json(getUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(ex.Message);
        //    }
        //}
        [HttpGet]
        [Route("api/retailso/login/{username}/{password}")]
        public IHttpActionResult Login(string username, string password)
        {
            try
            {
                List<User> userList = new List<User>();

                if (username == "admin" && password == "_admin123")
                {
                    return Json("ADMIN");
                }
                else
                {
                    string sql = string.Format(@"SELECT [STMNAME]
                                                  ,[STMPASSWORD]
                                                  ,[STMSALESSTORETYPE]
                                                  ,[STMUSERNAME]
                                              FROM [dbo].[STMSALESUSER]
                                              WHERE STMUSERNAME = '{0}' AND STMPASSWORD = '{1}'", username, password);
                    string ConnectionString = @"Data Source=AXSQL2; 
                                Initial Catalog=AX63_STM_Live; 
                                Persist Security Info=True; 
                                User ID=stmm;
                                Password=stmm@48624; 
                                Pooling=false;
                                Application Name=UpdateMRPTrack;";
                    SqlConnection con = new SqlConnection(ConnectionString);
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sql, con);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userList.Add(new User
                            {
                                Username = reader["STMUSERNAME"].ToString(),
                                Password = reader["STMPASSWORD"].ToString(),
                                Name = reader["STMNAME"].ToString(),
                                Type = reader["STMSALESSTORETYPE"].ToString()
                            });
                        }
                        //Connection.Close();
                    }
                    

                    if (userList.Count == 0)
                    {
                        return Json("NOK");
                    }
                    else
                    {
                        return Json(userList);
                    }

                }            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
