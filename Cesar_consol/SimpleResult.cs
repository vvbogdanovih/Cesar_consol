using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesar_consol
{
    public class SimpleResult
    {
        public string ElapsedTime { get; private set; }
        public int Size {  get; private set; }
        public SimpleResult(int size, string time)
        {
            Size = size;
            ElapsedTime = time;
        }

        public override string ToString()
        {
            return "Time: " + ElapsedTime + "; Size: " + Size;
        }

    }
}
