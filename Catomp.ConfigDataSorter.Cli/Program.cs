using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using CommandLine;
using CommandLine.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Catomp.ConfigDataSorter.Lib;
using Catomp.ConfigDataSorter.Lib.Models;

namespace CWC.D365.JsonObjectOrganiser
{
    

    class Program
    {

        static List<SortPreferences> sortPreferences = new List<SortPreferences>();

        static void Main(string[] args)
        {
            var options = new Options();
            ParserResult<Options> parsed = CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed(
                    options =>
                    {
                        // consume Options instance properties
                        if (options.Verbose)
                        {
                            Console.WriteLine("Input File : {0}", options.Input);
                            Console.WriteLine("Output File : {0}", options.Output);
                        }
                        //parse the config file
                        string inputPath = Path.GetFullPath(options.Input);
                        string sortPrefsPath = Path.GetFullPath(options.SortingData);
                        string outputPath = "";

                        if (options.Output == null)
                        {
                            outputPath = Path.GetDirectoryName(options.Input) + "/" + Path.GetFileNameWithoutExtension(options.Input) + "_Sorted.json";
                        }
                        else
                        {
                            outputPath = Path.GetFullPath(options.Output);
                        }

                        if (options.Verbose)
                        {
                            Console.WriteLine("Input File : {0}", inputPath);
                            Console.WriteLine("Output File : {0}", outputPath);
                        }

                        //load json fike and parse it
                        string jsonContent = System.IO.File.ReadAllText(inputPath);
                        string sortPrefsString = System.IO.File.ReadAllText(sortPrefsPath);

                        sortPreferences = JsonConvert.DeserializeObject<List<SortPreferences>>(sortPrefsString);

                        DataSorter dataSorter = new DataSorter(sortPreferences);
                        JObject outputJsonObj = dataSorter.ProcessDataFile(jsonContent);

                        //write output object to file

                        //load json fike and parse it
                        System.IO.File.WriteAllText(outputPath, JsonConvert.SerializeObject(outputJsonObj, Newtonsoft.Json.Formatting.Indented));

                    }
                )
                .WithNotParsed(
                    errors =>
                    {

                    }
                );
        }
    }
}
