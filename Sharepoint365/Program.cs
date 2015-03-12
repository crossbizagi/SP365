using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using System.Security;
using System.Configuration;
using System.Threading;

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
                string user = ReadValue("Username", defaultUser);
                string password = ReadValue("Password", defaultPassword);
                string file = ReadValue("File to upload", "c:\\temp\\bmw.jpg");
                string parentFolder = ReadValue("Parent Folder", "Documents");
                string folder = ReadValue("Folder", "BizagiFolder");

                byte[] content = System.IO.File.ReadAllBytes(file);

                sbyte[] scontent = new sbyte[content.Length];

                Buffer.BlockCopy(content, 0, scontent, 0, content.Length);

                Console.WriteLine("Uploading file....");
//                BizagiCL.SP365.PublishFile(website, scontent, file, user, password, "Compliance_Case_Documents", "Shared Documents/BizagiFolder");
                BizagiCL.SP365.PublishFile(website, scontent, file, user, password, parentFolder, folder);
                Console.WriteLine("File uploaded");

            }
            catch (Exception e)
            {
                Console.Write("Error: " + e.Message);
            }
        }

        static void TestDelete()
        {
            try
            {
                string user = ReadValue("Username", defaultUser);
                string password = ReadValue("Password", defaultPassword);
                string file = ReadValue("File to delete", "c:\\temp\\I did it.jpg");
                string parentFolder = ReadValue("Parent Folder", "Documents");
                string folder = ReadValue("Folder", "BizagiFolder");

                Console.WriteLine("Deleting file....");
                BizagiCL.SP365.DeleteFile(website, file, user, password, parentFolder, folder);
                Console.WriteLine("Deleted. Done");
            }
            catch (Exception e)
            {
                Console.Write("Error: " + e.Message);
            }
        }


        static void TestFolder()
        {
            string user = ReadValue("Username", defaultUser);
            string password = ReadValue("Password", defaultPassword);
            string parentFolder = ReadValue("Folder", "Documents");
            string folder = ReadValue("Folder", "TestFolder");

            Console.WriteLine("Creating folder....");
            BizagiCL.SP365.CreateFolder(website, user, password, parentFolder, folder);
            Console.WriteLine("Done");
        }

        public delegate void f();

        class Option
        {
            public string text;
            public f function;
        };

        static void DrawMenu(List<Option> options, bool repeat)
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("Select the option");
                Console.WriteLine("=================");
                int x = 1;
                Dictionary<string, f> events = new Dictionary<string, f>();
                foreach (Option option in options)
                {
                    Console.WriteLine(string.Format("   {0}. {1}", x, option.text));
                    events.Add(Convert.ToString(x), option.function);
                    x++;
                }
                Console.WriteLine("\n  99. Exit");
                Console.Write("Select an option: ");
                string selectedOption = Console.ReadLine();
                if (events.ContainsKey(selectedOption))
                {
                    f function = events[selectedOption];
                    Console.Clear();
                    function();
                }
                else if (selectedOption == "99")
                {
                    Console.WriteLine("Bye!");
                    break;
                }
                else
                {
                    Console.WriteLine("Really?");
                }
                Thread.Sleep(1000);
            }

        }

        static void RunMenu()
        {
            List<Option> options = new List<Option>();

            options.Add(new Option
            {
                text = "Add File",
                function = testNewFile
            });

            options.Add(new Option
            {
                text = "Create Folder",
                function = TestFolder
            });

            options.Add(new Option
            {
                text = "Test Delete",
                function = TestDelete
            });

            DrawMenu(options, true);
        }

        static void Main(string[] args)
        {
            RunMenu();
            /*
            //            TestFolder();
            testNewFile();

            string user = ReadValue("User", defaultUser);
            string password = ReadValue("Password", defaultPassword);
            TestDelete(user, password);
            Console.ReadLine();
             * */
        }
    }
}
