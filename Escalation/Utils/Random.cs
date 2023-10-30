using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escalation.Utils
{
    internal class Random
    {


        //Tiny mersenne twister
        static TinyMT64 TMT64 = new TinyMT64(1, 0xfa051f40, 0xffd0fff4, 0x58d02ffeffbfffbc);



        public static int Next(int max)
        {
            return (int)(TMT64.GetRandInt() % (ulong)max);
        }
        

        public static ulong Next(ulong min, ulong max)
        {
            return (min + (TMT64.GetRandInt() % (max - min)));
        }

        public static double NextDouble()
        {
            return TMT64.GetRandDouble();
        }
    }
}
