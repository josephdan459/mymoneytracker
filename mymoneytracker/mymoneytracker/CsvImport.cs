using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mymoneytracker
{
    class CsvImport
    {
        public List<TransactionModel> it;
        public string status;
        public const string successStatus = "Transactions loaded successfully. Confirm preview below to save:";
        public enum DirectionBehavior
        {
            Inflow,
            Outflow,
            Both
        }

        public static CsvImport TransactionsFromCsv(string filePath, DateTime? currentDate, int dateCol, int amountCol, int payeeCol, DirectionBehavior db)
        {
            CsvImport ci = new CsvImport();
            ci.it = new List<TransactionModel>();
            ci.status = CsvImport.successStatus;

            // open csv file
            var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(filePath);
            parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
            parser.SetDelimiters(new string[] { "," });

            // for each row
            int line = 0;
            while (!parser.EndOfData)            {
                line++;
                string[] row = parser.ReadFields();
                
                // skip if not enough columns in this row
                if (row.Length - 1 < Math.Max(Math.Max(dateCol, amountCol), payeeCol))
                {
                    ci.it = null;
                    ci.status = $"Not enough columns ({row.Length}) for import on line {line}";
                    return ci;
                }

                // set date
                TransactionModel t = new TransactionModel();
                t.Payee = "";
                t.Category = "";
                t.Custom_notes = "";
                if (currentDate != null)
                {
                    t.Date = (DateTime)currentDate;
                } else
                {
                    try
                    {
                        t.Date = Convert.ToDateTime(row[dateCol]);
                    }
                    catch
                    {
                        ci.it = null;
                        ci.status = $"Invalid date {row[dateCol]} on line {line}";
                        return ci;
                    }
                }

                // set payee
                t.Payee = row[payeeCol];

                // set amount based off of direction format
                try
                {
                    switch (db)
                    {
                        case DirectionBehavior.Inflow:
                            t.Amount = Math.Abs(Convert.ToDecimal(row[amountCol].Replace("$", "")));
                            break;
                        case DirectionBehavior.Outflow:
                            t.Amount = -Math.Abs(Convert.ToDecimal(row[amountCol].Replace("$", "")));
                            break;
                        case DirectionBehavior.Both:
                            t.Amount = Convert.ToDecimal(row[amountCol].Replace("$", ""));
                            break;
                    }
                    if (t.Amount == 0) {
                        continue;
                    }
                }
                catch
                {
                    ci.it = null;
                    ci.status = $"Invalid amount {row[amountCol]} on line {line}";
                    return ci;
                }

            ci.it.Add(t);
            }            

            return ci;
        }
    }
}
