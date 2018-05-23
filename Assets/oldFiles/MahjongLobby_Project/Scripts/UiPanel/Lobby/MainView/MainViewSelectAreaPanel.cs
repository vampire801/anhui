using UnityEngine;
using UnityEngine.UI;
using MahjongLobby_AH.Data;
using System;
using DG.Tweening;
using System.Collections.Generic;
using MahjongLobby_AH.Network;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewSelectAreaPanel : MonoBehaviour
    {
        public const string MESSAGE_BTNSELECTCITY = "MainViewSelectAreaPanel.MESSAGE_BTNSELECTCITY";  //点击选择城市按钮
        public const string MESSAGE_BTNSELECTCOUNTY = "MainViewSelectAreaPanel.MESSAGE_BTNSELECTCOUNTY";  //点击选择城市下级县城按钮
        public const string MESSAGE_BTNCOUNTYCLOSE = "MainViewSelectAreaPanel.MESSAGE_BTNCOUNTYCLOSE";  //点击关闭选择县城的按钮
        public GameObject _gPanelBg;
        public GameObject[] _NeedClose;
        public Transform []_pos=new Transform [6];
        public Sprite[] Sprite_Choice;  //选择按钮的颜色。0表示灰色，其他颜色随机
        [HideInInspector]
        public float fDelayTime = 5f;//延迟时间  

        public float fSpeed = 100f;  //移动速度

        SelectAreaPanelData sapd;
        float speed;
        public Transform[] CityPosition; //所有城市的位置
        public GameObject cityMap_go; //城市面板显示               
        public GameObject[] countyMap_go;  //县的显示面板 
        public Button BtnClose; //关闭按钮
        public GameObject proviceText;  //显示省份的文字

        public void Awake()
        {
            sapd = GameData.Instance.SelectAreaPanelData;
            speed = fSpeed;
            if (GameData.Instance.SelectAreaPanelData == null)
            {
                return;
            }
            if (!sapd.IsGetCityMessage)
            {
                if (SDKManager.Instance.IOSCheckStaus == 1)
                {
                    //获取所有城市的信息
                    anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL_T + SelectAreaPanelData.url_city
                        , null, sapd.GetCityMessage, "CityData");

                    //获取所有县城的信息
                    anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL_T + SelectAreaPanelData.url_county
                        , null, sapd.GetCountyMessage, "CountyData");
                    sapd.IsGetCityMessage = true;
                }
                else
                {
                    //获取所有城市的信息
                    anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL + SelectAreaPanelData.url_city
                        , null, sapd.GetCityMessage, "CityData");

                    //获取所有县城的信息
                    anhui.MahjongCommonMethod.Instance.GetPlayerMessageData_IE(LobbyContants.MAJONG_PORT_URL + SelectAreaPanelData.url_county
                        , null, sapd.GetCountyMessage, "CountyData");
                    sapd.IsGetCityMessage = true;
                }


            }
            else
            {
                ChangeScale();
            }
        }

        /// <summary>
        /// 改变地图大小
        /// </summary>
        public void ChangeScale()
        {
            //先把地图从小变大
            Tweener tweener_0 = cityMap_go.transform.Find("CityMap").DOScale(Vector3.one, 1f);
            tweener_0.OnComplete(ChangeScale_Oncomplete);
        }


        /// <summary>
        /// 实现显示哪个区域
        /// </summary>
        void ChangeScale_Oncomplete()
        {
            //如果没有获取到区域的信息
            if (sapd.listCityMessage.Count == 0)
            {
                Debug.LogWarning("玩家获取区域信息失败");
            }
            //Debug.LogError("如果没有获取到区域的信息：" + sapd.listCityMessage.Count);
            for (int i = 0; i < sapd.listCityMessage.Count; i++)
            {
                int id = Convert.ToInt32(sapd.listCityMessage[i].cityId);
               // Debug.LogError ("id = " + id + ",flag = " + sapd.listCityMessage[i].flag);
                if (Convert.ToInt32(sapd.listCityMessage[i].flag) == 0)
                {
                    continue;
                }
                else
                {
                    Transform trans = null;
                    for (int j = 0; i < CityPosition.Length; j++)
                    {
                        if (Convert.ToInt32(CityPosition[j].name) == Convert.ToInt32(sapd.listCityMessage[i].cityId))
                        {
                            trans = CityPosition[j];
                            break;
                        }
                    }
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/SelectAreaPanel/SelectArea"));
                    go.transform.SetParent(trans);
                    //go.transform.localScale = Vector3.zero;
                    //go.transform.localPosition = new Vector3(0, 50f, 0);                    
                    go.transform.localEulerAngles = Vector3.zero;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localScale = Vector3.one;
                    go.transform.Find("SelectArea/City").gameObject.SetActive(true);
                    go.transform.Find("SelectArea/City").GetComponent<Text>().text = sapd.listCityMessage[i].cityName;
                    go.transform.Find("SelectArea/County").gameObject.SetActive(false);
                    go.transform.Find("SelectArea/Parlor").gameObject.SetActive(false);
                    //Debug.LogError("------" + sapd.listCityMessage[i].flag);
                    if (Convert.ToInt32(sapd.listCityMessage[i].flag) == 1)
                    {
                        go.transform.Find("SelectArea/Image").GetComponent<Image>().sprite = Sprite_Choice[0];
                        go.transform.Find("SelectArea/Status").gameObject.SetActive(true);
                    }
                    else
                    {
                        int index = UnityEngine.Random.Range(1, Sprite_Choice.Length);
                        go.transform.Find("SelectArea/Image").GetComponent<Image>().sprite = Sprite_Choice[index];
                        go.transform.Find("SelectArea/Status").gameObject.SetActive(false);
                        //为选择城市按钮添加方法                                   
                        go.transform.Find("SelectArea").GetComponent<Button>().onClick.AddListener(delegate () { ChoiceCity(id); });
                    }

                    sapd.isStartMove = true;
                    //sapd.isMove = true;
                    //暂时不用按钮的动态效果 TODO:
                    //FlyEffect(go, Vector3.zero, 1);
                }
            }
        }


        /// <summary>
        /// 点击城市选择按钮
        /// </summary>
        /// <param name="CityIndex"></param>
        public void ChoiceCity(int CityIndex)
        {
            Debug.LogWarning(CityIndex )    ;
            sapd.isStartMove = false;
            sapd.isMove = false;
            sapd.iCityId = CityIndex;
            ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            for (int i = 0; i < countyMap_go.Length; i++)
            {
                //Debug.LogError("城市按钮：" + countyMap_go[i].name + "," + CityIndex+","+ countyMap_go.Length);
                if (Convert.ToInt32(countyMap_go[i].name) == CityIndex)
                {
                    countyMap_go[i].SetActive(true);
                    if (!sapd.IsGetCountyMessage[CityIndex- int.Parse(pspd.listCityMessage[0].cityId )])
                    {
                        SpwanCountyChioceBtn(countyMap_go[i], CityIndex);
                    }
                    //关闭省份的名字
                    proviceText.SetActive(false);
                }
                else
                {
                    countyMap_go[i].SetActive(false);
                }
            }

            if (sapd.iOpenStatus == 3)
            {
                pspd.iCityId[0] = CityIndex;
            }

            if (sapd.iOpenStatus == 4)
            {
                pspd.iCityId[1] = CityIndex;
            }


            anhui.MahjongCommonMethod.Instance.HasClicked((int)anhui.MahjongCommonMethod.StateType.CompeletChoiceMap);
        }


        /// <summary>
        /// 产生选择具体县城的按钮
        /// </summary>
        /// <param name="countyId"></param>
        void SpwanCountyChioceBtn(GameObject county, int cityIndex)
        {
            //Debug.LogError("county.transform.FindChild:" + county.name+","+ cityIndex);
            ContyData[] pos = county.transform.Find("Map").GetComponentsInChildren<ContyData>();

            //产生对应的县的预置体     
            for (int i = 0; i < sapd.dicCountyMessage[cityIndex].Count; i++)
            {
                //Debug.LogError("县城的信息:" + sapd.dicCountyMessage[cityIndex][i].countyName);
                int id = Convert.ToInt32(sapd.dicCountyMessage[cityIndex][i].countyId);
                Transform trans = null;
                if (Convert.ToInt32(sapd.dicCountyMessage[cityIndex][i].flag) == 0)
                {
                    continue;
                }
                else
                {
                    for (int j = 0; j < pos.Length; j++)
                    {
                        //Debug.LogError("pos[j].name：" + pos[j].name + ",sapd.dicCountyMessage[cityIndex][i].countyId：" + sapd.dicCountyMessage[cityIndex][i].countyId);
                        if (Convert.ToInt32(pos[j].name) == Convert.ToInt32(sapd.dicCountyMessage[cityIndex][i].countyId))
                        {
                            trans = pos[j].transform;
                            //Debug.LogError("pos[j].transform:" + trans.transform.localPosition);
                        }
                    }
                    if (trans == null) continue;
                    GameObject go = Instantiate(Resources.Load<GameObject>("Lobby/SelectAreaPanel/SelectArea"));
                    go.transform.SetParent(county.transform.Find("Map"));
                    go.transform.name = trans.name;
                    go.transform.Find("SelectArea/City").gameObject.SetActive(false);
                    go.transform.Find("SelectArea/County").gameObject.SetActive(true);
                    go.transform.Find("SelectArea/County").GetComponent<Text>().text = sapd.dicCountyMessage[cityIndex][i].countyName;
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, 0);
                    go.transform.localEulerAngles = Vector3.zero;

                    //显示麻将馆数量
                    //go.transform.Find("SelectArea/Parlor").gameObject.SetActive(true);
                    //go.transform.Find("SelectArea/Parlor/Text").GetComponent<Text>().text = sapd.dicCountyMessage[cityIndex][i].num;

                    //即将开启
                    if (Convert.ToInt32(sapd.dicCountyMessage[cityIndex][i].flag) == 1)
                    {
                        go.transform.Find("SelectArea/Image").GetComponent<Image>().sprite = Sprite_Choice[0];
                        go.transform.Find("SelectArea/Status").gameObject.SetActive(true);
                    }
                    //已经开启
                    else
                    {
                        int index = UnityEngine.Random.Range(1, Sprite_Choice.Length);
                        go.transform.Find("SelectArea/Image").GetComponent<Image>().sprite = Sprite_Choice[index];
                        go.transform.Find("SelectArea/Status").gameObject.SetActive(false);
                        go.transform.Find("SelectArea").GetComponent<Button>().onClick.AddListener(delegate { AddDelgeteToCountyBtn(id); });
                    }
                    //FlyEffect(go, trans.transform.localPosition, 2);
                }
            }
            GameData.Instance.SelectAreaPanelData.IsGetCountyMessage[cityIndex - int.Parse(GameData.Instance.ParlorShowPanelData.listCityMessage[0].cityId)] = true;
        }

        void ScaleAreaPanel()
        {
            SelectAreaPanelData sd = GameData.Instance.SelectAreaPanelData;
            DOTweenAnimation DoTwAnim = _gPanelBg.GetComponent<DOTweenAnimation>();
            Tweener dt = DoTwAnim.tween as Tweener;
            Debug.Log(sd.pos_index + "__" + _pos[sd.pos_index].localPosition);
            dt.ChangeEndValue(_pos[sd.pos_index].localPosition);
            CloseNeed();
            DoTwAnim.DORestartById("1");
            //  Debug.Log("动画前原来：" + _gPanelBg.transform.localScale);
            DoTwAnim.DORestartById("2");
            //dt.Restart();
            DoTwAnim.tween.OnComplete(() =>
            {
                //DoTwAnim.DORestartById("3");
                //  Debug.LogWarning("缩放完成");
                Ok();
            });
        }
        //绑定按钮
        void AddDelgeteToCountyBtn(int index)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            sapd.iCountyId = index;
            NetMsg.ClientCityCountyReq msg = new NetMsg.ClientCityCountyReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iCityId = sapd.iCityId;
            msg.iCountyId = sapd.iCountyId;

            Debug.Log("msg.iCityId：" + msg.iCityId + ",msg.iCountyId:" + msg.iCountyId);
            if (sapd.iCountyId != GameData.Instance.PlayerNodeDef.iCountyId)
                NetworkMgr.Instance.LobbyServer.SendCityCountyReq(msg);
            //给出选择成功提示，同时修改玩法界面
            ScaleAreaPanel();
          //  UIMgr.GetInstance().GetUIMessageView().Show("恭喜您修改地区成功", Ok);

            //如果是正常写入玩家市县id
            if (sapd.iOpenStatus == 1)
            {
                //sapd.iCountyId = index;
                //NetMsg.ClientCityCountyReq msg = new NetMsg.ClientCityCountyReq();
                //msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
                //msg.iCityId = sapd.iCityId;
                //msg.iCountyId = sapd.iCountyId;
                //NetworkMgr.Instance.LobbyServer.SendCityCountyReq(msg);
            }
            //如果是在创建房间点击选择市县id
            else if (sapd.iOpenStatus == 2)
            {
                sapd.iCountyId = index;
                GameData.Instance.ParlorShowPanelData.iCountyId[2] = index;
                PlayerPrefs.SetInt("CityId" + GameData.Instance.PlayerNodeDef.iUserId, sapd.iCityId);
                PlayerPrefs.SetInt("CountyId" + GameData.Instance.PlayerNodeDef.iUserId, sapd.iCountyId);
                //修改玩法id
                anhui.MahjongCommonMethod.Instance.lsMethodId = new List<int>();
                string[] id = anhui.MahjongCommonMethod.Instance._dicDisConfig[index].METHOD.Split('_');
                for (int k = 0; k < id.Length; k++)
                {
                    int ID = Convert.ToInt16(id[k]);
                    if (ID != 0)
                    {
                        Debug.LogWarning("绑定按钮");
                        anhui.MahjongCommonMethod.Instance.lsMethodId.Add(ID);
                    }
                }

                //修改初始方法的id
                GameData.Instance.CreatRoomMessagePanelData.MethodId = anhui.MahjongCommonMethod.Instance.lsMethodId[0];

                CreatRoomMessagePanelData crmpd = GameData.Instance.CreatRoomMessagePanelData;

                //在这里初始化创建房间的参数
                for (int i = 0; i < crmpd.iParamOpenRoomMessage.Length; i++)
                {
                    crmpd.iParamOpenRoomMessage[i] = 0;
                }
                Debug.LogWarning (GameData.Instance.ParlorShowPanelData.iCountyId[2] + "," + crmpd.CreatRoomType);
                //更新面板
                UIMainView.Instance.CreatRoomMessagePanel.UpdateShowMethod(GameData.Instance.CreatRoomMessagePanelData.MethodId);
                UIMainView.Instance.CreatRoomMessagePanel.SpwanPlayMethodBtn();
                SystemMgr.Instance.CreatRoomMessageSystem.UpdateShow();
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
            }


            //if (sapd.iOpenStatus == 5)
            //{
            //    sapd.iCountyId = index;
            //    PlayerPrefs.SetInt("CityId", sapd.iCityId);
            //    PlayerPrefs.SetInt("CountyId", sapd.iCountyId);
            //}

            //if (sapd.iOpenStatus > 1)
            //{

            //}

            //ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
            //if (sapd.iOpenStatus == 3)
            //{
            //    pspd.iCountyId[0] = index;
            //}

            //if (sapd.iOpenStatus == 4)
            //{
            //    pspd.iCountyId[1] = index;
            //}

            //if (sapd.iOpenStatus == 6)
            //{
            //    GameData.Instance.GamePlayingMethodPanelData.CountyId = index;
            //}

            //申请开馆信息
            if (sapd.iOpenStatus == 7)
            {
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                pspd.iCountyId[0] = index;
                UIMainView.Instance.ParlorShowPanel.ApplyCreatParlorCertl.GetTgMessage(index);
            }
        }
        private void OpenNeed()
        {
            for (int i = 0; i < _NeedClose.Length; i++)
            {
                _NeedClose[i].SetActive(true );
            }
        }
        private void CloseNeed()
        {
            for (int i = 0; i < _NeedClose.Length ; i++)
            {
                _NeedClose[i].SetActive(false);
            }
        }

        void Ok()
        {  
            _gPanelBg.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            _gPanelBg.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
            _gPanelBg.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            sapd.isPanelShow = false;
            for (int i = 0; i < countyMap_go.Length; i++)
            {
                countyMap_go[i].SetActive(false);
            }
            SystemMgr.Instance.SelectAreaSystem.UpdateShow();
            //刷新大厅面板
            if (sapd.iOpenStatus == 5 && GameData.Instance.LobbyMainPanelData.isPanelShow)
            {
                SystemMgr.Instance.LobbyMainSystem.UpdateShow();
                return;
            }


            //如果选择玩法规则
            if (sapd.iOpenStatus == 6)
            {
                Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_PALYINGMETHOD);
                //SystemMgr.Instance.GamePlayingMethodSystem.UpdateShow(1);
            }

            if (sapd.iOpenStatus == 3 || sapd.iOpenStatus == 4)
            {                
                ParlorShowPanelData pspd = GameData.Instance.ParlorShowPanelData;
                pspd.pageId = new int[] { 1, 1, 1 };
                pspd.iNowCheckType = 1;
                pspd.isGetAllData_PointCountyParlor = new bool[] { false, false, false };
                pspd.TimeOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
                pspd.MemberCountOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
                pspd.ActivityOrderParlorMessage = new List<NetMsg.ParlorInfoDef>();
                UIMainView.Instance.ParlorShowPanel.BtnSelectAreaOk(sapd.iOpenStatus);               
                return;
            }

            //第一次进入麻将馆
            if (sapd.iOpenStatus == 8)
            {
                //获取玩家的申请馆的id
                GameData.Instance.ParlorShowPanelData.FromWebGetApplyParlorId(6, 1);
                SDKManager.Instance.GetComponent<anhui.CameControler>().PostMsg("loading", "正在获取您的麻将馆信息");
            }                     
        }

        /// <summary>
        /// 实现按钮飞的效果
        /// </summary>
        /// <param name="go"></param>
        /// <param name="target"></param>
        /// <param name="status"></param>
        void FlyEffect(GameObject go, Vector3 target, int status)
        {
            if (go == null)
            {
                return;
            }
            //将按钮移动
            Tweener tweener_1 = go.transform.DOLocalMoveY(target.y, 0.4f);
            tweener_1.SetEase(Ease.InElastic);
            //将按钮放大
            Tweener tweener_2 = go.transform.DOScale(Vector3.one, 0.2f);
            tweener_2.SetEase(Ease.InElastic);
            if (status == 1)
            {
                tweener_2.OnComplete(FlyOnComplete);
            }

        }

        /// <summary>
        /// 地图图标之后开始移动地图
        /// </summary>
        void FlyOnComplete()
        {
            sapd.isStartMove = true;
            //sapd.isMove = true;
        }


        void Update()
        {

#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                sapd.isMove = false;
                fDelayTime = 5f;
            }

#elif UNITY_ANDROID || UNITY_IPHONE
                              
            if (Input.touchCount >= 1&&(Input.GetTouch(0).phase == TouchPhase.Moved))
            {                              
                sapd.isMove = false;
                fDelayTime = 5f;             
            }
#endif
            if (sapd.isStartMove)
            {
                if (sapd.isMove)
                {
                    cityMap_go.GetComponent<RectTransform>().Translate(Vector3.up * speed * Time.deltaTime);
                    //如果地图运动到顶部时或底部时，开始反向移动                
                    if (cityMap_go.transform.localPosition.y > -375f)
                    {
                        speed = -fSpeed;
                    }

                    if (cityMap_go.transform.localPosition.y < -1255f)
                    {
                        speed = fSpeed;
                    }
                }
                else
                {
                    if (fDelayTime <= 0)
                    {
                        //sapd.isMove = true;
                    }
                    else
                    {
                        fDelayTime -= Time.deltaTime;
                    }
                }
            }
        }


        /// <summary>
        /// 更新选择区域的面板
        /// </summary>
        public void UpdateShow()
        {
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            if (sapd.isPanelShow)
            {
                gameObject.SetActive(true);
                OpenNeed();
                _gPanelBg.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
                _gPanelBg.transform.localScale = new Vector3(1, 1, 1);
                //修改选择地区的关闭按钮状态
                if (sapd.iOpenStatus == 1)
                {
                    BtnClose.gameObject.SetActive(false);
                }
                else
                {
                    BtnClose.gameObject.SetActive(true);
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 点击关闭县城选择按钮
        /// </summary>
        public void BtnCloseCountyPanel()
        {

            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            for (int i = 0; i < countyMap_go.Length; i++)
            {
                countyMap_go[i].SetActive(false);
            }
            //打开省份的名字
            proviceText.SetActive(true);
            //sapd.isStartMove = true;
            //sapd.isMove = true;
        }
        public void PlayCloseVoice()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            SelectAreaPanelData sapd = GameData.Instance.SelectAreaPanelData;
            ScaleAreaPanel();
            for (int i = 0; i < countyMap_go.Length; i++)
            {
                countyMap_go[i].SetActive(false);
            }
        }


        #region 玩法收集相关       
        public GameObject SharePanel; //分享界面     

        //收集玩法
        public void BtnMethod()
        {
            //跳转到客服界面
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Messenger_anhui.Broadcast(MainViewLobbyPanel.MESSAGE_CUSTOMSEVERBTN);
            UIMainView.Instance.CustomSeverPanel.PointIndex = 2;
        }


        //寻求代理
        public void Daili()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            SharePanel.SetActive(true);
        }

        //了解详情
        public void Message()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Application.OpenURL("https://sx.ibluejoy.com/m/pages/extend/area_extend.html");
        }       


        /// <summary>
        /// 分享
        /// </summary>
        /// <param name="index">0表示分享到微信 1表示朋友圈</param>
        public void SharePointPlace(string index)
        {
            Messenger_anhui<string>.Broadcast(MainViewShareWxPanel.MESSAGE_SHAREWX, index);
        }

        #endregion

    }

}
