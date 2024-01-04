using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;

namespace Game.Network
{
    public static class ProtoBufTool
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="msgBase"></param>
        /// <returns></returns>
        public static byte[] Encode<T>(T msgBase) where T : IExtensible
        {
            using (MemoryStream memory = new MemoryStream())
            {
                Serializer.Serialize(memory, msgBase);
                return memory.ToArray();
            }
        }

        /// <summary>
        /// 解码
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T Decode<T>(byte[] bytes) where T : IExtensible
        {
            using (MemoryStream memory = new MemoryStream(bytes))
            {
                return Serializer.Deserialize<T>(memory);
            }
        }

        private static Dictionary<string, Type> protoTypeCache = new Dictionary<string, Type>();
        public static IExtensible Decode(string protoTypeName, byte[] bytes)
        {
            using (MemoryStream memory = new MemoryStream(bytes))
            {
                Type protoType = null;
                if (protoTypeCache.ContainsKey(protoTypeName))
                {
                    protoType = protoTypeCache[protoTypeName];
                }
                else
                {
                    protoType = Type.GetType(protoTypeName);
                    protoTypeCache.Add(protoTypeName, protoType);
                }
                
                return (IExtensible)Serializer.Deserialize(protoType,memory);
            }
        }
    }
}
