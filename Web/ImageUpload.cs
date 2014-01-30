/*
 * Author:			Vex Tatarevic
 * Date Created:	2007-01-20
 * Copyright:       VEXIT Pty Ltd - www.vexit.com
 * 
 */


using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web;
using System.IO;

namespace vEX.Web
{
    public class ImageUpload
    {
        /// <param name="uploadPath">Rooted Virtual Path of the Directory in which to save the file e.g. /images/upload/ </param>
        public ImageUpload(string virtualUploadDirPath) { VirtualUploadDirPath = virtualUploadDirPath; }
       

        #region [ PROPERTIES ]
        const string thumbExtention = "_t";
        public enum DeleteError { FileDoesNotExist, Other }
        public enum UploadError { DirNotExist, FileNotValid, FileWrongType, FileNoData, FileTooLarge, FileWithSameNameExists, Other }
        public DeleteError DeleteErrorType;
        public UploadError UploadErrorType;
        public int FrameWidth = 800;
        /// <summary>
        ///  if 0, will be set to same as FrameWidth
        /// </summary>
        public int FrameHeight { get { return _FrameHeight; } set { if (value == 0)_FrameHeight = FrameWidth; } }
        private int _FrameHeight = 800;
        public int FrameWidthThumb = 200;
        /// <summary>
        ///  If 0, will be set to same as FrameWidthThumb
        /// </summary>
        public int FrameHeightThumb { get { return _FrameHeightThumb; } set { if (value == 0)_FrameHeightThumb = FrameWidthThumb; } }
        public int _FrameHeightThumb = 200;
        /// <summary>
        ///  Maximum file size
        /// </summary>
        public int MaxSize = 0; //maxSize = 0 means no limit on size
        /// <summary>
        ///  File Name with extention
        /// </summary>
        public string FileName = string.Empty;
        public string ThumbFileName { get { return GetThumbFileName(FileName); } }
        /// <summary>
        ///  Returns true if there are any error messages
        /// </summary>
        public bool HasFailed { get { return Message != string.Empty && Message.Contains("Failed"); } }
        public string Message = string.Empty;
        /// <summary>
        ///   Virtual Path of the Directory in which to save the file
        /// </summary>
        public string VirtualUploadDirPath = string.Empty;
        #endregion
       

        //------------------------------
        //      UPLOAD FILE
        //------------------------------

        public string Upload(HttpPostedFile postedFile)
        {
            HttpPostedFileBase fileBase = new HttpPostedFileWrapper(postedFile);
            return Upload(fileBase);
        }

        public string Upload(HttpPostedFileBase postedFile)
        {
            string fileSaveName = GetImageName();
            string[] acceptedFileExtentions = { ".jpg", ".jpeg", ".gif", ".png" };
            //if (!VirtualUploadDirPath.Contains("~/")) VirtualUploadDirPath = "~/" + VirtualUploadDirPath;
            if (VirtualUploadDirPath[0] != '/') VirtualUploadDirPath = "/" + VirtualUploadDirPath;
            string fileDir = HttpContext.Current.Server.MapPath(VirtualUploadDirPath);

            // Directory Exists     ?             
            if (!(new DirectoryInfo(fileDir)).Exists)
            {
                Message = "Failed to UPLOAD file : Directory location does not exist on server.";
                UploadErrorType = UploadError.DirNotExist;
                return Message;
            }

            // File Supplied        ? 
            if (postedFile == null || String.IsNullOrEmpty(postedFile.FileName) || postedFile.ContentLength == 0 || postedFile.InputStream == null)
            {
                Message = "Failed to UPLOAD file : That is not a valid file!";
                UploadErrorType = UploadError.FileNotValid;
                return Message;
            }
            else
            {
                // NOTE: PostedFile.FileName gives the entire path
                HttpPostedFileBase httpPostedFile = postedFile;
                int fileSize = httpPostedFile.ContentLength; // in bytes
                string fileType = httpPostedFile.ContentType; // MIME Type :  image/jpeg , image/gif , image/png ...
                string fileNameWitExt = Path.GetFileName(httpPostedFile.FileName); // file name and extension : myPhoto.jpg
                string fileExtention = Path.GetExtension(fileNameWitExt).ToLower(); // .jpeg , .gif , .png ...                

                FileName = fileSaveName + fileExtention;
                string fileSavePath = fileDir + FileName;
                string fileThumbSavePath = fileDir + GetThumbFileName(fileSaveName, fileExtention);

                string strAcceptedFileExtentions = "";

                // File Extention In Accepted Extentions ?
                bool isUnacceptableFileType = true;
                foreach (string acceptedFileExtention in acceptedFileExtentions)
                {
                    if (fileExtention == acceptedFileExtention)
                    {
                        isUnacceptableFileType = false;
                        break;
                    }
                    strAcceptedFileExtentions += acceptedFileExtention + ",";
                }

                // File Type Accepted   ?
                if (isUnacceptableFileType)
                {
                    Message = "Failed to UPLOAD file : Wrong file type. The file must have one of accepted extensions: " + strAcceptedFileExtentions;
                    UploadErrorType = UploadError.FileWrongType;
                    return Message;
                }

                // File Size > 0        ?
                if (fileSize <= 0)
                {
                    Message = "Failed to UPLOAD file : Size is less or equal to 0 bytes.";
                    UploadErrorType = UploadError.FileNoData;
                    return Message;
                }

                // File Size < Max      ?
                // maxSize = 0 means no limit on size
                if (MaxSize != 0 && fileSize > MaxSize)
                {
                    Message = "Failed to UPLOAD file : File is too large to be uploaded  - size of the file is " + fileSize + " bytes. The maximum accepted file size is " + MaxSize + " bytes. ";
                    UploadErrorType = UploadError.FileTooLarge;
                    return Message;
                }

                // File Exists          ?
                if (File.Exists(fileSavePath))
                {
                    Message = "Failed to UPLOAD file : File with the same name already exists on the server at the same location.";
                    UploadErrorType = UploadError.FileWithSameNameExists;
                    return Message;
                }

                // SAVE   FILE      to disk
                // Save the stream to disk
                //http://www.codeproject.com/KB/aspnet/netimageupload.aspx
                //http://forums.asp.net/t/1166931.aspx
                //http://www.csharp-station.com/Articles/Thumbnails.aspx
                //http://aspnet.4guysfromrolla.com/articles/120606-1.aspx
                //********************************

                // Try Upload
                try
                {
                    ImageFormat imgFormat = GetFormat(fileExtention);
                    // Get the bitmap data from the uploaded file    
                    Bitmap src = Bitmap.FromStream(postedFile.InputStream) as Bitmap;
                    // Resize and save Image    
                    Bitmap result = ResizeBitmap(src, FrameWidth, FrameHeight);
                    result.Save(fileSavePath, imgFormat);
                    // Resize and save Thumbnail            
                    result = ResizeBitmap(src, FrameWidthThumb, FrameHeightThumb);
                    result.Save(fileThumbSavePath, imgFormat);

                    Message = fileSaveName + fileExtention;
                }
                catch (Exception e)
                {
                    // Rollback Uploads
                    if (File.Exists(fileSavePath))
                        System.IO.File.Delete(fileSavePath);
                    if (File.Exists(fileThumbSavePath))
                        System.IO.File.Delete(fileThumbSavePath);
                    // Return Error Message
                    Message = "Failed to UPLOAD file : Email this error message to admin: " + e.Message;
                    UploadErrorType = UploadError.Other;
                    return Message;
                    //throw;
                }
            }
            return Message;
        }


        //------------------------------
        //      COPY FILE
        //------------------------------

        public bool Copy(string toDirVirtualPath, string fileName)
        {
            try
            {
                Copy(VirtualUploadDirPath, toDirVirtualPath, fileName);
            }
            catch (Exception ex) { Message += "Failed to copy file: " + ex.Message; return false; }
            return true;
        }

        public void CopyMultiple(string toDirVirtualPath, string[] fileNames) { foreach (var fileName in fileNames) { if (!Copy(toDirVirtualPath, fileName)) break; } }

        /// <summary>
        ///     Copy both image and its thumbnail from temporary directory to the storage directory
        ///     Thumbnail is derived from image name
        /// </summary>
        public static void Copy(string fromDirVirtualPath, string toDirVirtualPath, string fileName)
        {
            string fromDirPath = HttpContext.Current.Server.MapPath("/" + fromDirVirtualPath) + "/";
            string toDirPath = HttpContext.Current.Server.MapPath("/" + toDirVirtualPath) + "/";
            if (!string.IsNullOrEmpty(fileName))
            {
                string thumbFileName = GetThumbFileName(fileName);
                File.Move(fromDirPath + fileName, toDirPath + fileName);
                File.Move(fromDirPath + thumbFileName, toDirPath + thumbFileName);
            }
        }

        /// <summary>
        /// Copy array of both images and its thumnails from virtual directory to the concrete one
        /// Thumbnail is derived from image name
        /// </summary>
        public static void CopyMultiple(string fromDirVirtualPath, string toDirVirtualPath, string[] fileNames) { foreach (var fileName in fileNames) Copy(fromDirVirtualPath, toDirVirtualPath, fileName); }


        //------------------------------
        //      DELETE FILE
        //------------------------------
        
        /// <summary>
        ///  Deletes both original files and thumbnails based on thumbnail image urls comma separated list
        /// </summary>
        /// <param name="thumbFileNames">Comma separated string of thumbnail image file names</param>
        /// <returns></returns>
        public string DeleteRangeFromThumbURLs(string thumbURLs)
        {
            string toReturn = string.Empty;
            string[] urls = thumbURLs.Split(',');
            foreach (string url in urls)
            {
                toReturn += DeleteFromThumbURL(url);
            }
            return toReturn;
        }

        /// <summary>
        ///  Deletes both original file and thumbnail based on thumbnail image url
        /// </summary>
        public string DeleteFromThumbURL(string thumbURL)
        {
            string thumbFileName = thumbURL.Substring(thumbURL.LastIndexOf("/") + 1);
            return Delete(GetImageFileNameFromThumbFileName(thumbFileName));
        }

        /// <summary>
        ///  Deletes both original file and thumbnail based on original image filename
        /// </summary>
        public string Delete(string fileNameWitExt)
        {
            Message = Delete(fileNameWitExt, VirtualUploadDirPath);
            return Message;
        }

        /// <summary>
        ///  STATIC DELETE - Deletes file and a thumbnail at the specified virtual directory path location
        /// </summary>
        public static string Delete(string fileNameWitExt, string virtualDirPath)
        {
            string message = string.Empty;
            if (string.IsNullOrEmpty(fileNameWitExt))
                message += "Failed to DELETE file : File name not supplied";
            if (string.IsNullOrEmpty(virtualDirPath))
                message += "Failed to DELETE file : File path not supplied";
            string fileDir = HttpContext.Current.Server.MapPath("/" + virtualDirPath);
            string fileExtention = Path.GetExtension(fileNameWitExt).ToLower(); // .jpeg , .gif , .png ... 
            string fileSaveName = Path.GetFileNameWithoutExtension(fileNameWitExt);
            string fileSavePath = fileDir + fileNameWitExt;
            string fileThumbSavePath = fileDir + GetThumbFileName(fileSaveName, fileExtention);
            if (File.Exists(fileSavePath))
                try
                {
                    System.IO.File.Delete(fileSavePath);
                }
                catch (Exception ex)
                {
                    message += "Failed to DELETE file : " + ex.Message + Environment.NewLine;
                }
            else
            {
                message += "Failed to DELETE file : File does not exist at the location " + fileSavePath + Environment.NewLine;
            }
            if (File.Exists(fileThumbSavePath))
                try
                {
                    System.IO.File.Delete(fileThumbSavePath);
                }
                catch (Exception ex)
                {
                    message += "Failed to DELETE file : " + ex.Message + Environment.NewLine;
                }
            else
            {
                message += "Failed to DELETE file : File does not exist at the location " + fileThumbSavePath + Environment.NewLine;
            }
            return message;
        }
        

        //------------------------------
        //      HELPER METHODS
        //------------------------------

        /// <summary>
        ///  Autogen file name with extention using GUID with removed hyphens and prefix with img_ .
        ///  Check if file with same name already exists on server. if yes then recursively call this method again.
        /// </summary>
        public static string GetFileName(string saveDir, string fileExtention)
        {
            string guidUniqueName = System.Guid.NewGuid().ToString();
            guidUniqueName = guidUniqueName.Replace("-", string.Empty);
            string fileName = "img_" + guidUniqueName + fileExtention;
            string filePath = saveDir + fileName;
            // File Exists          ? call again this method to generate new name
            if (File.Exists(filePath))
                fileName = GetFileName(saveDir, fileExtention);
            return fileName;
        }

        /// <summary>
        ///  Get image format based on the file extension string
        /// </summary>
        public static ImageFormat GetFormat(string extension)
        {
            ImageFormat imgFormat = null;
            switch (extension)
            {
                case ".jpeg":
                case ".jpg": imgFormat = ImageFormat.Jpeg; break;
                case ".gif": imgFormat = ImageFormat.Gif; break;
                case ".png": imgFormat = ImageFormat.Png; break;
                case ".bmp": imgFormat = ImageFormat.Bmp; break;
                case ".tiff": imgFormat = ImageFormat.Tiff; break;
            }
            return imgFormat;
        }

        public static Bitmap ResizeBitmap(Bitmap src, int FrameWidth, int FrameHeight)
        {
            Size newSize = Resize(src.Size, FrameWidth, FrameHeight);
            // Create new Bitmap at new dimensions    
            Bitmap result = new Bitmap(newSize.Width, newSize.Height);
            using (Graphics g = Graphics.FromImage((System.Drawing.Image)result))
            {
                g.DrawImage(src, 0, 0, newSize.Width, newSize.Height);
            }
            return result;
        }

        /// <summary>
        ///   Get new size without losing image quality,
        ///     Resize proportionally to original image and within the bounds of FrameWidth and FrameHeight
        /// </summary>
        public static Size Resize(Size oldSize, int FrameWidth, int FrameHeight)
        {
            // original dimensions    
            int w = oldSize.Width;
            int h = oldSize.Height;
            // Longest and shortest dimension    
            int longestDimension = (w > h) ? w : h;
            int shortestDimension = (w < h) ? w : h;
            // propotionality    
            float factor = ((float)longestDimension) / shortestDimension;
            // default width is greater than height    
            double newWidth = FrameWidth;
            double newHeight = FrameWidth / factor;
            // if height greater than width recalculate    
            if (w < h)
            {
                newWidth = FrameHeight / factor;
                newHeight = FrameHeight;
            }
            Size newSize = new Size((int)newWidth, (int)newHeight);
            return newSize;
        }

        #region [ Name and Path manipulation ]

        /// <summary>
        ///  Dynamic Thumbnail Creation and retreival protocol
        /// </summary>
        public static string GetThumbnailURL(string imgURL)
        {
            int startIndex = (imgURL.LastIndexOf("/") == -1 ? 0 : imgURL.LastIndexOf("/") + 1);
            string imgNameWithExt = imgURL.Substring(startIndex);
            string thumbDir = imgURL.Substring(0, imgURL.IndexOf(imgNameWithExt));
            string thumbURL = thumbDir + GetThumbFileName(imgNameWithExt);
            return thumbURL;
        }

        /// <summary>
        ///  Dynamic Image Name Creation protocol
        /// </summary>
        public static string GetImageName()
        {
            return "img_" + Util.GUID_UniqueName();
        }

        /// <summary>
        ///  Dynamic Thumbnail Creation and retreival protocol
        /// </summary>
        public static string GetThumbFileName(string fileName)
        {
            string thumbFileName = string.Empty;
            if (fileName != null && fileName != string.Empty && fileName != "undefined")
            {
                char[] splitter = { '.' };
                string[] parts = new string[2];
                parts = fileName.Split(splitter);
                thumbFileName = GetThumbFileName(parts[0], "." + parts[1]);
            }
            return thumbFileName;
        }

        /// <summary>
        ///  Dynamic Thumbnail Creation and retreival protocol
        /// </summary>
        public static string GetThumbFileName(string fileName, string fileExtention)
        {
            return fileName + thumbExtention + fileExtention;
        }

        public static string GetImageFileNameFromThumbFileName(string thumbFileName)
        {
            return thumbFileName.Replace(thumbExtention, string.Empty);
        }

        /// <summary>
        ///  extract file name from URL
        /// </summary>
        public static string GetFileNameFromURL(string url) { return url.Substring(url.LastIndexOf('/')); }

        #endregion


    }
}
