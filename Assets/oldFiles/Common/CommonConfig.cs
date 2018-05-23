using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CommonConfig : MonoBehaviour
{


    /// <summary>
    /// 城市配置类
    /// </summary>
    public class CityConfig
    {
        /// <summary>
        /// 城市ID
        /// </summary>
        public int CITY_ID;
        /// <summary>
        /// 城市名字
        /// </summary>
        public string CITY_NAME;
        /// <summary>
        /// 是否激活
        /// </summary>
        public int VALID;
        /// <summary>
        /// 分享游戏
        /// </summary>
        public string SHARING;
        /// <summary>
        /// 分享邀请
        /// </summary>
        public string SHARING_INVITATION;
    }

    /// <summary>
    /// 城市对应的县的配置
    /// </summary>
    public class DistrictConfig
    {
        /// <summary>
        /// 区县ID
        /// </summary>
        public int COUNTY_ID;
        /// <summary>
        /// 城市ID
        /// </summary>
        public int CITY_ID;
        /// <summary>
        /// 区县名字
        /// </summary>
        public string COUNTY_NAME;
        /// <summary>
        /// 是否激活
        /// </summary>
        public int VALID;
        /// <summary>
        /// 区县包含玩法
        /// </summary>
        public string METHOD;
        /// <summary>
        /// 该县对应的qq群
        /// </summary>
        public int QQ;
    }
    public class MethodConfig
    {
        /// <summary>
        /// 玩法ID
        /// </summary>
        public int METHOD;
        /// <summary>
        /// 玩法名字
        /// </summary>
        public string METHOD_NAME;
        /// <summary>
        /// 玩法描述
        /// </summary>
        public string METHOD_DISCRIPTION;
        /// <summary>
        /// 玩法类型，1表示以圈为单位，2表示以局为单位，3表示打锅玩法
        /// </summary>
        public string type;
        /// <summary>
        /// 玩法的数量，例：1_2_3表示三种选择，1圈2圈3圈
        /// </summary>
        public string sum;
        /// <summary>
        /// 上述数量应该支付的房卡数量，与上面的数量一一对应
        /// </summary>
        public string pay;
        /// <summary>
        /// 玩法底分
        /// </summary>
        public int base_score;
        /// <summary>
        /// 游戏的ID
        /// </summary>
        public int gameid;
    }

    /// <summary>
    /// 玩法对应的牌型
    /// </summary>
    public class CardType
    {
        /// <summary>
        /// 玩法id
        /// </summary>
        public int ID;   //
        /// <summary>
        ///  //玩法的名字
        /// </summary>
        public string card_type;
        ///该玩法的注释 
        public string notes;  //
        /// <summary>
        /// 规则之间的关联，如果不为零，表示之后对应的规则选择之后，才可以选择这个规则
        /// </summary>
        public int relevance;  //
        /// <summary>
        /// 取消点击的时候的注释
        /// </summary>
        public string notNotes;//
    }

    /// <summary>
    /// 玩法选择牌型的配置
    /// </summary>
    public class MethodToCardType
    {
        /// <summary>
        /// 玩法的id
        /// </summary>
        public int METHOD;  //
        /// <summary>
        /// 玩法可以选择的牌型的id
        /// </summary>
        public int RuleId;  //
        /// <summary>
        /// 该牌型类型，1表示单选，2表示多选
        /// </summary>
        public int Choice;  //
        /// <summary>
        ///表示摆放位置 上下分布层级1 第一层2 第二层3 第三层
        /// </summary>
        public int Hierarchy;  //
        /// <summary>
        /// 是否默认选中的标志位0表示不选，1表示默认选中
        /// </summary>
        public int Options; //
    }

    /// <summary>
    /// 县对应玩法对应的名字
    /// </summary>
    public class PlayNameConfig
    {
        /// <summary>
        /// 县的id
        /// </summary>
        public int COUNTY_ID; //
        /// <summary>
        /// 方法id
        /// </summary>
        public int METHOD;  //
        /// <summary>
        /// 方法名
        /// </summary>
        public string METHOD_NAME; //
    }
    /// <summary>
    /// IOS支付道具配置
    /// </summary>
    /// <summary>
    /// 微信分享配置
    /// </summary>

    public class Json_ShareConfig
    {
        public int status;
        public string value;
        public string value2;
    }


    #region JsonFAQ
    [Serializable]
    public class Json_FAQ
    {
        public int num = 0;
        public FaqContent[] data;
        public Json_FAQ() { }
        public Json_FAQ(int a)
        {
            num = a;
            data = new FaqContent[a];
        }
    }
    [Serializable]
    public class Json_SpecialTypeCardName
    {
        public int num;
        public Names[]data;
    }
    [Serializable]
    public struct Names
    {
        public int index;
        public string name;
        public string name_a;
        public string name_b;
    }
    [Serializable]
    public struct FaqContent
    {
        public string title;
        public string content;
    }
    #endregion JsonFAQ

    #region JoinUs
    [Serializable]
    public class Json_Join
    {
        public int status;
        public Data_join[] data;
    }
    [Serializable]
    public struct Data_join
    {
        public int ID;
        public string NICKNAME;
        public string WEIXIN;
        public string PHONE;
        public string IMG_URL;
        public string COUNTY_NAME;
    }
    #endregion

    #region 活动json
    [Serializable]
    public class Json_Activity
    {
        public int status;
        public Data_Activity[] data;
    }
    [Serializable]
    public struct Data_Activity
    {
        public int ACTIVITY_ID;//1: 活动编号(int)
        public string TITLE;   //2活动标题(string)
        public int BEGIN_TIME; //3活动开始时间(int)
        public int END_TIME;   //4活动结束时间(int)
        public int ACTIVITY_TYPE;//5活动种类，1图片型，2文字型-(int)
        public int RP12_FLAG;    //6是否有分享红包 0无 1有-(int)
        public int SHOW_FLAG;    //7是否首页弹框展示 0不 1是-(int)

        public int SHARE_TYPE;   //8分享类型 1朋友圈 2好友
        public string SHARE_FRIEND_TITLE;//9分享好友标题
        /// <summary>
        /// //10 分享好友内容
        /// </summary>
        public string SHARE_FRIEND_CONTENT;
        /// <summary>
        /// //11 分享朋友圈内容
        /// </summary>
        public string SHARE_MOMENTS_CONTENT;
        public string SHARE_MOMENTS_TYPE;   //12 分享朋友圈内容类型 0文字1图片

        public string IMG_URL;           //13图片型活动 图片地址-(string)
        /// <summary>
        /// 文字型活动文字标题-(string)
        /// </summary>
        public string TITLE2;            //14文字型活动文字标题-(string)
        public string TITLE3;            //15副标题
        public string DESCRIPTION;       //16文字型活动文字描述-(string)
        public string REDIRECT_URL;      //17跳转地址-(string)
        public string BUTTON_CHAR;       //18按钮文字
    }
    #endregion 活动json
}
public class CommonDatas
{
    public List<CommonConfig.CityConfig> cityConfig = new List<CommonConfig.CityConfig>();
    public List<CommonConfig.DistrictConfig> districtConfig = new List<CommonConfig.DistrictConfig>();
    public List<CommonConfig.MethodConfig> methodConfig = new List<CommonConfig.MethodConfig>();
    public List<CommonConfig.CardType> cardType = new List<CommonConfig.CardType>();
    public List<CommonConfig.MethodToCardType> methodToCardType = new List<CommonConfig.MethodToCardType>();
    public List<CommonConfig.PlayNameConfig> playNameConfig = new List<CommonConfig.PlayNameConfig>();
}


