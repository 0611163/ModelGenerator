using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace FQDService.Utils
{
    /// <summary>
    /// XML帮助类
    /// </summary>
    public class XMLHelper
    {
        #region 获取XmlDocument
        /// <summary>
        /// XmlDocument
        /// </summary>
        public static XmlDocument GetXmlDocument()
        {
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string xmlFilePath = appPath.Remove(appPath.LastIndexOf('\\') + 1) + "FQDRunProcSettings.xml";

            if (File.Exists(xmlFilePath))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFilePath);
                return doc;
            }
            else
            {
                throw new FileNotFoundException("未能找到服务名称配置文件 FQDRunProcSettings.xml！");
            }
        }
        #endregion

    }
}
