using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public interface IImageController
    {
        /// <summary>
        /// function gets arguments and matches command to sender, for execution.
        /// </summary>
        /// <param name="commandID"> int representing command to be executed. </param>
        /// <param name="args"> for executing command. </param>
        /// <param name="resultSuccessful"> true if execution successfel, else false. </param>
        /// <returns> command name. </returns>
        string ExecuteCommand(int commandID, string[] args, out bool result);          // Executing the Command Requet
    }
}
