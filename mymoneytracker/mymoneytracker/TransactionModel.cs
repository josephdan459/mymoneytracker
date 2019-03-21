﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mymoneytracker
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Payee { get; set; }
        public int Amount { get; set; }
        public string Custom_notes { get; set; }
        public string Category { get; set; }
        public string Category_override { get; set; }        
    }
}