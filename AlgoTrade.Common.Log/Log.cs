using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
//using AlgoTrade.DAL.Provider;

namespace AlgoTrade.Common.Log
{
    public sealed class LoggerManager : Object
    {
        private static Logger _logger=LogManager.GetCurrentClassLogger();
        public LoggerManager()
            : base()
        {
            if (_logger == null)
                throw new System.ApplicationException("Logger cannot be null");

        }
        public static void SetInfoLogFile(string InfoLogFile) 
        {
            if (String.IsNullOrEmpty(InfoLogFile))
            {
                _logger.Error(" InfoLogFile is empty");
                return;
            }
            try
            {
                LoggingConfiguration config = LogManager.Configuration;

                var logFile = new FileTarget();
                config.AddTarget("file", logFile);

                logFile.FileName = InfoLogFile + ".log";
                logFile.Layout = "${date} | ${message}";

                var rule = new LoggingRule("*", LogLevel.Info, logFile);
                config.LoggingRules.Add(rule);

                LogManager.Configuration = config;
            }
            catch (Exception er)
            {
                _logger.Error(er.Message);

            }
            _logger.Info(" Log Info file set to " + InfoLogFile);

        }
        public static void LogTrace(string Message)
        {
            _logger.Trace(Message);
            LogToDB("Trace", Message);
        }
        public static void LogDebug(string Message)
        {
            _logger.Trace(Message);
        }
        public static void LogError(string Message)
        {
            _logger.Trace(Message);
        }
        public static void LogInfo(string Message)
        {
            _logger.Trace(Message);
        }

        public static void LogProfit(string Symbol,float Profit)
        {
            _logger.Trace("");
        }

       private static void LogToDB(string LogType,string Message)
       {
        //    DAL.Provider.Log theLog = new DAL.Provider.Log();
        //    theLog.LogType = LogType;
        //    theLog.Message = Message;
        //    theLog.CreateDate = DateTime.Now;
        //    DataAdapter dataAdapter = new DataAdapter();
        //    dataAdapter.LogData(theLog);
            
        }

        
    }
}
