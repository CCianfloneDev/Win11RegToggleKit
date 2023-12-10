using System.Diagnostics;
using System.Reflection;

namespace Win11RegToggleKit
{
    /// <summary>
    /// Represents the main page.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private static string BaseRegEditsDirectory => "Win11RegToggleKit.Resources.RegistryEdits.";

        /// <summary>
        /// Initializes the main page.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }


        private void ApplyRegistryChangesFromResource(string resourceName, bool restartExplorer = false)
        {
            try
            {
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
        

        private void ApplyOldPhotoViewer()
        {
            ApplyRegistryChangesFromResource($"{BaseRegEditsDirectory}Restore_Windows_Photo_Viewer_CURRENT_USER.reg");
        }

        private void RemoveOldPhotoViewer()
        {
            ApplyRegistryChangesFromResource($"{BaseRegEditsDirectory}Undo_Restore_Windows_Photo_Viewer_CURRENT_USER.reg");
        }

        //private void ApplyWindows10ContextMenu()
        //{
        //    string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); // Get the directory where the .sln file is located
        //    string filePath = Path.Combine(directory, "AddWindows10ContextMenu.reg");
        //    ApplyRegistryChangesFromFile(filePath, restartExplorer: true); // Restart explorer when applying changes
        //}

        //private void RemoveWindows10ContextMenu()
        //{
        //    string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); // Get the directory where the .sln file is located
        //    string filePath = Path.Combine(directory, "RemoveWindows10ContextMenu.reg");
        //    ApplyRegistryChangesFromFile(filePath);
        //}

        private void RestartExplorer()
        {
            Process[] explorerProcesses = Process.GetProcessesByName("explorer");
            foreach (Process process in explorerProcesses)
            {
                process.Kill();
            }
            Process.Start("explorer.exe");
        }


        /// <summary>
        /// Handles the switch toggled event.
        /// </summary>
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            Microsoft.Maui.Controls.Switch switchControl = (Microsoft.Maui.Controls.Switch)sender;

            if (switchControl.StyleId == switchWin10Menu.StyleId)
            {
                //if (switchControl.IsToggled)
                //{
                //    ApplyWindows10ContextMenu();
                //}
                //else
                //{
                //    RemoveWindows10ContextMenu();
                //}
            }
            else if (switchControl.StyleId == switchPhotoViewer.StyleId)
            {
                if (switchControl.IsToggled)
                {
                    ApplyOldPhotoViewer();
                }
                else
                {
                    RemoveOldPhotoViewer();
                }
            }
        }
    }
}
