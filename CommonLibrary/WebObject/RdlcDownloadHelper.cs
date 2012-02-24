using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.Reporting.WebForms;
using System.IO;
namespace CommonLibrary.WebObject
{
    /// <summary>
    /// RdlcDownloadHelper 的摘要说明
    /// </summary>
    public class RdlcDownloadHelper
    {
        public RdlcDownloadHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        /// <summary>
        /// 下载报表文件
        /// </summary>
        /// <param name="s_rptType">打印的文件类型("Excel,PDF,Image")</param>
        public static void DowloadReportFile(HttpResponse response, ReportViewer rpvObject, ReportPrintType rptType, string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || response == null || rpvObject == null) return;
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = rpvObject.LocalReport.Render(
            rptType.ToString(), null, out mimeType, out encoding, out extension,
            out streamids, out warnings);
            //Download
            response.Buffer = true;
            response.Clear();
            response.ContentType = "application/" + extension;
            response.AddHeader("content-disposition", "attachment; filename=" + fileName + "." + extension);
            response.BinaryWrite(bytes);
            response.Flush();
            response.End();


        }
        /// <summary>
        /// 保存报表文件为文件
        /// </summary>
        /// <param name="rpvObject">Reportview控件实例</param>
        /// <param name="rptType">打印的文件类型</param>
        /// <param name="filePath">文件存放路径</param>
        /// <param name="fileName">文件名</param>
        public static void SaveReportFile(ReportViewer rpvObject, ReportPrintType rptType, string filePath, string fileName)
        {
            if (rpvObject == null || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(filePath)) return;
            Warning[] warnings;
            string[] streamids;
            string mimeType;
            string encoding;
            string extension;

            byte[] bytes = rpvObject.LocalReport.Render(
            rptType.ToString(), null, out mimeType, out encoding, out extension,
            out streamids, out warnings);
            //file
            FileStream stream = new FileStream(filePath + fileName + "." + extension, FileMode.Create);
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
        }

        /// <summary>
        /// 保存报表文件为文件
        /// </summary>
        /// <param name="rpvObject">Reportview控件实例</param>
        /// <param name="rptType">打印的文件类型</param>
        /// <param name="filePath">文件存放路径</param>
        /// <param name="fileName">文件名</param>
        public static void Dowload(HttpResponse response, string filePath, string fileName, string extension)
        {
            if (response == null || string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(extension)) return;
            FileStream stream = new FileStream(filePath + fileName + "." + extension, FileMode.Open);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            stream.Close();
            //Download
            response.Buffer = true;
            response.Clear();
            response.ContentType = "application/" + extension;
            response.AddHeader("content-disposition", "attachment; filename=" + fileName + "." + extension);
            response.BinaryWrite(bytes);
            response.Flush();
            response.End();
        }
        /// <summary>
        /// 下载文件类型
        /// </summary>
        public enum ReportPrintType
        {
            PDF,
            Excel,
            Image,
        }
    }
}