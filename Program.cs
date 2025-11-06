/*
 *  SimpleLogManager
 *  
 *  Configuration File for
 *      - Log File Path
 *      - Directory Logs should back-up to
 *      - Back-Up Conditions
 *          - File Size
 *          - Last Modification Date
 *          - Creation Date
 *      - Back-Up Maintenance
 *          - Max # of logs
 *          - Max file size
 *          - Creation Date
 *      
 *  Every time this runs, SLM will:
 *      - Append data on Start (Default: Date & Time)
 *      - Check if Back-Up Conditions are True
 *          - If So
 *              - Copy file to specified location
 *              - Clear the original file's data
 *              - Append data (Default: Date & Time)
 *              - Check if Back-Up Maintenance needs done
 *                  - If So, Do It.
 */

using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Xml.Serialization;

namespace SimpleLogManager
{
    public class Program
    {
        public static void Run()
        {
            var slm = new SimpleLogManager();

            slm.Execute();
        }
        
        public static void TempTesting()
        {
            
        }

        public static void Main(string[] args)
        {
            //string dir = "D:\\Scrap\\BackUps";

            //List<string> sortedFiles = 
            //    Helpers.GetSortedLogFiles(dir, "log");

            //foreach (var sortedFile in sortedFiles)
            //{
            //    Console.WriteLine(sortedFile);
            //}

            //Helpers.FillFile(
            //    "D:\\Scrap\\slm_testing\\test.log",
            //    4, ByteSize.KiloByte);

            // Run();
            Run();
        }
    }
}