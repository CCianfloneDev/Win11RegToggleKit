using System.Diagnostics;

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

        /// <summary>
        /// Handles the switch toggled event.
        /// </summary>
        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            Microsoft.Maui.Controls.Switch switchControl = (Microsoft.Maui.Controls.Switch)sender;
            Process[] explorerProcesses = Process.GetProcessesByName("explorer");

            List<string> registryKeys = GetRegistryKeys(switchControl.StyleId);

            foreach (string registryKeyPath in registryKeys)
            {

                // Check if the registry key exists before attempting to modify it.
                if (switchControl.IsToggled && RegistryKeyExists(registryKeyPath))
                {
                    Debug.WriteLine("Registry key already exists. No modification needed.");
                    return;
                }

                Process process = new(); 
                ProcessStartInfo startInfo = new()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "reg.exe"
                };

                string commandType;
                if (switchControl.IsToggled)
                {
                    commandType = "created";
                    startInfo.Arguments = $"ADD HKCU\\{registryKeyPath} /f /ve";
                }
                else
                {
                    commandType = "deleted";
                    startInfo.Arguments = $"DELETE HKCU\\{registryKeyPath} /f";
                }

                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();


                // Check if the registry key creation was successful.
                if (process.ExitCode == 0)
                {
                    Debug.WriteLine($"Registry key {commandType} successfully.");
                }
                else
                {
                    Debug.WriteLine($"Failed to modify the registry key.");
                }

                // Restart the windows explorer if we created the reg key, dont need to for deleting. 
                // only for the windows 10 context menu regedit
                if (switchControl.StyleId == switchWin10Menu.StyleId && commandType == "created")
                {
                    foreach (Process p in explorerProcesses)
                    {
                        p.Kill();
                    }

                    Process.Start("explorer.exe");
                }
            }
        }

        /// <summary>
        /// Indicates if the passed registry key exists or not.
        /// </summary>
        /// <param name="keyPath">Path of registry key to check.</param>
        /// <returns>True if the passed registry key exists on this system or not.</returns>
        private static bool RegistryKeyExists(string keyPath)
        {
            try
            {
                using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(keyPath);
                return key != null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking registry key: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Returns the registry key to modify for the given switch name.
        /// </summary>
        /// <param name="switchName">Switch associated with registry key.</param>
        /// <returns>List of Registry Key Path(s).</returns>
        public static List<string> GetRegistryKeys(string switchName)
        {
            List<string> registryKeys = new();

            if (switchName == "switchWin10Menu")
            {
                registryKeys.Add("Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32");
            }
            if (switchName == "switchPhotoViewer")
            {
                registryKeys.Add(@"SOFTWARE\Classes\.bmp");
                registryKeys.Add(@"SOFTWARE\Classes\.gif");
                registryKeys.Add(@"SOFTWARE\Classes\.ico");
                registryKeys.Add(@"SOFTWARE\Classes\.jpeg");
                registryKeys.Add(@"SOFTWARE\Classes\.jpg");
                registryKeys.Add(@"SOFTWARE\Classes\.png");
                registryKeys.Add(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.gif\OpenWithProgids");
                registryKeys.Add(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.ico\OpenWithProgids");
                registryKeys.Add(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.jpeg\OpenWithProgids");
                registryKeys.Add(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.bmp\OpenWithProgids");
                registryKeys.Add(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.jpg\OpenWithProgids");
                registryKeys.Add(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.png\OpenWithProgids");
            }
             // Add more keys here if needed

            return registryKeys;
        }
    }
}
