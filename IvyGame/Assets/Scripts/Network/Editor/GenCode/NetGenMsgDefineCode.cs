using IAEngine;
using IAToolkit;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Network
{
    internal class MsgDefine
    {
        public string msgCodeName;
        public int msgCode;
    }

    internal class MsgDefinePackage
    {
        public string packageName;

        public List<MsgDefine> msgs = new List<MsgDefine>();
    }

    internal class MsgDefineMapping
    {
        public List<MsgDefinePackage> msgPackages = new List<MsgDefinePackage>();
    }

    internal static class NetGenMsgDefineCode
    {
        private const string FilePath = "../Proto/Proto/ProtoMsgDefine.define";

        private const string DefineOutputPath = "./Assets/Scripts/Network/GenProto/";

        public static MsgDefineMapping LoadMapping()
        {
            MsgDefineMapping mapping = new MsgDefineMapping();


            string jsonStr = IOHelper.ReadText(FilePath);

            JsonData jsonData = JsonMapper.ToObject(jsonStr);
            foreach (string packageName in jsonData.Keys)
            {
                MsgDefinePackage msgPackage = new MsgDefinePackage();
                msgPackage.packageName = packageName;

                JsonData definesData = jsonData[packageName];
                foreach (JsonData defineData in definesData)
                {
                    foreach (string msgName in defineData.Keys)
                    {
                        int msgCode = int.Parse(defineData[msgName].ToString());
                        MsgDefine msgDefine = new MsgDefine();
                        msgDefine.msgCode = msgCode;
                        msgDefine.msgCodeName = msgName;

                        msgPackage.msgs.Add(msgDefine);
                    }
                }

                mapping.msgPackages.Add(msgPackage);
            }

            foreach (var item in mapping.msgPackages)
            {
                GenMsgDefinePackageFile(item);
            }
            return mapping;
        }

        private static void GenMsgDefinePackageFile(MsgDefinePackage msgPackage)
        {
            string namespaceStr = @"
namespace Proto
{
    public enum #PackageName#Define : ushort
    {
        #CONTENT#
    }
}";

            string defineStr = @"
        #MsgCodeName# = #MsgCode#,";

            string resStr = namespaceStr;

            //排序
            msgPackage.msgs.Sort((x, y) =>
            {
                if (x.msgCode > y.msgCode)
                    return 1;
                else
                    return -1;
            });

            string definesStr = "";
            for (int i = 0; i < msgPackage.msgs.Count; i++)
            {
                MsgDefine define = msgPackage.msgs[i];

                string tStr = defineStr;
                tStr = tStr.Replace("#MsgCodeName#", define.msgCodeName);
                tStr = tStr.Replace("#MsgCode#", define.msgCode.ToString());
                definesStr += tStr;
            }

            resStr = resStr.Replace("#PackageName#", msgPackage.packageName);
            resStr = resStr.Replace("#CONTENT#", definesStr);

            string dirPath = Path.GetFullPath(DefineOutputPath + msgPackage.packageName);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            dirPath += "/";
            IOHelper.WriteText(resStr, dirPath + msgPackage.packageName + "Define.cs");

            Debug.Log("MsgDefine生成成功:" + dirPath + msgPackage.packageName + "Define.cs");
        }
    }
}
