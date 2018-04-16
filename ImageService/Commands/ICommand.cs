using ImageService.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{

    public interface ICommand
    {
        /// <summary>
        /// executes command.
        /// </summary>
        /// <param name="args"> string arguments that include file path and directory. </param>
        /// <returns> The String Will Return result if result = true, and will return the error message otherwise. </returns>
        string Execute(string[] args, out bool result);
    }
}
