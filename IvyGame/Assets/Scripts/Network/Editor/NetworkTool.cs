using Game.Network;
using IAToolkit;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class NetworkTool
{
    [MenuItem("Tools/Network/生成消息码")]
    public static void CreateMsg()
    {
        string tmpPath = Path.GetFullPath("../GenProtoTemp");

        IOHelper.MoveDirectory(Application.dataPath + "/Scripts/Network/GenProto", tmpPath);

        Debug.Log($"CreateMsg;{tmpPath}");

        try
        {
            //执行批处理
            MiscHelper.ExecuteBat("../Proto/GenProto.bat", "", () =>
            {
                //生成消息码映射文件
                NetGenDispatcherCode.GenCode();

                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }, Path.GetFullPath("../Proto/"));
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"CreateMsg;{ex.ToString()}");

            IOHelper.MoveDirectory(tmpPath, Application.dataPath + "/Scripts/Network/GenProto");
        }


    }
}
