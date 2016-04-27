using System;

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
