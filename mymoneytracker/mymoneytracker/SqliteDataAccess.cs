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

        private static string loadTransactionsQuery = "select * from Transactions";
        private static string loadRulesQuery = "select * from Rules";
        private static string saveTransactionQuery = "insert into Transactions (Date, Payee, Amount, Custom_notes, Category) values (@Date, @Payee, @Amount, @Custom_notes, @Category)";
        private static string deleteTransactionByIdQuery = "delete from Transactions where Id = @id";
        private static string deleteRuleByNameQuery = "delete from Rules where Rule_name = @name";

        public static List<TransactionModel> LoadTransactions()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = conn.Query<TransactionModel>(loadTransactionsQuery, new DynamicParameters());

                // todo: fix precision loss for dollar/cent amount

                return output.ToList();
            }
        }

        public static List<RuleModel> LoadRules()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = conn.Query<RuleModel>(loadRulesQuery, new DynamicParameters());
                return output.ToList();
            }
        }


        public static void SaveTransaction(TransactionModel transaction)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(saveTransactionQuery, transaction);
            }
        }

        public static void DeleteTransactionById(int id)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(deleteTransactionByIdQuery, new { id });
            }
        }

        public static void DeleteRuleByName(string name)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(deleteRuleByNameQuery, new { name });
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
