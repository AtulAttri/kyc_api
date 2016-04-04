using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using log4net;
using System.Web;

namespace ecomm.util
{
    public class ExternalLogger
    {
        public log4net.ILog logger;
        private static object syncRoot = new object();
        protected static ExternalLogger Instance;
        public string className;

        private ExternalLogger()
        {           

        }
        #region Internal methods
        public static void LogError(Exception ex, object objName, string User)
        {
            
            ExternalLogger elogger = GetInstance();
            Type objectType = objName.GetType();
            string className = objectType.Name.ToString();
            lock (syncRoot)
            {
                elogger.logger.Error("Error Occurred In Class: " + className);
                //elogger.logger.Error("User Experiencing Error: " + User);
                elogger.logger.Error("Core Error Message: " + ex.Message);
                elogger.logger.Error(ex.StackTrace);
            }
        }
        public static void LogInfo(string Message, object objName, string User)
        {
            ExternalLogger elogger = GetInstance();
            Type objectType = objName.GetType();
            string className = objectType.Name.ToString();
            lock (syncRoot)
            {
                elogger.logger.Info(className + " " + User + "==>" + Message);
            }
        }
        public static void LogDebug(string Message, object objName)
        {
            ExternalLogger elogger = GetInstance();
            Type objectType = objName.GetType();
            string className = objectType.Name.ToString();
            lock (syncRoot)
            {
                elogger.logger.Debug(className + "==>" + Message);
            }
        }
        public static ExternalLogger GetInstance()
        {
            log4net.Config.XmlConfigurator.Configure();
            if (Instance == null)
            {
                lock (syncRoot)
                {
                    if (Instance == null)
                    {
                        Instance = new ExternalLogger();
                        Instance.logger = log4net.LogManager.GetLogger("LogFile");
                    }
                }
            }

            return Instance;
        }

        /*
        public static string GetLoginUser()
        {
            if (ContextFactory.IsBatch())
                return "";
            else
                return (string)HttpContext.Current.Session["login"];
        }
         */ 
        #endregion
    }
}
