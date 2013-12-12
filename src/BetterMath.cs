using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OstBot_2_1
{
     class BetterMath
    {
        public  int Fibonacci(int i)
        {
            if (i < 1)
            {
                return 0;
            }
            else
            {
                int j = 1;
                int k = 0;


                for (int l = 1; l < i; l++)
                {
                    j += k;
                    k = j;
                }

                return j;
            }
        }
    }
}
