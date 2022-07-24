using ModelGenerator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGenerator.Models
{
    /// <summary>
    /// 命名方式
    /// </summary>
    public class NameMode
    {
        public string Key { get; set; }

        public NameModeEnum Value { get; set; }

        public NameMode(string key, NameModeEnum value)
        {
            Key = key;
            Value = value;
        }
    }
}
