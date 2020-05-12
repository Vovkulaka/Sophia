using System;

namespace MessageModelLib
{
    public class MessageFileModel : IMessageFileModel
    {
        public MessageFileModel(
            string idFile,
            int idPart,
            int quantity,
            byte[] data,
            string fileName,
            int sourceId,
            int lengthFile,
            int lengthPart,
            int lengthLastPart)
        {
            IdFile = idFile;
            IdPart = idPart;
            Quantity = quantity;
            Data = data;
            FileName = fileName;
            SourceId = sourceId;
            LengthFile = lengthFile;
            LengthPart = lengthPart;
            LengthLastPart = lengthLastPart;
        }

        public string IdFile { get; }
        public int IdPart { get; }
        public int Quantity { get; }
        public byte[] Data { get; }
        public string FileName { get; }
        public int SourceId { get; }
        public int LengthFile { get; }
        public int LengthPart { get; }
        public int LengthLastPart { get; }
    }
}
