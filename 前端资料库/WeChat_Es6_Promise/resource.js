import request from '../lib/request';
import Promise from '../lib/promiseEs6Fix';

const host = 'http://ifs.chankor.cc/api/webapi';
export default {
  //授权登陆
  authorLogin(openid)
  {
    return request({
      url: urlFor(`/Login/AuthorLogin`),
      data: {
        OpenId: openid
      },
      method: 'post'
    });
  },
  //发送登录验证码
  sendValidCode(phone)
  {
    return request({
      url: urlFor(`/Message/identifyingCode`),
      data: {
        phone: phone,
        type:1
      },
      method: 'post'
    });
  },

  //手机验证登录
  mobileLogin(phone,code) {
    return request({
      url: urlFor(`/Login/CheckLogin`),
      data: {
        Mobile: phone,
        Code:code
      },
      method: 'post'
    });
  },



  //订单礼包
  fetchOrderList(type) {
    //模拟请求数据
    //var $promise = new Promise(function(resolve,reject) {
    //  resolve({statusCode:200, data:serviceData.orderData});
    //});
    //return $promise;
    let param = '';
    if (type) param = `?type=${type}`;
    return request({
      url: urlFor(`/clientOrder${param}`)
    });
  },
  //商品列表
  getbannerlist() {
    return request({
      url: urlFor(`/Marketing/GetBanner`)
    });
  },
  //商品列表
  getGoodslist(index, size, keyWord)
  {
    return request({
      url: urlFor(`/Goods/GetGoodsJson`),
      data: {
        index: index,
        size: size,
        keyWord: keyWord
      }
    });
  },
  //得到商品详情
  getGoodsdetail(goodId) {
    return request({
      url: urlFor(`/Goods/GetGoodsModelJson`),
      data: {
        productid: goodId
      }
    });
  },
  //取消订单
  cancelOrder(id) {
    return request({
      url: urlFor(`/Order/abolishment`),
      data: {
        source: id
      },
      method: 'post'
    });
  },
  //确认收货
  finishOrder(id) {
    return request({
      url: urlFor(`/Order/finshOrder`),
      data: {
        orderId: id
      },
      method: 'post'
    });
  },
  //退款申请
  drawbackOrder(id) {
    return request({
      url: urlFor('/aftermarket'),
      data: {
        sub_out_trade_no: id,
        reason: '前端退款'
      },
      method: 'post'
    });
  },
  //确认订单
  confirmOrder(couponvalue, regionId, shippingType, couponCode, shippingId, productSku, remarksku,buynum) {
    let user = this.getcurrentUser();
    var uid = user.userId;
    return request({
      url: urlFor('/Order/SaveOrder'),
      data: {
        UserId: uid, useBalance: '0', useMembersPoint: '0', PointNumber: 0, selectCouponValue: couponvalue, Shippingcity: regionId, shippingType: shippingType, paymentType: '88', couponCode: couponCode, shippingId: shippingId, productSku: productSku, remarksku: remarksku, buyAmount: buynum, from: 'signBuy', shiptoDate: '', groupbuyId: 0, remark: '', bargainDetialId: 0, limitedTimeDiscountId:0,UserBindName:''
      },
      method: 'post'
    });
  },
  //查询用户订单
  fetchuserorder(userid,index,size,status)
  {
    return request({
      url: urlFor(`/Order/GetOrderList?userid=${userid}&index=${index}&size=${size}&status=${status}`)
    });
  },
  //查询用户订单详情
  fetchuserorderdetail(orderid) {
    return request({
      url: urlFor(`/Order/getOrderDetail?orderid=${orderid}`)
    });
  },
  //收货地址
  fetchAddresses(userid) {
    return request({
      url: urlFor(`/Shipper/GetShipper?userid=${userid}`)
    });
  },
  //收货地址详情
  fetchDetailAddress(id) {
    let user = this.getcurrentUser();
    let userid = user.userId;
    return request({
      url: urlFor(`/Shipper/GetShippingModel?id=${id}&userid=${userid}`)
    });
  },
  //收货默认地址
  fetchDefaultAddress(selector) {
    let user = this.getcurrentUser();
    let userid = user.userId;
    const url = selector=='' ? urlFor(`/Shipper/GetDefaultShipper?userid=${userid}`) : urlFor(`/Shipper/GetShippingModel?userid=${userid}&&id=${selector}`);
    return request({
      url: url
    });
  },
  //添加地址
  postDetailAddress(id, data) {
    const url = id ? urlFor(`/Shipper/UpdateShippingAddress`) : urlFor('/Shipper/AddShippingAddress');
    const method = 'post';
    return request({
      url:url,
      data:data,
      method: method
    });
  },
  //删除地址
  deleteAddress(id) {
    let user = this.getcurrentUser();
    return request({
      url: urlFor(`/Shipper/DelShippingAddress`),
      data: {
        ShippingId: id,
        UserId: user.userId
      },
      method: 'post'
    });
  },
  //设为默认地址
  setDefaultAddress(id) {
    let user = this.getcurrentUser();
    return request({
      url: urlFor(`/Shipper/SetDefault`),
      data: {
        ShippingId: id,
        UserId: user.userId
      },
      method: 'post'
    });
  },

  //购物车
  fetchCartIndex(userid) {
    return request({
      url: urlFor(`/ShopCart/GetCartList?userid=${userid}`)
    });
  },
  //添加进入购物车
  addToCart(userId, optionvalue, number) {
    return request({
      url: urlFor(`/ShopCart/AddToCart`),
      data: {
        UserId: userId,
        SkuId: optionvalue,
        FreightTemplateId: 0,
        Type: 0,
        Quantity: number
      },
      method: 'post'
    });
  },
  //修改购物车数量
  updCartNumber(skuId, num) {
    return request({
      url: urlFor(`/ShopCart/ChangeNum`),
      data: { UserId:34, SkuId: skuId, Quantity: num, Type: 0, LimitedTimeDiscountId:0},
      method: 'post'
    });
  },
  //删除购物车产品
  delCartProduct(skuid) {
    let user = this.getcurrentUser();
    return request({
      url: urlFor(`/ShopCart/DeleteFromCart`),
      data: {
        UserId: user.userId,
        SkuId: skuid,
        Type: 0,
        LimitedTimeDiscountId: 0
      },
      method: 'post'
    });
  },
  //计算邮费
  getShipping(shopId, code) {
    return request({
      url: urlFor('/getShippingFare?shop_id=' + shopId + '&suppliers_id=1&code=' + code)
    })
  },
  //得到优惠券
  getcoupons() {
    let user = this.getcurrentUser();
    let userid = user.userId;
    return request({
      url: urlFor(`/Coupons/GetList?userid=${userid}`)
    });
  },
  //调用微信接口获取openid和unionid
  jscode2session(code)
  {
    return request({
      url: urlFor(`/jscode2session/get?code=${code}`),
      method: 'get'
    })
  },

  //小程序预支付
  wechatPay(orderid) {
    let user = this.getcurrentUser();
    return request({
      url: urlFor(`/wxPay/payment`),
      data: { orderId: orderid, userId: user.userId},
      method: 'get'
    })
  },
  //获取用户信息
  getUserInfo(openid) {
    return request({
      url: urlFor(`/Member/GetUserInfo?openid=${openid}`),
      method: 'get'
    })
  },
  //得到当前登录用户
  getcurrentUser(){
    let userinfo = wx.getStorageSync('userinfo');
    return userinfo;
  },
  //用户未授权跳转到授权界面
  author()
  {
    let openid = wx.getStorageSync('openid');
    if (openid == '' || openid == null) {
      wx.redirectTo({
        url: '../auth/index',
      })
    }
  },
  //添加用户信息
  saveUserInfo(userHead, openId, userName, referralUserId) {
    return request({
      url: urlFor('/Member/CreateUser'),
      data: { UserHead: userHead, MinOpenId: openId, UserName: userName, ReferralUserId: referralUserId},
      method: 'post'
    })
  },
  //修改用户信息
  updateUserInfo(data) {
    return request({
      url: urlFor('/Member/updateuser'),
      data: data,
      method: 'post'
    })
  },
  //提交意见反馈
  postFeedback(data) {
    return request({
      url: urlFor('/Member/feedback'),
      data: data,
      method: 'post'
    })
  },
  successToast(callback) {
    wx.showToast({
      title: '成功',
      icon: 'success',
      duartion: '80000',
      success: callback()
    });
  },
  loadingToast() {
    wx.showToast({
      title: '设置中，请稍后',
      icon: 'loading'
    });
  },
  confirmToast(callback) {
    wx.showModal({
      title: '提示框',
      content: '确定要删除吗？',
      showCancel: true,
      success: (res) => {
        if (res.confirm) callback();
      }
    });
  },
  //提示框
  showTips(event, msg) {
    event.setData({
      toast: {
        toastClass: 'yatoast',
        toastMessage: msg
      }
    });
    setTimeout(() => {
      event.setData({
        toast: {
          toastClass: '',
          toastMessage: ''
        }
      });
    }, 2000);
  },
  getAuthUrl(path) {
    return authHost + path;
  },
  getUrl(path) {
    return host + path;
  },
};

function urlFor(path) {
  return host + path;
}

function authUrlFor(path) {
  return authHost + path;
}