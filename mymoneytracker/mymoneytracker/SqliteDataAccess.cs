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

        private static string loadTransactionsQuery = "select * from Transactions order by Date desc";
        private static string loadRulesQuery = "select * from Rules";
        private static string getStartingBalanceQuery = "select Setting_value from Configuration where Setting_name = 'starting_balance'";
        private static string setStartingBalanceQuery = "replace into Configuration (Setting_name, Setting_value) values ('starting_balance', @sb)";
        private static string saveTransactionQuery = "insert into Transactions (Date, Payee, Amount, Custom_notes, Category) values (@Date, @Payee, @Amount, @Custom_notes, @Category)";
        private static string updateTransactionQuery = "update Transactions set Date = @Date, Payee = @Payee, Amount = @Amount, Custom_notes = @Custom_notes, Category = @Category where Id = @id";
        private static string saveRuleQuery = "insert into Rules (Rule_name, Payee_regex, Direction, Category) values (@Rule_name, @Payee_regex, @Direction, @Category)";
        private static string updateRuleQuery = "update Rules set Rule_name = @Rule_name, Payee_regex = @Payee_regex, Direction = @Direction, Category = @Category where Id = @id";
        private static string deleteTransactionByIdQuery = "delete from Transactions where Id = @id";
        private static string deleteRuleByNameQuery = "delete from Rules where Rule_name = @name";


        // Return current transactions stored in DB
        // (Requirement 1.0.1)
        public static List<TransactionModel> LoadTransactions()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                    var output = conn.Query<TransactionModel>(loadTransactionsQuery, new DynamicParameters());
                    return output.ToList();
            }
        }

        // Return current rules stored in DB
        // (Requirement 1.0.1)
        public static List<RuleModel> LoadRules()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                    var output = conn.Query<RuleModel>(loadRulesQuery, new DynamicParameters());
                    return output.ToList();
            }
        }

        // Get starting balance of user's account
        public static Decimal GetStartingBalance()
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                    var output = conn.Query<Decimal>(getStartingBalanceQuery, new DynamicParameters());

                    if (output.Count() <= 0)
                    {
                        return Decimal.MinValue;
                    }
                    return output.Single();
            }
        }


        // Set or update starting balance of user's account
        public static void SetStartingBalance(Decimal sb)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(setStartingBalanceQuery, new { sb });
            }
        }

        // Add new transaction 
        // (Requirement 1.0.3)
        public static void SaveTransaction(TransactionModel transaction)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(saveTransactionQuery, transaction);
            }
        }

        // Edit transaction 
        // (Requirement 1.0.5)
        public static void UpdateTransaction(TransactionModel transaction)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(updateTransactionQuery, transaction);
            }
        }

        // Add new rule
        // (Requirement 1.0.4)
        public static void SaveRule(RuleModel rule)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(saveRuleQuery, rule);
            }
        }

        // Edit rule
        // (Requirement 1.0.6)
        public static void UpdateRule(RuleModel rule)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(updateRuleQuery, rule);
            }
        }

        // Delete rule by its internal ID
        public static void DeleteTransactionById(int id)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(deleteTransactionByIdQuery, new { id });
            }
        }

        // Delete rule by its name
        public static void DeleteRuleByName(string name)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                conn.Execute(deleteRuleByNameQuery, new { name });
            }
        }

        // Return connection string pointing to local DB in program folder
        // (Requirement 1.0.0)
        private static string LoadConnectionString(string id = "Default")
        {
            string cs = ConfigurationManager.ConnectionStrings[id].ConnectionString;
            return cs;
        }

    }
}
