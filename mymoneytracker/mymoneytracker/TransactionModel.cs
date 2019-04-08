using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mymoneytracker
{
    //todo : implement INotify
    public class TransactionModel
    {
        public TransactionModel()
        {
            DefaultAll();
        }
        public int Id { get; }
        public string Date { get; set; }
        public string Payee { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Category { get; set; }
        public string Custom_notes { get; set; }   
        public void DefaultAll()
        {
            Date = DateTime.Today.ToString();
            Payee = "Payee";
            Amount = 0;
            Category = "Category";
            Custom_notes = "Notes";
        }
    }
}