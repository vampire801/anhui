using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XLua;

namespace MahjongGame_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class ShortTalkData
    {
        public bool isShowPanel;
        //配置读取快捷语保存
        public List<string >[] szShortTalk = {new List<string>(), new List<string>() } ;
        //实时语音保存信息队列
        public List<VoiceData> _voiceData = new List<VoiceData>();
        public Dictionary <string,string> _DownLoadFilePath = new Dictionary<string, string> ();
      //  private static List<string> a;

        public void AddVoiceData(VoiceData prVoice)
        {
            _voiceData.Add(prVoice);
        }
        public void DeleteVoiceData()
        {
            _voiceData.RemoveAt(0);
        }
        public class VoiceData
        {
            internal string _url;
            internal float _time;
            internal bool _hasRead;
            internal bool _isSelf;
            internal string _headUrl;
        }
    }
    //最长255
    // enum TextID : int
    //{
    //    DajiaHao = 1,//.大家好，很高兴见到各位！
    //    Kuaidianba = 2,//.快点吧，等到花儿都谢了！
    //    Jiaopen = 3,//.我们交个朋友吧，能不能告诉我你的联系方式
    //    Paitaihao = 4,//.你的牌打得太好了！
    //    Buhaoyisi = 5,//.哎，各位，真是不好意思啊，我得离开一会儿啊！
    //    WoHule = 6,//.哈哈，我胡了！
    //    Duanxian = 7,//.怎么又断线了，网络怎么这么差啊！
    //    Chiyige = 8,//.好歹让我吃一个嘛！
    //    Hehe = 9,//.呵呵！
    //    Xiaci = 10//.下次咋们再玩吧，我要走了！
    //}
    enum FaceID
    {
        CiYa=1,
        NanShou,
        Weixiao,
        LengHan,
        Ciya,
        JinXia=6,
        Jiong,
        Keshui,
        Fanu,
        Yun,
        Bang=11,
        Touxiang,
        Se,
        Haixiu,
        Shaoxiang,
        Xiexie=16,
        Liulei,
        OuTu,
        Kelian,
        Jiayou,
        Chijing=21,
        Baoyou,
        Caidao,
        Jizhi,
        XiangKu,
        Goust=26,
        BaoChou
    }

}

