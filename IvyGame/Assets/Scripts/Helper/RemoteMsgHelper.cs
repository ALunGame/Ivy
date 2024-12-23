using IAEngine;
using IAFramework.Log;
using IAToolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.SharpZipLib.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Game.Helper
{
    public static class RemoteMsgHelper
    {
        private static string API_PATH = "/robot/api/robot/feishu/chat/send/";

        public static string API_HOST = string.Empty;
        public static string TEXT_URL = string.Empty;
        public static string FILE_URL = string.Empty;

        class UpLogRequestData
        {
            public UnityWebRequest request = null;
            public Action successCallBack = null;
            public Action errorCallBack = null;
        }

        private static List<UpLogRequestData> upLogRequests = new List<UpLogRequestData>();

        class SendTextRequestData
        {
            public UnityWebRequest request = null;
        }

        private static List<SendTextRequestData> sendTextRequests = new List<SendTextRequestData>();

        static RemoteMsgHelper()
        {
            Init("https://ycmg-ms01.dayukeji.com");
        }
            
        public static void Init(string pAPIHost)
        {
            API_HOST = pAPIHost;
            TEXT_URL = API_HOST + "/robot/api/robot/feishu/chat/send/text";
            FILE_URL = API_HOST + "/robot/api/robot/feishu/chat/send/rawfile";

            ClearRequests();

#if UNITY_EDITOR
            EditorApplication.update -= UpdateRequests;
            if (!Application.isPlaying)
            {
                EditorApplication.update -= UpdateRequests;
                EditorApplication.update += UpdateRequests;
            }
#endif
        }

        //[MenuItem("RemoteMsgHelper/Test")]
        //private static void Test()
        //{
        //    Init("https://ycmg-ms01.dayukeji.com");
        //    RemoteMsgHelper.SendText("IAGame", "Test");
        //}

        //[MenuItem("RemoteMsgHelper/TestLog")]
        //private static void TestLog()
        //{
        //    Init("https://ycmg-ms01.dayukeji.com");
        //    RemoteMsgHelper.SendLog("IAGame", "Test11", "aaaa", null, null);
        //}

        public static void UpdateRequests()
        {
            for (int i = 0; i < upLogRequests.Count; i++)
            {
                if (upLogRequests[i] != null)
                {
                    UpLogRequestData tData = upLogRequests[i];
                    UnityWebRequest request = tData.request;
                    if (request != null)
                    {
                        if (request.isDone)
                        {
                            UnityWebRequest.Result result = request.result;
                            request.Dispose();
                            upLogRequests.Remove(tData);

                            if (result == UnityWebRequest.Result.Success)
                            {
                                tData.successCallBack?.Invoke();
                            }
                            else
                            {
                                tData.errorCallBack?.Invoke();
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < sendTextRequests.Count; i++)
            {
                if (sendTextRequests[i] != null)
                {
                    SendTextRequestData tData = sendTextRequests[i];
                    UnityWebRequest request = tData.request;
                    if (request != null)
                    {
                        if (request.isDone)
                        {
                            UnityWebRequest.Result result = request.result;
                            if (result != UnityWebRequest.Result.Success)
                            {
                                Debug.LogWarning($"发送失败->{request.url}---->{result}");
                            }
                            else
                            {
                                Debug.Log($"发送成功->{request.url}---->{result}");
                            }

                            request.Dispose();
                            sendTextRequests.Remove(tData);


                        }
                    }
                }
            }
        }

        public static void ClearRequests()
        {
            for (int i = 0; i < upLogRequests.Count; i++)
            {
                if (upLogRequests[i] != null && upLogRequests[i].request != null)
                {
                    UnityWebRequest request = upLogRequests[i].request;
                    request.Dispose();
                }
            }
            upLogRequests.Clear();

            for (int i = 0; i < sendTextRequests.Count; i++)
            {
                if (sendTextRequests[i] != null && sendTextRequests[i].request != null)
                {
                    UnityWebRequest request = sendTextRequests[i].request;
                    request.Dispose();
                }
            }
            sendTextRequests.Clear();
        }

        public static void SendText(string pGroupName, string pContent, bool pIsAtAll = false, string pAtName = "")
        {
            if (string.IsNullOrEmpty(API_HOST))
            {
                return;
            }

            try
            {
                JsonData tMsgData = new JsonData();
                string tAtName = pAtName == "" ? "all" : pAtName;
                string tNotify = pIsAtAll ? tAtName : "";

                var tSec = (int)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                tMsgData["receive_name"] = pGroupName;
                tMsgData["content"] = pContent;
                tMsgData["at"] = tNotify;
                tMsgData["user_id"] = CalcSalt(tSec);
                tMsgData["sign"] = CalcSign(tSec.ToString());
                string jsonStr = tMsgData.ToJson();

                UnityWebRequest www = UnityWebRequest.PostWwwForm(TEXT_URL, jsonStr);
                www.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonStr);
                www.uploadHandler.Dispose();
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.timeout = 100;
                www.disposeDownloadHandlerOnDispose = true;
                www.disposeUploadHandlerOnDispose = true;
                www.disposeCertificateHandlerOnDispose = true;

                SendTextRequestData tData = new SendTextRequestData();
                tData.request = www;
                sendTextRequests.Add(tData);

                www.SendWebRequest();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"消息上报失败：{ex}");
            }
        }

        public static void SendLog(string pGroupName, string pZipName, string pContent, Action pSuccessCallBack, Action pErrorCallBack, bool pIsAtAll = false)
        {
            try
            {
                string tNotify = pIsAtAll ? "all" : "";
                string zipFilePath = CreateFullLog(pZipName);
                var tSec = (int)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
                byte[] bytes = File.ReadAllBytes(zipFilePath);
                formData.Add(new MultipartFormDataSection("receive_name", pGroupName));

                if (!string.IsNullOrEmpty(tNotify))
                    formData.Add(new MultipartFormDataSection("at", tNotify));

                formData.Add(new MultipartFormDataSection("user_id", CalcSalt(tSec)));
                formData.Add(new MultipartFormDataSection("sign", CalcSign(tSec.ToString())));
                formData.Add(new MultipartFormDataSection("user_comment", pContent));
                formData.Add(new MultipartFormFileSection("raw_file", bytes, Path.GetFileName(zipFilePath), null));

                UnityWebRequest www = UnityWebRequest.Post(FILE_URL, formData);
                www.timeout = 100;
                www.disposeDownloadHandlerOnDispose = true;
                www.disposeUploadHandlerOnDispose = true;
                www.disposeCertificateHandlerOnDispose = true;

                UpLogRequestData tData = new UpLogRequestData();
                tData.request = www;
                tData.successCallBack = pSuccessCallBack;
                tData.errorCallBack = pErrorCallBack;
                upLogRequests.Add(tData);

                www.SendWebRequest();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"消息上报失败：{ex}");
                pErrorCallBack?.Invoke();
            }
        }

        private static string CreateFullLog(string pZipName)
        {
            var strLogSaveDir = LogSetting.LogRootPath;
            if (!Directory.Exists(strLogSaveDir))
                return string.Empty;

            List<string> tFiles = IOHelper.GetAllFilePath(strLogSaveDir);
            if (tFiles.Count == 0)
                return string.Empty;

            string fileNameTime = string.Format("{0}.{1}({2:D2}.{3:D2}.{4:D2})",
                  DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            string zipFileName = pZipName + string.Format("_full_log_{0}", fileNameTime);

            string tOutputDir = PathHelper.SandboxDir + "/PostUnzip";
            if (!Directory.Exists(tOutputDir))
                Directory.CreateDirectory(tOutputDir);
            string tOutputFile = tOutputDir + "/" + zipFileName;
            if (!tOutputFile.Contains(".zip"))
                tOutputFile += ".zip";

            ZipUtility.CompressFolderToZip(tOutputFile, null, strLogSaveDir);

            return tOutputFile;
        }

        #region Calc

        public static string CalcSalt(int pSec)
        {
            var rnd = new System.Random(pSec);
            var tNumberArr = pSec.ToString().ToCharArray();
            var tLen = tNumberArr.Length;
            var tNewLen = tLen + 2;

            var tNewNumberArr = new char[tNewLen];
            tNewNumberArr[0] = tNumberArr[tLen - 2];
            tNewNumberArr[1] = tNumberArr[tLen - 1];
            tNewNumberArr[tNewLen - 4] = (char)('0' + rnd.Next(10));
            tNewNumberArr[tNewLen - 3] = (char)('0' + rnd.Next(10));
            tNewNumberArr[tNewLen - 2] = tNumberArr[tLen - 4];
            tNewNumberArr[tNewLen - 1] = tNumberArr[tLen - 3];

            var tOffset = 2;
            var tLeftLen = tLen - 4;
            var tLeftMaxIdx = tLen - 5;
            for (var i = 0; i < tLeftLen; i++)
            {
                tNewNumberArr[tOffset + i] = tNumberArr[tLeftMaxIdx - i];
            }

            var tResult = new string(tNewNumberArr);
            return tResult;
        }

        public static string CalcSign(string pPart1)
        {
            return CalcMD5(pPart1 + "xYz");
        }

        public static string CalcSign(string pPart1, string pPart2)
        {
            return CalcMD5(pPart1 + "xYz" + pPart2);
        }

        public static string CalcMD5(string pContent)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytValue, bytHash;
            bytValue = Encoding.UTF8.GetBytes(pContent);
            bytHash = md5.ComputeHash(bytValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < bytHash.Length; i++)
            {
                sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToLower();
        }

        #endregion
    }
}
