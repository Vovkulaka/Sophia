using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace DataProvider.LogicLayer.Services
{
    public class ZipService
    {
        //public Ionic.Zip.ZipFile GetZipFile(Stream stream)
        //{
        //    return Ionic.Zip.ZipFile.Read(stream);
        //}

        public Stream ExtractStreamFromZipStream(Stream zipFileStream)
        {
            var zipArchive = new ZipArchive(zipFileStream);

            var zipArchiveEntries = GetArchivedFiles(zipArchive);

            ZipArchiveEntry archiveEntry = null;

            archiveEntry = zipArchiveEntries.Count > 10 ? 
                zipArchiveEntries.FirstOrDefault(z => z.Name == "sharedStrings.xml") : 
                zipArchiveEntries.FirstOrDefault();

            if (archiveEntry == null)
            {
                throw new Exception("В zip архиве отсутствуют файлы.");
            }

            return archiveEntry.Open();
        }

        /// <summary>
        ///  Получить перечень файлов в архиве
        /// </summary>
        /// <param name="zipFile"></param>
        /// <returns></returns>
        private List<ZipArchiveEntry> GetArchivedFiles(ZipArchive zipFile)
        {
            return zipFile.Entries.ToList();
        }
    }
}
