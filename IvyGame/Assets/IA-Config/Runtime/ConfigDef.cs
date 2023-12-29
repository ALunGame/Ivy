namespace IAConfig
{
    public static class ConfigDef
    {
        /// <summary>
        /// 配置扩展名
        /// </summary>
        public const string CnfExName = ".txt";

        /// <summary>
        /// 配置标志名
        /// </summary>
        public const string CnfKeyName = "Tb";


        public static string GetCnfName(string name)
        {
            return CnfKeyName + name + CnfExName;
        }

        /// <summary>
        /// 获得没有扩展名的文件名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetCnfNoExName(string name)
        {
            return CnfKeyName + name;
        }
    }
}