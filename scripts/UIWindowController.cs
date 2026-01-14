using Godot;
using System;
using System.Collections.Generic;

// Add new window name(s) here
public enum WindowType
{
	GenericInfoWindow = 1,
	TestWindow = 2,
}
// Settings that should not be editable by player
public partial class WindowFixedSettings
{
	public WindowType Type;
	public bool Unresizable;
	public bool Borderless;
	public bool Transparent;
	public bool AlwaysOnTop;
	public bool DisableInputWhenActive;
	public float MinSizeX; // Normalized
	public float MinSizeY; // Normalized
	public float MaxSizeX; // Normalized
	public float MaxSizeY; // Normalized
	public float DefaultSizeX; // Normalized
	public float DefaultSizeY; // Normalized
	public float DefaultPositionX; // Normalized
	public float DefaultPositionY; // Normalized
	public string ContentsPrefabPath; // path to PackedScene in project
}

// Add new windows Here, in Enum, and in _Ready of UIWindowController
public partial class UISettings
{
	public WindowSavedSettings GenericInfoWindow {get; set;}
	public WindowSavedSettings TestWindow {get; set;}
}

public partial class UIWindowController : Node
{   
	static UIWindow ActiveWindow;
	static List<UIWindow> ActiveWindows = [];
	static UIWindow InfoSearchWindow;
	static UIWindow InfoWindow;
	static UIWindow TestWindow;
	
	bool InputAllowed = true;

	public override void _Ready()
	{
		Vector2I screenSize = (Vector2I)GetViewport().GetVisibleRect().Size;

		WindowFixedSettings GenericInfoWindowSettings = new()
		{
			Type = WindowType.GenericInfoWindow,
			Unresizable = false,
			Borderless = false,
			Transparent = true,
			AlwaysOnTop = false,
			DisableInputWhenActive = false,
			MinSizeX = 0.0f,
			MinSizeY = 0.0f,
			MaxSizeX = 0.0f,
			MaxSizeY = 0.0f,
			DefaultSizeX = 0.3f,
			DefaultSizeY = 0.6f,
			DefaultPositionX = 0.25f,
			DefaultPositionY = 0.25f,
			ContentsPrefabPath = "res://scenes/ui/generic_info_panel.tscn"
		};
		InfoWindow = new UIWindow(GenericInfoWindowSettings, this, screenSize, RemoveWindow, SetActiveWindow, ControlInput);

		WindowFixedSettings TestWindowSettings = new()
		{
			Type = WindowType.TestWindow,
			Unresizable = false,
			Borderless = false,
			Transparent = true,
			AlwaysOnTop = false,
			DisableInputWhenActive = false,
			MinSizeX = 0.0f,
			MinSizeY = 0.0f,
			MaxSizeX = 0.0f,
			MaxSizeY = 0.0f,
			DefaultSizeX = 0.3f,
			DefaultSizeY = 0.6f,
			DefaultPositionX = 0.25f,
			DefaultPositionY = 0.25f,
			ContentsPrefabPath = "res://scenes/ui/test_transparent_bg.tscn"
		};
		TestWindow = new UIWindow(TestWindowSettings, this, screenSize, RemoveWindow, SetActiveWindow, ControlInput);
	}
	
	// Not a great way to handle inputs, works well enough for demo. This method only accepts commands while scene is the active window
	public override void _Input(InputEvent _event)
	{
		
		if (_event is InputEventKey _key){
			if(_key.Keycode == Key.Escape){
				ActiveWindow.Close();
			}
		
		// Typically check if input is allowed here
			if  (_key.Keycode == Key.T){
				SetActiveWindow(InfoWindow);
			}
			if (_key.Keycode == Key.R){
				SetActiveWindow(TestWindow);
			}
		}
	}

	static int GetIndexOfWindow(UIWindow _window)
	{
		return ActiveWindows.IndexOf(_window);
	}

	// If window is already open then bring to the front and replace it as active window in array
	static void SetActiveWindow(UIWindow _window)
	{
		ActiveWindow = _window;
		int windowIndex = GetIndexOfWindow(_window);
		if(windowIndex >= 0)
		{
			ActiveWindows.RemoveAt(windowIndex);
			ActiveWindows.Add(_window);
			return;
		}
		ActiveWindows.Add(_window);
		ActiveWindow.Open();
		ActiveWindow.GrabFocus();
	}

	// Called by UIWindow, closes window and brings next window on the list to active
	public static void RemoveWindow(UIWindow _window)
	{
		ActiveWindows.Remove(_window);
		if(ActiveWindows.Count > 0){
			ActiveWindow = ActiveWindows[^1];
			ActiveWindow.GrabFocus();
		}
	}
	
	public static void ControlInput(bool _bool){
		// Functionality removed for demo
	}
}
