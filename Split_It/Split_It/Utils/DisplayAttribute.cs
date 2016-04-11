using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It.Utils
{
    public class DisplayAttribute : Attribute
    {
        public string Name { get; private set; }

        public DisplayAttribute(string name)
        {
            Name = name;
        }
    }
}
