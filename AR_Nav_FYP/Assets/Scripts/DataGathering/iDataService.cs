public interface iDataService
{
    bool SaveData<T>(string RelativePath, T Data);
    T LoadData<T>(string RelativePath);
}