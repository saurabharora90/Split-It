using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Service
{
    public interface IDataService
    {
        string AccessToken { get; set; }
        string AccessTokenSecret { get; set; }
    }
}
