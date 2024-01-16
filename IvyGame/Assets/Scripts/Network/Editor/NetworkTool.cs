using Game.Network;
using IAToolkit;
using UnityEditor;
using UnityEngine;

public static class NetworkTool
{
    [MenuItem("Tools/Network/生成消息码")]
    public static void CreateMsg()
    {
        //删除文件
        IOHelper.DelDirectoryAllFile(Application.dataPath + "/Scripts/Network/GenProto");

        //执行批处理
        MiscHelper.ExecuteBat("GenProto.bat", "", Application.dataPath + "/../../Proto/");

        //生成消息码映射文件
        NetGenDispatcherCode.GenCode();

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        
    }
}
