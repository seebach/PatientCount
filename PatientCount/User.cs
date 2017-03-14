using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace PatientCount
{
       // we need to check if this should be a singleton class
    public class User
    {
        public string _UserName;
        public int _IsUser;
        public int _IsAdmin;

        public string UserName  
        {
            get { return _UserName; }
            private set { _UserName = value; }
        }
        public int IsUser {
            get { return _IsUser;  } 
            private set { _IsUser = value; }
        }
        public int IsAdmin
        {
            get { return _IsAdmin; }
            private set { _IsAdmin = value; }
        }
        

        public User()
        {
           
             // get the user
            UserName = HttpContext.Current.User.Identity.Name.ToString();
            string dbConnection = Properties.Settings.Default.dbConnection;
            
                using (SqlConnection conn = new SqlConnection(dbConnection))
                {
                try
                {

                    string sql = @"SELECT top 1 id as UserID,UserName,IsUser,IsAdmin from[PCM].[dbo].PCMUsers Where Upper(UserName) = Upper('@UserName') AND IsUser = 1";

                    using (SqlCommand comm = new SqlCommand(sql, conn))
                    {
                        conn.Open();

                        comm.Parameters.AddWithValue("@UserName", UserName);

                        using (var reader = comm.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                IsUser = Convert.ToInt32(reader.GetBoolean(2));
                                IsAdmin = Convert.ToInt32(reader.GetBoolean(3));
                                Console.WriteLine("username" + reader.GetOrdinal("UserName"));
                            }
                           
                        }
                        }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("sql problem: "+ex.Message);
                }
            }


            }
        
    }
}
