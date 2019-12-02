using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KtaCase
{
    public class Misc
    {
        public bool CompareObjectsAsString(object obj1, object obj2)
        {
            var x = obj1.ToString() == obj2.ToString();

            return x;
        }

        public bool CompareStrings(string obj1, string obj2)
        {
            var x = obj1 == obj2;

            return x;
        }

        public void Wait(int seconds)
        {
            Thread.Sleep(seconds * 1000);
        }
    }
}
