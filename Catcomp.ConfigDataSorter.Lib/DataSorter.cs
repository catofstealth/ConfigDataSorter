using Catomp.ConfigDataSorter.Lib.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Catomp.ConfigDataSorter.Lib
{
    public class DataSorter
    {
        public List<SortPreferences> sortPreferences { get; }

        public DataSorter(List<SortPreferences> sortPrefs)
        {
            sortPreferences = sortPrefs;
        }
        public JObject ProcessDataFile(string jsonContent)
        {
            JObject jsonObj = JsonConvert.DeserializeObject<JObject>(jsonContent);
            JObject outputJsonObj = JsonConvert.DeserializeObject<JObject>(jsonContent);
            //loop over records and get our sorting field
            // need a while loop....
            int index = 0;
            foreach (JArray recordSet in jsonObj["RecordSets"].Value<JArray>())
            {
                if (recordSet.Count > 0)
                {
                    string recordType = (string)recordSet[0]["LogicalName"];
                    var sortOpts = GetSortingOptionsForRecordType(recordType);
                    if (sortOpts != null)
                    {
                        Console.WriteLine(string.Format("Record set {0} at position {1} : Sorted By {2} in {3} order", recordType, index++, sortOpts.SortingField, (sortOpts.Reverse) ? "parent first" : "child first"));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Record set {0} at position {1} : Not Sorted", recordType, index++));
                    }
                }
            }

            index = 0;
            foreach (JArray recordSet in jsonObj["RecordSets"].Value<JArray>())
            {
                if (recordSet.Count > 0)
                {
                    string recordType = (string)recordSet[0]["LogicalName"];
                    var sortOpts = GetSortingOptionsForRecordType(recordType);
                    if (sortOpts != null)
                    {
                        List<JObject> sortedRecordSet = SortRecordSet(sortOpts, recordSet);

                        //save our sorted rec9rd set back to our output object
                        outputJsonObj["RecordSets"][index] = JArray.FromObject(sortedRecordSet); //not sure if this will work!
                    }
                }

                index++;
            }

            return outputJsonObj;
        }


        public SortPreferences GetSortingOptionsForRecordType(string recordType)
        {
            return sortPreferences.Where(p => p.RecordType == recordType).FirstOrDefault();
        }

        public List<JObject> SortRecordSet(SortPreferences sortPrefs, JArray recordSet)
        {
            Dictionary<string, int> recordDepth = new Dictionary<string, int>();
            Dictionary<string, int> unresolved = new Dictionary<string, int>();
            Dictionary<string, string> processing = new Dictionary<string, string>();
            List<JObject> unsortedItems = new List<JObject>();

            //record set is the array per query. To make this unstoppable, we need to capture each block and put it back in the right place
            foreach (JObject rec in recordSet)
            {

                unsortedItems.Add(rec); // add to a list so we can sort it

                //Console.WriteLine((string)rec["Id"]);

                string nextRelative = null; // (string)rec["Attributes"]["AttributesName"][options.SortingField];

                foreach (JToken attribute in rec["Attributes"])
                {
                    if ((string)attribute["AttributeName"] == sortPrefs.SortingField)
                    {
                        nextRelative = (string)attribute["AttributeValue"]["Id"];
                    }
                }

                //first pass extract all non root items
                if (nextRelative != null)
                {
                    processing.Add((string)rec["Id"], nextRelative);
                }
                else
                {
                    // our field does not exist, were the first/last page
                    recordDepth.Add(rec.Value<string>("Id"), 1);
                }
            }


            do
            {

                Dictionary<string, string> notfound = new Dictionary<string, string>();

                foreach (KeyValuePair<string, string> kvp in processing)
                {

                    int parentDepth = 0;

                    bool attributeExists = recordDepth.TryGetValue(kvp.Value, out parentDepth);

                    if (attributeExists)
                    {
                        // our field exists and we have a depth, lets increase it by 1
                        recordDepth.Add(kvp.Key, parentDepth + 1);
                    }
                    else
                    {
                        notfound.Add(kvp.Key, kvp.Value);
                    }
                }

                //repopulate processing with not found - break linkage?
                processing = notfound;


            } while (processing.Count > 0);

            //Console.WriteLine(JsonConvert.SerializeObject(recordDepth.OrderBy(d => d.Value), Newtonsoft.Json.Formatting.Indented));

            List<string> sortingOrder = recordDepth.OrderBy(r => r.Value).Select(r => r.Key).ToList();

            //Console.WriteLine(JsonConvert.SerializeObject(sortingOrder, Newtonsoft.Json.Formatting.Indented));

            List<JObject> sorted = new List<JObject>();

            if (sortPrefs.Reverse)
            {
                Console.WriteLine(JsonConvert.SerializeObject(recordDepth.OrderByDescending(r => r.Value), Newtonsoft.Json.Formatting.Indented));
                sorted = unsortedItems.OrderByDescending(
                        item => sortingOrder.IndexOf((string)item["Id"])
                        ).ToList();
            }
            else
            {
                Console.WriteLine(JsonConvert.SerializeObject(recordDepth.OrderBy(r => r.Value), Newtonsoft.Json.Formatting.Indented));
                sorted = unsortedItems.OrderBy(
                    item => sortingOrder.IndexOf((string)item["Id"])
                    ).ToList();
            }
            //Console.WriteLine(JsonConvert.SerializeObject(sorted, Newtonsoft.Json.Formatting.Indented));

            //Console.WriteLine(JsonConvert.SerializeObject(processing, Newtonsoft.Json.Formatting.Indented));

            return sorted;
        }
    }
}
