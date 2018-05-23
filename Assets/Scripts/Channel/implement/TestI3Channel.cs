namespace GameChannel
{
    /// <summary>
    /// iOS正式测试渠道
    /// </summary>
    public class TestI3Channel : BaseChannel
    {
        public override void Init()
        {
            ChannelManager.instance.InitSDKComplete("");
        }

        public override void Login()
        {
        }

        public override void Logout()
        { 
        
        }

        public override void ShowUserCenter(int serverId, string roleId)
        { 
        
        }

        public override void Pay(params object[] paramList)
        { 
        
        }

        public override void SubmitUserConfig(params object[] paramList)
        { 
        
        }

        public override string GetPackageName()
        {
            return "xluaframework";
        }

        public override bool IsInternalChannel()
        {
            return true;
        }

        public override void DownloadGame(params object[] paramList)
        {
        }

        public override string GetBundleID()
        {
            return "com.blugame.framework";
        }

        public override bool HasAndroidSDK()
        {
            return false;
        }

        public override string GetAssetBundleServerUrl()
        {
            return "http://192.168.0.11/update/xlua-framework-master/AssetBundles/iOS/TestI3/";
        }
    }
}
