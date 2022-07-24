using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGenerator.Models
{
    /// <summary>
    /// 数据库表字段
    /// </summary>
    public class DBColumn
    {
        /// <summary>
        /// 字段名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 字段注释
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// 字段数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 字段数据长度
        /// </summary>
        public string DataScale { get; set; }

        /// <summary>
        /// 字段数据精度
        /// </summary>
        public string DataPrecision { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool PrimaryKey { get; set; }

        /// <summary>
        /// 不为空
        /// </summary>
        public bool NotNull { get; set; }

        public DBColumn()
        {
            PrimaryKey = false;
        }

    }
}
