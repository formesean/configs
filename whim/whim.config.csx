#nullable enable
#r "C:\Program Files\Whim\whim.dll"
#r "C:\Program Files\Whim\plugins\Whim.Bar\Whim.Bar.dll"
#r "C:\Program Files\Whim\plugins\Whim.CommandPalette\Whim.CommandPalette.dll"
#r "C:\Program Files\Whim\plugins\Whim.FloatingLayout\Whim.FloatingLayout.dll"
#r "C:\Program Files\Whim\plugins\Whim.FocusIndicator\Whim.FocusIndicator.dll"
#r "C:\Program Files\Whim\plugins\Whim.Gaps\Whim.Gaps.dll"
#r "C:\Program Files\Whim\plugins\Whim.LayoutPreview\Whim.LayoutPreview.dll"
#r "C:\Program Files\Whim\plugins\Whim.SliceLayout\Whim.SliceLayout.dll"
#r "C:\Program Files\Whim\plugins\Whim.TreeLayout\Whim.TreeLayout.dll"
#r "C:\Program Files\Whim\plugins\Whim.TreeLayout.Bar\Whim.TreeLayout.Bar.dll"
#r "C:\Program Files\Whim\plugins\Whim.TreeLayout.CommandPalette\Whim.TreeLayout.CommandPalette.dll"
#r "C:\Program Files\Whim\plugins\Whim.Updater\Whim.Updater.dll"

using System;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Whim;
using Whim.Bar;
using Whim.CommandPalette;
using Whim.FloatingLayout;
using Whim.FocusIndicator;
using Whim.Gaps;
using Whim.LayoutPreview;
using Whim.SliceLayout;
using Whim.TreeLayout;
using Whim.TreeLayout.Bar;
using Whim.TreeLayout.CommandPalette;
using Whim.Updater;
using Windows.Win32.UI.Input.KeyboardAndMouse;

/// <summary>
/// This is what's called when Whim is loaded.
/// </summary>
/// <param name="context"></param>
void DoConfig(IContext context)
{
	context.Logger.Config = new LoggerConfig();

	// Bar plugin.
	List<BarComponent> leftComponents = new() { WorkspaceWidget.CreateComponent() };
	List<BarComponent> centerComponents = new() { FocusedWindowWidget.CreateComponent() };
	List<BarComponent> rightComponents =
		new()
		{
			ActiveLayoutWidget.CreateComponent(),
			DateTimeWidget.CreateComponent(100, "MM/dd/yyyy HH:mm")
		};

	BarConfig barConfig = new(leftComponents, centerComponents, rightComponents);
	BarPlugin barPlugin = new(context, barConfig);
	context.PluginManager.AddPlugin(barPlugin);

	// Gap plugin.
	GapsConfig gapsConfig = new() { OuterGap = 0, InnerGap = 10 };
	GapsPlugin gapsPlugin = new(context, gapsConfig);
	context.PluginManager.AddPlugin(gapsPlugin);

	// Floating window plugin.
	FloatingLayoutPlugin floatingLayoutPlugin = new(context);
	context.PluginManager.AddPlugin(floatingLayoutPlugin);

	// Focus indicator.
	FocusIndicatorConfig focusIndicatorConfig = new() { Color = new SolidColorBrush(Colors.Transparent), BorderSize = 6, FadeTimeout = TimeSpan.FromSeconds(1.0), FadeEnabled = true };
	FocusIndicatorPlugin focusIndicatorPlugin = new(context, focusIndicatorConfig);
	context.PluginManager.AddPlugin(focusIndicatorPlugin);

	// Command palette.
	CommandPaletteConfig commandPaletteConfig = new(context);
	CommandPalettePlugin commandPalettePlugin = new(context, commandPaletteConfig);
	context.PluginManager.AddPlugin(commandPalettePlugin);

	// Slice layout.
	SliceLayoutPlugin sliceLayoutPlugin = new(context);
	context.PluginManager.AddPlugin(sliceLayoutPlugin);

	// Tree layout.
	TreeLayoutPlugin treeLayoutPlugin = new(context);
	context.PluginManager.AddPlugin(treeLayoutPlugin);

	// Tree layout bar.
	TreeLayoutBarPlugin treeLayoutBarPlugin = new(treeLayoutPlugin);
	context.PluginManager.AddPlugin(treeLayoutBarPlugin);
	rightComponents.Add(treeLayoutBarPlugin.CreateComponent());

	// Tree layout command palette.
	TreeLayoutCommandPalettePlugin treeLayoutCommandPalettePlugin =
		new(context, treeLayoutPlugin, commandPalettePlugin);
	context.PluginManager.AddPlugin(treeLayoutCommandPalettePlugin);

	// Layout preview.
	LayoutPreviewPlugin layoutPreviewPlugin = new(context);
	context.PluginManager.AddPlugin(layoutPreviewPlugin);

	// Updater.
	UpdaterConfig updaterConfig = new() { ReleaseChannel = ReleaseChannel.Alpha };
	UpdaterPlugin updaterPlugin = new(context, updaterConfig);
	context.PluginManager.AddPlugin(updaterPlugin);

	// Set up workspaces.
	context.WorkspaceManager.Add("web");
	context.WorkspaceManager.Add("code");
	context.WorkspaceManager.Add("music");
	context.WorkspaceManager.Add("chat");
	context.WorkspaceManager.Add("other");

	// Dictionary<string, string> workspaces = new Dictionary<string, string>();
    // void AddWorkspace(string name, string icon) {
    //     workspaces.Add(name, icon);
    //     context.WorkspaceManager.Add(icon);
    // }

	// AddWorkspace("web", "\udb80\udde7");
	// AddWorkspace("code", "\udb80\udd74");
	// AddWorkspace("music", "\uf1bc");
	// AddWorkspace("chat", "\udb81\ude6f");
	// AddWorkspace("other", "\udb80\uddd8");

	// Set up layout engines.
	context.WorkspaceManager.CreateLayoutEngines = () =>
		new CreateLeafLayoutEngine[]
		{
			(id) => SliceLayouts.CreateMultiColumnLayout(context, sliceLayoutPlugin, id, 1, 2, 0),
			(id) => SliceLayouts.CreatePrimaryStackLayout(context, sliceLayoutPlugin, id),
			(id) => SliceLayouts.CreateSecondaryPrimaryLayout(context, sliceLayoutPlugin, id),
			(id) => new FocusLayoutEngine(id),
			(id) => new TreeLayoutEngine(context, treeLayoutPlugin, id)
		};

	// Routing
	context.RouterManager.AddProcessFileNameRoute("Brave.exe", "web");
	context.RouterManager.AddProcessFileNameRoute("Chrome.exe", "web");
	context.RouterManager.AddProcessFileNameRoute("WindowsTerminal.exe", "code");
	context.RouterManager.AddProcessFileNameRoute("Code.exe", "code");
	context.RouterManager.AddProcessFileNameRoute("Spotify.exe", "music");
	context.RouterManager.AddProcessFileNameRoute("Discord.exe", "chat");
	
	// context.RouterManager.AddProcessFileNameRoute("Brave.exe", workspaces["web"]);
	// context.RouterManager.AddProcessFileNameRoute("Chrome.exe", workspaces["web"]);
	// context.RouterManager.AddProcessFileNameRoute("WindowsTerminal.exe", workspaces["code"]);
	// context.RouterManager.AddProcessFileNameRoute("Code.exe", workspaces["code"]);
	// context.RouterManager.AddProcessFileNameRoute("Spotify.exe", workspaces["music"]);
	// context.RouterManager.AddProcessFileNameRoute("Discord.exe", workspaces["chat"]);

	// Filtering
	context.FilterManager.AddTitleMatchFilter("Steam");
	context.FilterManager.AddTitleMatchFilter("Dota 2");
	context.FilterManager.AddTitleMatchFilter("Riot Client");
	context.FilterManager.AddTitleMatchFilter("League of Legends");
	context.FilterManager.AddTitleMatchFilter("Settings");
	context.FilterManager.AddTitleMatchFilter("Fluent Search");
	
	// KeyBinds
	KeyModifiers Alt = KeyModifiers.LAlt;
    KeyModifiers AltShift = KeyModifiers.LAlt | KeyModifiers.LShift;

	void Bind(KeyModifiers mod, string key, string cmd)
    {
        VIRTUAL_KEY vk = (VIRTUAL_KEY)Enum.Parse(typeof(VIRTUAL_KEY), "VK_" + key);
        context.KeybindManager.SetKeybind(cmd, new Keybind(mod, vk));
    }

	Bind(Alt, "1", "whim.core.activate_workspace_1");
	Bind(Alt, "2", "whim.core.activate_workspace_2");
	Bind(Alt, "3", "whim.core.activate_workspace_3");
	Bind(Alt, "4", "whim.core.activate_workspace_4");
	Bind(Alt, "5", "whim.core.activate_workspace_5");
	Bind(Alt, "LEFT", "whim.core.activate_previous_workspace");
	Bind(Alt, "RIGHT", "whim.core.activate_next_workspace");
	Bind(Alt, "OEM_COMMA", "whim.core.focus_window_in_direction.left");
	Bind(Alt, "OEM_PERIOD", "whim.core.focus_window_in_direction.right");
	Bind(AltShift, "OEM_COMMA", "whim.core.cycle_layout_engine.previous");
	Bind(AltShift, "OEM_PERIOD", "whim.core.cycle_layout_engine.next");
	Bind(AltShift, "C", "whim.command_palette.toggle");
	Bind(AltShift, "M", "whim.command_palette.move_window_to_workspace");
	Bind(AltShift, "Q", "whim.core.exit_whim");
	Bind(AltShift, "R", "whim.core.restart_whim");
}

// We return doConfig here so that Whim can call it when it loads.
return DoConfig;
