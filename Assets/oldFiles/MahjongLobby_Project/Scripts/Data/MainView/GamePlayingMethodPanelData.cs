using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;

namespace MahjongLobby_AH.Data
{
    [Hotfix]
    [LuaCallCSharp]
    public class GamePlayingMethodPanelData
    {

        static GamePlayingMethodPanelData instance;
        public bool PanelShow;  //是否显示的标志位
        public string Content;  //要显示的对应的麻将玩法的内容
        public int GameIndex;  //对应麻将游戏的下标，通过下标去读取对应的text文档
        public Button Button_0;  //第一种玩法的按钮
        public int CountyId;  //在规则面板切换县城id
        public int Status; //切换状态

        private bool isOnce = true;
        /// <summary>
        /// 读取游戏玩法的txt文档
        /// </summary>
        /// <returns></returns>
        public IEnumerator ReadPlayingMethond_(string FileName, bool isGame = false)
        {
#if UNITY_EDITOR
            string path = "file:///" + Application.dataPath + "/StreamingAssets/GamePlayMethoding/" + FileName + ".txt";
#elif UNITY_IPONE || UNITY_ANDROID||UNITY_IOS
            string path = Application.streamingAssetsPath +"/GamePlayMethoding/"+FileName + ".txt";
#endif            
            if (path.Contains("//"))
            {
                WWW www = new WWW(path);
                while (!www.isDone)
                {
                    yield return www;
                }
                if (www.error != null)
                {
                    Debug.LogErrorFormat("读取游戏玩法的txt文件失败{0}：{1}" ,www.error,FileName);
                }
                Content = www.text;
            }
            else
            {
                Content = System.IO.File.ReadAllText(path);
            }

            if (isGame)
            {
                MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.content.text = Content;
                //UIMainView.Instance.GamePlayingMethodPanel.content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, UIMainView.Instance.GamePlayingMethodPanel.content.preferredHeight);
                if (MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.content.preferredHeight > MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.height)
                {
                    MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.rect.enabled = true;
                    MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.PmParent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.content.preferredHeight + 50);
                    //MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.PmParent.transform.localPosition = MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.PmParentPos;
                    //                UIMainView.Instance.GamePlayingMethodPanel.content.transform.localPosition = new Vector3(0, -15, 0);
                }
                //else
                //{
                //    MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.rect.enabled = true;
                //}

                Vector3 pos = MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.content.transform.localPosition;
                pos.x = MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.rect.preferredWidth / 2;
                MahjongGame_AH.UIMainView.Instance.GamePlayingMethodPanel.content.transform.localPosition = pos;
            }
            else
            {
                UIMainView.Instance.GamePlayingMethodPanel.content.text = Content;
                //UIMainView.Instance.GamePlayingMethodPanel.content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, UIMainView.Instance.GamePlayingMethodPanel.content.preferredHeight);
                if (UIMainView.Instance.GamePlayingMethodPanel.content.preferredHeight > UIMainView.Instance.GamePlayingMethodPanel.height)
                {
                    UIMainView.Instance.GamePlayingMethodPanel.rect.enabled = true;
                    UIMainView.Instance.GamePlayingMethodPanel.PmParent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, UIMainView.Instance.GamePlayingMethodPanel.content.preferredHeight + 50);
                    UIMainView.Instance.GamePlayingMethodPanel.PmParent.transform.localPosition = UIMainView.Instance.GamePlayingMethodPanel.PmParentPos;
                    //                UIMainView.Instance.GamePlayingMethodPanel.content.transform.localPosition = new Vector3(0, -15, 0);
                }
                else
                {
                    UIMainView.Instance.GamePlayingMethodPanel.rect.enabled = true;
                }
            }
        }


        /// <summary>
        /// 获取已开放城市的信息
        /// </summary>
        /// <param name="json"></param>
        public void GetCityMessage(string json, int status)
        {
            anhui.MahjongCommonMethod mcm = anhui.MahjongCommonMethod.Instance;
            SelectAreaPanelData.CityMessageData data = new SelectAreaPanelData.CityMessageData();
            data = JsonBase.DeserializeFromJson<SelectAreaPanelData.CityMessageData>(json.ToString());
            if (data.CityData[0].status != 1)
            {
                Debug.LogError("获取网页json数据状态错误,status:" + data.CityData[0].status);
                return;
            }

            //保存获取的城市信息
            for (int i = 0; i < data.CityData.Count; i++)
            {
                for (int j = 0; j < data.CityData[i].data.Count; j++)
                {
                    int cityId = System.Convert.ToInt32(data.CityData[i].data[j].cityId);
                    if (mcm._dicCityConfig.ContainsKey(cityId))
                    {
                        mcm._dicCityConfig[cityId].VALID = System.Convert.ToInt32(data.CityData[i].data[j].flag);
                    }
                }
            }
        }

        /// <summary>
        /// 获取已开放县城的信息
        /// </summary>
        /// <param name="json"></param>
        public void GetCountyMessage(string json, int status)
        {
            anhui.MahjongCommonMethod mcm = anhui.MahjongCommonMethod.Instance;
            SelectAreaPanelData.CountyMessageData data = new SelectAreaPanelData.CountyMessageData();
            data = JsonBase.DeserializeFromJson<SelectAreaPanelData.CountyMessageData>(json.ToString());
            if (data.CountyData[0].status != 1)
            {
                Debug.LogError("获取已开放县城的信息错误,status:" + data.CountyData[0].status);
                return;
            }

            //保存获取的城市信息
            for (int i = 0; i < data.CountyData.Count; i++)
            {
                for (int j = 0; j < data.CountyData[i].data.Count; j++)
                {
                    int countyId = System.Convert.ToInt32(data.CountyData[i].data[j].countyId);
                    if (mcm._dicDisConfig.ContainsKey(countyId))
                    {
                        mcm._dicDisConfig[countyId].VALID = System.Convert.ToInt32(data.CountyData[i].data[j].flag);
                    }
                }
            }

            //为方法id赋值
            for (int i = 0; i < mcm._districtConfig.Count; i++)
            {
                if (mcm._districtConfig[i].COUNTY_ID == GameData.Instance.SelectAreaPanelData.iCountyId && mcm._districtConfig[i].VALID == 2)
                {
                    string[] id = mcm._districtConfig[i].METHOD.Split('_');
                    for (int k = 0; k < id.Length; k++)
                    {
                        int ID = System.Convert.ToInt16(id[k]);
                        if (ID != 0)
                        {
                            Debug.LogWarning("获取已开放县城的信息");
                            mcm.lsMethodId.Add(ID);
                        }
                    }
                }
            }

            GameData gd = GameData.Instance;
            GamePlayingMethodPanelData gpmpd = gd.GamePlayingMethodPanelData;
            gpmpd.PanelShow = true;
            //gpmpd.GameIndex = 1; //默认打开第一个     
            SystemMgr.Instance.GamePlayingMethodSystem.UpdateShow();

        }
    }
}
