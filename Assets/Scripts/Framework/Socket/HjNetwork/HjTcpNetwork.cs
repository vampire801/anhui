//#define LOG_SEND_BYTES
//#define LOG_RECEIVE_BYTES
using System;
using System.Net.Sockets;
using CustomDataStruct;
using System.Threading;
using System.Collections.Generic;
using XLua;

namespace Networks
{
    [Hotfix]
    public class HjTcpNetwork : HjNetworkBase
    {
        private Thread mSendThread = null;
        private volatile bool mSendWork = false;
        private HjSemaphore mSendSemaphore = null;

        protected IMessageQueue mSendMsgQueue = null;

        public HjTcpNetwork(int maxBytesOnceSent = 1024 * 100, int maxReceiveBuffer = 1024 * 512) : base(maxBytesOnceSent, maxReceiveBuffer)
        {
            mSendSemaphore = new HjSemaphore();
            mSendMsgQueue = new MessageQueue();
        }

        public override void Dispose()
        {
            mSendMsgQueue.Dispose();
            base.Dispose();
        }

        protected override void DoConnect()
        {
            String newServerIp = "";
            AddressFamily newAddressFamily = AddressFamily.InterNetwork;
            IPv6SupportMidleware.getIPType(mIp, mPort.ToString(), out newServerIp, out newAddressFamily);
            if (!string.IsNullOrEmpty(newServerIp))
            {
                mIp = newServerIp;
            }

            mClientSocket = new Socket(newAddressFamily, SocketType.Stream, ProtocolType.Tcp);
            mClientSocket.BeginConnect(mIp, mPort, (IAsyncResult ia) =>
            {
                mClientSocket.EndConnect(ia);
                OnConnected();
            }, null);
            mStatus = SOCKSTAT.CONNECTING;
        }
        
        protected override void DoClose()
        {
            // 关闭socket，Tcp下要等待现有数据发送、接受完成
            // https://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.shutdown(v=vs.90).aspx
            // mClientSocket.Shutdown(SocketShutdown.Both);
            base.DoClose();
        }

        public override void StartAllThread()
        {
            base.StartAllThread();

            if (mSendThread == null)
            {
                mSendThread = new Thread(SendThread);
                mSendWork = true;
                mSendThread.Start(null);
            }
        }

        public override void StopAllThread()
        {
            base.StopAllThread();
            //先把队列清掉
            mSendMsgQueue.Dispose();

            if (mSendThread != null)
            {
                mSendWork = false;
                mSendSemaphore.ProduceResrouce();// 唤醒线程
                mSendThread.Join();// 等待子线程退出
                mSendThread = null;
            }
        }

        private void SendThread(object o)
        {
            List<byte[]> workList = new List<byte[]>(10);

            while (mSendWork)
            {
                if (!mSendWork)
                {
                    break;
                }

                if (mClientSocket == null || !mClientSocket.Connected)
                {
                    continue;
                }

                mSendSemaphore.WaitResource();
                if (mSendMsgQueue.Empty())
                {
                    continue;
                }

                mSendMsgQueue.MoveTo(workList);
                try
                {
                    for (int k = 0; k < workList.Count; ++k)
                    {
                        var msgObj = workList[k];
                        if (mSendWork)
                        {
                            mClientSocket.Send(msgObj, msgObj.Length, SocketFlags.None);
                        }
                    }
                }
                catch (ObjectDisposedException e)
                {
                    ReportSocketClosed(ESocketError.ERROR_1, e.Message);
                    break;
                }
                catch (Exception e)
                {
                    ReportSocketClosed(ESocketError.ERROR_2, e.Message);
                    break;
                }
                finally
                {
                    for (int k = 0; k < workList.Count; ++k)
                    {
                        var msgObj = workList[k];
                        StreamBufferPool.RecycleBuffer(msgObj);
                    }
                    workList.Clear();
                }
            }
            
            if (mStatus == SOCKSTAT.CONNECTED)
            {
                mStatus = SOCKSTAT.CLOSED;
            }
        }

        protected override void DoReceive(StreamBuffer streamBuffer, ref int bufferCurLen)
        {
            try
            {
                // 组包、拆包
                byte[] data = streamBuffer.GetBuffer();
                int start = 0;
                streamBuffer.ResetStream();
                while (true)
                {
                    if (bufferCurLen - start < sizeof(int))
                    {
                        break;
                    }

                    // 包头4各字节
                    if (data[0] != PACKAGE_IDENTIFY)
                    {
                        throw new Exception(string.Format("Package identity error: 0x{0:X2}", data[0]));
                    }
                    if (data[1] != 1)
                    {
                        throw new Exception(string.Format("NOAES flag error: 0x{0:X2}", data[1]));
                    }
                    int packLen = data[2] << 8 | data[3];
                    if (bufferCurLen < packLen)
                    {
                        break;
                    }

                    // 提取字节流，只去掉包头的4字节，消息头留到反序列化时还有用
                    start += 4;
                    var bytes = streamBuffer.ToArray(start, packLen - 4);
#if LOG_RECEIVE_BYTES
                    var sb = new System.Text.StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        sb.AppendFormat("{0}\t", bytes[i]);
                    }
                    Logger.Log("HjTcpNetwork receive bytes : " + sb.ToString());
#endif
                    mReceiveMsgQueue.Add(bytes);

                    // 下一次组包
                    start += packLen;
                }

                if (start > 0)
                {
                    bufferCurLen -= start;
                    streamBuffer.CopyFrom(data, start, 0, bufferCurLen);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(string.Format("Tcp receive package err : {0}\n {1}", ex.Message, ex.StackTrace));
            }
        }

        public override void SendMessage(int msgId, byte[] msgObj)
        {
#if LOG_SEND_BYTES
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < msgObj.Length; i++)
            {
                sb.AppendFormat("{0}\t", msgObj[i]);
            }
            Logger.Log("HjTcpNetwork send bytes : " + sb.ToString());
#endif
            byte[] packObj = new byte[12 + msgObj.Length];
            // 包头4个字节：包识别符（1字节）+noaes标志（1字节）+包长度（2字节，Big Endian）
            packObj[0] = PACKAGE_IDENTIFY;
            packObj[1] = 0x01;
            packObj[2] = (byte)((packObj.Length >> 8) & 0xFF);
            packObj[3] = (byte)(packObj.Length & 0xFF);
            // 消息头8个字节：消息识别符（1字节）+消息来源（1字节）+消息类型（2字节）+套接字索引（2字节）+ 消息体长度（2字节）
            packObj[4] = MESSAGE_VERSION;
            packObj[6] = (byte)(msgId & 0xFF);
            packObj[7] = (byte)((msgId >> 8) & 0xFF);
            // 消息体长度2个字节改为
            packObj[10] = (byte)(msgObj.Length & 0xFF);
            packObj[11] = (byte)((msgObj.Length >> 8) & 0xFF);
            // 消息体
            Array.Copy(msgObj, 0, packObj, 12, msgObj.Length);
            mSendMsgQueue.Add(packObj);
            mSendSemaphore.ProduceResrouce();
        }
    }

#if UNITY_EDITOR
    public static class HjTcpNetworkExporter
    {
        [LuaCallCSharp]
        public static List<Type> LuaCallCSharp = new List<Type>()
        {
            typeof(HjTcpNetwork),
        };

        [CSharpCallLua]
        public static List<Type> CSharpCallLua = new List<Type>()
        {
            typeof(Action<object, int, string>),
            typeof(Action<byte[]>),
        };
    }
#endif
}