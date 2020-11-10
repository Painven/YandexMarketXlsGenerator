using System.IO;
using YandexMarketXlsGenerator.ViewModels;

namespace YandexMarketXlsGenerator
{
    internal class TextStorage : IRawDataRepository
    {
        private readonly string fileName;

        public TextStorage(string fileName)
        {
            this.fileName = fileName;
        }
        
        public string Load()
        {
            if (File.Exists(fileName))
            {
                return File.ReadAllText(fileName);
            }
            return string.Empty;
        }

        public void Save(string rawStringData)
        {
            File.WriteAllText(fileName, rawStringData);
        }
    }
}