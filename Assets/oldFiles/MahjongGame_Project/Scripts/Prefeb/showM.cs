using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using DG.Tweening;
using XLua;

namespace MahjongGame_AH {
    [Hotfix]
    [LuaCallCSharp]
    public class showM : MonoBehaviour
    {
        
        public Image[] m;
        internal  bool isOpen;
        private Tweener []tt=new Tweener [3];
        public void Play()
        {
            for (int i = 0; i < m.Length ; i++)
            {
                tt[i] = m[i].DOColor(new Color(1, 1, 1, 0), 0.4f).From (false  );
                tt[i].SetLoops(-1,(LoopType)1);
                tt[i].Restart ();
            }
        }
        public void Stop()
        {
            for (int i = 0; i < m.Length ; i++)
            {
                tt[i].OnPause (()=> {
                    tt[i].ForceInit();
                    m[i].color = new Color(1, 1, 1, 1);
                });
                tt[i].Pause ();
            }
        }
      
    }
}

