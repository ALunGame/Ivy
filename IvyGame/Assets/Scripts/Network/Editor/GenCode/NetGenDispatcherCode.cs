﻿using IAToolkit;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Game.Network
{
    internal static class NetGenDispatcherCode
    {
        private const string ServerDispatcherOutputPath = "./Assets/Scripts/Network/Server/Dispatcher/";

        private const string ClientDispatcherOutputPath = "./Assets/Scripts/Network/Client/Dispatcher/";

        //PlayerMsg - PlayerMsg.JoinRoomC2s - JoinRoomC2s
        private static Dictionary<string, Dictionary<int, string>> msgMapping = new Dictionary<string, Dictionary<int, string>>();

        [MenuItem("Tools/Network/Test")]
        public static void GenCode()
        {
            MsgDefineMapping msgDefineMapping = NetGenMsgDefineCode.LoadMapping();

            foreach (var item in msgDefineMapping.msgPackages)
            {
                GenDispatcherFile(item,true);
                GenDispatcherFile(item,false);
            }

            NetGenDispatcherMappingCode.GenDispatcherMappingFile(msgDefineMapping, true);
            NetGenDispatcherMappingCode.GenDispatcherMappingFile(msgDefineMapping, false);
        }

        private static void GenDispatcherFile(MsgDefinePackage msgPackage,bool isServer)
        {
            string filePath = GetDispatcherFilePath(msgPackage.packageName, isServer);

            string usingStr = "";
            Dictionary<string, string> msgContentDict = new Dictionary<string, string>();
            if (File.Exists(filePath))
            {
                string fileStr = IOHelper.ReadText(filePath);
                usingStr = GetFileUsingStr(fileStr);
                msgContentDict = GetFuncContentStr(fileStr, msgPackage.msgs);
            }

            string namespaceStr = isServer ? "Game.Network.SDispatcher" : "Game.Network.CDispatcher";
            string classStr = isServer ? $"S{msgPackage.packageName}Dispatcher" : $"C{msgPackage.packageName}Dispatcher";
            string checkMsgCode = isServer ? "S2c" : "C2s";

            string fileStr1 = @"namespace #NameSpace#
{
    public class #ClassName# : NetDispatcher
    {
        public #ClassName#(NetDispatcherMapping InMapping) : base(InMapping)
        {
            //AddDispatch<JoinRoomC2s>(1,OnJoinRoomC2s);
            #AddDispatch#
        }
        
        #DispatcherFunc#

    }
}
";
            string fileStr2 = @"
            AddDispatch<#MsgDefine#>((ushort)#PackageName#Define.#MsgDefine#,On#MsgDefine#);
";

            string fileStr3 = @"
        private void On#MsgDefine#(#MsgDefine# MsgData)
        {
#FuncContent#
        }
";

            string resUsingStr = usingStr;
            if (string.IsNullOrEmpty(usingStr))
            {
                resUsingStr = "using Proto;\n";
            }

            string resAddDispatchStr = "";
            string resFuncStr = "";
            for (int i = 0; i < msgPackage.msgs.Count; i++)
            {
                MsgDefine define = msgPackage.msgs[i];
                if (define.msgCodeName.Contains(checkMsgCode))
                {
                    string tDispatchStr = fileStr2;
                    tDispatchStr = tDispatchStr.Replace("#MsgDefine#", define.msgCodeName);
                    tDispatchStr = tDispatchStr.Replace("#PackageName#", msgPackage.packageName);
                    resAddDispatchStr += tDispatchStr;

                    string tFuncStr = fileStr3;
                    tFuncStr = tFuncStr.Replace("#MsgDefine#", define.msgCodeName);
                    tFuncStr = tFuncStr.Replace("#FuncContent#", msgContentDict.ContainsKey(define.msgCodeName) ? msgContentDict[define.msgCodeName] : "");
                    resFuncStr += tFuncStr;
                }
            }

            string resStr = resUsingStr + fileStr1;
            resStr = resStr.Replace("#NameSpace#", namespaceStr);
            resStr = resStr.Replace("#ClassName#", classStr);
            resStr = resStr.Replace("#PackageName#", msgPackage.packageName);
            resStr = resStr.Replace("#AddDispatch#", resAddDispatchStr);
            resStr = resStr.Replace("#DispatcherFunc#", resFuncStr);

            IOHelper.WriteText(resStr, filePath);
        }

        private static string GetFileUsingStr(string fileStr)
        {
            if (string.IsNullOrEmpty(fileStr))
            {
                return "";
            }

            Regex regex = new Regex(@"(using)[\d\D]*(?=namespace)");
            return regex.Match(fileStr).Value;
        }

        private static Dictionary<string,string> GetFuncContentStr(string fileStr, List<MsgDefine> msgs)
        {
            Dictionary<string, string> msgContentDict = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(fileStr))
            {
                return msgContentDict;
            }

            for (int i = 0; i < msgs.Count; i++)
            {
                string contentStr = "";

                MsgDefine define = msgs[i];
                string match = @"(?<=private\s+void\s+On#CodeName#\(#CodeName#\s+MsgData\)\s*\{)((?>\{(?<Depth>)|\}(?<-Depth>)|[^{}]+)*(?(Depth)(?!)))(?=\})";
                match = match.Replace("#CodeName#", define.msgCodeName);
                Regex regex = new Regex(match);
                if (regex.IsMatch(fileStr))
                {
                    Match matchStr = regex.Match(fileStr);
                    contentStr = matchStr.Value;
                    //regex = new Regex(@"(?<={\n|{\r\n)[\d\D]*(?=}\n|}\r\n)");
                    //contentStr = regex.Match(matchStr.Value).Value;
                    //contentStr = contentStr.Substring(0, contentStr.IndexOf("}"));
                }

                if (!string.IsNullOrEmpty(contentStr))
                {
                    if (!msgContentDict.ContainsKey(contentStr))
                    {
                        msgContentDict.Add(define.msgCodeName, contentStr);
                    }
                }
            }

            return msgContentDict;
        }

        private static string GetDispatcherFilePath(string packageName, bool isServer)
        {
            string serverStr = isServer ? "S" : "C";
            string outPath = isServer ? ServerDispatcherOutputPath : ClientDispatcherOutputPath;
            return outPath + serverStr + packageName + "Dispatcher.cs";
        }
    }
}
