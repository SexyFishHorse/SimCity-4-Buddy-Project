namespace NIHEI.SC4Buddy.Utils
{
    using System.IO;

    public interface IJsonFileWriter
    {
        void WriteToFile(FileInfo fileInfo, object objectToWrite, bool indented = true);
    }
}