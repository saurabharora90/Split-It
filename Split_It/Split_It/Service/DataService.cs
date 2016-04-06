using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Service
{
    public class DataService : IDataService
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }
}
