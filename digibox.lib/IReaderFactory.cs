using System;
using System.Collections.Generic;
using System.Text;

namespace digibox.lib
{
    public interface IReaderFactory
    {
        string[] data { get; set; }
        void ReadData();
    }
}
