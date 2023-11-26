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
            Process process = new();
            Process[] explorerProcesses = Process.GetProcessesByName("explorer");
            ProcessStartInfo startInfo = new()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "reg.exe"
            };

            string registryKeyPath = "Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32";


            // Check if the registry key exists before attempting to modify it.
            if (!cmenuSwitch.IsToggled && RegistryKeyExists(registryKeyPath))
            {
                Debug.WriteLine("Registry key already exists. No modification needed.");
                return;
            }

            string commandType;
            if (!cmenuSwitch.IsToggled)
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
            if (commandType == "created")
            {
                foreach (Process p in explorerProcesses)
                {
                    p.Kill();
                }

                Process.Start("explorer.exe");
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
    }
}
