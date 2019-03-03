/*
 *作者：追逐时光
 * 2019.1.21
 */
using System;

namespace 深入了解委托和事件
{
    class Program
    {
        //第一步：通过delagate关键字定义委托类型(委托类型和方法必须与所要传递的方法是一致的)【我在这里的方法是void无返回值的，并且没有参数，假如有参数必须要与其他调用的方法相统一】
        public delegate void DelegateWays();

        static void Main(string[] args)
        {
            /*
             * 今天所要描述的是一个关于积分获得的问题，曾经遇到过一个项目，他可以通过各种途径获取对应的积分，一个程序下来最少有十几二十个if...else或者switch运用到了多个case
             * 这样的一个方法看起来真的让人很不舒服，而且这对于今后的程序扩展也非常的不良好，今天简单的举三个途径
             * 积分领取途径：注册领取，登录领取，任务领取三张
             */
           
            //Console.WriteLine("请输入你的领取方式：");
            //string Channel = Console.ReadLine();

            #region 通过委托的方式将对应的方法通过参数来传递调用
            //第二步：定义委托变量
            DelegateWays Delegate;

            //第三步：实例化委托[委托链拼接又称多播+=，-=]
            Delegate = SignUp;
            Delegate += SignIn;
            Delegate += Duty;

            //第四步：将方法作为参数传递
            //new Program().WriteLines(Delegate);

            //第五步：调用委托[直接调用]
            Delegate();
            #endregion

            #region 首先正常逻辑在我们实际开发中往往会遇到许多的条件判断，插入不同的值
            //switch (Channel)
            //{
            //    case "注册领取":
            //        SignUp(Channel);
            //        break;
            //    case "登录领取":
            //        SignIn(Channel);
            //        break;
            //    case "任务领取":
            //        Duty(Channel);
            //        break;
            //    default:; break;
            //        /*
            //         *.....在此省略很多了
            //         */
            //}
            #endregion
        }

        /// <summary>
        //第五步：调用委托[间接调用]
        /// </summary>
        /// <param name="way"></param>
        public void WriteLines(DelegateWays way)
        {
            way();//调用对应的委托
        }

        /// <summary>
        /// 注册领取
        /// </summary>
        public static void SignUp()
        {
            Console.WriteLine("注册领取积分");
        }

        /// <summary>
        /// 登录领取
        /// </summary>
        public static void SignIn()
        {
            Console.WriteLine("登录领取金额");
        }

        /// <summary>
        /// 任务领取
        /// </summary>
        public static void Duty()
        {
            Console.WriteLine("完成任务领取积分");
        }
    }
}
