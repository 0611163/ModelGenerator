using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGenerator.Enums
{
    /// <summary>
    /// 命名方式
    /// </summary>
    public enum NameModeEnum
    {
        /// <summary>
        /// 和数据库中的表名字段名保持一致
        /// </summary>
        Default,

        Pascal,

        AllUpper,

        AllLower
    }
}
