using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class FileHelper
    {
        #region 写文件
        /// <summary>
        /// 写文件
        /// </summary>
        public static void WriteFile(string path, string str, string tableName)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = path + "\\" + tableName + ".cs";

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(str);
                    sw.Flush();
                }
            }
        }
        #endregion

        #region 读文件
        /// <summary>
        /// 读文件
        /// </summary>
        public static string ReadFile(string path)
        {
            string result = string.Empty;

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }
        #endregion

    }
}
