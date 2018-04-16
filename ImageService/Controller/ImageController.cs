using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;                      // The Modal Object
        private Dictionary<int, ICommand> commands;
        /// <summary>
        /// constructor for program controller. saves modal and creates command dictionairy.
        /// </summary>
        /// <param name="modal"> program modal for command execution. </param>
        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            commands = new Dictionary<int, ICommand>()
            {
                { 1, new NewFileCommand(m_modal)}
            };
        }
        /// <summary>
        /// function gets arguments and matches command to sender, for execution.
        /// </summary>
        /// <param name="commandID"> int representing command to be executed. </param>
        /// <param name="args"> for executing command. </param>
        /// <param name="resultSuccessful"> true if execution successfel, else false. </param>
        /// <returns> command name. </returns>
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            if (!commands.ContainsKey(commandID))
            {
                resultSuccesful = false;
                return "Command not found!";
            }
            return commands[commandID].Execute(args, out resultSuccesful);
        }
    }
}
