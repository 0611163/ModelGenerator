using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace FQDService.Utils
{
    /// <summary>
    /// 写日志类
    /// </summary>
    public class FileLogger
    {
        #region 字段
        public static object _lock = new object();
        #endregion

        #region 写文件
        /// <summary>
        /// 写文件
        /// </summary>
        public static void WriteFile(string log, string path)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(delegate(object obj)
            {
                lock (_lock)
                {
                    if (!File.Exists(path))
                    {
                        using (FileStream fs = new FileStream(path, FileMode.Create)) { }
                    }

                    using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            #region 日志内容
                            string value = string.Format(@"{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), obj.ToString());
                            #endregion

                            sw.WriteLine(value);
                            sw.Flush();
                        }
                    }
                }
            }));
            thread.Start(log);
        }
        #endregion

        #region 写日志
        /// <summary>
        /// 写日志
        /// </summary>
        public static void WriteLog(string log)
        {
            string logPath = ConfigurationManager.AppSettings["LogPath"] + "\\FQDService_Log.txt";
            WriteFile(log, logPath);
        }
        #endregion

        #region 写错误日志
        /// <summary>
        /// 写错误日志
        /// </summary>
        public static void WriteErrorLog(string log)
        {
            string logPath = ConfigurationManager.AppSettings["LogPath"] + "\\FQDService_ErrorLog.txt";
            WriteFile(log, logPath);
        }
        #endregion

    }
}
