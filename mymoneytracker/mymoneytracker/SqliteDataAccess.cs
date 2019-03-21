using Dapper;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace mymoneytracker
{
    public class SqliteDataAccess
    {
        public static List<TransactionModel> LoadTransactions()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = conn.Query<TransactionModel>("select * from Transactions", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveTransaction(TransactionModel transaction)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute("insert into Transactions (Date, Payee, Amount, Custom_notes, Category, Category_override) values (@Date, @Payee, @Amount, @Custom_notes, @Category, @Category_override)", transaction);
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            string cs = ConfigurationManager.ConnectionStrings[id].ConnectionString;
            string dbPathRel = cs.Split('=', ';')[1].Substring(2);
            string dbPathAbs = Path.Combine(Directory.GetCurrentDirectory(), dbPathRel);
            Console.WriteLine($"I loaded DB {dbPathAbs} from relative path {dbPathRel} from connection string {cs}");
            return cs;
        }
    }
}
