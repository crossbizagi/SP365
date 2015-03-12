using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using System.Security;
using System.IO;
using SP365;

namespace BizagiCL
{
    public class SP365
    {
        private static Folder GetFolder(ClientContext clientContext, string parentFolder, string subFolder)
        {
            try
            {
                if (Logger.Instance.IsDebug) Logger.Instance.Debug(string.Format("GetFolder: {0}/{1}", parentFolder, subFolder));
                Web web = clientContext.Web;

                clientContext.Load(web);

                // Loads the folder
                List docs = web.Lists.GetByTitle(parentFolder);
                Folder spFolder;
                if (!string.IsNullOrEmpty(subFolder))
                {
                    clientContext.Load(docs.RootFolder);
                    clientContext.ExecuteQuery();
                    string url = docs.RootFolder.Name;
                    url += "/" + subFolder;

                    try
                    {
                        spFolder = web.GetFolderByServerRelativeUrl(url);
                        clientContext.Load(spFolder);
                        clientContext.ExecuteQuery();
                    }
                    catch
                    {
                        if (Logger.Instance.IsInfo) Logger.Instance.Info(string.Format("Failed to load the subfolder. Creating folder {0}/{1}", parentFolder, subFolder));
                        internalCreateFolder(clientContext, parentFolder, subFolder);
                        spFolder = web.GetFolderByServerRelativeUrl(url);
                        clientContext.Load(spFolder);
                        clientContext.ExecuteQuery();
                    }

                }
                else
                {
                    spFolder = docs.RootFolder;
                    clientContext.Load(spFolder);
                }

                return spFolder;
            }
            catch (Exception e)
            {
                Logger.Instance.Error(string.Format("Error in GetFolder: {0}\r\nStackTrace: {1}", e.Message, e.StackTrace.ToString()));
                throw e;
            }
        }

        public static void PublishFile(string website, sbyte[] sdata, string fileName, string userName, string password, string parentFolder, string folder)
        {
            if (Logger.Instance.IsDebug) Logger.Instance.Debug(string.Format("PublishFile: {0}/{1}/{2}/{3}", website, fileName, parentFolder, folder));

            try
            {
                byte[] data = new byte[sdata.Length];
                Buffer.BlockCopy(sdata, 0, data, 0, sdata.Length);

                using (ClientContext clientContext = new ClientContext(website))
                {
                    SecureString passWord = new SecureString();

                    foreach (char c in password.ToCharArray()) passWord.AppendChar(c);

                    clientContext.Credentials = new SharePointOnlineCredentials(userName, passWord);

                    Web web = clientContext.Web;

                    clientContext.Load(web);

                    Folder spFolder = GetFolder(clientContext, parentFolder, folder);

                    if (folder == null)
                    {
                        folder = "";
                    }
                    MemoryStream ms = new MemoryStream();
                    ms.Write(data, 0, data.Length);
                    //                ms.Flush();
                    string fileNameref = fileName;
                    if (fileNameref.IndexOf("\\") > 0)
                    {
                        fileNameref = fileNameref.Substring(fileNameref.LastIndexOf("\\") + 1);
                    }
                    string relativeUrlPath = spFolder.ServerRelativeUrl + "/" + fileNameref;

                    ms.Seek(0, SeekOrigin.Begin);

                    if (Logger.Instance.IsInfo) Logger.Instance.Info(string.Format("Uploading file {0} to: {1}", fileName, relativeUrlPath));

                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, relativeUrlPath, ms, true);
                    ms.Close();
                    if (Logger.Instance.IsInfo) Logger.Instance.Info(string.Format("Sucessfully uploaded file {0} to: {1}", fileName, relativeUrlPath));

                }
            } 
            catch (Exception e)
            {
                Logger.Instance.Error(string.Format("Error in PublishFile: {0}\r\nStackTrace: {1}", e.Message, e.StackTrace.ToString()));
                throw e;
            }
        }

        public static void DeleteFile(string website, string fileName, string userName, string password, string folder, string subFolder)
        {
            using (ClientContext clientContext = new ClientContext(website))
            {
                SecureString passWord = new SecureString();

                foreach (char c in password.ToCharArray()) passWord.AppendChar(c);

                clientContext.Credentials = new SharePointOnlineCredentials(userName, passWord);

                Web web = clientContext.Web;

                clientContext.Load(web);

                string fileNameref = fileName;
                if (fileNameref.IndexOf("\\") > 0)
                {
                    fileNameref = fileNameref.Substring(fileNameref.LastIndexOf("\\") + 1);
                }
                // Get list document
                Folder sFolder = GetFolder(clientContext, folder, subFolder);

                FileCollection files = sFolder.Files;
                clientContext.Load(files);
                clientContext.ExecuteQuery();

                foreach (Microsoft.SharePoint.Client.File file in files)
                {
                    if (file.Name == fileNameref)
                    {
                        file.DeleteObject();
                        break;
                    }
                }
                clientContext.ExecuteQuery();
            }
        }

        private static void internalCreateFolder(ClientContext clientContext, string parentFolder, string folder)
        {
            Web web = clientContext.Web;

            clientContext.Load(web);

            List docs = web.Lists.GetByTitle(parentFolder);
            //clientContext.Load(docs);
            ListItemCreationInformation list = new ListItemCreationInformation();
            Folder spFolder = docs.RootFolder.Folders.Add(folder);

            docs.Update();

            clientContext.ExecuteQuery();

        }

        public static void CreateFolder(string website, string userName, string password, string parentFolder, string newFolder)
        {
            using (ClientContext clientContext = new ClientContext(website))
            {
                SecureString passWord = new SecureString();

                foreach (char c in password.ToCharArray()) passWord.AppendChar(c);

                clientContext.Credentials = new SharePointOnlineCredentials(userName, passWord);

                internalCreateFolder(clientContext, parentFolder, newFolder);
            }
        }
    }
}
