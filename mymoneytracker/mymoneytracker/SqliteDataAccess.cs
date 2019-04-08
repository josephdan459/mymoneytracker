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
        private static string getStartingBalanceQuery = "select Setting_value from Configuration where Setting_name = 'starting_balance'";
        private static string setStartingBalanceQuery = "replace into Configuration (Setting_name, Setting_value) values ('starting_balance', @sb)";
        private static string saveTransactionQuery = "insert into Transactions (Date, Payee, Amount, Custom_notes, Category) values (@Date, @Payee, @Amount, @Custom_notes, @Category)";
        private static string saveRuleQuery = "insert into Rules (Rule_name, Payee_regex, Direction, Category) values (@Rule_name, @Payee_regex, @Direction, @Category)";
        private static string deleteTransactionByIdQuery = "delete from Transactions where Id = @id";
        private static string deleteRuleByNameQuery = "delete from Rules where Rule_name = @name";

        public static List<TransactionModel> LoadTransactions()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = conn.Query<TransactionModel>(loadTransactionsQuery, new DynamicParameters());
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

        public static Decimal GetStartingBalance()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = conn.Query<Decimal>(getStartingBalanceQuery, new DynamicParameters());

                if (output.Count() <= 0 )
                {
                    return Decimal.MinValue;
                }
                return output.Single();
            }
        }

        public static void SetStartingBalance(Decimal sb)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(setStartingBalanceQuery, new { sb });
            }
        }


        public static void SaveTransaction(TransactionModel transaction)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(saveTransactionQuery, transaction);
            }
        }

        public static void SaveRule(RuleModel rule)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(saveRuleQuery, rule);
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
