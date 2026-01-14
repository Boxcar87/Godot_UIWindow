using Godot;
using System;

// This class handles the popup window only. It takes constructor arguements for the following:
// WindowFixedSettings _uneditableSettings  
// Node _parentNode 	// Used to handle visibility in scene
// Vector2I _screenSize // Size and Position are normalized in execution so we use GetViewport().GetVisibleRect().Size to handle window manipulation
// Action<UIWindow> _removeTrackingCallback // Used to handle window focus logic on closing a window
// Action<UIWindow> _setActiveFunc // Used to assist in window ordering
// Action<bool> _allowInputs // Not used in demo

public partial class UIWindow : Window
{   
	Node ParentNode;
	Action<UIWindow> RemoveTracking;
	Action<UIWindow> SetActive;
	Action<bool> AllowInputs;
	bool DisableInputWhenActive;
	string Name;
	Vector2I ScreenSize;

	WindowSavedSettings Settings;

	public UIWindow(WindowFixedSettings _fixedSettings, Node _node, Vector2I _screenSize, Action<UIWindow> _removeTrackingFunc, Action<UIWindow> _setActiveFunc, Action<bool> _allowInputs)
	{   
		ScreenSize = _screenSize;
		Name = _fixedSettings.Type.ToString();
		Settings = SettingsManager.GetUISettings(Name);
		
		// Check if saved data exists for this window
		if(Settings == null)
		{	
			Settings = new WindowSavedSettings();
		}
		if(Settings.PositionX == 0)
		{
			Settings.Width = _fixedSettings.DefaultSizeX ;
			Settings.Height = _fixedSettings.DefaultSizeY;
			Settings.PositionX = _fixedSettings.DefaultPositionX;
			Settings.PositionY = _fixedSettings.DefaultPositionY;        

			SettingsManager.UpdateUISettings(Settings, Name);		
			Size = new Vector2I((int)(_fixedSettings.DefaultSizeX * ScreenSize.X), (int)(_fixedSettings.DefaultSizeY * ScreenSize.Y));
			Position = new Vector2I((int)(_fixedSettings.DefaultPositionX * ScreenSize.X), (int)(_fixedSettings.DefaultPositionY * ScreenSize.Y));
		}
		else
		{
			Settings = SettingsManager.GetUISettings(Name);
			Size = new Vector2I((int)(Settings.Width * ScreenSize.X), (int)(Settings.Height * ScreenSize.Y));
			Position = new Vector2I((int)(Settings.PositionX * ScreenSize.X), (int)(Settings.PositionY * ScreenSize.Y));
		}		

		// Derived
		Unresizable = _fixedSettings.Unresizable;
		Transparent = _fixedSettings.Transparent;
		TransparentBg = _fixedSettings.Transparent;
		CloseRequested += Close;	
		FocusEntered += MakeWindowActive;
		FocusExited += RemoveFocus;

		PackedScene _uiElements = ResourceLoader.Load<PackedScene>(_fixedSettings.ContentsPrefabPath);
		AddChild(_uiElements.Instantiate());	
		PanelContainer background = (PanelContainer)GetChild(0).FindChild("Background");
		StyleBoxFlat backgroundColorBox = (StyleBoxFlat)background.GetThemeStylebox("panel");
		Godot.Color backgroundColor = backgroundColorBox.BgColor;
		StyleBoxFlat unfocusedBox = InitStyle(backgroundColor);
		StyleBoxFlat focusedBox = InitStyle(backgroundColor);		
		if (Transparent)
		{			
			focusedBox.BgColor = new Godot.Color(backgroundColor.R, backgroundColor.G, backgroundColor.B, 0.7f);
			backgroundColorBox.BgColor = new Godot.Color(backgroundColor.R, backgroundColor.G, backgroundColor.B, 0.7f);
		}		
		unfocusedBox.BorderColor = new Godot.Color(0.4f, 0.4f, 0.4f, 0.9f);
		AddThemeStyleboxOverride("embedded_border", focusedBox);
		AddThemeStyleboxOverride("embedded_unfocused_border", unfocusedBox);

		DisableInputWhenActive = _fixedSettings.DisableInputWhenActive;
		ParentNode = _node;
		RemoveTracking = _removeTrackingFunc;
		SetActive = _setActiveFunc;
		AllowInputs = _allowInputs;	
	}
	
	public void Open()
	{
		ParentNode.AddChild(this);
	}

	// Handles removing the window from scene, remains in memory.
	public void Close()
	{
		LineEdit inputField = (LineEdit)GetChild(0).FindChild("LineEdit");
		if(inputField != null)
			inputField.Text = "";

		Settings.PositionX = (float)Position.X/ScreenSize.X;
		Settings.PositionY = (float)Position.Y/ScreenSize.Y;
		if (!Unresizable)
		{
			Settings.Width = (float)Size.X/ScreenSize.X;
			Settings.Height = (float)Size.Y/ScreenSize.Y;
		}
		SettingsManager.UpdateUISettings(Settings, Name);

		RemoveFocus();
		RemoveTracking(this);
		ParentNode.RemoveChild(this);
	}

	void MakeWindowActive()
	{
		SetActive(this);
		if(DisableInputWhenActive)
			AllowInputs(false);
	}

	void RemoveFocus()
	{
		if(DisableInputWhenActive)
			AllowInputs(true);
	}

	StyleBoxFlat InitStyle(Godot.Color _backgroundColor)
	{
		return new(){
			BorderWidthLeft = 6,
			BorderWidthRight = 6,
			BorderWidthBottom = 6,
			BorderWidthTop = 30,
			CornerDetail = 20,
			CornerRadiusBottomLeft = 5,
			CornerRadiusBottomRight = 5,
			CornerRadiusTopLeft = 5,
			CornerRadiusTopRight = 5,
			DrawCenter = true,
			ExpandMarginLeft = 8.0f,
			ExpandMarginRight = 8.0f,
			ExpandMarginBottom = 8.0f,
			ExpandMarginTop = 30.0f,
			BorderColor = new Godot.Color(0.25f, 0.25f, 0.25f, 1f),
			BgColor = _backgroundColor
		};
	}
}
