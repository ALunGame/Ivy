using UnityEngine;
using System.IO;
using IAToolkit;
using IAEngine;

namespace GameContext
{
    /// <summary>
    /// 存档数据
    /// </summary>
    internal class RecordData
    {

        public static T LoadRecord<T>(string fileName) where T : RecordData, new()
        {
            string filePath = GetFileSavePath(fileName);
            if (!Directory.Exists(Path.GetPathRoot(filePath)))
                Directory.CreateDirectory(filePath);

            if (!File.Exists(filePath))
            {
                return new T();
            }
            string fileStr = IOHelper.ReadText(filePath);
            return JsonMapper.ToObject<T>(fileStr);
        }

        public static void SaveRecord(string fileName, RecordData data)
        {
            string filePath = GetFileSavePath(fileName);
            if (!Directory.Exists(Path.GetPathRoot(filePath)))
                Directory.CreateDirectory(filePath);

            string fileStr = JsonMapper.ToJson(data);
            IOHelper.WriteText(fileStr, filePath);
        }

        public static string GetFileSavePath(string fileName)
        {
            string dirPath = Application.persistentDataPath + "/Record";
            string filePath = dirPath + "/" + fileName + ".json";
            return filePath;
        }
    }
}
