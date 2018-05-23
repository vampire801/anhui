using UnityEngine;
using System.Collections;
using GDGeek;
using System;
using XLua;
namespace anhui
{
    [Hotfix]
    [LuaCallCSharp]
    public class CameControler : MonoBehaviour
    {
        public View _view = null;
        // Use this for initialization
        public void PostMsg(string msg, string txt = null)
        {
            //Debug.LogError(msg+"..............");
            _fsm.init(msg);
            if (txt != null)
            {
                setText(txt);
            }
            else
            {
                setText("玩命登录中 . . . .");
            }
        }
        void setText(string txt)
        {
            _view._text.text = txt;
        }

        private FSM _fsm = new FSM();

        void Awake()
        {
            //     Debug.LogError("SDK==================");
            _fsm.addState("loading", LoadingState());
            _fsm.addState("uloading", ULoadingState());
            _fsm.init("uloading");

        }
        void Start()
        {
            // StartCoroutine(LoadCamera());
        }
        IEnumerator LoadCamera()
        {
            while (Camera.main != null)
            {
                Debug.LogWarning(Camera.main);
                transform.GetComponent<Canvas>().worldCamera = Camera.main;
                yield break;
            }
        }
        private State ULoadingState()
        {

            StateWithEventMap state = new StateWithEventMap();
            state.onStart += delegate
            { //Debug.LogError("uloading状态");
            };
            state.onOver += delegate { };
            return state;
        }

        private State LoadingState()
        {
            StateWithEventMap state = new StateWithEventMap();
            state.onStart += delegate
            {
                //   Debug.LogError("Loading++++++++++状态+++");
                //  _view._loadingPanel.GetComponent<Canvas>().worldCamera = Camera.main;
                if (MahjongLobby_AH.UIMainView.Instance)
                {
                    _view._bGIm.texture = _view._sprite[0];
                }
                else
                {
                    _view._bGIm.texture = _view._sprite[1];
                }
                _view._loadingPanel.SetActive(true);
            };
            state.onOver += delegate { _view._loadingPanel.SetActive(false); };

            return state; ;
        }

        // Update is called once per frame
        void Update()
        {
        }

    }
}