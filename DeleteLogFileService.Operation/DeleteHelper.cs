using System;
using System.Timers;
using System.ComponentModel;
using System.Threading;
using System.Reflection;
using Newegg.BigData.Framework.Common;
using DeleteLogFileService.Utility;

namespace DeleteLogFileService.Operation
{
    public class DeleteHelper
    {
        private static System.Timers.Timer aTimer;
        private static DateTime lastDateCheckTime;
        private static DateTime lastSizeCheckTime;
        private static FileDeleter deleter;
        private static bool isFirstEvent;
        private static readonly object timeLock = new object();

        public void Start()
        {
            deleter = new FileDeleter();
            isFirstEvent = true;

            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.AutoReset = true;
            aTimer.Start();

            GC.KeepAlive(aTimer);
            GC.KeepAlive(timeLock);
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (aTimer.Enabled)
            {
                lock (timeLock)
                {
                    if (aTimer.Enabled)
                    {
                        aTimer.Stop();

                        if (DateTime.Now > lastDateCheckTime.AddSeconds(Utilities.CON_PerSecondsRunforRemainDays) || isFirstEvent)
                        {
                            int delNo = deleter.DeleteOutdated();
                            lastDateCheckTime = DateTime.Now;
                            LogHelper.TraceProgramDebugLog(string.Format("Check Remaindays: {0} files deleted at {1}", delNo, lastDateCheckTime));
                        }

                        if (DateTime.Now > lastSizeCheckTime.AddSeconds(Utilities.CON_PerSecondsRunforFreeDiskSpaceCheck) || isFirstEvent)
                        {
                            int delNum = deleter.DeleteOversized();
                            lastSizeCheckTime = DateTime.Now;
                            LogHelper.TraceProgramDebugLog(string.Format("Check DiskFreeSpace: {0} files deleted at {1}", delNum, lastSizeCheckTime));
                        }

                        if (isFirstEvent)
                        {
                            isFirstEvent = false;
                        }

                        aTimer.Start();
                    }
                }
            }
        }

        public void Stop()
        {
            lock (timeLock)
            {
                aTimer.Stop();
                aTimer.Dispose();
            }
        }
    }
}
