using PlainPlayer.Data.Settings;

namespace PlainPlayer.Data.Interfaces;

public interface ISettingsManager
{
    public AppSettings Load(string filename);
}