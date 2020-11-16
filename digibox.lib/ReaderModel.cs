using System;
using System.Collections.Generic;
using System.Text;

namespace digibox.lib
{
    public class ReaderModel
    {
        public string type { get; set; }
        public string version { get; set; }
        public string Address { get; set; }
        public string Power { get; set; }
        public string MinFrequency { get; set; }
        public string MaxFrequency { get; set; }
        public string Protocol { get; set; }
        public string MaxInventoryScanTime { get; set; }
    }
}
