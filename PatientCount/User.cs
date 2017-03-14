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
        
        
        public User(string _UserName)
        {
            UserName = _UserName;
            // set default in case user is not found
            IsUser = -1;
            IsAdmin = -1;

            using (SqlConnection conn = new SqlConnection(Properties.Settings.Default.dbConnection))
            {
                conn.Open();

                string sql = @"SELECT top 1 Convert(int,IsUser) as IsUser,Convert(int,IsAdmin) as IsAdmin from[PCM].[dbo].PCMUsers Where Upper(UserName) = Upper(@UserName) AND IsUser = 1";
                
                using (SqlCommand comm = new SqlCommand(sql, conn))
                {
                    comm.Parameters.AddWithValue("@UserName", UserName);

                    using (var reader =  comm.ExecuteReader())
                    {
                        // if no user exists keep default values
                        if (!reader.Read())
                            return;
                                //throw new Exception("Something is very wrong");
                            
                        int __IsAdmin = reader.GetOrdinal("IsAdmin");
                        int __IsUser = reader.GetOrdinal("IsUser");

                        this.IsAdmin = reader.GetInt32(__IsAdmin);
                       this.IsUser = reader.GetInt32(__IsUser);
                    }
                }
            }
                        
        }

    }
}
