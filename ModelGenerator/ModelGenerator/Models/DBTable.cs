using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGenerator.Models
{
    /// <summary>
    /// 数据库表信息
    /// </summary>
    public class DBTable
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表注释
        /// </summary>
        public string Comments { get; set; }
    }
}
