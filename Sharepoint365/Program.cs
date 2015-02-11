using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Configuration;

namespace Sharepoint365
{
    class Program
    {
        static string website = ConfigurationSettings.AppSettings["Sharepoint_URL"];
        static string defaultUser = ConfigurationSettings.AppSettings["DefaultUser"];
        static string defaultPassword = ConfigurationSettings.AppSettings["DefaultPassword"];

        static string ReadValue(string text, string defaultValue)
        {
            Console.Write(string.Format("{0} ({1}): ", text, defaultValue));
            string value = Console.ReadLine();
            if (string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }

            return value;
        }

        static void testNewFile()
        {
            try
            {


                Console.Write(string.Format("Username ({0}):", defaultUser));
                string user = Console.ReadLine();
                if (string.IsNullOrEmpty(user))
                {
                    user = defaultUser;
                }

                Console.Write(string.Format("Password ({0}):", defaultPassword));
                string password = Console.ReadLine();
                if (string.IsNullOrEmpty(password))
                {
                    password = defaultPassword;
                }

                Console.Write("File to upload (c:\\temp\\bmw.jpg: ");
                string file = Console.ReadLine();
                if (string.IsNullOrEmpty(file))
                {
                    file = "c:\\temp\\bmw.jpg";
                }

                string folder = ReadValue("Folder", null);

                byte[] content = System.IO.File.ReadAllBytes(file);

                sbyte[] scontent = new sbyte[content.Length];

                Buffer.BlockCopy(content, 0, scontent, 0, content.Length);

                Console.WriteLine("Uploading file....");
//                BizagiCL.SP365.PublishFile(website, scontent, file, user, password, "Compliance_Case_Documents", "Shared Documents/BizagiFolder");
                BizagiCL.SP365.PublishFile(website, scontent, file, user, password, "Documents", "BizagiFolder");
                Console.WriteLine("File uploaded");

            }
            catch (Exception e)
            {
                Console.Write("Error: " + e.Message);
            }
            Console.ReadLine();
        }

        static void TestDelete(string user, string password)
        {
            try
            {
                string file = ReadValue("File to delete", "c:\\temp\\I did it.jpg");

                string folder = ReadValue("Folder", null);

                Console.WriteLine("Deleting file....");
                BizagiCL.SP365.DeleteFile(website, file, user, password, "Documents", folder);
                Console.WriteLine("Deleted. Done");
            }
            catch (Exception e)
            {
                Console.Write("Error: " + e.Message);
            }
            Console.ReadLine();
        }


        static void TestFolder()
        {
            Console.Write(string.Format("Username ({0}):", defaultUser));
            string user = Console.ReadLine();
            if (string.IsNullOrEmpty(user))
            {
                user = defaultUser;
            }

            Console.Write(string.Format("Password ({0}):", defaultPassword));
            string password = Console.ReadLine();
            if (string.IsNullOrEmpty(password))
            {
                password = defaultPassword;
            }

            Console.Write("Folder to create (TestFolder)");
            string file = Console.ReadLine();
            if (string.IsNullOrEmpty(file))
            {
                file = "TestFolder";
            }

            Console.WriteLine("Deleting file....");
            BizagiCL.SP365.CreateFolder(website, user, password, "Documents", file);
            Console.WriteLine("Deleted. Done");
        }

        static void testEmptyFolder()
        {
            Console.Write("File to upload (c:\\temp\\I did it.jpg");
            string file = Console.ReadLine();
            if (string.IsNullOrEmpty(file))
            {
                file = "c:\\temp\\I did it.jpg";
            }

            byte[] content = System.IO.File.ReadAllBytes(file);

            sbyte[] scontent = new sbyte[content.Length];

            Buffer.BlockCopy(content, 0, scontent, 0, content.Length);

            BizagiCL.SP365.PublishFile(website, scontent, file, defaultUser, defaultPassword, "Documents", null);
        }

        static void Main(string[] args)
        {
//            TestFolder();
            testNewFile();

            string user = ReadValue("User", defaultUser);
            string password = ReadValue("Password", defaultPassword);
            TestDelete(user, password);
            Console.ReadLine();
        }
    }
}
