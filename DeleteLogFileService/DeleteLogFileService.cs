using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using Newegg.BigData.Framework.Common;
using DeleteLogFileService.Operation;

namespace DeleteLogFileService
{
    public partial class DeleteLogFileService : ServiceBase
    {
        private static DeleteHelper delOperaror;

        public DeleteLogFileService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                delOperaror = new DeleteHelper();
                delOperaror.Start();
            }
            catch (Exception ex)
            {
                LogHelper.TraceLog(MethodBase.GetCurrentMethod(), ex);
                throw;
            }
        }

        protected override void OnStop()
        {
            try
            {
                delOperaror.Stop();
            }
            catch (Exception ex)
            {
                LogHelper.TraceLog(MethodBase.GetCurrentMethod(), ex);
                throw;
            }
        }
    }
}
