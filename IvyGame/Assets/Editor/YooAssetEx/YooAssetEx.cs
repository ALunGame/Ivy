using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YooAsset.Editor;


namespace YooAsset.Editor
{

    [DisplayName("收集除UserData中其他资源")]
    public class CollectAllIgnoreOthers : IFilterRule
    {
        public bool IsCollectAsset(FilterRuleData data)
        {
            string extension = Path.GetExtension(data.AssetPath);
            string[] ignoreExNames = data.UserData.Split(",");

            if (ignoreExNames.Length <= 0)
            {
                return true;
            }

            for (int i = 0; i < ignoreExNames.Length; i++)
            {
                if (extension == ignoreExNames[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
