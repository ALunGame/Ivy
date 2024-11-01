using UnityEngine;
using System.IO;
using IAToolkit;
using IAEngine;

namespace GameContext
{
    /// <summary>
    /// 存档辅助接口
    /// </summary>
    public static class ArchiveHelper
    {
        public static T LoadRecord<T>(string fileName) where T : ArchiveData, new()
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

        public static void SaveRecord(string fileName, ArchiveData data)
        {
            string filePath = GetFileSavePath(fileName);
            if (!Directory.Exists(Path.GetPathRoot(filePath)))
                Directory.CreateDirectory(filePath);

            string fileStr = JsonMapper.ToJson(data);
            IOHelper.WriteText(fileStr, filePath);
        }

        public static void DelRecord(string fileName)
        {
            string filePath = GetFileSavePath(fileName);
            IOHelper.DelFile(filePath);
        }

        public static string GetFileSavePath(string fileName)
        {
            string dirPath = Application.persistentDataPath + "/Record";
            string filePath = dirPath + "/" + fileName + ".json";
            return filePath;
        }
    }
}
