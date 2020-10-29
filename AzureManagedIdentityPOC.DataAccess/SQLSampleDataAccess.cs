using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureManagedIdentityPOCAPI.DataAccess
{
    public class SQLSampleDataAccess
    {
        public string GetSampleData(int id)
        {
            string name = string.Empty;

            //Making Connection to SQL through system-assigned Managed Identity
            using (SqlConnection connection = new SqlConnection("Server=tcp:pocdbca.database.windows.net,1433;Database=POCDB;;UID=AnyString;Authentication=Active Directory Interactive"))
            {
                //Coomand with Query to be executed on SQL
                SqlCommand command = new SqlCommand(
                  "SELECT ID, Name FROM Resources where ID=1;",
                  connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        name = reader.GetString(1);
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }
            return name;
        }

        public string GetSampleDataEntity(int id)
        {
            using (POCDBEntities2 db = new POCDBEntities2())
            {
                string res= db.Resources.Where((x) => x.ID == id).Select(x=>x.Name).FirstOrDefault<string>();
                return res;
            }
        }
    }
}
