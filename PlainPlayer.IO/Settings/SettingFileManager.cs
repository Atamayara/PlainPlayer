using System.Text;
using PlainPlayer.Data.Interfaces;
using PlainPlayer.Data.Settings;
using YamlDotNet.RepresentationModel;

namespace PlainPlayer.IO.Settings;

public class SettingFileManager : ISettingsManager
{
    public AppSettings Load(string filename)
    {
        var filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PlainPlayer",
            filename
        );
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Notice: 設定ファイルが見つかりませんでした");
            try
            {
                string? directoryPath = Path.GetDirectoryName(filePath);
                if (directoryPath != null && !Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                File.Create(filePath).Close();
                Console.WriteLine("Notice: 設定ファイルを生成しました");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: 設定ファイルの生成に失敗しました: {ex.Message}");
            }
        }
        var settings = new AppSettings();
        try
        {
            var input = new StreamReader(filePath, Encoding.UTF8);
            var yaml = new YamlStream();
            yaml.Load(input);

            var documents = yaml.Documents;
            if (documents.Count > 0)
                settings.RekordboxLibraryPath = (string?)documents[0].RootNode["Paths"]["Rekordbox"];
            
            Console.WriteLine("Notice: 設定ファイルを読み込みました");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Error: 設定ファイルを読み込みに失敗しました");
        }
        return settings;
    }
}