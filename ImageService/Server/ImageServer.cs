using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Server
{
    public class ImageServer
    {
        private ILoggingService m_logging;
        private IImageController m_controller;
        private List<IDirectoryHandler> listOfHandlers;

        // The event that notifies about a new Command being recieved
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;
        // The event that notifies the handlers about closing s
        public event EventHandler<DirectoryCloseEventArgs> CloseService;

        /// <summary>
        /// Image server constructor, create handler for each path in the app config file.
        /// </summary>
        /// <param name="logging"> The application's logger </param>
        /// <param name="modal"> The service modal </param>
        /// <param name="controller"> The image controller </param>
        public ImageServer(ILoggingService logging, IImageController controller)
        {
            m_logging = logging;
            m_controller = controller;
            this.listOfHandlers = new List<IDirectoryHandler>();
        }
        /// <summary>
        /// Create new handlers for the given paths.
        /// </summary>
        public void createHandlers()
        {
            //Read from App.config
            string paths = ConfigurationManager.AppSettings["Handler"];
            string[] listOfPaths = paths.Split(';');

            foreach (string path in listOfPaths)
            {
                IDirectoryHandler handler = new DirectoyHandler(path, this.m_controller, this.m_logging);
                CommandRecieved += handler.OnCommandRecieved;
                handler.DirectoryClose += OnCloseServer;
                CloseService += handler.OnCloseService;
                handler.StartHandleDirectory(path);
                this.listOfHandlers.Add(handler);

                m_logging.Log("Created Handler for path: " + path + ".", Logging.Modal.MessageTypeEnum.INFO);
            }
        }
        /// <summary>
        /// sending the given command from the server to all the application's handlers.
        /// </summary>
        /// <param name="e"> The command that will be sent. </param>
        public void SendCommand()
        {
            string[] args = new string[1];
            args[0] = "";
            CommandRecieved?.Invoke(this, new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, args, ""));
        }
        /// <summary>
        /// Closing the handlers when sever is closing.
        /// </summary>
        public void OnCloseServer(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler handler = (IDirectoryHandler)sender;
            // handler.OnCommandRecieved removes itself from CommandRecieved EventHandler
            CommandRecieved -= handler.OnCommandRecieved;
            // OnCloseServer removes itself from DirectoryClose EventHandler
            handler.DirectoryClose -= OnCloseServer;
            // Remove this handler from the listOfHandlers
            this.listOfHandlers.Remove(handler);
        }
    }
}
