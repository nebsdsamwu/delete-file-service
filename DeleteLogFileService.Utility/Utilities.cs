using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Newegg.BigData.Framework.Common;

namespace DeleteLogFileService.Utility
{
    public class Utilities
    {
        public static readonly string CON_FileExts = ShareFunctions.GetStringValue(ConfigurationManager.AppSettings["FileExts"]);
        public static readonly string CON_DefaultPathPattern = ShareFunctions.GetStringValue(ConfigurationManager.AppSettings["DefaultPathPattern"]);
        public static readonly int CON_DefaultRemainDays = ShareFunctions.GetIntValueNotNull(ConfigurationManager.AppSettings["DefaultRemainDays"]);
        public static readonly int CON_PerSecondsRunforRemainDays = ShareFunctions.GetIntValueNotNull(ConfigurationManager.AppSettings["PerSecondsRunforRemainDays"]);
        public static readonly int CON_PerSecondsRunforFreeDiskSpaceCheck = ShareFunctions.GetIntValueNotNull(ConfigurationManager.AppSettings["PerSecondsRunforFreeDiskSpaceCheck"]);
        public static readonly int CON_FreeDiskSpaceThreshold = ShareFunctions.GetIntValueNotNull(ConfigurationManager.AppSettings["FreeDiskSpaceThreshold"]);
        public static readonly int CON_EveryFolderFileSizeRemain = ShareFunctions.GetIntValueNotNull(ConfigurationManager.AppSettings["EveryFolderFileSizeRemain"]);
        public static readonly int CON_DefaultSetDays = 7;
        public static readonly char CON_FileExtsSplitter = ',';

        #region For LogHelper
        public static readonly string CON_LogFolder = ShareFunctions.GetStringValue(ConfigurationManager.AppSettings["LogFolder"]);
        public static readonly string CON_MinLogType = ShareFunctions.GetStringValue(ConfigurationManager.AppSettings["MinLogType"]);
        public static readonly bool CON_TraceProgramDebugLog = ShareFunctions.GetBoolValueNotNull(ConfigurationManager.AppSettings["TraceProgramDebugLog"]);
        public static readonly int CON_MaxLogSizeMB = ShareFunctions.GetIntValueNotNull(ConfigurationManager.AppSettings["MaxLogSizeMB"]);
        public static readonly int CON_LogFileNoLength = ShareFunctions.GetIntValueNotNull(ConfigurationManager.AppSettings["LogFileNoLength"]);
        public static readonly bool CON_Log4NetEnabled = ShareFunctions.GetBoolValueNotNull(ConfigurationManager.AppSettings["Log4NetEnabled"]);
        #endregion
    }
}
