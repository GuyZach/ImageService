using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        private IImageController m_controller;             
        private ILoggingService m_logging;
        private string m_path;
        private FileSystemWatcher m_dirWatcher;           
        static readonly string[] extentions = { ".jpg", ".png", ".gif", ".bmp" };      
        private DateTime lastRead;

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;
        /// <summary>
        /// constructor. initializes directory handler.
        /// </summary>
        /// <param name="path"> path of specific directory that wil be handled by handler. </param>
        /// <param name="controller"> program's controller for executing commands. </param>
        /// <param name="logging"> program's logger for messaging. </param>
        public DirectoyHandler(string path, IImageController controller, ILoggingService logging)
        {
            m_logging = logging;
            m_controller = controller;
            m_path = path;
            m_dirWatcher = new FileSystemWatcher(path);
        }
        /// <summary>
        /// function that starts listening and handling the directory recieved.
        /// </summary>
        /// <param name="dirpath"> directory path to listen to for changes. </param>
        public void StartHandleDirectory(string dirPath)
        {
            m_logging.Log("Start to handle directory: " + m_path, MessageTypeEnum.INFO);

            // making sure the filesystem watcher litens to the specific directory if changes happens
            m_dirWatcher.Created += newFileCreation;
            m_dirWatcher.EnableRaisingEvents = true;
        }
        /// <summary>
        /// function will be called when a change occurs in directory, notifing logger and command event.
        /// </summary>
        /// <param name="sender"> the class from which the event was invoked to call this function. </param>
        /// <param name="e"> arguments for a file system event. </param>
        private void newFileCreation(object sender, FileSystemEventArgs e)
        {
            m_logging.Log("directory changed: " + e.Name, MessageTypeEnum.INFO);
            m_logging.Log("change type: " + e.ChangeType.GetType(), MessageTypeEnum.INFO);
            // making sure that the new file creation will happen only once and not twice.
            DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
            if (lastWriteTime != lastRead)
            {
                string[] args = { e.FullPath };
                if (checkFileExtention(e.FullPath))
                    OnCommandRecieved(this, new CommandRecievedEventArgs(1, args, m_path));
                lastRead = lastWriteTime;
            }
        }
        /// <summary>
        /// check if the extention is a proper extention to handle the specifc path.
        /// </summary>
        /// <param name="filePath"> is the path to check</param>
        /// <returns>true - if proper extetion. false - otherwise</returns>
        private bool checkFileExtention(string filePath)
        {
            string fileExtention = Path.GetExtension(filePath);
            bool isMatchExtention = false;
            foreach (string extention in extentions)
            {
                if (fileExtention.Equals(extention, StringComparison.CurrentCultureIgnoreCase))
                    isMatchExtention = true;
            }
            return isMatchExtention;
        }
        /// <summary>
        /// The Event that will be activated upon new Command
        /// </summary>
        /// <param name="sender"> the class from which the event was invoked to call this function. </param>
        /// <param name="e"> arguments for a command event. </param>
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            bool result;
            string messageFromExecution = m_controller.ExecuteCommand(e.CommandID, e.Args, out result);
            // 0 is close handler
            if (e.CommandID == 0)
            {
                m_logging.Log("Handler closed", MessageTypeEnum.INFO);
            }

            // Write the execution in the log.
            if (result)
                m_logging.Log(messageFromExecution, MessageTypeEnum.INFO);
            else
                m_logging.Log(messageFromExecution, MessageTypeEnum.FAIL);
        }

        public void OnCloseService(object send, DirectoryCloseEventArgs e)
        {
            m_logging.Log("closing handler for directory: " + m_path, MessageTypeEnum.INFO);
            m_dirWatcher.EnableRaisingEvents = false;
            m_dirWatcher.Dispose();
            DirectoryClose?.Invoke(this, e);
        }
    }
}
