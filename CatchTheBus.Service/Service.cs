﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using CatchTheBus.Service.Tasks;
using log4net;
using log4net.Config;
using NCron.Fluent.Crontab;
using NCron.Service;

namespace CatchTheBus.Service
{
    public partial class Service : ServiceBase
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static Service()
        {
            // init logging
            var log4netConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            XmlConfigurator.ConfigureAndWatch(new FileInfo(log4netConfig));
        }

        public Service()
        {
            InitializeComponent();

            ServiceName = "CatchTheBus.Service";

            CanHandlePowerEvent = true;
            CanHandleSessionChangeEvent = true;
            CanPauseAndContinue = true;
            CanShutdown = true;
            CanStop = true;
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Log.Info("Service started");
                Debugger.Launch();
                AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
                var schedulingService = new SchedulingService();
                schedulingService.At("* * * * *").Run(() => new TrackScheduleTask());
                schedulingService.Start();
            }
            catch (Exception ex)
            {
                Log.Fatal("Error work thread", ex);
                Environment.Exit(-1);
            }
        }

        protected override void OnStop()
        {
            Log.Info("Service stopping");
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Log.Fatal(args.ExceptionObject as Exception);
        }
    }
}