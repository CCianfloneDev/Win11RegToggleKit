using System.Diagnostics;
using System.Reflection;

namespace Win11RegToggleKit
{
    /// <summary>
    /// Represents the main page.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// Initializes the main page.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }


        private void ApplyRegistryChangesFromResource(string resourceName, bool restartExplorer = false)
        {
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using StreamReader reader = new(stream);
                string registryContent = reader.ReadToEnd();
                string temporaryPath = Path.GetTempFileName(); 

                // Create a temporary .reg file Write the content of the embedded resource to the file
                File.WriteAllText(temporaryPath, registryContent);

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
                }
                else
                {
                    Debug.WriteLine("Failed to apply registry changes.");
                }
                //ApplyRegistryChangesFromFile(temporaryPath, restartExplorer);

                // Delete the temporary .reg file after applying changes
                File.Delete(temporaryPath);
            }
            else
            {
                Debug.WriteLine("Resource not found.");
            }
        }

        ///// <summary>
        ///// Runs the registry edit file from the specified location.
        ///// </summary>
        ///// <param name="filePath">Path of .REG file.</param>
        ///// <param name="restartExplorer">Indicates if the regedit requires a explorer.exe restart.</param>
        //private void ApplyRegistryChangesFromFile(string filePath, bool restartExplorer = false)
        //{
        //    Process process = new();
        //    ProcessStartInfo startInfo = new()
        //    {
        //        WindowStyle = ProcessWindowStyle.Hidden,
        //        FileName = "reg.exe",
        //        Arguments = $"import \"{filePath}\""
        //    };

        //    process.StartInfo = startInfo;
        //    process.Start();
        //    process.WaitForExit();

        //    if (process.ExitCode == 0)
        //    {
        //        Debug.WriteLine("Registry changes applied successfully.");

        //        if (restartExplorer)
        //        {
        //            RestartExplorer();
        //        }
        //    }
        //    else
        //    {
        //        Debug.WriteLine("Failed to apply registry changes.");
        //    }
        //}


        private void ApplyOldPhotoViewer()
        {
            ApplyRegistryChangesFromResource("Win11RegToggleKit.Restore_Windows_Photo_Viewer_CURRENT_USER.reg");
        }

        private void RemoveOldPhotoViewer()
        {
            ApplyRegistryChangesFromResource("Win11RegToggleKit.Undo_Restore_Windows_Photo_Viewer_CURRENT_USER.reg");
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
