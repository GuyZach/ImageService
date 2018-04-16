using ImageService.Infrastructure;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    public class NewFileCommand : ICommand
    {
        private IImageServiceModal m_modal;
        /// <summary>
        /// Constructor. saves modal as member.
        /// </summary>
        /// <param name="modal"> The modal that will make the command operations. </param>
        public NewFileCommand(IImageServiceModal modal)
        {
            m_modal = modal;           
        }

        public string Execute(string[] args, out bool result)
        {
            /// <summary>
            /// executes command to move new file in directory to output directory.
            /// </summary>
            /// <param name="args"> string arguments that include file path to be moved. </param>
            /// <returns> The String Will Return the New Path if result = true, and will return the error message. </returns>
            return m_modal.AddFile(args[0], out result);
        }
    }
}
