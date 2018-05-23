using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using MahjongLobby_AH.Network.Message;
using MahjongLobby_AH.LobbySystem.SubSystem;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class ParlorMessage : MonoBehaviour
    {
        public RawImage BossImage;  //馆主头像
        public Text BossName;  //馆主昵称
        public Text BossId; //老板id
        public Text ParlorId; //馆的id
        public Text BossWx; //馆主微信
        public Text MemberCount; //会员数
        public Text MonthVilty; //月活跃度
        public Text AllVility;  //总活跃度
        public Text Bulletin;  //公告
        [HideInInspector]
        public int iParlorId; //馆id
        public Button[] ParlorBtn; //0表示申请  1取消申请  2表示退出按钮


        //更新麻将馆的界面
        public void UpdateShow(NetMsg.ParlorInfoDef infoDef, int type)
        {
            anhui.MahjongCommonMethod.Instance.GetPlayerAvatar(BossImage, infoDef.szBossHeadimgurl);
            BossName.text = infoDef.szBossNickname;
            BossId.text ="ID: "+ infoDef.iBossId.ToString();
            ParlorId.text = infoDef.iParlorId.ToString();
            BossWx.text = infoDef.szContact;
            MemberCount.text = (infoDef.iMemberNum).ToString();
            MonthVilty.text = infoDef.iMonthVitality.ToString();
            AllVility.text = infoDef.iVitality.ToString();
            Bulletin.text = infoDef.szBulletin;
            iParlorId = infoDef.iParlorId;
            //判断该麻将馆是不是自己的麻将馆
            bool isSelfParlor = false;

            for (int i = 0; i < 4; i++)
            {
                //Debug.LogError("infoDef.iParlorId：" + infoDef.iParlorId + "，加入的麻将馆:" + GameData.Instance.PlayerNodeDef.iaJoinParlorId[i]);
                if (infoDef.iParlorId == GameData.Instance.PlayerNodeDef.iaJoinParlorId[i])
                {
                    isSelfParlor = true;
                    break;
                }
            }

            if (isSelfParlor)
            {
                ParlorBtn[type - 1].gameObject.SetActive(false);
                ParlorBtn[2 - type].gameObject.SetActive(false);
                ParlorBtn[2].gameObject.SetActive(true);
            }
            else
            {
                ParlorBtn[type - 1].gameObject.SetActive(true);
                ParlorBtn[2 - type].gameObject.SetActive(false);
                ParlorBtn[2].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 点击复制按钮
        /// </summary>
        /// <param name="tex"></param>
        public void BtnCopyWx(Text tex)
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            anhui.MahjongCommonMethod.Instance.CopyString(tex.text.ToString());
            //处理玩家复制成功之后提示文字
            anhui.MahjongCommonMethod.Instance.ShowRemindFrame("复制成功", false);
        }


        /// <summary>
        /// 加入麻将馆
        /// </summary>
        public void BtnJoinParlor()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);

            ////判断加馆冷却时间
            //if ((int)MahjongCommonMethod.Instance.DateTimeToUnixTimestamp(DateTime.Now) - GameData.Instance.PlayerNodeDef.userDef.iLeaveParlorTime < GameData.Instance.ParlorShowPanelData.ColdTimer
            //    || (int)MahjongCommonMethod.Instance.DateTimeoUnixTimestamp(DateTime.Now) - GameData.Instance.PlayerNodeDef.userDef.iKickParlorTime < GameData.Instance.ParlorShowPanelData.ColdTimer)
            //{
            //    MahjongCommonMethod.Instance.ShowRemindFrame("您最近两小时退出或者解散过馆，不可加入！");
            //    return;
            //}

            NetMsg.ClientJoinParlorReq msg = new NetMsg.ClientJoinParlorReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iParlorId = iParlorId;
            Network.NetworkMgr.Instance.LobbyServer.SendJoinParlorReq(msg);
            //关闭玩家界面
            Destroy(gameObject);
        }

        /// <summary>
        /// 取消申请加入麻将馆
        /// </summary>
        public void BtnCanelParlor()
        {
            NetMsg.ClientCAncelApplyOrJudgeApplyTooReq msg = new NetMsg.ClientCAncelApplyOrJudgeApplyTooReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iParlorId = iParlorId;
            msg.iType = 2;
            Network.NetworkMgr.Instance.LobbyServer.SendClientCAncelApplyOrJudgeApplyTooReq(msg);
            //关闭玩家界面
            Destroy(gameObject);
        }

        /// <summary>
        /// 退出麻将馆按钮
        /// </summary>
        public void BtnLevelParlor()
        {
            UIMgr.GetInstance().GetUIMessageView().Show(@"亲，退出后再加入要重新申请哦
                                                            而且会记录退馆次数的
                                                            您确定现在就退出本馆吗？", Ok, () => { });


        }

        void Ok()
        {
            NetMsg.ClientLeaveParlorReq msg = new NetMsg.ClientLeaveParlorReq();
            msg.iUserId = GameData.Instance.PlayerNodeDef.iUserId;
            msg.iParlorId = iParlorId;
            Network.NetworkMgr.Instance.LobbyServer.SendLevelParlorReq(msg);
        }

        public void BtnDestroy()
        {
            SystemMgr.Instance.AudioSystem.PlayAuto(AudioSystem.AudioType.VIEW_CLOSE);
            Destroy(gameObject);
        }
    }

}
