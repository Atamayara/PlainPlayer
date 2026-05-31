using System.Text;
using PlainPlayer.Data.Interfaces;
using PlainPlayer.Data.Settings;
using YamlDotNet.RepresentationModel;

namespace PlainPlayer.IO.Settings;

public class SettingFileManager : ISettingsManager
{
    public AppSettings Load(string path)
    {
        var settings = new AppSettings();
        try
        {
            var input = new StreamReader(path, Encoding.UTF8);
            var yaml = new YamlStream();
            yaml.Load(input);

            var paths = yaml.Documents[0].RootNode["Paths"];
            settings.RekordboxLibraryPath = (string?)paths["Rekordbox"];

        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Notice: 設定ファイルが見つかりませんでした\n");
        }
        return settings;
    }
}