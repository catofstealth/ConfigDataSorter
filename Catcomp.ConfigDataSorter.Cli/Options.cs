using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text; 

namespace CWC.D365.JsonObjectOrganiser
{
    class Options
    {
        [Option('s', "sortingPreferences", Required = true, HelpText = "Path to the json file containing the sorting parameters")]
        public string SortingData { get; set; }

        //[Option('i', "idfield", Required = true, HelpText = "Name of the Id element in each object")]
        //public string IdField { get; set; }
        


        [Option('i', "input", Required = true, HelpText = "Data source, a json file created from the Data Mover tool")]
        public string Input { get; set; }

        [Option('o', "output", Required = false, HelpText = "Output file")]
        public string Output { get; set; }

        [Option('v', "verbose", HelpText = "Print details during execution")]
        public bool Verbose { get; set; }

//        []
//        public string GetUsage()
//        {
//            var help = new HelpText
//            {
//                Heading = new HeadingInfo("Catcomp.ConfigDataMover.Cli", Assembly.GetExecutingAssembly().GetName().Version.ToString()),
//                AdditionalNewLineAfterOption = true,
//                AddDashesToOption = true
//            };
//            help.Copyright = string.Format(@"
//Copyright 2021-{0} Rob Loach

//This program comes with ABSOLUTELY NO WARRANTY. This is free software, and you are welcome to redistribute it and/or modify it under the terms of the Apache License, Version 2.0. You may obtain a copy at http://www.apache.org/licenses/LICENSE-2.0.
//", DateTime.Now.Year);

//            help.AddPreOptionsLine("Usage: Catcomp.ConfigDataSorter.Cli.exe -i \"C:/Users/Stealthcat/Downloads/PortalData.json\" -s \"C:/Users/Stealthcat/Downloads/SortingPrefs.json\"");
//            help.AddOptions(this);
//            return help;
//        }
    }
}
