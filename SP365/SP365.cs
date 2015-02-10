using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using System.Security;
using System.IO;

namespace BizagiCL
{
    public class SP365
    {
        private static Folder GetFolder(ClientContext clientContext, string parentFolder, string subFolder)
        {
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

        public static void PublishFile(string website, sbyte[] sdata, string fileName, string userName, string password, string parentFolder, string folder)
        {
            byte[] data = new byte[sdata.Length];
            Buffer.BlockCopy(sdata, 0, data, 0, sdata.Length);

            using (ClientContext clientContext = new ClientContext(website))
            {
                SecureString passWord = new SecureString();

                foreach (char c in password.ToCharArray()) passWord.AppendChar(c);

                clientContext.Credentials = new SharePointOnlineCredentials(userName, passWord);

                if (folder == null)
                {
                    folder = "";
                }
                MemoryStream ms = new MemoryStream();
                ms.Write(data, 0, data.Length);
                ms.Flush();
                string fileNameref = fileName;
                if (fileNameref.IndexOf("\\") > 0)
                {
                    fileNameref = fileNameref.Substring(fileNameref.LastIndexOf("\\") + 1);
                }
                string relativeUrlPath = "/" + Path.Combine(parentFolder, folder, fileNameref).Replace("\\", "/");
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, relativeUrlPath, ms, true);
                ms.Close();

                /*
                Web web = clientContext.Web;

                clientContext.Load(web);

                FileCreationInformation newFile = new FileCreationInformation();
                newFile.Content = data;
                string fileNameref = fileName;
                if (fileNameref.IndexOf("\\") > 0)
                {
                    fileNameref = fileNameref.Substring(fileNameref.LastIndexOf("\\") + 1);
                }
                //if (folder.IndexOf("/") > -1)
                //{
                //    fileNameref = folder.Substring(folder.IndexOf("/") + 1) + "/" + fileNameref;
                //    folder = folder.Substring(0, folder.LastIndexOf("/"));
                //}
                newFile.Url = fileNameref;

                Folder spFolder = GetFolder(clientContext, parentFolder, folder);
                //List docs = web.Lists.GetByTitle(folder);
                Microsoft.SharePoint.Client.File uploadFile = spFolder.Files.Add(newFile);
                clientContext.Load(uploadFile);
                clientContext.ExecuteQuery();
                 */
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
