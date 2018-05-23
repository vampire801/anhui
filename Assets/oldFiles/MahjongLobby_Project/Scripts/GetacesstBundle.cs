using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
//-----------联系QQ1763977935
namespace MahjongLobby_AH
{
    public class GetacesstBundle : MonoBehaviour
    {
//----------------此处为测试代码------------
        // Use this for initialization
        void Start()
        {
            StartCoroutine(LoadAssets());
        }
        IEnumerator LoadAssets()
        {
            string strurl = "AssetBundles/wall/share.unitydd";
            string strur2 = "AssetBundles/wall/cube.unitydd";
            string str = @"file://E:\ShuangXi_workspace\ShuangXi_Client\shuangxi\AssetBundles\wall\cube.unitydd";
            //string str1 = @"http://localhost/AssetBundles/wall/cube.unitydd";
            string str1 = @"http://localhost/AssetBundles/AssetBundles";
            //-------------------------1---------------------------
            // AssetBundle ab1 = AssetBundle.LoadFromFile(strurl );
            // yield return  ab1;
            // AssetBundle ab = AssetBundle.LoadFromFile(strur2);
            // GameObject cude = ab.LoadAsset<GameObject>("Cube");
            //-------------------------2---------------------------异步

            //  AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(strur2));
            // yield return request;
            //AssetBundle ab = request.assetBundle;
            // GameObject cude = ab.LoadAsset<GameObject>("Cube");
            //AssetBundle ab3 = WWW.LoadFromCacheOrDownload("", Hash128.Parse());
            //--------------------------3-------------------------------
            while (!Caching .ready )
            {
                yield return null;
            }
            WWW www= WWW.LoadFromCacheOrDownload(str1, 1);
            yield return www;
            if (!string.IsNullOrEmpty (www.error))
            {
                Debug.Log(www.error);
                yield break;
            }
            AssetBundle ab = www.assetBundle;
            AssetBundleManifest mainfest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
           // string[] str_name = mainfest.GetAllAssetBundles ();
             string[] str_name = mainfest.GetAllDependencies("wall/cube.unitydd");
            for (int i = 0; i < str_name.Length ; i++)
            {
                Debug.LogWarning(str_name[i]);
                //AssetBundle ad=
            }
            GameObject cude = ab.LoadAsset<GameObject>("Cube");
           // Instantiate(cude);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
    //----------------以上为测试代码----------------------------------
    static public class AssetBundleManager
    {
        // A dictionary to hold the AssetBundle references
        static private Dictionary<string, AssetBundleRef> dictAssetBundleRefs;
        static AssetBundleManager()
        {
            dictAssetBundleRefs = new Dictionary<string, AssetBundleRef>();
        }
        // Class with the AssetBundle reference, url and version
        private class AssetBundleRef
        {
            public AssetBundle assetBundle = null;
            public int version;
            public string url;
            public AssetBundleRef(string strUrlIn, int intVersionIn)
            {
                url = strUrlIn;
                version = intVersionIn;
            }
        };
        // Get an AssetBundle
        public static AssetBundle getAssetBundle(string url, int version)
        {
            string keyName = url + version.ToString();
            AssetBundleRef abRef;
            if (dictAssetBundleRefs.TryGetValue(keyName, out abRef))
                return abRef.assetBundle;
            else
                return null;
        }
        // Download an AssetBundle
        public static IEnumerator downloadAssetBundle(string url, int version)
        {
            while (!Caching.ready)
            {
                yield return null;
            }
            string keyName = url + version.ToString();
            if (dictAssetBundleRefs.ContainsKey(keyName))
                yield return null;
            else
            {
                using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
                {
                    yield return www;
                    if (!string.IsNullOrEmpty (www.error ))
                        throw new System.Exception("WWW download:" + www.error);
                    AssetBundleRef abRef = new AssetBundleRef(url, version);
                    abRef.assetBundle = www.assetBundle;
                    dictAssetBundleRefs.Add(keyName, abRef);
                }
            }
        }
        // Unload an AssetBundle
        public static void Unload(string url, int version, bool allObjects)
        {
            string keyName = url + version.ToString();
            AssetBundleRef abRef;
            if (dictAssetBundleRefs.TryGetValue(keyName, out abRef))
            {
                abRef.assetBundle.Unload(allObjects);
                abRef.assetBundle = null;
                dictAssetBundleRefs.Remove(keyName);
            }
        }
    }
}

