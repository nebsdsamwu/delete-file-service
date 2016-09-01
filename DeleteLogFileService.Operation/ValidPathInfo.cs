using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Text.RegularExpressions;
using Newegg.BigData.Framework.Common;
using DeleteLogFileService.Configuration;
using DeleteLogFileService.Utility;

namespace DeleteLogFileService.Operation
{
    public class ValidPathInfo
    {
        private Dictionary<string, DriveInfo> disks;
        private List<ValidDirInfo> validDirs;

        public Dictionary<string, DriveInfo> Disks
        {
            get { return disks; }
            set { disks = value; }
        }

        public List<ValidDirInfo> ValidDirs
        {
            get { return validDirs; }
            set { validDirs = value; }
        }

        #region Constructor
        public ValidPathInfo(FolderConfigurationSection configSection = null)
        {
            var dNames = new List<string>();
            var validDirs = new List<ValidDirInfo>();

            try
            {
                configSection = configSection ?? ConfigurationManager.GetSection("folderSettings") as FolderConfigurationSection;
                string defaultPattern = Utilities.CON_DefaultPathPattern.Equals("") ? ".*" : Utilities.CON_DefaultPathPattern;

                foreach (FolderConfig setting in configSection.Settings)
                {
                    // find valid disks
                    string dName = setting.Path.Split(':')[0];
                    if (!Regex.IsMatch(dName, @"^[a-zA-Z]$"))
                    {
                        LogHelper.TraceLog("Invalid path: [" + setting.Path + "]", LogType.Error);
                    }
                    else if (!dNames.Contains(dName, StringComparer.OrdinalIgnoreCase))
                    {
                        dNames.Add(dName);
                    }

                    // get directories info from config 
                    try
                    {
                        var dir = new DirectoryInfo(setting.Path);
                        if (dir.Exists)
                        {
                            int days = ShareFunctions.GetIntValueNotNull(setting.RemainDays);
                            days = (days > 0) ? days : Utilities.CON_DefaultRemainDays;

                            if (days <= 0)
                            {
                                days = Utilities.CON_DefaultSetDays; // 7;
                                LogHelper.TraceLog("Invalid DefaultRemainDays value in config file. Set as " + days, LogType.Error);
                            }

                            string aPattern = ShareFunctions.GetStringValue(setting.Pattern);
                            aPattern = aPattern.Equals("") ? defaultPattern : aPattern;

                            var vDir = new ValidDirInfo(dir, days, aPattern);
                            validDirs.Add(vDir);
                        }
                        else
                        {
                            throw new System.NotSupportedException();
                        }
                    }
                    catch (System.NotSupportedException)
                    {
                        LogHelper.TraceLog("Invalid path: [" + setting.Path + "]", LogType.Error);
                        continue;
                    }
                }

                this.disks = GetValidDisksInfo(dNames);

                // validates directories and all subdirectories
                var resultDirs = new List<ValidDirInfo>();

                foreach (ValidDirInfo vd in validDirs) // each Directories
                {
                    if (this.disks.ContainsKey(vd.DirInfo.Root.Name.Substring(0, 1)))
                    {
                        // get all the subdirectories
                        List<string> mDirs = Directory.EnumerateDirectories(vd.DirInfo.FullName, "*", SearchOption.AllDirectories).ToList();

                        // add parent path itself
                        mDirs.Add(vd.DirInfo.FullName);

                        // filter with regular expression
                        foreach (string d in mDirs)
                        {
                            if (!Regex.IsMatch(d, vd.Pattern, RegexOptions.IgnoreCase))
                            {
                                continue;
                            }
                            var subDir = new ValidDirInfo(new DirectoryInfo(d), vd.RemainDays, vd.Pattern);
                            resultDirs.Add(subDir);
                        }
                    }
                }

                this.validDirs = resultDirs;
            }
            catch (Exception ex)
            {
                LogHelper.TraceLog(ex.ToString(), LogType.Exception);
            }
        }
        #endregion

        public List<ValidPathInfo> GetPatternMatchedPath(string pattern)
        {
            var rDirs = new List<ValidPathInfo>();

            return rDirs;
        }

        public Dictionary<string, DriveInfo> GetValidDisksInfo(List<string> dNames = null)
        {
            Dictionary<string, DriveInfo> localDisks = GetLocalDisksInfo();
            if (localDisks == null) return null;

            var validDisks = new Dictionary<string, DriveInfo>(StringComparer.OrdinalIgnoreCase);
            dNames = dNames ?? GetConfigDiskNames();

            foreach (string name in dNames)
            {
                DriveInfo info = new DriveInfo(name);
                if (localDisks.TryGetValue(name, out info))
                {
                    validDisks.Add(name, info);
                }
                else
                {
                    LogHelper.TraceLog("Invalid disk name: [" + name + ":\\]", LogType.Error);
                }
            }
            return validDisks;
        }

        // Get disk names from config setting.
        public List<string> GetConfigDiskNames(FolderConfigurationSection configSection = null)
        {
            try
            {
                configSection = configSection ?? ConfigurationManager.GetSection("folderSettings") as FolderConfigurationSection;

                List<string> disks = new List<string>();
                foreach (FolderConfig setting in configSection.Settings)
                {
                    string dName = setting.Path.Split(':')[0];
                    if (!Regex.IsMatch(dName, @"^[a-zA-Z]$"))
                    {
                        LogHelper.TraceLog("Invalid path: [" + setting.Path + "]", LogType.Error);
                    }
                    else if (!disks.Contains(dName))
                    {
                        disks.Add(dName);
                    }
                }
                return disks;
            }
            catch (Exception ex)
            {
                LogHelper.TraceLog(MethodBase.GetCurrentMethod(), ex);
                return null;
            }
        }

        public Dictionary<string, DriveInfo> GetDisksDictionary(List<DriveInfo> dList)
        {
            if (dList == null || dList.Count == 0) return null;

            var disks = new Dictionary<string, DriveInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (DriveInfo d in dList)
            {
                disks.Add(d.Name.Substring(0, 1), d);
            }
            return disks;
        }

        public Dictionary<string, DriveInfo> GetLocalDisksInfo()
        {
            return GetDisksDictionary(GetDisksInfo(DriveType.Fixed));
        }

        public List<DriveInfo> GetLocalDisksInfoList()
        {
            return GetDisksInfo(DriveType.Fixed);
        }

        public List<DriveInfo> GetDisksInfo(DriveType? type = null)
        {
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                if (drives == null || drives.Length == 0) return null;

                if (type != null)
                {
                    drives = drives.Where(d => d.DriveType == type).ToArray();
                }
                return (drives.Length != 0) ? drives.ToList<DriveInfo>() : null;
            }
            catch (Exception ex)
            {
                LogHelper.TraceLog(MethodBase.GetCurrentMethod(), ex);
                return null;
            }
        }
    }

    public class ValidDirInfo
    {
        public DirectoryInfo DirInfo { get; set; }
        public int RemainDays { get; set; }
        public string Pattern { get; set; }

        public ValidDirInfo(DirectoryInfo info, int days, string pattern)
        {
            DirInfo = info;
            RemainDays = days;
            Pattern = pattern;
        }
    }
}
