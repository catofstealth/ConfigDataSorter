using System;
using System.Collections.Generic;
using System.Text;

namespace Catomp.ConfigDataSorter.Lib.Models
{
    public class SortPreferences
    {

        public string RecordType { get; set; }
        public string SortingField { get; set; }
        public bool Reverse { get; set; } = false;

    }
}
