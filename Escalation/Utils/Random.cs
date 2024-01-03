﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escalation.Utils
{
    public class Random
    {


        /*
        public static UInt64 Seed_64 = 1;
        public static UInt32 Mat1_64 = 0xfa051f40;
        public static UInt32 Mat2_64 = 0xffd0fff4;
        public static UInt64 TMat_64 = 0x58d02ffeffbfffbc;

        */
        public static UInt64 Seed_64 = 4;
        public static UInt32 Mat1_64 = 5;
        public static UInt32 Mat2_64 = 4;
        public static UInt64 TMat_64 = 4;

        //Tiny mersenne twister
        static TinyMT64 TMT64 = new TinyMT64(15564642, 0xfa051f40, 0xffd0fff4, 0x58d02ffeffbfffbc);



        public static int Next(int max)
        {
            return (int)(TMT64.GetRandInt() % (ulong)max);
        }
        

        public static ulong Next(ulong min, ulong max)
        {
            return min==max ? min : (min + (TMT64.GetRandInt() % (max - min)));
        }

        public static decimal Next(decimal min, decimal max)
        {
            return min==max ? min : (min +(TMT64.GetRandInt() % (max - min)));
        }

        public static int Next(int min, int max)
        {
            return min==max? min : (min + (int)(TMT64.GetRandInt() % (ulong)(max - min)));
        }

       
        public static double NextDouble()
        {
            return TMT64.GetRandDouble();
        }

        public static double Gaussian(double mean, double deviation)
        {
            double u1 = NextDouble();
            double u2 = NextDouble();
            double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            return mean + deviation * z;
        }
    }
}
