﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    public class Currency
    {
        public string currency_code { get; set; }
        public string unit { get; set; }

        public override string ToString()
        {
            return currency_code;
        }
    }
}
