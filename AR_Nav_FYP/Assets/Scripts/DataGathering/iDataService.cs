public interface iDataService
{
    bool SaveData<T>(string RelativePath, T Data);
}