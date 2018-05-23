using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class TextConstant
    {
        public const string GENERA_TITILE = "温馨提示";
        public const string NET_MSG_CONNECT_LOBBY_SERVER_FAILED = "亲，您的网络不给力哦，请检查网络后重试";
        public const string NET_MSG_SERVER_UNOPEN = "服务器正在维护中，请稍后登录";
        public const string PLAYER_QUITAPPLATION = "确定要离开游戏吗？";
        public const string DISAGREEUSERDEAL = "请仔细阅读用户协议，如同意该协议请勾选确认";
        public const string LOADINGTEXT = "正在加载游戏资源，请等待...（此过程不消耗流量）"; //加载场景的显示文字
        public const string LOADINGFINISH = "加载完成，祝您游戏愉快! ";  //加载完成的显示文字
        public const string LOOKDAILI = "您不是代理，无法使用该功能，是否前往了解！";
    }
    // 回应消息的公共错误编号
    // 公共错误编号用负数，回应消息独有的错误编号用正数
    public class Err_Server
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "在服务器中没找到用户节点，可能是没登录";
                    break;
                case 2:
                    str = "在服务器找到的用户节点，其中用户编号错误";
                    break;
                case 3:
                    str = "用户的桌节点错误";
                    break;
                case 4:
                    str = "用户的座位号错误";
                    break;
                case 5:
                    str = "同桌用户节点为空";
                    break;
                case 10:
                    str = "curl_easy_init 执行错误";
                    break;
                case 11:
                    str = "curl_easy_perform 执行错误";
                    break;
                default:
                    break;
            }
            return str;
        }
    }

    /// <summary>
    /// 登录认证回应错误
    /// </summary>
    public   class Err_ClientAuthenRes
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "认证类型错误";
                    break;
                case 2:
                    str = "现在不是服务器开放时间";
                    break;
                case 3:
                    str = "用户服务器没有连接";
                    break;
                case 4:
                    str = "服务器在线人数满了";
                    break;
                case 5:
                    str = "检查认证错误";
                    break;
                case 6:
                    str = "新增用户错误";
                    break;
                case 7:
                    str = "获取用户错误";
                    break;
                case 8:
                    str = "用户被禁用";
                    break;
                case 9:
                    str = "普通用户不能在仅允许内部用户进入时进入";
                    break;
                case 10:
                    str = "用户数据库设置登录信息错误";
                    break;
                case 11:
                    str = "重复登录";
                    break;
                case 12:
                    str = "令牌错误";
                    break;
                case 13:
                    str = "查找套接字节点错误";
                    break;
                case 14:
                    str = "用户编号错误";
                    break;
                case 15:
                    str = "版本号不匹配";
                    break;
                case 16:
                    str = "重入失败";
                    break;
                case 17:
                    str = "根据用户编号找的的用户节点是别人的";
                    break;
                case 18:
                    str = "游戏服务器不支持重入";
                    break;
                case 19:
                    str = "获取下一个用户编号错误";
                    break;
                case 20:
                    str = "服务器不可用";
                    break;
                case 21:
                    str = "用户不存在";
                    break;
                case 22:
                    str = "重复认证";
                    break;
                default:
                    break;
            }
            return str;
        }
    }

    /// <summary>
    /// 充值请求回应错误
    /// </summary>
    public class Err_ClientChargeRes
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "充值模式错误";
                    break;
                case 2:
                    str = "充值编号错误";
                    break;
                case 3:
                    str = "检查收据失败，不再检查";
                    break;
                case 4:
                    str = "检查收据错误，下次再检查";
                    break;
                case 5:
                    str = "充值金额错误";
                    break;
                case 6:
                    str = "";
                    break;
                case 7:
                    str = "充值被禁用";
                    break;
                case 8:
                    str = "获取用户代金券数据错误";
                    break;
                case 9:
                    str = "使用代金券的订单号不匹配";
                    break;
                case 10:
                    str = "检查充值失败";
                    break;
                case 11:
                    str = "老板不能充值会员的";
                    break;
                case 12:
                    str = "会员不能充值老板的";
                    break;
                case 13:
                    str = "老板参数错误";
                    break;
                case 14:
                    str = "老板充值不能用苹果充值";
                    break;
                default:
                    break;
            }
            return str;
        }
    }
    /// <summary>
    /// 0x0221	// [厅服]->[厅客]或[计服]->[厅服]修改用户信息回应消息
    /// </summary>
    public class Err_ClientChangeUserInfoRes
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "修改的内容与原来相同";
                    break;
                case 2:
                    str = "操作数据库失败";
                    break;
                default:
                    break;
            }
            return str;
        }
    }
    /// <summary>
    /// 	0x0223	// [厅客]->[厅服]实名认证回应消息
    /// </summary>
    public class Err_ClientFullNameRes
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "已经设置过了";
                    break;
                case 2:
                    str = "设置的内容太短";
                    break;
                default:
                    break;
            }
            return str;
        }
    }
    /// <summary>
    /// 0x0225	// [厅客]->[厅服]选择市县回应消息
    /// </summary>
    public class Err_ClientCityCountyRes
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "和原来相同";
                    break;
                case 2:
                    str = "操作数据库失败";
                    break;
                default:
                    break;
            }
            return str;
        }
    }
    /// <summary>
    /// 0x0231	// [厅服]->[厅客]使用推广码回应消息
    /// </summary>
    public class Err_ClientSpreadCodeRes
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "推广码错误";
                    break;
                case 2:
                    str = "推广码的用户不存在";
                    break;
                case 3:
                    str = "推广码的用户不是推广员";
                    break;
                case 4:
                    str = "推广码的用户被禁用";
                    break;
                case 5:
                    str = "操作数据库失败";
                    break;
                default:
                    break;
            }
            return str;
        }
    }
    /// <summary>
    /// 0x0241	// [厅服]->[厅客]绑定代理回应消息
    /// </summary>
    public class Err_ClientBindProxyRes
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "已经有代理";
                    break;
                case 2:
                    str = "获取用户信息错误";
                    break;
                case 3:
                    str = "获取代理信息错误";
                    break;
                case 4:
                    str = "指定代理不是代理";
                    break;
                case 5:
                    str = "数据库操作失败";
                    break;
                default:
                    break;
            }
            return str;
        }
    }
    /// <summary>
    /// 0x0261	// [厅服]->[厅客]开房回应消息
    /// </summary>
    public class Err_ClientOpenRoomRes
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "没可用的服务器";
                    break;
                case 2:
                    str = "支持的最大人数错误";
                    break;
                case 3:
                    str = "没可用的桌";
                    break;
                case 4:
                    str = "获取游戏服务器信息失败";
                    break;
                case 5:
                    str = "错误的游戏编号";
                    break;
                case 6:
                    str = "错误的游戏服务器编号";
                    break;
                case 7:
                    str = "错误的桌号";
                    break;
                case 8:
                    str = "空的桌节点";
                    break;
                case 9:
                    str = "开房状态不为0";
                    break;
                case 10:
                    str = "开房模式错误";
                    break;
                case 11:
                    str = "开房达到上限";
                    break;
                case 12:
                    str = "设置开房参数错误";
                    break;
                default:
                    break;
            }
            return str;
        }


    }
    /// <summary>
    /// 0x1007	// [游服]->[游客]坐下回应消息
    /// </summary>
    public class Err_ClientSitRes
    {
        public Dictionary<int, string> dic = new Dictionary<int, string>();
        public Err_ClientSitRes()
        {
            dic.Add(1, "用户状态错误，不是找座位状态");
            dic.Add(2, "找座位失败");
            dic.Add(3, "找到座位的桌号错误");
            dic.Add(4, "找到座位的座位号错误");
            dic.Add(5, "找到座位的桌节点错误");
            dic.Add(6, "找到座位上已经有人了");
            dic.Add(7, "赞不够");
            dic.Add(8, "掉线率过高");
            dic.Add(9, "出牌速度过慢");
        }
    }
    /// <summary>
    /// 出牌回应消息
    /// </summary>
    public class Err_ClientDiscardTileRes
    {
        public Dictionary<int, string> dic = new Dictionary<int, string>();
        public Err_ClientDiscardTileRes()
        {
            dic.Add(1, "出牌座位错误");
            dic.Add(2, "桌状态错误");
            dic.Add(3, "用户状态错误");
            dic.Add(4, "手里没有要出的牌");

        }
    }
    public class Err_ClientSpecialTileRes
    {
        public Dictionary<int, string> dic = new Dictionary<int, string>();
        public Err_ClientSpecialTileRes()
        {
            dic.Add(1, "操作类型错误");
            dic.Add(2, "操作类型标志错误");
            dic.Add(3, "桌状态错误");
            dic.Add(4, "用户状态错误");
            dic.Add(5, "不是出牌用户");
            dic.Add(6, "出牌状态只能杠和胡");
            dic.Add(7, "放弃失败");
            dic.Add(8, "吃失败");
            dic.Add(9, "碰失败");
            dic.Add(10, "杠失败");
            dic.Add(11, "胡失败");
            dic.Add(12, "吃(长治花三门的十三幺)失败");
            dic.Add(13, "抢(长治花三门的十三幺)失败");
        }
    }
    public class Err_ClientThirteenOrphansRes
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "没开启十三幺";
                    break;
                case 2:
                    str = "设置的和原来相同";
                    break;
                case 3:
                    str = "关闭失败";
                    break;
                case 4:
                    str = "开启失败";
                    break;
                default:
                    break;
            }
            return str;
        }
    }
    public class Err_ClientUseActiveCodeRes
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "获取用户这批次的激活码使用数量错误";
                    break;
                case 2:
                    str = "同批次的激活码只能使用一次";
                    break;
                case 3:
                    str = "输入激活码错误";
                    break;
                case 4:
                    str = "激活码过期";
                    break;
                case 5:
                    str = "使用激活码失败";
                    break;
                default:
                    break;
            }
            return str;
        }
    }
    public class Err_ClientCreateOrderRes
    {
        public static string Err(int a)
        {
            string str = "未知错误";
            switch (a)
            {
                case 1:
                    str = "获取充值数据错误";                    break;
                case 2:
                    str = "不支持的充值模式";                    break;
                case 3:
                    str = "禁止充值";                    break;
                case 4:
                    str = "充值金额错误";                    break;
                case 5:
                    str = "获取用户代金券数据错误";                    break;
                case 6:
                    str = "代金券金额错误";                    break;
                case 7:
                    str = "代金券被使用过了";                    break;
                case 8:
                    str = "折扣后价格没达到代金券使用金额限制";                    break;
                case 9:
                    str = "代金券过期";                    break;
                case 10:
                    str = "使用代金券失败";                    break;
                case 11:
                    str = "生成订单信息错误";                    break;
                case 12:
                    str = "微信统一下单错误";                    break;
                case 13:
                    str = "老板不能充值会员的"; break;
                case 14:
                    str = "会员不能充值老板的"; break;
                case 15:
                    str = "老板参数错误"; break;
                case 16:
                    str = "老板充值不能用苹果充值"; break;
                default:
                    break;
            }
            return str;
        }
    }
  


}
