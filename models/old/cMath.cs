using System;
using System.Security.Cryptography;

namespace wstcp
{
    public class cMath
    {
        public static int GetRandom(int maxValue)
        {          
            var buf = new byte[4];
            var rand = new System.Security.Cryptography.RNGCryptoServiceProvider(new CspParameters());
            rand.GetBytes(buf);
            int randomValue = BitConverter.ToInt32(buf, 0);
            rand = null;
            if (randomValue < 0) randomValue *= -1;

            int resI = Convert.ToInt32(maxValue * ((((decimal)randomValue) / Int32.MaxValue)));
            
            return resI;
        }
    }
}