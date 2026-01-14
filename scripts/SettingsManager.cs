using Godot;
using System;
using System.IO;
using System.Text.Json;

public partial class SettingsManager : Node
{
	readonly static string AppDataPath = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)}";
	readonly static string TestSettings = Path.Combine(AppDataPath, "ExampleGame", "TestSettings");
	readonly static string UISettingsFile = "UISettings.json";
	public static WindowSavedSettings GetUISettings(string _windowEnumName)
	{			
		
		WindowSavedSettings windowSettings;
		Directory.CreateDirectory(TestSettings);

		if (!File.Exists(Path.Combine(TestSettings, UISettingsFile)))
		{
			return new WindowSavedSettings();
		}
		
		string jsonString = File.ReadAllText(Path.Combine(TestSettings, UISettingsFile));
		try
		{
			UISettings UISettings = JsonSerializer.Deserialize<UISettings>(jsonString);
			windowSettings = (WindowSavedSettings)UISettings.GetType().GetProperty(_windowEnumName).GetValue(UISettings);
		}
		catch(JsonException _ex)
		{
			// GD.Print(_ex);
			
			// Currently returning an object where all values are null in the event it cannot parse the file
			// In this situation the UIWindowController will reset/repopulate the default values as the windows are opened for the first time
			windowSettings = new WindowSavedSettings();
		}
		
		return windowSettings;        
	}

	public static void UpdateUISettings(WindowSavedSettings _settings, string _windowEnumName)
	{
		UISettings uiSettings = GetAllUISettings();
		uiSettings.GetType().GetProperty(_windowEnumName).SetValue(uiSettings, _settings, null);
		string jsonString = "";
		try
		{
			jsonString = JsonSerializer.Serialize(uiSettings, new JsonSerializerOptions{WriteIndented = true});
			File.WriteAllText(Path.Combine(TestSettings, UISettingsFile), jsonString);
		}
		catch(JsonException _ex)
		{
			// GD.Print(_ex);
		}
	}

	static UISettings GetAllUISettings()
	{	
		if (!File.Exists(Path.Combine(TestSettings, UISettingsFile)))
		{
			return new UISettings();
		}	
		string jsonString = File.ReadAllText(Path.Combine(TestSettings, UISettingsFile));
		return JsonSerializer.Deserialize<UISettings>(jsonString);
	}
}
