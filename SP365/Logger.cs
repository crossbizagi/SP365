using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace SP365
{
    public class Logger
    {
        private static Logger _instance;
        private static string dateFormat = "yyyy/MM/dd hh:mm::ss";

        public bool IsDebug { get; set; }
        public bool IsInfo { get; set; }
        public bool IsWarning { get; set; }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    Logger log = new Logger();
                    log.IsInfo = true;
                    string loglevel = ConfigurationSettings.AppSettings["loglevel"];
                    if (string.IsNullOrEmpty(loglevel))
                    {
                        loglevel = "0";
                    }
                    switch (loglevel)
                    {
                        case "1":
                            log.IsInfo = true;
                            break;
                        case "2":
                            log.IsInfo = true;
                            log.IsWarning = true;
                            break;
                        case "3":
                            log.IsInfo = true;
                            log.IsWarning = true;
                            log.IsDebug = true;
                            break;
                        default:
                            log.IsInfo = false;
                            log.IsWarning = false;
                            log.IsDebug = false;
                            break;
                    }
                    _instance = log;
                }
                return _instance;
            }
        }

        private void WriteLog(string message)
        {
            try
            {
                string fileName = string.Format("LOG_{0}.log", DateTime.Now.ToString("yyyyMMdd"));
                string fullFileName = Path.Combine(System.IO.Path.GetTempPath(), fileName);

                byte[] bMessage = Encoding.Default.GetBytes(message);


                FileStream fs = new FileStream(fullFileName, FileMode.Append);
                fs.Write(bMessage, 0, bMessage.Length);
                fs.Close();
            } catch {}
        }

        public void Info(string message)
        {
            if (IsInfo)
            {
                WriteLog(string.Format("INFO  {0}: {1}\r\n", DateTime.Now.ToString(dateFormat), message));
            }
        }

        public void Debug(string message)
        {
            if (IsDebug)
            {
                WriteLog(string.Format("DEBUG {0}: {1}\r\n", DateTime.Now.ToString(dateFormat), message));
            }
        }

        public void Warning(string message)
        {
            if (IsDebug)
            {
                WriteLog(string.Format("WARN  {0}: {1}\r\n", DateTime.Now.ToString(dateFormat), message));
            }
        }

        public void Error(string message)
        {
            WriteLog(string.Format("ERROR {0}: {1}\r\n", DateTime.Now.ToString(dateFormat), message));
        }

    }
}
