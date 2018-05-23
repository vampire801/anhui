using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using MahjongGame_AH.Data;
using DG.Tweening;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class MainViewAntiCheatingPanel : MonoBehaviour
    {
        public GameObject CheckImage;  //防作弊检测的图标显示
        public GameObject Rotate; //要旋转的图片
        public Text CheckText;  //检测的文字
        public Text NoCheatingAction;  //检测完毕
        public GameObject content;
        public List<GameObject> Point;  //点的集合
        float timer = 2f;  //检测时间
        float interval = 0.75f;  //间隔
        bool isSpwanPoint;   //是否产生点的标志位    
        float fDownTimer = 3f;  //关闭面板的倒计时时间 
        bool isStartClosePanel;  //开始倒计时关闭面板

        void Update()
        {
            if (isSpwanPoint)
            {
                interval -= Time.deltaTime;
                if (interval <= 0)
                {
                    interval = 0.75f;
                    SpwanPoint();
                }
            }

            if (isStartClosePanel)
            {
                fDownTimer -= Time.deltaTime;
                if (fDownTimer <= 0)
                {
                    isStartClosePanel = false;
                    //再打开防作弊面板之前，关闭消息接口
                    Network.NetworkMgr.Instance.GameServer.Unlock();
                    gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 播放地图寻找动画
        /// </summary>
        public void PlayAnimation()
        {
            StartCoroutine(StopAnimTion());
        }

        void Awake()
        {
            SpwanPoint();
        }


        void OnEnable()
        {
            if (GameData.Instance.AntiCheatingPanelData.iReadyShowAnti > 1)
            {
                if (Rotate.GetComponent<DOTweenAnimation>() != null)
                {
                    Rotate.GetComponent<DOTweenAnimation>().delay = 0f;
                }
                CheckImage.transform.localPosition = new Vector3(0, 80f, 0);
                CheckText.gameObject.SetActive(false);
            }
            else
            {
                if (Rotate.GetComponent<DOTweenAnimation>() != null)
                {
                    Rotate.GetComponent<DOTweenAnimation>().delay = 1f;
                }
                CheckImage.transform.localPosition = Vector3.zero;
                CheckText.gameObject.SetActive(true);
            }
        }


        void Start()
        {
            NoCheatingAction.gameObject.SetActive(false);
            if (GameData.Instance.AntiCheatingPanelData.iReadyShowAnti <= 1)
            {
                //播放动画
                PlayAnimation();
                isSpwanPoint = true;
            }
        }


        /// <summary>
        /// 产生地图的点
        /// </summary>
        void SpwanPoint()
        {
            if (Point.Count > 0)
            {
                for (int i = 0; i < Point.Count; i++)
                {
                    Destroy(Point[i].gameObject);
                }
            }
            int count = Random.Range(2, 4);
            for (int i = 0; i < count; i++)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Game/GameResult/Point0"));
                go.transform.SetParent(CheckImage.transform);
                Point.Add(go);
                go.transform.localPosition = Random.insideUnitCircle * 60f;
                go.transform.localScale = Vector3.one;
            }
        }


        /// <summary>
        /// 停止播放动画
        /// </summary>
        /// <returns></returns>
        IEnumerator StopAnimTion()
        {
            yield return new WaitForSeconds(timer);
            //开始移动检测地图图标
            Tweener tweener = CheckImage.transform.DOLocalMoveY(CheckImage.transform.localPosition.y + 80f, 0.5f);
            tweener.OnComplete(AniMationOnComplete);
            Destroy(Rotate.GetComponent<DOTweenAnimation>());
            CancelInvoke();
            CheckText.gameObject.SetActive(false);
            isSpwanPoint = false;
        }


        void AniMationOnComplete()
        {
            SpwanWarningMessage();
        }

        /// <summary>
        /// 面板更新
        /// </summary>
        public void UpdateShow()
        {
            AntiCheatingPanelData acpd = GameData.Instance.AntiCheatingPanelData;
            if (acpd.isPanelShow)
            {
                gameObject.SetActive(true);
            }
            else
            {
                isStartClosePanel = false;
                //再打开防作弊面板之前，关闭消息接口
                Debug.LogError("==========================1");
                Network.NetworkMgr.Instance.GameServer.Unlock();
                gameObject.SetActive(false);
            }
        }



        public void InitPanel()
        {
            //如果上次的代码没有执行完毕，则重新产生新的预置体

            if (GameData.Instance.AntiCheatingPanelData.isSpwanEnd)
            {
                return;
            }

            //删除之前的预置体
            Waring[] warn = content.GetComponentsInChildren<Waring>();
            for (int i = 0; i < warn.Length; i++)
            {
                Destroy(warn[i].gameObject);
            }
            SpwanWarningMessage();
        }
        public void BtnClose()
        {
            SystemMgr.Instance.AudioSystem.PlayManual(GameSystem.SubSystem.AudioSystem.AudioMenel.Btn_Click, false, false);

            if (GameData.Instance.AntiCheatingPanelData.isFinishPutCard)
            {
                GameData.Instance.AntiCheatingPanelData.isFinishPutCard = false;
                //再打开防作弊面板之前，关闭消息接口
                Network.NetworkMgr.Instance.GameServer.Unlock();
            }

            gameObject.SetActive(false);
        }

        /// <summary>
        /// 产生警告信息
        /// </summary>
        void SpwanWarningMessage()
        {
            int count = 0;  //表示显示几条警告作弊信息
            AntiCheatingPanelData acpd = GameData.Instance.AntiCheatingPanelData;
            //先获取玩家玩家的ip信息
            Dictionary<string, List<int>> ip = acpd.GetPlayerIpSame();

            bool isSpwanIp = false;

            foreach (string key in ip.Keys)
            {
                if (ip[key].Count >= 2)
                {
                    isSpwanIp = true;
                }
            }

            if (isSpwanIp)
            {
                //产生预置体
                GameObject go = Instantiate(Resources.Load<GameObject>("Game/AntiCheck/Warning"));
                go.transform.SetParent(content.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                go.GetComponent<Waring>().Ip(ip, 1);
                count++;
            }
            //Dictionary<string, List<int>> mac = acpd.GetPlayerMacIdSame();

            //if (mac.Count > 0)
            //{
            //    //产生预置体
            //    GameObject go = Instantiate(Resources.Load<GameObject>("Game/AntiCheck/Warning"));
            //    go.transform.SetParent(content.transform);
            //    go.transform.localScale = Vector3.one;
            //    go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
            //    go.GetComponent<Waring>().Ip(mac, 2);
            //    count++;
            //}

            bool isSpwanDistance = false;
            Dictionary<string, List<int>> distance = acpd.GetPlayerDistance();

            foreach (string key in distance.Keys)
            {//0--4
                Debug.LogWarning("distancekey：" + key + ",distancevalue:" + distance[key].Count + "======================");
                if (distance[key].Count >= 2)
                {
                    isSpwanDistance = true;
                }
            }

            if (isSpwanDistance)
            {
                //产生预置体
                GameObject go = Instantiate(Resources.Load<GameObject>("Game/AntiCheck/Warning"));
                go.transform.SetParent(content.transform);
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y, 0);
                go.GetComponent<Waring>().Ip(distance, 3);
                count++;
            }

            content.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100 * count);

            //如果没有作弊行为
            if (count <= 0)
            {
                NoCheatingAction.gameObject.SetActive(true);
            }

            //在产生所有预置体之后，2s之后，关闭面板
            if (GameData.Instance.AntiCheatingPanelData.iReadyShowAnti <= 1)
            {
                isStartClosePanel = true;
            }

            acpd.isSpwanEnd = true;

        }

    }

}
