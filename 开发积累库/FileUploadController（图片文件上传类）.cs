using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SLY.Controllers
{
    public class FileUploadController : Controller
    {
        //
        // GET: /FileUpload/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Upladify 上传控件
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="guid"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FileLoad(FormContext context)
        {

            HttpPostedFileBase fileData = Request.Files[0];
            if (fileData != null)
            {
                try
                {
                    ControllerContext.HttpContext.Request.ContentEncoding = Encoding.GetEncoding("UTF-8");
                    ControllerContext.HttpContext.Response.Charset = "UTF-8";

                    string fileName = Path.GetFileName(fileData.FileName);      //原始文件名称
                    string fileExtension = Path.GetExtension(fileName);         //文件扩展名

                    //SysFileUpLoad info = new SysFileUpLoad();
                    //info.ID = Result.GetNewId();
                    byte[] FileData = ReadFileBytes(fileData);
                    //if (FileData != null)
                    //{
                    //    info.FileSize = FileData.Length;
                    //}
                    //info.Category = folder;
                    //info.FileName = fileName;
                    //info.FileExtend = fileExtension;
                    //info.AttachmentGUID = guid;
                    //info.CreateTime = DateTime.Now;
                    //info.Editor = GetCurrentPerson();//登录人
                    //info.OwnerID = "";//所属主表记录ID

                    string result = SaveFile(fileExtension, FileData);
                    if (string.IsNullOrEmpty(result))
                    {
                        //LogTextHelper.Error("上传文件失败:" + result.ErrorMessage);
                    }
                    return Json(new { isSuccess = true, path = result });
                }
                catch (Exception ex)
                {
                    //LogTextHelper.Error(ex);
                    return Json(new { isSuccess = false, path = "" });
                }
            }
            else
            {
                return Content("0");
            }
        }

        private byte[] ReadFileBytes(HttpPostedFileBase fileData)
        {
            byte[] data;
            using (Stream inputStream = fileData.InputStream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }
            return data;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="FileData"></param>
        /// <returns></returns>
        private string SaveFile(string fileextend, byte[] FileData)
        {
            string result = string.Empty;
            try
            {

                string saveName = Guid.NewGuid().ToString() + fileextend; //保存文件名称

                // 文件上传后的保存路径
                string basePath = "UploadFile";

                string saveDir = DateTime.Now.ToString("yyyy-MM-dd");
                //DirectoryUtil.AssertDirExist(filePath);
                string savePath = System.IO.Path.Combine(saveDir, saveName);

                string serverDir = System.IO.Path.Combine(Server.MapPath("~/"), basePath, saveDir);
                if (!System.IO.Directory.Exists(serverDir))
                {
                    System.IO.Directory.CreateDirectory(serverDir);
                }
                string fileNme = System.IO.Path.Combine(serverDir, saveName);//保存文件完整路径
                System.IO.File.WriteAllBytes(fileNme, FileData);
                result = "\\" + basePath + "\\" + saveDir + "\\" + saveName;
            }
            catch (Exception)
            {
                result = "发生错误";
            }
            return result;

        }

    }
}