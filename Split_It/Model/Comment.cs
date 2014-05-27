﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Model
{
    public class Comment
    {
        public int id { get; set; }
        public string content { get; set; }
        public string comment_type { get; set; }
        public string relation_type { get; set; }
        public int relation_id { get; set; }
        public string created_at { get; set; }
        public string deleted_at { get; set; }
        public User user { get; set; }
    }
}
