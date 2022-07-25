using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGenerator.Template
{
    /// <summary>
    /// 属性字符串
    /// </summary>
    public class AttributeStrings
    {
        /// <summary>
        /// 表属性
        /// </summary>
        public static string TableAttr { get { return "DBTable"; } }

        /// <summary>
        /// 字段属性
        /// </summary>
        public static string FieldAttr { get { return "DBField"; } }

        /// <summary>
        /// 主键属性
        /// </summary>
        public static string FieldKeyAttr { get { return "DBKey"; } }

    }
}
