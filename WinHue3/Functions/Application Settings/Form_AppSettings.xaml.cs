using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using IWshRuntimeLibrary;
using WinHue3.Functions.Application_Settings.Settings;
using File = System.IO.File;

namespace WinHue3.Functions.Application_Settings
{
    /// <summary>
    /// Interaction logic for AppSettings.xaml
    /// </summary>
    public partial class Form_AppSettings
    {
 
        private AppSettingsViewModel _appSettingsViewModel;
        public Form_AppSettings()
        {
            InitializeComponent();  
            _appSettingsViewModel = DataContext as AppSettingsViewModel;    
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (WinHueSettings.settings.Language != _appSettingsViewModel.MainSettingsModel.Language)
            {
                MessageBox.Show(GlobalStrings.Language_Change_Warning, GlobalStrings.Warning, MessageBoxButton.OK, MessageBoxImage.Information);
            }

            WinHueSettings.settings.DetectProxy = _appSettingsViewModel.MainSettingsModel.DetectProxy;
            WinHueSettings.settings.EnableDebug = _appSettingsViewModel.MainSettingsModel.Debug;
            WinHueSettings.settings.ShowHiddenScenes = _appSettingsViewModel.MainSettingsModel.ShowHidden;
            WinHueSettings.settings.UpnpTimeout = _appSettingsViewModel.MainSettingsModel.UpnpTimeout;
            WinHueSettings.settings.AllOffTT = _appSettingsViewModel.DefaultModel.AllOffTt;
            WinHueSettings.settings.AllOnTT = _appSettingsViewModel.DefaultModel.AllOnTt;
            WinHueSettings.settings.Timeout = _appSettingsViewModel.MainSettingsModel.Timeout;
            WinHueSettings.settings.DefaultTT = _appSettingsViewModel.DefaultModel.DefaultTt;
            WinHueSettings.settings.WrapText = _appSettingsViewModel.ViewSettingsModel.Wrap;
            WinHueSettings.settings.ShowID = _appSettingsViewModel.ViewSettingsModel.ShowId;
            WinHueSettings.settings.Sort = _appSettingsViewModel.ViewSettingsModel.Sort;
            WinHueSettings.settings.DefaultBriGroup = _appSettingsViewModel.DefaultModel.DefaultGroupBri;
            WinHueSettings.settings.DefaultBriLight = _appSettingsViewModel.DefaultModel.DefaultLightBri;
            WinHueSettings.settings.Language = _appSettingsViewModel.MainSettingsModel.Language;
            WinHueSettings.settings.StartMode = _appSettingsViewModel.MainSettingsModel.StartMode;
            WinHueSettings.settings.CheckForUpdate = _appSettingsViewModel.MainSettingsModel.CheckUpdate;
            WinHueSettings.settings.CheckForBridgeUpdate = _appSettingsViewModel.MainSettingsModel.CheckForBridgeUpdate;
            WinHueSettings.settings.UseLastBriState = _appSettingsViewModel.DefaultModel.UseLastBriState;
            WinHueSettings.settings.MinimizeToTray = _appSettingsViewModel.MainSettingsModel.MinimizeToTray;
            WinHueSettings.settings.UsePropertyGrid = _appSettingsViewModel.MainSettingsModel.UsePropGrid;
            WinHueSettings.settings.SlidersBehavior = _appSettingsViewModel.DefaultModel.SlidersBehavior;
            WinHueSettings.settings.ThemeLayout = _appSettingsViewModel.MainSettingsModel.ThemeLayout;
            WinHueSettings.settings.Theme = _appSettingsViewModel.MainSettingsModel.Theme;

            string pathtostartupfile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "WinHue3.lnk");

            if (WinHueSettings.settings.StartMode > 0)
            {
                
                if (!File.Exists(pathtostartupfile))
                {
                 
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(pathtostartupfile);
                    shortcut.Description = "WinHue Startup";
                    shortcut.TargetPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                    shortcut.WorkingDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                    shortcut.Save();
                }

            }
            else
            {
                if (File.Exists(pathtostartupfile))
                {
                    File.Delete(pathtostartupfile);
                }

            }
            //registryKey.Close();

            WinHueSettings.SaveSettings();

            //----BEGIN THEME CHANGE----
            //prepare to change theme resources
            ResourceDictionary theme = new ResourceDictionary();
            ResourceDictionary layout = new ResourceDictionary();
            Application.Current.Resources.MergedDictionaries.Clear();

            //change theme
            theme.Source = new Uri(@"/Theming/Themes/" + WinHueSettings.settings.Theme + ".xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(theme);

            //change layout
            layout.Source = new Uri(@"/Theming/Layouts/layout" + WinHueSettings.settings.ThemeLayout + ".xaml", UriKind.Relative);
            Application.Current.Resources.MergedDictionaries.Add(layout);

            //change accent
            //Application.Current.Resources["accent"] = WinHueSettings.settings.ThemeColor;
            Application.Current.Resources["accent"] = AccentColorSet.ActiveSet["SystemAccent"];

            if (WinHueSettings.settings.ThemeColor.R + WinHueSettings.settings.ThemeColor.G + WinHueSettings.settings.ThemeColor.B > 382)
            {
                Application.Current.Resources["textOnAccent"] = System.Windows.Media.Color.FromRgb(0, 0, 0);
            }
            else
            {
                Application.Current.Resources["textOnAccent"] = System.Windows.Media.Color.FromRgb(255, 255, 255);
            }

            //MessageBox.Show(WinHue3.Functions.Application_Settings.Settings.WinHueSettings.settings.Theme);
            //----END THEME CHANGE----

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            
           // Fluent.ThemeManager.ChangeAppStyle(Application.Current, Fluent.ThemeManager.GetAccent(WinHueSettings.settings.ThemeColor), Fluent.ThemeManager.GetAppTheme(WinHueSettings.settings.Theme));
            
            Close();
        }

        private void btnAccent_Click(object sender, RoutedEventArgs e)
        {
            //Work in Progress

            var curColor = WinHueSettings.settings.ThemeColor;
            System.Drawing.Color _color = System.Drawing.Color.FromArgb(curColor.A, curColor.R, curColor.G, curColor.B);
            var cp = new System.Windows.Forms.ColorDialog();
            cp.Color = _color;
            cp.ShowDialog();
            System.Windows.Media.Color newAccent = System.Windows.Media.Color.FromArgb(cp.Color.A, cp.Color.R, cp.Color.G, cp.Color.B);
            WinHueSettings.settings.ThemeColor = newAccent;

        }

        // TODO: Add a listener for WM_SETTINGCHANGE to detect changes of the active color scheme automatically.
        //   Add a listener for WM_SETTINGCHANGE and trigger an event, like ActiveSetChanged.
        class AccentColorSet
        {
            public static AccentColorSet[] AllSets
            {
                get
                {
                    if (_allSets == null)
                    {
                        UInt32 colorSetCount = UXTheme.GetImmersiveColorSetCount();

                        List<AccentColorSet> colorSets = new List<AccentColorSet>();
                        for (UInt32 i = 0; i < colorSetCount; i++)
                        {
                            colorSets.Add(new AccentColorSet(i, false));
                        }

                        AllSets = colorSets.ToArray();
                    }

                    return _allSets;
                }
                private set
                {
                    _allSets = value;
                }
            }

            public static AccentColorSet ActiveSet
            {
                get
                {
                    UInt32 activeSet = UXTheme.GetImmersiveUserColorSetPreference(false, false);
                    ActiveSet = AllSets[Math.Min(activeSet, AllSets.Length - 1)];
                    return _activeSet;
                }
                private set
                {
                    if (_activeSet != null) _activeSet.Active = false;

                    value.Active = true;
                    _activeSet = value;
                }
            }

            public Boolean Active { get; private set; }

            public Color this[String colorName]
            {
                get
                {
                    IntPtr name = IntPtr.Zero;
                    UInt32 colorType;

                    try
                    {
                        name = Marshal.StringToHGlobalUni("Immersive" + colorName);
                        colorType = UXTheme.GetImmersiveColorTypeFromName(name);
                        if (colorType == 0xFFFFFFFF) throw new InvalidOperationException();
                    }
                    finally
                    {
                        if (name != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(name);
                            name = IntPtr.Zero;
                        }
                    }

                    return this[colorType];
                }
            }

            public Color this[UInt32 colorType]
            {
                get
                {
                    UInt32 nativeColor = UXTheme.GetImmersiveColorFromColorSetEx(this._colorSet, colorType, false, 0);
                    //if (nativeColor == 0)
                    //    throw new InvalidOperationException();
                    return Color.FromArgb(
                        (Byte)((0xFF000000 & nativeColor) >> 24),
                        (Byte)((0x000000FF & nativeColor) >> 0),
                        (Byte)((0x0000FF00 & nativeColor) >> 8),
                        (Byte)((0x00FF0000 & nativeColor) >> 16)
                        );
                }
            }

            AccentColorSet(UInt32 colorSet, Boolean active)
            {
                this._colorSet = colorSet;
                this.Active = active;
            }

            static AccentColorSet[] _allSets;
            static AccentColorSet _activeSet;

            UInt32 _colorSet;

            // HACK: GetAllColorNames collects the available color names by brute forcing the OS function.
            //   Since there is currently no known way to retrieve all possible color names,
            //   the method below just tries all indices from 0 to 0xFFF ignoring errors.
            public List<String> GetAllColorNames()
            {
                List<String> allColorNames = new List<String>();
                for (UInt32 i = 0; i < 0xFFF; i++)
                {
                    IntPtr typeNamePtr = UXTheme.GetImmersiveColorNamedTypeByIndex(i);
                    if (typeNamePtr != IntPtr.Zero)
                    {
                        IntPtr typeName = (IntPtr)Marshal.PtrToStructure(typeNamePtr, typeof(IntPtr));
                        allColorNames.Add(Marshal.PtrToStringUni(typeName));
                    }
                }

                return allColorNames;
            }

            static class UXTheme
            {
                [DllImport("uxtheme.dll", EntryPoint = "#98", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
                public static extern UInt32 GetImmersiveUserColorSetPreference(Boolean forceCheckRegistry, Boolean skipCheckOnFail);

                [DllImport("uxtheme.dll", EntryPoint = "#94", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
                public static extern UInt32 GetImmersiveColorSetCount();

                [DllImport("uxtheme.dll", EntryPoint = "#95", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
                public static extern UInt32 GetImmersiveColorFromColorSetEx(UInt32 immersiveColorSet, UInt32 immersiveColorType,
                    Boolean ignoreHighContrast, UInt32 highContrastCacheMode);

                [DllImport("uxtheme.dll", EntryPoint = "#96", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
                public static extern UInt32 GetImmersiveColorTypeFromName(IntPtr name);

                [DllImport("uxtheme.dll", EntryPoint = "#100", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
                public static extern IntPtr GetImmersiveColorNamedTypeByIndex(UInt32 index);
            }
        }

    }
}
