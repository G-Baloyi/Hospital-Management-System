using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PRG281_Project
{
    internal class EnumLibrary
    {

        
        public static class SecurityHelper
        {
            private const int MaxLoginAttempts = 3;
            private const int LockoutMinutes = 15;
            private static Dictionary<string, SecurityAttempt> failedAttempts = new Dictionary<string, SecurityAttempt>();
            private static readonly object lockObject = new object();

            // Security event delegate
            public delegate void SecurityEventHandler(string action, string details, bool success);
            public static event SecurityEventHandler SecurityEvent;

            // Class to track attempts with timestamp
            private class SecurityAttempt
            {
                public int Count { get; set; }
                public DateTime LastAttempt { get; set; }
                public DateTime? LockoutUntil { get; set; }
            }

            
            // Raises security events for monitoring and logging
            
            private static void OnSecurityEvent(string action, string details, bool success = true)
            {
                SecurityEvent?.Invoke(action, details, success);

                // Also log to console for debugging
                Console.ForegroundColor = success ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"[SECURITY] {action}: {details}");
                Console.ResetColor();
            }

            
            /// Checks if a user account is locked due to too many failed attempts
            
            public static bool CheckLoginAttempts(string username)
            {
                lock (lockObject)
                {
                    if (failedAttempts.ContainsKey(username))
                    {
                        var attempt = failedAttempts[username];

                        // Check if account is currently locked
                        if (attempt.LockoutUntil.HasValue && DateTime.Now < attempt.LockoutUntil.Value)
                        {
                            OnSecurityEvent("AccountLocked",
                                $"Account {username} locked until {attempt.LockoutUntil.Value:HH:mm:ss}", false);
                            return false;
                        }

                        // Clear lockout if time has expired
                        if (attempt.LockoutUntil.HasValue && DateTime.Now >= attempt.LockoutUntil.Value)
                        {
                            attempt.LockoutUntil = null;
                            attempt.Count = 0;
                            OnSecurityEvent("AccountUnlocked",
                                $"Account {username} lockout expired", true);
                        }
                    }
                    return true;
                }
            }

            
            /// Records a failed login attempt and locks account if threshold reached
            
            public static void RecordFailedAttempt(string username)
            {
                lock (lockObject)
                {
                    if (!failedAttempts.ContainsKey(username))
                    {
                        failedAttempts[username] = new SecurityAttempt();
                    }

                    var attempt = failedAttempts[username];
                    attempt.Count++;
                    attempt.LastAttempt = DateTime.Now;

                    OnSecurityEvent("FailedLogin",
                        $"User {username} failed attempt #{attempt.Count}", false);

                    // Lock account if max attempts reached
                    if (attempt.Count >= MaxLoginAttempts)
                    {
                        attempt.LockoutUntil = DateTime.Now.AddMinutes(LockoutMinutes);
                        OnSecurityEvent("AccountLocked",
                            $"Account {username} locked for {LockoutMinutes} minutes due to {MaxLoginAttempts} failed attempts", false);
                    }
                }
            }

            
            /// Resets failed attempt counter on successful login
            
            public static void ResetAttempts(string username)
            {
                lock (lockObject)
                {
                    if (failedAttempts.ContainsKey(username))
                    {
                        failedAttempts[username].Count = 0;
                        failedAttempts[username].LockoutUntil = null;
                        OnSecurityEvent("LoginSuccess",
                            $"User {username} logged in successfully - attempts reset", true);
                    }
                }
            }

            
            /// Gets the current security status of a user
            
            public static string GetSecurityStatus(string username)
            {
                lock (lockObject)
                {
                    if (!failedAttempts.ContainsKey(username))
                        return "No failed attempts";

                    var attempt = failedAttempts[username];

                    if (attempt.LockoutUntil.HasValue && DateTime.Now < attempt.LockoutUntil.Value)
                        return $"Locked until {attempt.LockoutUntil.Value:HH:mm:ss}";

                    return $"{attempt.Count} failed attempt(s)";
                }
            }
        }


        //Enum to display at Login
        public enum LogInMenu
        {
            [Description("Log In")]
            LogIn,
            [Description("Sign Up")]
            SignUp,
            [Description("Exit")]
            Exit
        }

        //Enum to display Main Menu
        public enum MainMenu
        {
            [Description("Clinical Overview")]
            Clinical,

            [Description("Pharmaceutical Overview")]
            Pharmacy,

            [Description("Administrative Overview")]
            Administrative,

            [Description("Log Out")]
            LogOut
        }

        //Enum to display Admin Menu
        public enum AdminMenu
        {
            [Description("Employee Management")]
            Employee,

            [Description("Patient Management")]
            Patient,

            [Description("Inventory Configutation")]
            Inventory,

            [Description("Return to Main Menu")]
            BackToMenu
        }

        //Enum to display Pharmacy Menu
        public enum PharmaMenu
        {
            [Description("Medication Management")]
            MediManagement,

            [Description("Fulfill Prescriptions")]
            Prescription,

            [Description("Return to Main Menu")]
            BackToMenu,
        }

        //Enum to display Clinical Menu
        public enum ClinicMenu
        {
            [Description("Register Patient")]
            Register,

            [Description("Display Patient Information")]
            DisplayInfo,

            [Description("Create New Appointment")]
            CreateAppointment,

            [Description("Reschedule Appointment")]
            RescheduleAppoinment,

            [Description("Cancel Appointment")]
            CancelAppointment,

            [Description("Return to Main Menu")]
            BackToMenu,
        }

        //Enum to display the Admin Employee Management Menu
        public enum AdminEmpManagementMenu
        {
            [Description("Add Employee")]
            opt1,

            [Description("Remove Employee")]
            opt2,

            [Description("Return to Menu")]
            BackToMenu,
        }

        //Enum to display the Admin Patient Management Menu
        public enum AdminPatManagementMenu
        {
            [Description("Generate Legal Contract")]
            opt1,

            [Description("Ban Patient")]
            opt2,

            [Description("Return to Menu")]
            BackToMenu,
        }

        //Enum to display the Pharmacy Medication Menu
        public enum PharmaMedManagementMenu
        {
            [Description("Add Medication")]
            opt1,

            [Description("Remove Medication")]
            opt2,

            [Description("Return to Menu")]
            BackToMenu,
        }





        //Declare a variable for the UserFile that is not allowed to be changed.
        private const string UserFile = "UserFile.txt";

        //Creates a lock object to ensure that only one thread at a time can execute the code inside the lock block, preventing conflicts.
        private static readonly object UserFileLock = new object();

        //Main public Method to start the Login and Initial Startup
        public void RunLogin()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;
            Animations animations = new Animations();
            //Matrix rain intro (skippable with any key)
            animations.AniIntro(3500);

            //"Welcome to the project" animation (also skippable)
            animations.WelcomeProjectAni(1000);

            //Sets items as a store for all LogIn Options
            var classLibrary = new ClassLibrary();

            while (true)
            {
                var selected = MenusDisplayHelper.KeyMenuReaderEnumItem<LogInMenu>(
                    "Select an Option",
                    LogInMenuAccentColor,
                    null
                );

                if (!selected.HasValue)
                {
                    // User pressed Esc
                    return;
                }

                var chosen = selected.Value;

                Console.Clear();
                Console.ResetColor();
                Console.WriteLine();
                // If you added EnumMenuHelpers with GetEnumLabel, you can use it to show the friendly label.
                // Otherwise, chosen.ToString() works too.
                Console.WriteLine("You selected: " + EnumMenuDescriptionHandler.GetEnumLabel(chosen));
                Console.WriteLine();

                switch (chosen)
                {
                    case LogInMenu.LogIn:
                        LoginUser();
                        break;

                    case LogInMenu.SignUp:
                        RegisterUser();
                        break;

                    case LogInMenu.Exit:
                        classLibrary.ExitProgram();
                        return;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Press any key to return to the menu...");
                Console.ReadKey(true);
            }
        }

        //Private Method to register a new user in your system.
        private void RegisterUser()
        {
            Animations animations = new Animations();
            Console.WriteLine("Register a new user:\n");
            Console.Write("Enter your new username: ");
            string username = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter your new password: ");
            string password = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Please enter a username.");
                return;
            }
            if (username.Contains(":"))
            {
                Console.WriteLine("Username cannot contain a ':'. Please rewrite without a colon.");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Please enter a password.");
                return;
            }

            // Enables the lock object to avoid two threads checking and writing simultaneously that might cause a deadlock.
            lock (UserFileLock)
            {
                //Acts as a thread
                //Checks if the file exists while checking id the username also exists.
                bool exists = File.Exists(UserFile) && File.ReadLines(UserFile).AsParallel().
                              WithDegreeOfParallelism(Math.Max(1, Environment.ProcessorCount - 1)).Any
                    (line =>
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            return false;
                        }

                        var trimIn = line.Trim();
                        if (trimIn.StartsWith("#"))
                        {
                            return false;
                        }

                        // Split only once to allow ':' inside the password
                        var partIn = trimIn.Split(new[] { ':' }, 2);
                        if (partIn.Length != 2)
                        {
                            return false;
                        }
                        var fileUser = partIn[0];

                        return string.Equals(fileUser, username, StringComparison.Ordinal);
                    });

                if (exists)
                {
                    Console.Clear();
                    Console.WriteLine("This username already exists, please try again.");
                    return;
                }

                // Append the new user safely; allow readers but prevent writer collisions.
                using (var registerFS = new FileStream(UserFile, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (var registerWriter = new StreamWriter(registerFS))
                {
                    registerWriter.WriteLine($"{username}:{password}");
                    registerWriter.Flush();
                }
            }
            // Simple typewriter + spinner demo
            animations.TypeAni("Creating your account", ConsoleColor.Cyan);
            animations.Spinner(14, ConsoleColor.Cyan);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Clear();
            Console.WriteLine("Account Registered!");
            Console.ResetColor();
        }

        //Private Method to log in as a user
        private void LoginUser()
        {
            Animations animations = new Animations();
            Console.WriteLine("Log in to your account:\n");
            EnumLibrary enums = new EnumLibrary();
            
            int attemptCount = 0;
            bool loggedIn = false;

            // Allow up to 3 LOGIN attempts
            while (!loggedIn && attemptCount < 3) 
            {
                Console.Clear();
                Console.WriteLine("Log in to your account:\n");

                Console.Write("Enter username: ");
                string username = Console.ReadLine();

                // SECURITY CHECK - PREVENT BRUTE FORCE ATTACKS
                if (!SecurityHelper.CheckLoginAttempts(username))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Account temporarily locked due to too many failed attempts.");
                    Console.ResetColor();
                    Thread.Sleep(2000);
                    return; // Exit completely if account is locked
                }

                Console.Write("Enter password: ");
                string password = Console.ReadLine();

                if (!File.Exists(UserFile))
                {
                    Console.WriteLine("No users exist yet. Please register new users first.");
                    Thread.Sleep(2000);
                    return;
                }

                
                bool authenticated = File.ReadLines(UserFile)
                    .AsParallel()
                    .WithDegreeOfParallelism(Math.Max(1, Environment.ProcessorCount - 1))
                    .Any(line =>
                    {
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            return false;
                        }

                        var inTrim = line.Trim();
                        if (inTrim.StartsWith("#"))
                        {
                            return false;
                        }

                        var inParts = inTrim.Split(new[] { ':' }, 2);
                        if (inParts.Length != 2)
                        {
                            return false;
                        }

                        var fileUser = inParts[0];
                        var filePass = inParts[1];
                        return string.Equals(fileUser, username, StringComparison.Ordinal) &&
                               string.Equals(filePass, password, StringComparison.Ordinal);
                    });

                if (authenticated)
                {
                    // SUCCESS - reset attempts and proceed
                    SecurityHelper.ResetAttempts(username);
                    Console.Clear();
                    Console.WriteLine($"Welcome, {username}! Login successful.");
                    animations.LogInSuccessAni(username);
                    loggedIn = true;
                    DisplayMainMenu();
                }
                else
                {
                    // FAILURE - record attempt and shows error message
                    attemptCount++;
                    SecurityHelper.RecordFailedAttempt(username);
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid username or password. Try again.");

                    // Shows the security status to user
                    string status = SecurityHelper.GetSecurityStatus(username);
                    Console.WriteLine($"Security status: {status}");
                    Console.WriteLine($"Attempts remaining: {3 - attemptCount}");

                    Console.ResetColor();

                    // Only pause and continue if we have attempts left
                    if (attemptCount < 3)
                    {
                        Console.WriteLine("\nPress any key to try again...");
                        Console.ReadKey(true);
                    }
                    else
                    {
                        Console.WriteLine("\nToo many failed attempts. Returning to main menu.");
                        Thread.Sleep(2000);
                    }
                }
            }
        }

        //Returns the colors used in the Login Menu
        private static ConsoleColor LogInMenuAccentColor(LogInMenu option)
        {
            switch (option)
            {
                case LogInMenu.LogIn: return ConsoleColor.Cyan;
                case LogInMenu.SignUp: return ConsoleColor.Green;
                case LogInMenu.Exit: return ConsoleColor.Red;
                default: return ConsoleColor.Gray;
            }
        }



        //Displays the Main Menu
        public void DisplayMainMenu()
        {
            var classLibrary = new ClassLibrary();
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;

            while (true)
            {
                // Show a menu for the MainMenu enum.
                // - colorSelector: MainMenuAccentColor (optional)
                // - labelSelector: null => uses [Description] if present, otherwise enum name
                var selected = MenusDisplayHelper.KeyMenuReaderEnumItem<MainMenu>(
                    "Select an Option",
                    MainMenuAccentColor,
                    null
                );

                if (!selected.HasValue)
                {
                    // User pressed Esc
                    return;
                }

                var chosen = selected.Value;

                Console.Clear();
                Console.ResetColor();
                Console.WriteLine();
                // If you added EnumMenuHelpers with GetEnumLabel, you can use it to show the friendly label.
                // Otherwise, chosen.ToString() works too.
                Console.WriteLine("You selected: " + EnumMenuDescriptionHandler.GetEnumLabel(chosen));
                Console.WriteLine();

                switch (chosen)
                {
                    case MainMenu.Clinical:
                        MainMenuDispItems("Loading Clinical Overview.", ConsoleColor.Cyan);
                        Thread.Sleep(1000);
                        DisplayClinicMenu();
                        break;

                    case MainMenu.Pharmacy:
                        MainMenuDispItems("Loading Pharmaceutical Overview.", ConsoleColor.Green);
                        Thread.Sleep(1000);
                        DisplayPharmaMenu();
                        break;

                    case MainMenu.Administrative:
                        MainMenuDispItems("Loading Administrative Overview.", ConsoleColor.Yellow);
                        Thread.Sleep(1000);
                        DisplayAdminMenu();
                        break;

                    case MainMenu.LogOut:
                        MainMenuDispItems("Logging out.", ConsoleColor.Red);
                        Thread.Sleep(1000);
                        Console.Clear();
                        return;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Press any key to return to the menu...");
                Console.ReadKey(true);
            }
        }

        //Method to display which Main Menu Option was clicked
        static void MainMenuDispItems(string menuIn, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(menuIn);
            Console.ResetColor();
        }

        // Colors used by MainMenu Enum's items
        private static ConsoleColor MainMenuAccentColor(MainMenu option)
        {
            switch (option)
            {
                case MainMenu.Clinical: return ConsoleColor.Cyan;
                case MainMenu.Pharmacy: return ConsoleColor.Green;
                case MainMenu.Administrative: return ConsoleColor.Yellow;
                case MainMenu.LogOut: return ConsoleColor.Red;
                default: return ConsoleColor.Gray;
            }
        }



        //Public Method to display the Administrative Menu
        public void DisplayAdminMenu()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            //Sets items as a store for all LogIn Options
            var classLibrary = new ClassLibrary();

            while (true)
            {
                var selected = MenusDisplayHelper.KeyMenuReaderEnumItem<AdminMenu>(
                    "Select an Option",
                    AdminMenuAccentColor,
                    null
                );

                if (!selected.HasValue)
                {
                    // User pressed Esc
                    return;
                }

                var chosen = selected.Value;

                Console.Clear();
                Console.ResetColor();
                Console.WriteLine();
                // If you added EnumMenuHelpers with GetEnumLabel, you can use it to show the friendly label.
                // Otherwise, chosen.ToString() works too.
                Console.WriteLine("You selected: " + EnumMenuDescriptionHandler.GetEnumLabel(chosen));
                Console.WriteLine();

                switch (chosen)
                {
                    case AdminMenu.Employee:
                        AdminEmpManagement();
                        break;

                    case AdminMenu.Inventory:
                        classLibrary.InventoryConfig();
                        break;

                    case AdminMenu.Patient:
                        AdminPatManagement();
                        break;

                    case AdminMenu.BackToMenu:
                        Console.Clear();
                        return;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Press any key to return to the menu...");
                Console.ReadKey(true);
            }
        }
        //Returns the colors used in the Admin Menu
        private static ConsoleColor AdminMenuAccentColor(AdminMenu option)
        {
            switch (option)
            {
                case AdminMenu.Employee: return ConsoleColor.Cyan;
                case AdminMenu.Inventory: return ConsoleColor.Green;
                case AdminMenu.Patient: return ConsoleColor.Blue;
                case AdminMenu.BackToMenu: return ConsoleColor.Red;
                default: return ConsoleColor.Gray;
            }
        }

        //Public Method to display the Pharmacy Menu
        public void DisplayPharmaMenu()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            //Sets items as a store for all LogIn Options
            var classLibrary = new ClassLibrary();

            while (true)
            {
                var selected = MenusDisplayHelper.KeyMenuReaderEnumItem<PharmaMenu>(
                    "Select an Option",
                    PharmaMenuAccentColor,
                    null
                );

                if (!selected.HasValue)
                {
                    // User pressed Esc
                    return;
                }

                var chosen = selected.Value;

                Console.Clear();
                Console.ResetColor();
                Console.WriteLine();
                // If you added EnumMenuHelpers with GetEnumLabel, you can use it to show the friendly label.
                // Otherwise, chosen.ToString() works too.
                Console.WriteLine("You selected: " + EnumMenuDescriptionHandler.GetEnumLabel(chosen));
                Console.WriteLine();

                switch (chosen)
                {
                    case PharmaMenu.Prescription:
                        classLibrary.IssueInvUpdate();
                        break;

                    case PharmaMenu.MediManagement:
                        PharmaMedManagement();
                        break;

                    case PharmaMenu.BackToMenu:
                        Console.Clear();
                        return;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Press any key to return to the menu...");
                Console.ReadKey(true);
            }
        }
        //Returns the colors used in the Pharma Menu
        private static ConsoleColor PharmaMenuAccentColor(PharmaMenu option)
        {
            switch (option)
            {
                case PharmaMenu.Prescription: return ConsoleColor.Cyan;
                case PharmaMenu.MediManagement: return ConsoleColor.Green;
                case PharmaMenu.BackToMenu: return ConsoleColor.Red;
                default: return ConsoleColor.Gray;
            }
        }

        //Public Method to display the Clinic Menu
        public void DisplayClinicMenu()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            //Sets items as a store for all LogIn Options
            var classLibrary = new ClassLibrary();

            while (true)
            {
                var selected = MenusDisplayHelper.KeyMenuReaderEnumItem<ClinicMenu>(
                    "Select an Option",
                    ClinicMenuAccentColor,
                    null
                );

                if (!selected.HasValue)
                {
                    // User pressed Esc
                    return;
                }

                var chosen = selected.Value;

                Console.Clear();
                Console.ResetColor();
                Console.WriteLine();
                // If you added EnumMenuHelpers with GetEnumLabel, you can use it to show the friendly label.
                // Otherwise, chosen.ToString() works too.
                Console.WriteLine("You selected: " + EnumMenuDescriptionHandler.GetEnumLabel(chosen));
                Console.WriteLine();

                switch (chosen)
                {
                    case ClinicMenu.Register:
                        classLibrary.AddPat();
                        break;

                    case ClinicMenu.DisplayInfo:
                        classLibrary.DispPatientInfo();
                        break;
                    case ClinicMenu.CreateAppointment:
                        classLibrary.CreateApp();
                        break;

                    case ClinicMenu.RescheduleAppoinment:
                        classLibrary.RescheduleApp();
                        break;
                    case ClinicMenu.CancelAppointment:
                        classLibrary.CancelApp();
                        break;

                    case ClinicMenu.BackToMenu:
                        Console.Clear();
                        return;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Press any key to return to the menu...");
                Console.ReadKey(true);
            }
        }
        //Returns the colors used in the Clinic Menu
        private static ConsoleColor ClinicMenuAccentColor(ClinicMenu option)
        {
            switch (option)
            {
                case ClinicMenu.Register: return ConsoleColor.Cyan;
                case ClinicMenu.DisplayInfo: return ConsoleColor.Blue;
                case ClinicMenu.CreateAppointment: return ConsoleColor.Green;
                case ClinicMenu.RescheduleAppoinment: return ConsoleColor.DarkGreen;
                case ClinicMenu.CancelAppointment: return ConsoleColor.DarkRed;
                case ClinicMenu.BackToMenu: return ConsoleColor.Red;
                default: return ConsoleColor.Gray;
            }
        }



        //Public Method to display the Administrative Employee Management Menu
        public void AdminEmpManagement()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            //Sets items as a store for all LogIn Options
            var classLibrary = new ClassLibrary();

            while (true)
            {
                var selected = MenusDisplayHelper.KeyMenuReaderEnumItem<AdminEmpManagementMenu>(
                    "Select an Option",
                    AdminEmpManMenuAccentColor,
                    null
                );

                if (!selected.HasValue)
                {
                    // User pressed Esc
                    return;
                }

                var chosen = selected.Value;

                Console.Clear();
                Console.ResetColor();
                Console.WriteLine();
                // If you added EnumMenuHelpers with GetEnumLabel, you can use it to show the friendly label.
                // Otherwise, chosen.ToString() works too.
                Console.WriteLine("You selected: " + EnumMenuDescriptionHandler.GetEnumLabel(chosen));
                Console.WriteLine();

                switch (chosen)
                {
                    case AdminEmpManagementMenu.opt1:
                        classLibrary.AddEmp();
                        break;

                    case AdminEmpManagementMenu.opt2:
                        classLibrary.RemEmp();
                        break;

                    case AdminEmpManagementMenu.BackToMenu:
                        Console.Clear();
                        return;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Press any key to return to the menu...");
                Console.ReadKey(true);
            }
        }
        //Returns the colors used in the Administrative Employee Management Menu
        private static ConsoleColor AdminEmpManMenuAccentColor(AdminEmpManagementMenu option)
        {
            switch (option)
            {
                case AdminEmpManagementMenu.opt1: return ConsoleColor.Cyan;
                case AdminEmpManagementMenu.opt2: return ConsoleColor.Green;
                case AdminEmpManagementMenu.BackToMenu: return ConsoleColor.Red;
                default: return ConsoleColor.Gray;
            }
        }

        //Public Method to display the Administrative Patient Management Menu
        public void AdminPatManagement()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            //Sets items as a store for all LogIn Options
            var classLibrary = new ClassLibrary();

            while (true)
            {
                var selected = MenusDisplayHelper.KeyMenuReaderEnumItem<AdminPatManagementMenu>(
                    "Select an Option",
                    AdminPatManMenuAccentColor,
                    null
                );

                if (!selected.HasValue)
                {
                    // User pressed Esc
                    return;
                }

                var chosen = selected.Value;

                Console.Clear();
                Console.ResetColor();
                Console.WriteLine();
                // If you added EnumMenuHelpers with GetEnumLabel, you can use it to show the friendly label.
                // Otherwise, chosen.ToString() works too.
                Console.WriteLine("You selected: " + EnumMenuDescriptionHandler.GetEnumLabel(chosen));
                Console.WriteLine();

                switch (chosen)
                {
                    case AdminPatManagementMenu.opt1:
                        classLibrary.GenerateContract();
                        break;

                    case AdminPatManagementMenu.opt2:
                        classLibrary.BanPatient();
                        break;

                    case AdminPatManagementMenu.BackToMenu:
                        Console.Clear();
                        return;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Press any key to return to the menu...");
                Console.ReadKey(true);
            }
        }
        //Returns the colors used in the Administrative Patient Management Menu
        private static ConsoleColor AdminPatManMenuAccentColor(AdminPatManagementMenu option)
        {
            switch (option)
            {
                case AdminPatManagementMenu.opt1: return ConsoleColor.Cyan;
                case AdminPatManagementMenu.opt2: return ConsoleColor.Green;
                case AdminPatManagementMenu.BackToMenu: return ConsoleColor.Red;
                default: return ConsoleColor.Gray;
            }
        }

        //Public Method to display the Pharmacy Medication Management Menu
        public void PharmaMedManagement()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            //Sets items as a store for all LogIn Options
            var classLibrary = new ClassLibrary();

            while (true)
            {
                var selected = MenusDisplayHelper.KeyMenuReaderEnumItem<PharmaMedManagementMenu>(
                    "Select an Option",
                    PharmaMedManMenuAccentColor,
                    null
                );

                if (!selected.HasValue)
                {
                    // User pressed Esc
                    return;
                }

                var chosen = selected.Value;

                Console.Clear();
                Console.ResetColor();
                Console.WriteLine();
                // If you added EnumMenuHelpers with GetEnumLabel, you can use it to show the friendly label.
                // Otherwise, chosen.ToString() works too.
                Console.WriteLine("You selected: " + EnumMenuDescriptionHandler.GetEnumLabel(chosen));
                Console.WriteLine();

                switch (chosen)
                {
                    case PharmaMedManagementMenu.opt1:
                        classLibrary.AddMed();
                        break;

                    case PharmaMedManagementMenu.opt2:
                        classLibrary.RemoveMed();
                        break;
                    

                    case PharmaMedManagementMenu.BackToMenu:
                        Console.Clear();
                        return;
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Press any key to return to the menu...");
                Console.ReadKey(true);
            }
        }
        //Returns the colors used in the Pharmacy Medication Management Menu
        private static ConsoleColor PharmaMedManMenuAccentColor(PharmaMedManagementMenu option)
        {
            switch (option)
            {
                case PharmaMedManagementMenu.opt1: return ConsoleColor.Cyan;
                case PharmaMedManagementMenu.opt2: return ConsoleColor.Blue;
                case PharmaMedManagementMenu.BackToMenu: return ConsoleColor.Red;
                default: return ConsoleColor.Gray;
            }
        }




        //Method that returns the descriptions of the enums if there are any.
        public static class EnumMenuDescriptionHandler
        {

            // Returns the display text options for an enum if [Description] present
            public static string GetEnumLabel<TEnum>(TEnum value) where TEnum : struct, Enum
            {
                return GetEnumLabel((Enum)(object)value);
            }

            //Returns default enum items if no [Description] is present.
            public static string GetEnumLabel(Enum value)
            {
                var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
                if (member != null)
                {
                    var desc = member.GetCustomAttribute<DescriptionAttribute>();
                    if (desc != null) return desc.Description;
                }
                return value.ToString();
            }
        }

        //Generic method that takes in enums and helps display menu
        public static class MenusDisplayHelper
        {
            //Takes in a list of inputs/ enums as input
            public static int KeyMenuReader<EnumMenu>(IReadOnlyList<EnumMenu> items, string title, Func<EnumMenu, string> labelSelector = null, Func<EnumMenu, ConsoleColor> colorSelector = null)
            {
                //If the inputs are empty it throws an exception(If no enum is given)
                if (items == null) throw new ArgumentNullException(nameof(items));
                if (items.Count == 0) throw new ArgumentException("Menu must contain at least one item.", nameof(items));

                int selected = 0;

                //Runs the render menu Method
                RenderMenu(items, selected, title, labelSelector, colorSelector);

                while (true)
                {
                    //Puts in a delay when pressing keys, so that there isn't any input overloading
                    if (!Console.KeyAvailable)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    var key = Console.ReadKey(true);    //Reads available keys pressed
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:    //Up arrow Key pressed
                            selected = (selected - 1 + items.Count) % items.Count;
                            RenderMenu(items, selected, title, labelSelector, colorSelector);
                            break;

                        case ConsoleKey.DownArrow:  //Down arrow Key pressed
                            selected = (selected + 1) % items.Count;
                            RenderMenu(items, selected, title, labelSelector, colorSelector);
                            break;

                        case ConsoleKey.Enter:      //Enter Key pressed
                            Console.ResetColor();
                            Console.CursorVisible = false;
                            return selected;

                        case ConsoleKey.Escape:     //Escape key pressed
                            Console.ResetColor();
                            return -1;
                    }
                }
            }

            //Returns the selected menu option if one was selected
            public static inputEnum? KeyMenuReaderEnumItem<inputEnum>(string title, Func<inputEnum, ConsoleColor> colorSelector = null, Func<inputEnum, string> labelSelector = null)           //Takes in the different labels used for the enum
            where inputEnum : struct, Enum    //Runs only if the input Enum is of type Enum
            {

                var items = Enum.GetValues(typeof(inputEnum)).Cast<inputEnum>().ToList();
                Func<inputEnum, string> resolvedLabel = labelSelector ?? EnumMenuDescriptionHandler.GetEnumLabel;

                int outIndex = KeyMenuReader(items, title, resolvedLabel, colorSelector);
                if (outIndex < 0) return null;
                return items[outIndex];
            }

            //Loads the menu for display
            private static void RenderMenu<inEnumMenu>(IReadOnlyList<inEnumMenu> items, int selected, string title, Func<inEnumMenu, string> labelSelector, Func<inEnumMenu, ConsoleColor> colorSelector)
            {
                Console.Clear();

                //Sets the display location for the menus and automatically resizes
                int width = Math.Max(Console.WindowWidth, 40);
                int height = Math.Max(Console.WindowHeight, items.Count + 6);

                bool hasTitle = !string.IsNullOrWhiteSpace(title);      //Checks if there is a title for the Menu
                int totalLines = items.Count + (hasTitle ? 2 : 0);
                int topRow = Math.Max(0, (height - totalLines) / 2);
                int currentRow = topRow;

                if (hasTitle)   //Formats the display location for the title
                {
                    WriteCenteredPlain(title, width, currentRow++, ConsoleColor.White, ConsoleColor.Black);
                    string underline = new string('-', Math.Max(20, Math.Min(title.Length + 4, width / 2)));
                    WriteCenteredPlain(underline, width, currentRow++, ConsoleColor.DarkGray, ConsoleColor.Black);
                }

                for (int i = 0; i < items.Count; i++)
                {
                    bool isSelected = (i == selected);
                    string pointer = isSelected ? "▶ " : "  ";
                    string label = pointer + GetLabel(items[i], labelSelector);

                    var accent = GetAccentColor(items[i], colorSelector);
                    WriteCenteredMenuLine(label, width, currentRow++, isSelected, accent);  //Runs a method to center the highlight color for the enum items
                }

                string hint = "Use Up/Down Arrow to move, Enter to select, Esc to exit";
                int hintRow = Math.Min(topRow + totalLines + 1, height - 1);
                WriteCenteredPlain(hint, width, hintRow, ConsoleColor.DarkGray, ConsoleColor.Black);    //Runs a method to center the unchosen menu options
            }


            //Returns the options to display in the menu
            private static string GetLabel<inEnumMenu>(inEnumMenu item, Func<inEnumMenu, string> labelSelector)
            {
                if (labelSelector != null) return labelSelector(item);

                // Checks if the input items are enums and loads them into the EnumMenuDescriptionHandler method.
                if (item is Enum) return EnumMenuDescriptionHandler.GetEnumLabel((Enum)(object)item);

                return (item != null) ? item.ToString() : string.Empty;
            }

            //Returns the colors used for the menu items
            private static ConsoleColor GetAccentColor<inEnumMenu>(inEnumMenu item, Func<inEnumMenu, ConsoleColor> colorSelector)
            {
                if (colorSelector != null) return colorSelector(item);
                return ConsoleColor.Gray;
            }

            //Writes the unselected menu items in the center of the Console
            private static void WriteCenteredPlain(string text, int width, int row, ConsoleColor foregound, ConsoleColor background)
            {
                Console.ForegroundColor = foregound;
                Console.BackgroundColor = background;

                int left = Math.Max(0, (width - text.Length) / 2);
                left = Math.Min(left, Math.Max(0, Console.BufferWidth - Math.Max(text.Length, 1)));

                if (row >= 0 && row < Console.BufferHeight)
                {
                    Console.SetCursorPosition(left, row);
                    Console.Write(text);
                }

                Console.ResetColor();
            }

            //Highlights the menu items if selected and centers the selected menu item
            private static void WriteCenteredMenuLine(string text, int width, int row, bool isSelected, ConsoleColor accent)
            {
                var foreground = isSelected ? ConsoleColor.Black : accent;
                var background = isSelected ? accent : ConsoleColor.Black;

                Console.ForegroundColor = foreground;
                Console.BackgroundColor = background;

                int left = Math.Max(0, (width - text.Length) / 2);
                left = Math.Min(left, Math.Max(0, Console.BufferWidth - Math.Max(text.Length, 1)));

                if (row >= 0 && row < Console.BufferHeight)
                {
                    Console.SetCursorPosition(left, row);
                    Console.Write(text);
                }

                Console.ResetColor();
            }
        }

    }
}
