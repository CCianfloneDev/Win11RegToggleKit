﻿using System.Diagnostics;
using System.Reflection;

namespace Win11RegToggleKit
{
    /// <summary>
    /// Represents the main page.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Represents the base directory which stores the registry edit files.
        /// </summary>
        private static string BaseRegEditsDirectory => "Win11RegToggleKit.Resources.RegistryEdits.";

        /// <summary>
        /// Initializes the main page.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Applies Registry Edit changes from a specified embedded .reg resource file.
        /// </summary>
        /// <param name="resourceName">Name of the .REG file to run.</param>
        /// <param name="restartExplorer">Indicates if explorer.exe needs to restart.</param>
        private void ApplyRegistryChangesFromResource(string resourceName, bool restartExplorer = false)
        {
            try
            {
                if (!resourceName.EndsWith(".reg", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine($"Invalid resource file format: {resourceName}");
                    return;
                }

                using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

                if (stream == null)
                {
                    Debug.WriteLine($"Resource not found: {resourceName}");
                    return;
                }

                using StreamReader reader = new(stream);
                string registryContent = reader.ReadToEnd();
                string temporaryPath = Path.GetTempFileName();

                // Create a temporary .reg file Write the content of the embedded resource to the file
                File.WriteAllText(temporaryPath, registryContent);

                // Start reg.exe process and run the temporary .reg file
                Process process = new();
                ProcessStartInfo startInfo = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "reg.exe",
                    Arguments = $"import \"{temporaryPath}\""
                };

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Debug.WriteLine("Registry changes applied successfully.");

                    if (restartExplorer)
                    {
                        RestartExplorer();
                    }
                } else
                {
                    Debug.WriteLine($"Failed to apply registry changes: {process.ExitCode}");
                }

                // Delete the temporary .reg file after applying changes
                File.Delete(temporaryPath);
            } 
            catch (Exception error) 
            {
                string currentMethod = MethodBase.GetCurrentMethod()!.Name;
                Debug.WriteLine($"Error:{currentMethod}:{error}");
            }
        }

        /// <summary>
        /// Restarts explorer.exe
        /// </summary>
        private void RestartExplorer()
        {
            try
            {
                Process[] explorerProcesses = Process.GetProcessesByName("explorer");
                foreach (Process process in explorerProcesses)
                {
                    process.Kill();
                }
                Process.Start("explorer.exe");
            }
            catch ( Exception error )
            {
                string currentMethod = MethodBase.GetCurrentMethod()!.Name;
                Debug.WriteLine($"Error:{currentMethod}:{error}");
            }
        }


        /// <summary>
        /// Handles the switch toggled event.
        /// </summary>
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            try 
            {
                Microsoft.Maui.Controls.Switch switchControl = (Microsoft.Maui.Controls.Switch)sender;

                if (switchControl.StyleId == switchWin10Menu.StyleId)
                {
                    if (switchControl.IsToggled)
                    {
                       ApplyRegistryChangesFromResource($"{BaseRegEditsDirectory}Apply_Windows10ContextMenuForWindows11.reg", restartExplorer:true);
                    }
                    else
                    {
                        ApplyRegistryChangesFromResource($"{BaseRegEditsDirectory}Undo_Windows10ContextMenuForWindows11.reg");
                    }
                }
                else if (switchControl.StyleId == switchPhotoViewer.StyleId)
                {
                    if (switchControl.IsToggled)
                    {
                        ApplyRegistryChangesFromResource($"{BaseRegEditsDirectory}Apply_WindowsPhotoViewer.reg");
                    }
                    else
                    {
                        ApplyRegistryChangesFromResource($"{BaseRegEditsDirectory}Undo_WindowsPhotoViewer.reg");
                    }
                }
                else if (switchControl.StyleId == allowUnsupportedUpgrades.StyleId)
                {
                    if (switchControl.IsToggled)
                    {
                        ApplyRegistryChangesFromResource($"{BaseRegEditsDirectory}Apply_AllowUnsupportedUpgrades.reg");
                    }
                    else
                    {
                        ApplyRegistryChangesFromResource($"{BaseRegEditsDirectory}Undo_AllowUnsupportedUpgrades.reg");
                    }
                }
                else if (switchControl.StyleId == showFileExtensions.StyleId)
                {
                    if (switchControl.IsToggled) 
                    {
                        ApplyRegistryChangesFromResource($"{BaseRegEditsDirectory}Apply_ShowFileExtensions.reg");
                    }
                    else
                    {
                        ApplyRegistryChangesFromResource($"{BaseRegEditsDirectory}Undo_ShowFileExtensions.reg");
                    }
                }

            } 
            catch (Exception error) 
            {
                string currentMethod = MethodBase.GetCurrentMethod()!.Name;
                Debug.WriteLine($"Error:{currentMethod}:{error}");
            }
        }
    }
}
