using Split_It.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Design
{
    public class DesignDataService : IDataService
    {
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }
    }
}
