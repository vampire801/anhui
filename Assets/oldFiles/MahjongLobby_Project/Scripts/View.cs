using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;
namespace anhui
{
    [Hotfix]
    [LuaCallCSharp]
    public class View : MonoBehaviour
    {
        public GameObject _loadingPanel;
        public Texture2D[] _sprite;
        public RawImage _bGIm;
        public Text _text;

    }
}