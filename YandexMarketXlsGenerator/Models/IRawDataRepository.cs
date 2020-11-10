namespace YandexMarketXlsGenerator.ViewModels
{
    public interface IRawDataRepository
    {
        string Load();
        void Save(string rawStringData);
    }
}