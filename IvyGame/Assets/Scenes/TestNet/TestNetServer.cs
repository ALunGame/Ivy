using Game.Network;
using Game.Network.Client;
using Game.Network.Server;
using System.Collections.Generic;
using System.Net;
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


    private float discoveryInterval = 5.0f; //请求间隔时间
    private float lastDiscoveryTime = 0.0f;

    private bool openDiscovery = false;
    private int discoveryPort = NetworkGeneral.ServerPort;
    private List<IPEndPoint> discoveredServers = new List<IPEndPoint>();

    private void Awake()
    {
        //Discovery = gameObject.AddComponent<NetDiscovery>();
    }

    private void Update()
    {
        if (!openDiscovery)
        {
            return;
        }

        if (Time.time - lastDiscoveryTime > discoveryInterval)
        {
            SendDiscovery();
            lastDiscoveryTime = Time.time;
        }
    }

    public void StartServer()
    {
        NetServer netServer = ServerObj.AddComponent<NetServer>();
        netServer.StartServer();
    }

    NetClient netClient;
    public void StartConnect()
    {
        netClient = ClientObj.AddComponent<NetClient>();
        netClient.SetDiscoveryCallBack((endPoint) =>
        {
            openDiscovery = false;
            netClient.Connect(endPoint, null);
        });

        openDiscovery = true;
    }

    private void SendDiscovery()
    {
        NetClientLocate.Log.Log("寻找服务器...",discoveryPort);
        netClient.Discovery(discoveryPort);
    }
}
