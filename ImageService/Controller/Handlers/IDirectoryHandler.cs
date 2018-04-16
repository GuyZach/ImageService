using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller.Handlers
{
    public interface IDirectoryHandler
    {
        // The Event That Notifies that the Directory is being closed
        event EventHandler<DirectoryCloseEventArgs> DirectoryClose;
        
        void StartHandleDirectory(string dirPath);
        
        void OnCommandRecieved(object sender, CommandRecievedEventArgs e);
        // The Event that will be activated when service close
        void OnCloseService(object sender, DirectoryCloseEventArgs e);     
    }
}
