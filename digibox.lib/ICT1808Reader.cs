using System;
using System.Collections.Generic;
using System.Text;

namespace digibox.lib
{
    public interface ICT1808Reader:IReaderFactory
    {
        string readerAddress { get; set; }
        byte comAddress { get; set; }
        byte baudRate { get; set; }

         string Port { get; set; }
         string IPAddress { get; set; }

        void OpenPort();
    }
}
