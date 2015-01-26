namespace NIHEI.SC4Buddy.Utils
{
    using System.IO;
    using Newtonsoft.Json;

    public class JsonFileWriter : IJsonFileWriter
    {
        public void WriteToFile(FileInfo fileInfo, object objectToWrite, bool indented = true)
        {
            if (fileInfo.DirectoryName == null)
            {
                throw new DirectoryNotFoundException(string.Format("The path {0} does not contain a directory.", fileInfo.FullName));
            }

            Directory.CreateDirectory(fileInfo.DirectoryName);

            using (var fileStream = fileInfo.Create())
            using (var streamWriter = new StreamWriter(fileStream))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = indented ? Formatting.Indented : Formatting.None;

                var serializer = new JsonSerializer();

                serializer.Serialize(jsonWriter, objectToWrite);
            }
        }
    }
}
