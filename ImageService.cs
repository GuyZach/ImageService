using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ImageService.Logging;//////
using ImageService.Server;
using ImageService.Modal;
using ImageService.Controller;
using System.Configuration;
namespace ImageService
{
    public partial class ImageService : ServiceBase
    {
        private int eventId = 1;
        private ILoggingService logger;             // for documentation
        private ImageServer m_imageServer;          // The Image Server
        private IImageServiceModal modal;           // Have the AddFile mathod
        private IImageController controller;        // have ExecuteCommand mathod and dictionary of <int, commands>
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        /// <summary>
        /// service constructor. creating event log and logging service.
        /// </summary>
        /// <param name="args"> for source and log name. </param>
        public ImageService(string[] args)
        {
            InitializeComponent();
            string eventSourceName = "MySource";
            string logName = "MyNewLog";
            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Count() > 1)
            {
                logName = args[1];
            }
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;
        }

        /// <summary>
        /// starts service and creates the main objects of the exercise
        /// </summary>
        /// <param name="args"> for starting service. </param>
        protected override void OnStart(string[] args)
        {
            // In order to create the ImageServiceModal we need to read two fields from the App.config
            string outputFolder = ConfigurationManager.AppSettings["OutputDir"];
            int thumbnailSize = Int32.Parse(ConfigurationManager.AppSettings["ThumbnailSize"]);

            logger = new LoggingService();
            modal = new ImageServiceModal(outputFolder, 120);
          
            controller = new ImageController(modal);
            m_imageServer = new ImageServer(logger, controller);
            m_imageServer.createHandlers();
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            eventLog1.WriteEntry("In OnStart");
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
            timer.Enabled = true;

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        /// <summary>
        /// The Event that will be activated upon timer finishing lapse.
        /// </summary>
        /// <param name="sender"> the class from which the event was invoked to call this function. </param>
        /// <param name="e"> arguments for an elapsed event. </param>
        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.  
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }

        /// <summary>
        /// stops service
        /// </summary>
        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop.");
        }
    }
}
