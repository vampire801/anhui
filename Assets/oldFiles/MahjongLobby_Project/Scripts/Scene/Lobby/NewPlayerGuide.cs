using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class NewPlayerGuide : MonoBehaviour
    {

        #region 单例
        static NewPlayerGuide instance;
        public static NewPlayerGuide Instance
        {
            get
            {
                return instance;
            }
        }

        void Awake()
        {
            instance = this;
        }

        #endregion

        public GameObject[] PlayerGuide;  //新手引导的界面

        //引导的枚举
        public enum Guide
        {
            CreatRoom,  //创建房间
            ShareTowX_1, //分享按钮
            ShareToWx_2, //进入分享界面的分享按钮
            Promote,  //自己的推广码首次被使用
            HistroyGrade, //历史战绩
            JoinAgency,  //提示加入代理或申请代理
            JoinMe, //加我提示
            PlayerGiftCode, //玩家的礼包码的提示
            GameInvite,  //房间内邀请        
        }

        /// <summary>
        /// 打开某个新手引导
        /// </summary>
        /// <param name="index"></param>
        public void OpenIndexGuide(Guide guide)
        {
            int index = (int)guide;
            if (index < PlayerGuide.Length)
            {
                if (PlayerGuide[index] != null)
                {
                    PlayerGuide[index].SetActive(true);
                }
            }
        }


        /// <summary>
        /// 隐藏某个新手引导
        /// </summary>
        /// <param name="index"></param>
        public void HideIndexGuide(Guide guide)
        {
            int index = (int)guide;
            if (PlayerGuide[index] == null || !PlayerGuide[index].gameObject.activeInHierarchy)
            {
                return;
            }
            PlayerGuide[index].SetActive(false);
            //隐藏指定新手引导之后，修改指定的引导的对应的注册表的时间
            float timer = anhui.MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(System.DateTime.Now);
            PlayerPrefs.SetFloat(guide.ToString(), timer);
        }


        /// <summary>
        /// 固定时间隐藏某个新手引导
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="guide"></param>
        public void SetTimeHideGuide_Ie(float timer, Guide guide)
        {
            if (UIMainView.Instance.LobbyPanel.isActiveAndEnabled)
            {
                StartCoroutine(SetTimeHideGuide(timer, guide));
            }
        }

        /// <summary>
        /// 固定时间隐藏某个新手引导
        /// </summary>
        /// <param name="timer"></param>
        /// <returns></returns>
        IEnumerator SetTimeHideGuide(float timer, Guide guide)
        {
            yield return new WaitForSeconds(timer);
            HideIndexGuide(guide);
        }


        public class ProductData
        {
            public int status;  //状态
            public int sp_num;  //下级推广员的数量
        }

        public class ProductData_
        {
            public List<ProductData> Product = new List<ProductData>();
        }

        /// <summary>
        /// 获取玩家的下级推广员的数量
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public void GetProductCount(string json, int status)
        {
            int count = 0;
            ProductData_ data = new ProductData_();
            data = JsonBase.DeserializeFromJson<ProductData_>(json.ToString());

            if (data.Product[0].status == 1)
            {
                count = data.Product[0].sp_num;
                //Debug.LogError("count:" + data.Product[0].sp_num);
            }

            if (count > 0 && PlayerPrefs.GetFloat(Guide.Promote.ToString()) == 0)
            {
                OpenIndexGuide(Guide.Promote);
            }

        }
    }

}
