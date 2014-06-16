﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    public class Notifications
    {
        public bool added_as_friend { get; set; }
        public bool added_to_group { get; set; }
        public bool expense_added { get; set; }
        public bool expense_updated { get; set; }
        public bool bills { get; set; }
        public bool payments { get; set; }
        public bool monthly_summary { get; set; }
        public bool announcements { get; set; }
    }
}
