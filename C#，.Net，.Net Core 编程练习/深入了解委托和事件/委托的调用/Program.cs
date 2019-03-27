using System;

namespace 委托的调用
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] t = new int[5] {1,3,6,9,10 };
            Console.WriteLine(GetMax(t));
        }


        /// <summary>
        /// 求最大值
        /// </summary>
        /// <param name="Nums"></param>
        /// <returns></returns>
        static int GetMax(int[] Nums)
        {
            int max = Nums[0];
            for (int i = 0; i < Nums.Length; i++)
            {
                if (Nums[i]>max)
                {
                    max = Nums[i];
                }
            }

            return max;
        }
    }
}
