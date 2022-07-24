using ModelGenerator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelGenerator.Models
{
    public class NameUtil
    {
        public static string GetName(string name, NameModeEnum mode)
        {
            if (mode == NameModeEnum.Pascal)
            {
                StringBuilder sb = new StringBuilder();
                string[] strArr = name.Split('_');
                foreach (string str in strArr)
                {
                    if (str.Length > 0)
                    {
                        sb.Append(str[0].ToString().ToUpper());

                        if (str.Length > 1)
                        {
                            sb.Append(str.Substring(1).ToLower());
                        }
                    }
                }
                return sb.ToString();
            }

            if (mode == NameModeEnum.AllUpper)
            {
                return name.ToUpper();
            }

            if (mode == NameModeEnum.AllLower)
            {
                return name.ToLower();
            }

            throw new NotSupportedException();
        }
    }
}
