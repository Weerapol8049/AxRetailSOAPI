using AxRetailSOAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace AxRetailSOAPI.Classes
{
    public interface IYearTemplate
    {
        string Username { get; set; }
        string Password { get; set; }
    }
    public interface IYearTemplate2
    {
        string Username { get; set; }
        string Password { get; set; }
    }
    public class Connection
    {
        
        public static SqlConnection con;
        public static SqlDataReader Execute(string sql)
        {
            string connStr = @"Data Source=AXSQL2; 
                                Initial Catalog=AX63_STM_Live; 
                                Persist Security Info=True; 
                                User ID=stmm;
                                Password=stmm@48624; 
                                Pooling=false;
                                Application Name=UpdateMRPTrack;";
            SqlConnection con = new SqlConnection(connStr);
            con.Open();
            SqlCommand cmd = new SqlCommand(sql, con);
            var reader = cmd.ExecuteReader();
            con.Close();

            return reader;
        }

        public static void Open()
        {
            string ConnectionString = @"Data Source=AXSQL2; 
                                Initial Catalog=AX63_STM_Live; 
                                Persist Security Info=True; 
                                User ID=stmm;
                                Password=stmm@48624; 
                                Pooling=false;
                                Application Name=UpdateMRPTrack;";
            con = new SqlConnection(ConnectionString);
            con.Open();
        }

        public static void Close()
        {
            con.Close();
        }

        public void ExecuteQueries(string Query_)
        {
            SqlCommand cmd = new SqlCommand(Query_, con);
            cmd.ExecuteNonQuery();
        }

        public static SqlDataReader DataReader(string Query_)
        {
            Open();
            SqlCommand cmd = new SqlCommand(Query_, con);
            SqlDataReader dr = cmd.ExecuteReader();
        
            return dr;
        }

        public static List<T> Login<T>(string sql) where T: IYearTemplate, IYearTemplate2, new()
        {
            List<T> list = new List<T>();
         
            var a = Assembly.GetExecutingAssembly().GetTypes().OfType<T>();
            var result = a.Select(x => x.GetType());
            Type type = typeof(T);

            string name = type.Name;
            //var field = (((FieldInfo[])((TypeInfo)type).DeclaredFields)[0]).Name;
            T model = new T() ;

            foreach (var item in ((FieldInfo[])((TypeInfo)type).DeclaredFields))
            {
                string fieldType = item.FieldType.Name;

                string field = item.Name.Replace("<", "").Replace(">k__BackingField", "");
                list.Add(new T()
                {
                    
                });
            }

            //foreach (Type t in a.GetTypes())
            //{
            //    if (t is T)
            //    {
            //        TypeList.Add(t);
            //    }
            //}
            //using (var reader = DataReader(sql))
            //{
            //    while (reader.Read())
            //    {
            //        user.Add(new User
            //        {
            //            Username = reader["STMUSERNAME"].ToString(),
            //            Password = reader["STMPASSWORD"].ToString(),
            //            Name = reader["STMNAME"].ToString(),
            //            Type = reader["STMSALESSTORETYPE"].ToString()
            //        });
            //    }
            //}
            return list;
        }
      
    }
}