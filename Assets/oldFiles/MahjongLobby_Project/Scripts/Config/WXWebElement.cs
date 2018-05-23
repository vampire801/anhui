using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using XLua;

namespace MahjongLobby_AH
{
    [Hotfix]
    [LuaCallCSharp]
    public class WXWebElement
    {
        public EleAccessToken actoken=new EleAccessToken ();
        public EleRefreshToken retoken=new EleRefreshToken();
        public EleUserInfo usinfo=new EleUserInfo( );
        /// <summary>
        /// 保存网络读取微信数据
        /// </summary>
        /// <param name="0-Actoken,1-Retoken,2-Usinfo"></param>
 //       public void SaveWXElement(uint savetype, EleAccessToken Actoken, EleRefreshToken Retoken, EleUserInfo Usinfo)
 //       {
 //           switch (savetype )
 //           {
 //               case 0 :
 //                   actoken = Actoken;
 //                   StreamWriter writer0 = new StreamWriter(Application.persistentDataPath   + "/access.wx", false , Encoding.UTF8);
 //                   StringBuilder str0 = new StringBuilder("{\"Listaccesstoken\":[{");
 //                   str0.AppendFormat("\"access_token\":\"{0}\",\"expires_in\":{1},\"refresh_token\":\"{2}\",\"openid\":\"{3}\",\"scope\":\"{4}\",\"unionid\":\"{5}\"", actoken.access_token,actoken.expires_in,actoken.refresh_token,actoken.openid,actoken.scope,actoken.unionid);
 //                   str0.Append("}]}");
 //                   writer0.Write(str0);
 //                   writer0.Close();
 //                   writer0.Dispose();
 //                   break;
 //               case 1:
 //                   retoken  =Retoken ;
 //                   StreamWriter writer1 = new StreamWriter(Application.persistentDataPath + "/refresh.wx",false  , Encoding.UTF8);
 //                   StringBuilder str1 = new StringBuilder("{\"Listrefreshtoken\":[{");
 //                   str1.AppendFormat("\"access_token\":\"{0}\",\"expires_in\":{1},\"refresh_token\":\"{2}\",\"openid\":\"{3}\",\"scope\":\"{4}\"", retoken.access_token, retoken.expires_in,retoken.refresh_token,retoken.openid,retoken.scope);
 //                   str1.Append("}]}");
 //                   writer1.Write(str1);
 //                   writer1.Close();
 //                   writer1.Dispose();
 //                   break;
 //               case 2:
 //                   usinfo = Usinfo;
 //                   GameData.Instance.PlayerNodeDef.szHeadimgurl = usinfo.headimgurl;
 //                   GameData.Instance.PlayerNodeDef.szNickname = usinfo.nickname;
 //                   StreamWriter writer2 = new StreamWriter(Application.persistentDataPath + "/userinfo.wx", false ,Encoding.UTF8);
 //                   StringBuilder str2 = new StringBuilder("{\"Listuserinfo\":[{");
 //                   str2.AppendFormat(
 //"\"openid\":\"{0}\",\"nickname\":{1},\"sex\":{2},\"province\":\"{3}\",\"city\":\"{4}\",\"country\":\"{5}\",\"headimgurl\":\"{6}\",\"privilege\":[\"{7}\",\"{8}\"],\"unionid\":\"{9}\"", 
 //usinfo.openid, usinfo.nickname, usinfo.sex, usinfo.province, usinfo.city,usinfo .headimgurl ,usinfo .privilege [0],usinfo .privilege[1],usinfo.unionid );
 //                   str2.Append("}]}");
 //                   writer2.Write(str2);
 //                   writer2.Close();
 //                   writer2.Dispose();
 //                   break;
 //               default:
 //                   break;
 //           }
 //       }
        /// <summary>
        /// 加载网络保存微信数据
        /// </summary>
        /// <param name="loadtype"></param>
        /// <returns></returns>
        //public T LoadWXElement<T>(uint loadtype)
        //{
        //    string str=null ;
        //    switch (loadtype )
        //    {
        //        case 0:
        //            try
        //            {
        //                using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/access.wx"))
        //                {
        //                    while ((str = sr.ReadLine()) != null)
        //                    {
        //                        SDKManager.HeadData headData = new SDKManager.HeadData(); headData = JsonBase.DeserializeFromJson<SDKManager.HeadData>(str.ToString());
        //                        EleAccessToken elac = new EleAccessToken();
        //                        elac.openid = headData.Listaccesstoken[0].openid;
        //                        elac.access_token = headData.Listaccesstoken[0].access_token;
        //                        elac.expires_in = headData.Listaccesstoken[0].expires_in;
        //                        elac.refresh_token = headData.Listaccesstoken[0].refresh_token;
        //                        elac.scope = headData.Listaccesstoken[0].scope;
        //                        elac.unionid = headData.Listaccesstoken[0].unionid;
        //                        return (T)Convert.ChangeType(elac, typeof(T));
        //                    }
        //                    sr.Close();
        //                    sr.Dispose();
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.LogError("+++The file could not be read:");
        //                Debug.LogError(e.Message);
        //                return (T)(new object());
        //            }
        //            break;
        //        case 1:
        //            try
        //            {
        //                using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/refresh.wx"))
        //                {
        //                    while ((str = sr.ReadLine()) != null)
        //                    {
        //                        SDKManager.HeadData headData = new SDKManager.HeadData(); headData = JsonBase.DeserializeFromJson<SDKManager.HeadData>(str.ToString());
        //                        EleRefreshToken  elac = new EleRefreshToken ();
        //                        elac.openid = headData.Listrefreshtoken[0].openid;
        //                        elac.access_token  = headData.Listrefreshtoken[0].access_token ;
        //                        elac.expires_in  = headData.Listrefreshtoken[0].expires_in;
        //                        elac.refresh_token  = headData.Listrefreshtoken[0].refresh_token;
        //                        elac.scope  = headData.Listrefreshtoken[0].scope;
        //                        return (T)Convert.ChangeType(elac, typeof(T)); 
        //                    }
        //                    sr.Close();
        //                    sr.Dispose();
        //                }
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.LogError("+++The file could not be read:");
        //                Debug.LogError(e.Message);
        //                return (T)(new object());
        //            }
        //            break;
        //        case 2:
        //            try
        //            {
        //                using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/userinfo.wx"))
        //                {
        //                    while ((str = sr.ReadLine()) != null)
        //                    {
        //                        SDKManager.HeadData headData = new SDKManager.HeadData(); headData = JsonBase.DeserializeFromJson<SDKManager.HeadData>(str.ToString());
        //                        EleUserInfo elac = new EleUserInfo();
        //                        elac.openid = headData.Listuserinfo[0].openid;
        //                        elac.nickname = headData.Listuserinfo[0].nickname;
        //                        elac.sex = headData.Listuserinfo[0].sex;
        //                        elac.province = headData.Listuserinfo[0].province;
        //                        elac.city = headData.Listuserinfo[0].city;
        //                        elac.country = headData.Listuserinfo[0].country;
        //                        elac.headimgurl = headData.Listuserinfo[0].headimgurl;
        //                        elac.privilege = headData.Listuserinfo[0].privilege;
        //                        elac.unionid = headData.Listuserinfo[0].unionid;
        //                        return (T)Convert.ChangeType(elac, typeof(T)); ;
        //                    }
        //                    sr.Close();
        //                    sr.Dispose();
        //                }                       
        //            }
        //            catch (Exception e)
        //            {
        //                Debug.LogError("The file could not be read:");
        //                Debug.LogError (e.Message);
        //                return (T)(new object());
        //            }
        //            break;
        //        default:
        //            return (T)(new object());
        //    }
        //    return (T)(new object());
        //}
        public class EleAccessToken
        {
            public int errcode;
            public string access_token;
            public int expires_in;
            public string refresh_token;
            public string openid;
            public string scope;
            public string unionid;
        }
        public class EleRefreshToken
        {
            public int errcode;
            public string access_token;
            public int expires_in;
            public string refresh_token;
            public string openid;
            public string scope;
        }
        public class EleUserInfo
        {
            public int errcode;
            public string openid;
            public string nickname;
            public int sex;
            public string province;
            public string city;
            public string country;
            public string headimgurl;
            public string[]privilege;
            public string unionid;



        }


    }
   
}
