using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DataProvider.LogicLayer.Services
{
    public class DataConverterService
    {
        public string StreamToStringWin1251(Stream inputStream)
        {
            StreamReader streamReader = new StreamReader(inputStream, Encoding.GetEncoding("windows-1251"));
            return streamReader.ReadToEnd();
        }
    }
}
