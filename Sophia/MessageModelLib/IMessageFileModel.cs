using System;
using System.Collections.Generic;
using System.Text;

namespace MessageModelLib
{
    public interface IMessageFileModel
    {
        string IdFile { get; }
        int IdPart { get; }
        int Quantity { get; }
        byte[] Data { get; }
        string FileName { get; }
        int SourceId { get; }
        int LengthPart { get; }
        int LengthLastPart { get; }
    }
}
