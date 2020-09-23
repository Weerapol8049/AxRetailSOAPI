using AxRetailSOAPI.Classes;
using AxRetailSOAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AxRetailSOAPI.Controllers
{
    public class LoginController : ApiController
    {
        [HttpGet]
        [Route("api/retailso/login/{username}/{password}")]
        public IHttpActionResult Login(string username, string password)
        {
            try
            {
                DataTable tableUser = ExecuteStaticQuery.Get("STMSalesUser").Tables["StmSalesUser"];
                var getUser = (from a in tableUser.AsEnumerable()
                               where a.Field<string>("StmUserName") == username &&
                                     a.Field<string>("StmPassword") == password
                               select new User {
                                   Username = a.Field<string>("StmUserName"),
                                   Password = a.Field<string>("StmPassword"),
                                   Name = a.Field<string>("StmName"),
                                   Type = a.Field<byte>("StmSalesStoreType").ToString()
                               }).ToList();
               
                if (username == "admin" && password == "_admin123")
                {
                    return Json("ADMIN");
                }
                else if (getUser.Count == 0)
                {
                    return Json("NOK");
                }
                else
                    return Json(getUser);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}
