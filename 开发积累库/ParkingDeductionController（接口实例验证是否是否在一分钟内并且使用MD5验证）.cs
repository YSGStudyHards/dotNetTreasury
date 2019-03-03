using Framework.Data;
using JJHL.Models.adlx_DB;
using JJHL.WEB.Areas.PublicMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JJHL.WEB.Areas.PublicWebApi.Controllers
{
    public class ParkingDeductionController : Controller
    {

        /// <summary>
        /// 密钥（浏阳河婚庆园MD5加密）
        /// </summary>
        private string Token = "C8F8C3B36DBCCF00D1AD6DE2FFBFC4FF";

        /// <summary>
        /// 停车场请求抵扣接口
        /// </summary>
        /// <param name="serviceid">服务id</param>
        /// <param name="paypasswd">支付秘钥</param>
        /// <param name="out_trade_no">交易号</param>
        /// <param name="plate_number">车牌编号</param>
        /// <param name="orgphone">平台用户号码（停车场和婚庆园的唯一关联）</param>
        /// <param name="charged_duration">停车时长（分钟）</param>
        /// <param name="amount">消费金额（分）</param>
        /// <param name="parking_lot">停车消费地点</param>
        /// <param name="pay_time">支付时间（取整时间戳）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TimeDeductible(string serviceid, string paypasswd, string out_trade_no, string plate_number, string orgphone, string amount, string parking_lot, string pay_time, int charged_duration=0)
        {
            if (string.IsNullOrEmpty(serviceid) || string.IsNullOrEmpty(paypasswd) || string.IsNullOrEmpty(out_trade_no) || string.IsNullOrEmpty(plate_number) || string.IsNullOrEmpty(orgphone) || string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(parking_lot) || string.IsNullOrEmpty(pay_time) || charged_duration <= 0)
            {
                return Json(new { status = 0, detail="请求数据不能为空" }, JsonRequestBehavior.AllowGet);
            }
            //判断请求时间是否超过一分钟（时间搓相减以秒为单位）
            double tiemnowuinxtime = DateTime.Now.ToUinxTime();
            double Differencevalue = tiemnowuinxtime-Convert.ToDouble(pay_time);
            bool falg = Differencevalue>60;

            //时间为空或者超过一分钟则表示查询超时
            if (falg)
            {
                return Json(new { status = 2, detail = "超过查询时长" }, JsonRequestBehavior.AllowGet);
            }

            string SecrectKey = Token + pay_time;
            //密钥验证
            var md5Staff = ApplicationContext.Security.Md5.ComputeHash(SecrectKey).ToUpper(); 
            if (!md5Staff.Equals(paypasswd))
            {
                return Json(new { status =3, detail = "秘钥有误" }, JsonRequestBehavior.AllowGet);
            }

            //查询用户信息
            var UserInfo = new user { Account = orgphone}.SelectObject();
            if (UserInfo==null)
            {
                return Json(new { status =4, detail = "未找到此用户信息" }, JsonRequestBehavior.AllowGet);
            }

            var ParkInfo = new parkuserinfo {Plate_Number=plate_number,MobilePhone=orgphone }.SelectObject();
            if (ParkInfo==null)
            {
                return Json(new { status = 5, detail = "未找到账号为："+orgphone+"的车牌信息" }, JsonRequestBehavior.AllowGet);
            }

            //查看该订单是否已经抵扣了
            var parkdeductionrecords = new parkdeductionrecords {UserID=UserInfo.Id,out_trade_no=out_trade_no }.SelectObject();
            if (parkdeductionrecords!=null)
            {
                return Json(new { status = 9, detail = "该订单已抵扣无法再此抵扣" }, JsonRequestBehavior.AllowGet);
            }

            //开始抵扣判断是否存在合适的抵扣信息优先考虑停车券（为了方便账目合计采用了时间抵扣的方式）

            //积分时间抵扣比例
            var subscription = new parkdeductions { }.SelectObject();

            //时间有积分兑换比例
            Decimal BiLi = Convert.ToDecimal((Decimal)subscription.Parktime / (Decimal)subscription.DeductionIntegral);

            amount =(Convert.ToDecimal(amount)/100).ToString();//返回过去的金额为元（返回过来的值以分为单位，所以更改为元）

            Decimal ParkTime = Math.Ceiling(Convert.ToDecimal((Decimal)charged_duration/60));//转化为小时
            Dictionary<int, string> ParkCouponInfo = QueryParkCouons(ParkTime,Convert.ToInt32(UserInfo.Id));
            //存在停车卷
            int ParkID = 0;
            string ParkType = "";
            if (ParkCouponInfo.Count() > 0)
            {
                foreach (var item in ParkCouponInfo)
                {
                    ParkID = item.Key;
                    ParkType = item.Value + "小时停车抵扣券";
                }
                //停车券核销
                var Parkcoupondetails = new parkcoupons_detail { ParkID = ParkID }.SelectObject();
                int Detail = new parkcoupons_detail { PId = Parkcoupondetails.PId, Usestate = 1, UseTime = DateTime.Now.ToString(), UseAddress = parking_lot }.Update();
                var parkcoupon = new parkcoupons { ParkID = ParkID }.SelectObject();
                int ParkCoupons = new parkcoupons { ParkID = ParkID, UsedNum = parkcoupon.UsedNum - 1 }.Update();
                //抵扣成功
                if (Detail>0&&ParkCoupons>0)
                {
                    //停车抵扣记录添加
                   AddParkRecord(Convert.ToInt32(UserInfo.Id), 0, ParkID, plate_number, Convert.ToString(charged_duration), out_trade_no, Convert.ToInt32(serviceid), amount, parking_lot, pay_time);
                   return Json(new { status = 1, detail ="抵扣成功" }, JsonRequestBehavior.AllowGet); 
                }
                else
                {
                    return Json(new { status =7, detail = "抵扣失败未找到合适的停车券" }, JsonRequestBehavior.AllowGet); 
                }
            }
            //积分
            else
            {
                if (Convert.ToInt32(charged_duration) < (UserInfo.Bonus * BiLi))
                {
                    //向上取整(抵扣积分)
                   int Bonus = Math.Ceiling(Convert.ToDecimal(charged_duration) / BiLi).ToInt();
                   //用户积分扣除
                   BonusPublicMethod bonPul = new BonusPublicMethod();
                   string error = bonPul.UpdateBonus(UserInfo, "停车消费金额抵扣", Convert.ToInt32(Bonus), 4);
                   if (error=="扣除成功!")
                   {
                       //停车抵扣记录添加
                       AddParkRecord(Convert.ToInt32(UserInfo.Id),Bonus,0,plate_number,Convert.ToString(charged_duration),out_trade_no,Convert.ToInt32(serviceid),amount,parking_lot,pay_time);
                       return Json(new { status = 1, detail ="抵扣成功"}, JsonRequestBehavior.AllowGet); 
                   }
                   else
                   {
                       return Json(new { status =8, detail = "抵扣出错"}, JsonRequestBehavior.AllowGet);
                   }
                }
                else
                {
                    return Json(new { status = 6, detail ="抵扣失败没有足够的积分" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        /// <summary>
        /// 停车抵扣信息添加
        /// </summary>
        /// <param name="ClientUserId">用户id</param>
        /// <param name="Integral">积分（无传0）</param>
        /// <param name="Parkids">停车券id</param>
        /// <param name="plate_number">停车编号</param>
        /// <param name="charged_duration">停车时长</param>
        /// <param name="out_trade_no">交易号</param>
        /// <param name="serviceids">服务id</param>
        /// <param name="amount">消费金额</param>
        /// <param name="Address">地点</param>
        /// <returns></returns>
        private int AddParkRecord(int ClientUserId, int Integral, int Parkids, string plate_number, string charged_duration, string out_trade_no, int serviceids, string amount, string Address,string paytime)
        {
            var CreateDeduectionRecord = new parkdeductionrecords
            {
                UserID = ClientUserId,
                Integral = Convert.ToInt32(Integral),
                CouponID = Parkids,
                Plate_Number = plate_number,
                charged_duration = charged_duration,
                out_trade_no = out_trade_no,
                serviceid = Convert.ToInt32(serviceids),
                Amount = Convert.ToDecimal(amount),
                ParkAddress = Address, //地点,
                CreateTime =long.Parse(paytime)//支付时间
            }.Create();

            return CreateDeduectionRecord;
        }

        /// <summary>
        /// 查询用户是否有匹配的优惠券
        /// </summary>
        /// <param name="Hours"></param>
        /// <param name="ClientUserId"></param>
        /// <returns></returns>
        private Dictionary<int, string> QueryParkCouons(Decimal Hours, int ClientUserId)
        {
            Dictionary<int, string> TimeAndcoupond = new Dictionary<int, string>();
            //连接数据出
            var cmd = new XmlCommandHelper(parkcoupons_detail._DataBaseName);
            //实参
            List<XmlCommandParameter> ListP = new List<XmlCommandParameter>();
            //条件
            List<string> ListWhere = new List<string>();
            //数据分页
            //该用户
            ListWhere.Add(" AND s.UserID=@UserID");
            ListP.Add(new XmlCommandParameter { ParameterName = "@UserID", DbType = System.Data.DbType.Int32, Value = ClientUserId });
            //未使用的
            ListWhere.Add(" AND s.Usestate=@Usestate");
            ListP.Add(new XmlCommandParameter { ParameterName = "@Usestate", DbType = System.Data.DbType.Int32, Value = 0 });
            //开始使用小于等于当前时间
            ListWhere.Add(" And f.StartTime<=@StartTime");
            ListP.Add(new XmlCommandParameter { ParameterName = "@StartTime", DbType = System.Data.DbType.DateTime, Value = DateTime.Now });
            //结束时间大于等于当前时间
            ListWhere.Add(" AND f.EndTime>=@EndDate");
            ListP.Add(new XmlCommandParameter { ParameterName = "@EndDate", DbType = System.Data.DbType.DateTime, Value = DateTime.Now });
            ListWhere.Add(" AND f.ParkType>=@ParkType");
            ListP.Add(new XmlCommandParameter { ParameterName = "@ParkType", DbType = System.Data.DbType.Decimal, Value = Hours });
            ListWhere.Add(" ORDER BY f.EndTime,f.ParkType limit 1");
            string StrWhere = string.Join("", ListWhere);
            var list = cmd.ExecuteToList<parkcoupons_detail>(JJHL.Utility.Consts.XmlCommand.QueryParkCoupons, StrWhere, ListP.ToArray());
            foreach (var item in list)
            {
                TimeAndcoupond.Add(Convert.ToInt32(item.ParkID), item.ParkType);
            }
            return TimeAndcoupond;
        }

    }
}
