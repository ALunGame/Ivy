using IAToolkit;

namespace Game.Network
{
    internal static class NetGenDispatcherMappingCode
    {
        private const string ServerDispatcherOutputPath = "./Assets/Scripts/Network/Server/Dispatcher/";

        private const string ClientDispatcherOutputPath = "./Assets/Scripts/Network/Client/Dispatcher/";

        public static void GenDispatcherMappingFile(MsgDefineMapping mapping, bool isServer)
        {
            string filePath = GetDispatcherFilePath(isServer);

            string namespaceStr = isServer ? "Game.Network.SDispatcher" : "Game.Network.CDispatcher";
            string classStr = isServer ? "SDispatcherMapping" : "CDispatcherMapping";
            string titleStr = isServer ? "S" : "C";

            string fileStr1 = @"
namespace #NameSpace#
{
    public class #ClassName# : NetDispatcherMapping
    {
        public #ClassName#()
        {
            #AddDispatcher#
        }
    }
}
";
            string fileStr2 = @"
            AddDispatcher(new #TitleStr##PackageName#Dispatcher(this));
";

            string resAddDispatcherStr = "";

            foreach (MsgDefinePackage package in mapping.msgPackages)
            {
                string tStr = fileStr2;
                tStr = tStr.Replace("#TitleStr#", titleStr);
                tStr = tStr.Replace("#PackageName#", package.packageName);
                resAddDispatcherStr += tStr;
            }

            string resStr = fileStr1;
            resStr = resStr.Replace("#NameSpace#", namespaceStr);
            resStr = resStr.Replace("#ClassName#", classStr);
            resStr = resStr.Replace("#AddDispatcher#", resAddDispatcherStr);

            IOHelper.WriteText(resStr, filePath);
        }

        private static string GetDispatcherFilePath(bool isServer)
        {
            string serverStr = isServer ? "S" : "C";
            string outPath = isServer ? ServerDispatcherOutputPath : ClientDispatcherOutputPath;
            return outPath + serverStr + "DispatcherMapping.cs";
        }
    }
}
