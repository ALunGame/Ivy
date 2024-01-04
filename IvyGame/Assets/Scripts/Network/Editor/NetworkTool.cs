using IAToolkit;
using Proto;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class NetworkTool
{
    [MenuItem("Tools/Network/生成消息码")]
    public static void CreateMsg()
    {   
        //删除文件

        //执行批处理
        MiscHelper.ExecuteBat("GenProto.bat", "", Application.dataPath + "/../../Proto/");

        //生成消息码映射文件
    }


    private class NetMsgMapping
    {


        //private Dictionary<int,Action<JoinRoomC2s>>
    }
}
