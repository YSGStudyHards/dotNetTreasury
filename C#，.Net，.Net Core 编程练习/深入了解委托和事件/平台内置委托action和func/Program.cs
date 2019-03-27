using System;

namespace 平台内置委托action和func
{
    //定义委托
    public delegate void WriteLine();

    class Program
    {
        static void Main(string[] args)
        {
            //声明委托
            //WriteLine writeLine = new WriteLine(tt);
            //writeLine();//调用委托
            //Console.WriteLine();
            WriteLine t = tt;
            t();
        }

        static void tt()
        {
            Console.WriteLine("一个人");
        }
    }
}
