using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        private string outputFolder; //dst
        private int thumbnailSize;

        /// <summary>
        /// ImageServiceModal constructor
        /// </summary>
        /// <param name="outputFolderArg"> The backup file </param>
        /// <param name="thumbnailSizeArg"> The size of the thumbnail </param>
        public ImageServiceModal(string outputFolderArg, int thumbnailSizeArg)
        {
            outputFolder = outputFolderArg;
            thumbnailSize = thumbnailSizeArg;
        }

        /// <summary>
        /// The Function Addes A file to the system
        /// </summary>
        /// <param name="path">The Path of the Image from the file</param>
        /// <returns>Indication if the Addition Was Successful</returns>
        public string AddFile(string path, out bool result)
        {
            string success = "Success";
            string failure = "Failure";
            string nameOfImage = Path.GetFileName(path);
            // check if outputDir exists, if not – create it
            try
            {
                if (!Directory.Exists(this.outputFolder))
                {
                    DirectoryInfo di = Directory.CreateDirectory(this.outputFolder);
                    di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                result = false;
                return failure + ex.ToString();
            }

            // get file creation time, year and month
            DateTime date = GetFileDate(path);
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            // In order to sync between the two accesses to 'path' variable in functions 'GetFileDate' and 'CreateDirsAndMoveImage'
            System.Threading.Thread.Sleep(1000);

            // Create dirs "year" and "month" (unless they already exist), and move the image to its new dir
            string newPathToImage = CreateDirsAndMoveImage(path, "", year, month, nameOfImage);

            // create thumbnail image from path
            Image image = Image.FromFile(newPathToImage);
            Image thumb = image.GetThumbnailImage(this.thumbnailSize, this.thumbnailSize, () => false, IntPtr.Zero);
            image.Dispose();

            // check if Thumbnails dir exists, if not – create it
            try
            {
                if (!Directory.Exists(this.outputFolder + "\\" + "Thumbnails"))
                {
                    Directory.CreateDirectory(this.outputFolder + "\\" + "Thumbnails");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
                result = false;
                return failure + ex.ToString();
            }
            // Change the extention for the Thumbnail image
            string nameOfThumbnailImage = Path.GetFileName(newPathToImage);

            // Create dirs "year" and "month" within Thumbnails dir (unless they already exist)
            string newPathToThumbnailImage = CreateDirsAndMoveImage(path, "\\Thumbnails", year, month, nameOfThumbnailImage);
            // Save the Thumbnail Image into its new path
            thumb.Save(newPathToThumbnailImage);
            //image.Dispose();
            thumb.Dispose();
            result = true;
            return success + ".The new path is: " + newPathToImage;
        }


        public static DateTime GetFileDate(string filename)
        {
            DateTime now = DateTime.Now;
            TimeSpan localOffset = now - now.ToUniversalTime();
            return File.GetLastWriteTimeUtc(filename) + localOffset;
        }
        public string CreateDirsAndMoveImage(string path, string Thumbnails, string year, string month, string nameOfImage)
        {
            if (Directory.Exists(this.outputFolder + Thumbnails + "\\" + year))
            {
                if (Directory.Exists(this.outputFolder + Thumbnails + "\\" + year + "\\" + month))
                {
                    // Check if there is already file with this name in the dir
                    int i = 1;
                    string uniqueNameOfImage = nameOfImage;
                    while (File.Exists(this.outputFolder + Thumbnails + "\\" + year + "\\" + month + "\\" + uniqueNameOfImage))
                    {
                        uniqueNameOfImage = i.ToString() + nameOfImage;
                        i++;
                    }
                    if (Thumbnails == "")
                        File.Move(path, this.outputFolder + Thumbnails + "\\" + year + "\\" + month + "\\" + uniqueNameOfImage);
                    return this.outputFolder + Thumbnails + "\\" + year + "\\" + month + "\\" + uniqueNameOfImage;
                }
                else
                {
                    Directory.CreateDirectory(this.outputFolder + Thumbnails + "\\" + year + "\\" + month);
                    if (Thumbnails == "")
                        File.Move(path, this.outputFolder + Thumbnails + "\\" + year + "\\" + month + "\\" + nameOfImage);
                }
            }
            else
            {
                Directory.CreateDirectory(this.outputFolder + Thumbnails + "\\" + year + "\\" + month);
                if (Thumbnails == "")
                    File.Move(path, this.outputFolder + Thumbnails + "\\" + year + "\\" + month + "\\" + nameOfImage);
            }
            return this.outputFolder + Thumbnails + "\\" + year + "\\" + month + "\\" + nameOfImage;
        }

    }
}
