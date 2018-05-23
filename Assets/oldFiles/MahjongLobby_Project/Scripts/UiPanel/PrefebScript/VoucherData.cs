using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using XLua;

[Hotfix]
[LuaCallCSharp]
public class VoucherData : MonoBehaviour
{
    public Text Sign; //人民币符号
    public Image Guoqi;  //过期标示
    public Image Bg;  //背景图
    public Toggle Image_recomment;
    public Toggle Tog_isChoice;
    public Text Text_amount;
    public Text Text_limit;
    public Text Text_validityTime;
    public Image imageText;
    public GameObject _tNull;
    public int index_voucher;

}
