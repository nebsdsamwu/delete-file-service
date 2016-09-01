using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using Newegg.BigData.Framework.Common;
using DeleteLogFileService.Configuration;
using DeleteLogFileService.Operation;
using DeleteLogFileService.Utility;

namespace DeleteLogFileService.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //ValidPathInfoTest();
                //DeleteOutdatedTest();
                //DeleteOversizedTest();
                DeleteHelperTest();

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                LogHelper.TraceProgramDebugLog(ex.ToString());
            }
        }

        public static void DeleteHelperTest()
        {
            DeleteHelper delOperator = new DeleteHelper();
            delOperator.Start();
        }

        public static void DeleteOversizedTest()
        {
            FileDeleter del = new FileDeleter();
            del.DeleteOversized();
        }

        public static void DeleteOutdatedTest()
        {
            FileDeleter del = new FileDeleter();
            del.DeleteOutdated();
        }

        public static void ValidPathInfoTest()
        {
            ValidPathInfo pathInfo = new ValidPathInfo();
            Dictionary<string, DriveInfo> disks = pathInfo.Disks;
            List<ValidDirInfo> dirs = pathInfo.ValidDirs;

            foreach (string d in disks.Keys) Console.WriteLine(d);
            Console.WriteLine();
            foreach (ValidDirInfo vd in dirs) Console.WriteLine(vd.DirInfo);
        }
    }
}
