using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using Newegg.BigData.Framework.Common;
using DeleteLogFileService.Utility;

namespace DeleteLogFileService.Operation
{
    public class FileDeleter
    {
        ValidPathInfo pathInfo;

        private static readonly int bytesInMb = 1048576; // 1024 * 1024
        private List<string> targetExts;
        private DateTime curTime;

        public bool GetDirInfo()
        {
            pathInfo = new ValidPathInfo();

            if (pathInfo.Disks == null || pathInfo.Disks.Count == 0 || pathInfo.ValidDirs == null || pathInfo.ValidDirs.Count == 0)
            {
                LogHelper.TraceLog("Invalid folder settings in config.", LogType.Error);
                return false;
            }

            if (Utilities.CON_FileExts.Equals(""))
            {
                LogHelper.TraceLog("Extension setting [FileExts] dose not exist!", LogType.Exception);
                return false;
            }
            else
            {
                targetExts = Utilities.CON_FileExts.Split(Utilities.CON_FileExtsSplitter).ToList(); // split by char comma, ','
            }

            return true;
        }

        public int DeleteOutdated()
        {
            try
            {
                bool success = GetDirInfo();
                if (!success) return 0;

                int delCnt = 0;
                curTime = DateTime.Now;

                foreach (ValidDirInfo vd in pathInfo.ValidDirs) // each Path
                {
                    // get files from AllDirectories and their subdirectories
                    FileInfo[] filesInfo = vd.DirInfo.GetFiles("*", SearchOption.AllDirectories);

                    foreach (string ext in targetExts) // each Extension
                    {
                        List<FileInfo> tFiles = filesInfo.Where(f => f.Extension.Equals("." + ext, StringComparison.OrdinalIgnoreCase)).ToList();

                        foreach (FileInfo file in tFiles) // each File
                        {
                            file.Refresh();
                            if (curTime.Subtract(file.LastWriteTime).TotalDays > vd.RemainDays)
                            {
                                file.Delete();
                                delCnt += 1;
                            }
                        }
                    }
                }
                return delCnt;
            }
            catch (Exception ex)
            {
                LogHelper.TraceLog(MethodBase.GetCurrentMethod(), ex);
                return 0;
            }
        }

        public int DeleteOversized()
        {
            try
            {
                bool success = GetDirInfo();
                if (!success) return 0;

                int delCnt = 0;
                foreach (KeyValuePair<string, DriveInfo> dInfo in pathInfo.Disks) // each Disk
                {
                    int ratio = GetDiskFreeSpaceRatioInt(dInfo.Key, pathInfo.Disks);
                    if (ratio > 0 && ratio < Utilities.CON_FreeDiskSpaceThreshold)
                    {
                        foreach (ValidDirInfo vd in pathInfo.ValidDirs) // each Path
                        {
                            // check the path resides in this disk
                            if (vd.DirInfo.Root.Name.Equals(dInfo.Value.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                // get files from TopDirectoryOnly
                                List<FileInfo> tFiles = vd.DirInfo.EnumerateFiles("*", SearchOption.TopDirectoryOnly).ToList();
                                List<FileInfo> rFiles = new List<FileInfo>();

                                foreach (string ext in targetExts) // each Extension
                                {
                                    rFiles.AddRange(tFiles.Where(f => f.Extension.Equals("." + ext, StringComparison.OrdinalIgnoreCase)).ToList());
                                }

                                // Deletes extension matched files and the if-oversized last file. 
                                if (rFiles.Count > 0)
                                {
                                    FileInfo lastFile = rFiles.OrderByDescending(f => f.LastWriteTime).First();

                                    foreach (FileInfo file in rFiles) // each File
                                    {
                                        file.Refresh();
                                        if (file.Name.Equals(lastFile.Name, StringComparison.OrdinalIgnoreCase))
                                        {
                                            if (file.Length > bytesInMb * Utilities.CON_EveryFolderFileSizeRemain)
                                            {
                                                file.Delete();
                                                delCnt += 1;
                                            }
                                        }
                                        else
                                        {
                                            file.Delete();
                                            delCnt += 1;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return delCnt;
            }
            catch (Exception ex)
            {
                LogHelper.TraceLog(MethodBase.GetCurrentMethod(), ex);
                return 0;
            }
        }

        // Generate a two-digit-number as the decimal part of a percentage, e.g. 85 for 85%.
        public int GetDiskFreeSpaceRatioInt(string name, Dictionary<string, DriveInfo> disks)
        {
            int result = (int)GetDiskFreeSpaceRatio(name, disks);
            return result > 0 ? result : 0;
        }

        // Generate ratio as a double. e.g. 0.85... 
        public double GetDiskFreeSpaceRatio(string name, Dictionary<string, DriveInfo> disks)
        {
            DriveInfo disk = new DriveInfo(name);
            if (disks.TryGetValue(name, out disk))
            {
                double result = ((double)disk.TotalFreeSpace / disk.TotalSize) * 100;
                return result;
            }
            else
            {
                LogHelper.TraceLog("Invalid disk name: [" + name + ":\\]", LogType.Error);
                return 0;
            }
        }
    }
}
