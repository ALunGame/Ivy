using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Debug = UnityEngine.Debug;

namespace Game.Network
{

    /// <summary>
    /// 局域网Ip扫描器
    /// </summary>
    public static class LANScanner
    {
        private static List<string> hostIpList = new List<string>();

        public static void ScanHost(int pPort, Action<List<string>> pFinishCallBack,bool pTcp = true)
        {
            ScanIps(pPort,pTcp, pFinishCallBack).Forget();
        }

        private static async UniTaskVoid ScanIps(int pPort,bool pTcp, Action<List<string>> pFinishCallBack)
        {
            await UniTask.WaitForSeconds(0.2f);

            //获取本地机器名 
            string _myHostName = Dns.GetHostName();
            //获取本机IP 
            string _myHostIP = Dns.GetHostEntry(_myHostName).AddressList[0].ToString();
            //截取IP网段
            string ipDuan = _myHostIP.Remove(_myHostIP.LastIndexOf('.'));
            //string ipDuan = "192.168.0";
            //枚举网段计算机
            for (int i = 1; i <= 255; i++)
            {
                Ping myPing = new Ping();
                myPing.PingCompleted += new PingCompletedEventHandler(OnPingCompleted);
                string pingIP = ipDuan + "." + i.ToString();
                myPing.SendAsync(pingIP, 500, null);
            }

            //超时
            await UniTask.WaitForSeconds(2f);

            List<string> resIps = new List<string>();
            //检测端口开放
            for (int i = 1; i < hostIpList.Count; i++)
            {
                // 尝试连接端口
                try
                {
                    using (TcpClient client = new TcpClient())
                    {
                        client.Connect(IPAddress.Parse(hostIpList[i]), pPort);
                        resIps.Add(hostIpList[i]);
                    }
                }
                catch (Exception)
                {
                    // 端口未打开
                }
            }
        }

        private static void OnPingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (e.Reply.Status == IPStatus.Success)
            {
                hostIpList.Add(e.Reply.Address.ToString());
            }
            Debug.Log("局域网主机---" + e.Reply.Address.ToString());
        }
    }
}
