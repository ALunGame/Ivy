using Game.Network;
using Game.Network.Client;
using Game.Network.Server;
using IAEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestNetServer : MonoBehaviour
{
    [SerializeField]
    private GameObject ServerObj;
    [SerializeField]
    private GameObject ClientObj;
    [SerializeField]
    private Text ServerIpText;
    [SerializeField]
    private Text LogText;


    public void StartServer()
    {
        NetServer netServer = ServerObj.AddComponent<NetServer>();
        netServer.StartServer();

        ClientObj.SetActive(false);
    }

    NetClient netClient;
    public void StartConnect()
    {
        netClient = ClientObj.AddComponent<NetClient>();

        LANScanner.ScanHost(NetServer.ServerPort, OnScanHost, false);

        ServerObj.SetActive(false);
    }

    private void OnScanHost(List<string> ipList)
    {
        if (ipList.IsLegal())
        {
            netClient.Connect(ipList[0], null);
        }
    }

}
