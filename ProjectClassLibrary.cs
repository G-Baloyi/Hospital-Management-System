using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PRG281_Project
{
    internal class ClassLibrary
    {

        //Ensures that the dictionaries stored in the data store do not clear every time you call a function via the HospitalConsoleApp
        public static class KeepDataState
        {
            public static readonly DataStore myDataStore = new DataStore();
            public static readonly HospitalShell App = new HospitalShell(myDataStore);
        }


        //Private Method to Exit the program
        public void ExitProgram()
        {
            Environment.Exit(0);
        }

        //Calls the Inventory Config Method from Enum Library
        public void InventoryConfig()
        {
            KeepDataState.App.InventoryConfig();
        }

        //Calls the Add Employee Method from Enum Library
        public void AddEmp()
        {
            KeepDataState.App.AddEmp();
        }

        //Calls the Remove Employee Method from Enum Library
        public void RemEmp()
        {
            KeepDataState.App.RemEmp();
        }

        //Calls the Generate Patient Contract Method from Enum Library
        public void GenerateContract()
        {
            KeepDataState.App.GenerateContract();
        }

        //Calls the Ban Patient Method from Enum Library
        public void BanPatient()
        {
            KeepDataState.App.BanPatient();
        }

        //Calls the Issue Ineventory Upddate Method from Enum Library
        public void IssueInvUpdate()
        {
            KeepDataState.App.IssueInvUpdate();
        }

        //Calls the Add Medicine Method from Enum Library
        public void AddMed()
        {
            KeepDataState.App.AddMed();
        }

        //Calls the Remove Medicine Method from Enum Library
        public void RemoveMed()
        {
            KeepDataState.App.RemoveMed();
        }

        //Calls the Add Patient Method from Enum Library
        public void AddPat()
        {
            KeepDataState.App.AddPat();
        }

        //Calls the Display Patient Info Method from Enum Library
        public void DispPatientInfo()
        {
            KeepDataState.App.DispPatientInfo();
        }

        //Calls the Create Application Method from Enum Library
        public void CreateApp()
        {
            KeepDataState.App.CreateApp();
        }

        //Calls the Reschedule Application Method from Enum Library
        public void RescheduleApp()
        {
            KeepDataState.App.RescheduleApp();
        }

        //Calls the Cancel Application Method from Enum Library
        public void CancelApp()
        {
            KeepDataState.App.CancelApp();
        }
    }

    //People abstract Class
    public abstract class People
    {
        public string NationalID
        { get; }
        public string FirstName
        { get; set; }
        public string LastName
        { get; set; }
        public string PhoneNumber
        { get; set; }
        public string Email
        { get; set; }
        public string Street
        { get; set; }
        public string Suburb
        { get; set; }
        public string City
        { get; set; }
        public string PostCode
        { get; set; }

        public People(string nationalID, string firstName, string lastName,
                      string phoneNumber, string email, string street,
                      string suburb, string city, string postCode)
        {
            NationalID = nationalID;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
            Street = street;
            Suburb = suburb;
            City = city;
            PostCode = postCode;
        }
    }

    //Patient Abstract Class
    public class Patient : People
    {
        public string PatientID
        { get; }
        public string MedicalRecordNumber
        { get; set; }
        public bool IsCovered
        { get; set; }
        public string Coverage
        { get; set; }

        private int numOfEncounters
        { get; set; }

        public bool ActiveAppointment
        { get; set; }

        public Patient(string patientID, string nationalID, string firstName, string lastName, string phoneNumber, string email, string street, string suburb,
                       string city, string postCode, string medicalRecordNumber, bool isCovered, string coverage)
                       : base(nationalID, firstName, lastName, phoneNumber, email, street, suburb, city, postCode)
        {
            PatientID = patientID;
            MedicalRecordNumber = medicalRecordNumber;
            IsCovered = isCovered;
            Coverage = coverage;
        }

        //Adds to the users number of encounters
        public void AddEncounter()
        {
            numOfEncounters++;
        }

        public void CreateAppointment(string employeeID, DateTime appDateTime, string appStatus)
        {
            ActiveAppointment = true;
            AddEncounter();
        }

        public void RescheduleAppointment(string employeeID, DateTime newDateTime, string newStatus, string reason)
        {
            if (!ActiveAppointment) throw new InvalidOperationException("No active appointment to reschedule.");
            ActiveAppointment = true;
        }

        public void CancelAppointment(string employeeID, DateTime cancelDateTime)
        {
            if (!ActiveAppointment) throw new InvalidOperationException("No active appointment to cancel.");
            ActiveAppointment = false;
        }
    }

    //Employee Abstract Class
    public abstract class Employee
    {
        public string EmployeeID
        { get; }
        public string Permissions
        { get; set; }
        public bool Activity
        { get; set; }
        protected Employee(string employeeID, string permissions)
        {
            EmployeeID = employeeID;
            Permissions = permissions;
            Activity = true;
        }
    }

    //Doctor class inheriting from Employee
    public class Doctor : Employee
    {
        public string LicenseNumber
        { get; }
        public string Specialty
        { get; }

        public Doctor(string employeeID, string permissions, string licenseNumber, string specialty) : base(employeeID, permissions)
        {
            LicenseNumber = licenseNumber;
            Specialty = specialty;
        }
    }

    //Nurse class inheriting from Employee
    public class Nurse : Employee
    {
        public string RegistrationNumber
        { get; }
        public string Specialty
        { get; set; }

        public Nurse(string employeeID, string permissions, string registrationNumber, string specialty) : base(employeeID, permissions)
        {
            RegistrationNumber = registrationNumber;
            Specialty = specialty;
        }
    }

    //Pharmacist class inheriting from Employee
    public class Pharmacist : Employee
    {
        public string RegistrationNumber
        { get; }

        public Pharmacist(string employeeID, string permissions, string registrationNumber) : base(employeeID, permissions)
        {
            RegistrationNumber = registrationNumber;
        }
    }

    //Admin class inheriting from Employee
    public class Administrator : Employee
    {
        public string Department
        { get; }
        public string Role
        { get; }

        public Administrator(string employeeID, string permissions, string department, string role) : base(employeeID, permissions)
        {
            Department = department;
            Role = role;
        }
    }

    //Encounter Class
    public class Encounter
    {
        public int EncounterID
        { get; }
        public string EmployeeID
        { get; }
        public DateTime EncounterStartDate
        { get; set; }
        public DateTime EncounterEndDate
        { get; set; }
        public string Priority
        { get; set; }
        public bool OrderStatus
        { get; set; }
        public int Quantity
        { get; set; }
        public int Dosage
        { get; set; }
        public bool PresStatus
        { get; set; }

        //Adds an encounter 
        public Encounter(int encounterID, string employeeID, DateTime start, DateTime end)
        {
            EncounterID = encounterID;
            EmployeeID = employeeID;
            EncounterStartDate = start;
            EncounterEndDate = end;
        }

        //Adds a new order if need be
        public void AddOrder(string orderID, string employeeID, string priority, string testCode, DateTime orderDate, bool orderStatus)
        {
            Priority = priority;
            OrderStatus = orderStatus;
        }

        //Adds a prescription to patients
        public void CreatePrescription(string prescriptionID, string employeeID, string medicationID, int quantity, int dosage,
                                       DateTime presStart, DateTime presEnd, bool presStatus)
        {
            Quantity = quantity;
            Dosage = dosage;
            PresStatus = presStatus;
        }
    }

    //Medication Class
    public class Medication
    {
        public string MedID
        { get; }
        public string MedName
        { get; set; }
        public string AtcCode
        { get; set; }
        public string Strength
        { get; set; }
        public string TypeOfMed
        { get; set; }
        public bool Activity
        { get; set; }

        public Medication(string medID, string medName, string atcCode, string strength, string typeOfMed)
        {
            MedID = medID;
            MedName = medName;
            AtcCode = atcCode;
            Strength = strength;
            TypeOfMed = typeOfMed;
            Activity = true;
        }
    }

    //Inventory Class
    public class Inventory
    {
        public string InventoryID
        { get; }
        public int QuantityInStock
        { get; set; }
        public DateTime LastUpdated
        { get; set; }
        public int MaxAllowedSold
        { get; set; }
        public int MinBeforeWarn
        { get; set; }
        public string PlaceStored
        { get; set; }
        public bool Status
        { get; set; }

        public Inventory(string inventoryID, int quantityInStock, DateTime lastUpdated,
                         int maxAllowedSold, int minBeforeWarn, string placeStored, bool status)
        {
            InventoryID = inventoryID;
            QuantityInStock = quantityInStock;
            LastUpdated = lastUpdated;
            MaxAllowedSold = maxAllowedSold;
            MinBeforeWarn = minBeforeWarn;
            PlaceStored = placeStored;
            Status = status;
        }

        //Sends a warning if the stock is low
        public bool LowStockWarn(int quantityInStock, int minBeforeWarn)
        {
            return quantityInStock <= minBeforeWarn;
        }
    }



    //Class that stores all storage modules
    public class DataStore
    {
        public readonly Dictionary<string, Patient> Patients = new Dictionary<string, Patient>();
        public readonly Dictionary<string, Employee> Employees = new Dictionary<string, Employee>();
        public readonly Dictionary<string, Medication> Medications = new Dictionary<string, Medication>();
        public readonly Dictionary<string, Inventory> Inventories = new Dictionary<string, Inventory>();

        public HashSet<string> BannedPatientIDs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    //The main hospital class that links all other classes
    public class HospitalShell : IPatientManager, IEmployeeManager, IInventoryManager, IOrderProcessor
    {
        private readonly DataStore myDataStore;

        public HospitalShell(DataStore inDataStore)
        {
            myDataStore = inDataStore;
        }

        //InventoryConfig Method
        public void InventoryConfig()
        {
            Console.Write("Inventory ID to configure: ");
            string id = Console.ReadLine();

            Inventory inv;
            if (!myDataStore.Inventories.TryGetValue(id, out inv))
            {
                Console.WriteLine("Inventory not found. Creating a new record.");
                inv = new Inventory(id, 0, DateTime.UtcNow, 100, 10, "Main Store", true);
                myDataStore.Inventories[id] = inv;
            }

            Console.Write("New MinBeforeWarn (int): ");
            int min; int.TryParse(Console.ReadLine(), out min);
            inv.MinBeforeWarn = min;

            Console.Write("New MaxAllowedSold (int): ");
            int max; int.TryParse(Console.ReadLine(), out max);
            inv.MaxAllowedSold = max;

            Console.Write("New PlaceStored: ");
            inv.PlaceStored = Console.ReadLine();

            Console.Write("Set Status (active=true/inactive=false): ");
            bool status; bool.TryParse(Console.ReadLine(), out status);
            inv.Status = status;

            inv.LastUpdated = DateTime.UtcNow;

            Console.WriteLine("Inventory configuration updated.");
        }

        //Add Employee Method
        public void AddEmp()
        {
            Console.Write("EmployeeID: ");
            string id = Console.ReadLine();
            if (myDataStore.Employees.ContainsKey(id))
            {
                Console.WriteLine("Employee already exists.");
                return;
            }

            Console.Write("Type (Doctor/Nurse/Pharmacist/Administrator): ");
            string kind = (Console.ReadLine() ?? "").Trim();
            Console.Write("Permissions: ");
            string perms = Console.ReadLine();

            Employee emp = null;
            if (string.Equals(kind, "Doctor", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("LicenseNumber: ");
                string lic = Console.ReadLine();
                Console.Write("Specialty: ");
                string spec = Console.ReadLine();
                emp = new Doctor(id, perms, lic, spec);
            }
            else if (string.Equals(kind, "Nurse", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("RegistrationNumber: ");
                string reg = Console.ReadLine();
                Console.Write("Specialty: ");
                string spec = Console.ReadLine();
                emp = new Nurse(id, perms, reg, spec);
            }
            else if (string.Equals(kind, "Pharmacist", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write("RegistrationNumber: ");
                string reg = Console.ReadLine();
                emp = new Pharmacist(id, perms, reg);
            }
            else
            {
                Console.Write("Department: ");
                string dep = Console.ReadLine();
                Console.Write("Role: ");
                string role = Console.ReadLine();
                emp = new Administrator(id, perms, dep, role);
            }

            myDataStore.Employees[id] = emp;
            Console.WriteLine("Employee added.");
        }

        //Remove Employee Method
        public void RemEmp()
        {
            Console.Write("EmployeeID to remove: ");
            string id = Console.ReadLine();
            if (myDataStore.Employees.Remove(id))
            {
                Console.WriteLine("Employee removed.");
            }
            else
            {
                Console.WriteLine("Employee not found.");
            }
        }
        //Generate Waver Contract for Patients method
        public void GenerateContract()
        {
            Console.Write("PatientID to generate contract for: ");
            string patientId = Console.ReadLine();
            Contract(patientId); // Delegates to IPatientManager implementation
        }

        //Ban Patient Methodd
        public void BanPatient()
        {
            Console.Write("PatientID to ban: ");
            string patientId = Console.ReadLine();
            BanPatient(patientId); // Delegates to IPatientManager implementation
        }

        //Issue Inventory Update Method
        public void IssueInvUpdate()
        {
            Console.Write("Inventory ID: ");
            string invId = Console.ReadLine();
            Inventory inv;
            if (!myDataStore.Inventories.TryGetValue(invId, out inv))
            {
                Console.WriteLine("Inventory not found.");
                return;
            }

            Console.Write("Change in stock (negative to consume, positive to add): ");
            int delta;
            int.TryParse(Console.ReadLine(), out delta);

            inv.QuantityInStock += delta;
            inv.LastUpdated = DateTime.UtcNow;

            if (inv.LowStockWarn(inv.QuantityInStock, inv.MinBeforeWarn))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"WARNING: Low stock for '{inv.InventoryID}'. Current: {inv.QuantityInStock}, Threshold: {inv.MinBeforeWarn}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"Inventory '{inv.InventoryID}' updated. New qty: {inv.QuantityInStock}");
            }
        }

        //Add Medicine method
        public void AddMed()
        {
            Console.Write("Medication ID: ");
            string medId = Console.ReadLine();
            if (!myDataStore.Medications.ContainsKey(medId))
            {
                Console.Write("Name: ");
                string name = Console.ReadLine();
                Console.Write("ATC Code: ");
                string atc = Console.ReadLine();
                Console.Write("Strength: ");
                string strength = Console.ReadLine();
                Console.Write("TypeOfMed: ");
                string type = Console.ReadLine();

                var med = new Medication(medId, name, atc, strength, type);
                myDataStore.Medications[medId] = med;
                Console.WriteLine("Medication added to list.");
            }
            else
            {
                Console.WriteLine("Medication already exists; skipping list add.");
            }

            Console.Write("Inventory ID to link/create: ");
            string invId = Console.ReadLine();
            if (!myDataStore.Inventories.ContainsKey(invId))
            {
                Console.Write("Initial Quantity: ");
                int qty; int.TryParse(Console.ReadLine(), out qty);
                var inv = new Inventory(invId, qty, DateTime.UtcNow, 100, 10, "Main Store", true);
                myDataStore.Inventories[invId] = inv;
                Console.WriteLine("Inventory created for medication.");
            }
            else
            {
                Console.WriteLine("Inventory already exists for this ID.");
            }
        }

        //Remove Medicine Method
        public void RemoveMed()
        {
            Console.Write("Medication ID to remove: ");
            string medId = Console.ReadLine();

            if (myDataStore.Medications.Remove(medId))
                Console.WriteLine("Medication removed from list.");
            else
                Console.WriteLine("Medication not found in list.");

            Console.Write("Inventory ID to remove (if any): ");
            string invId = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(invId))
            {
                if (myDataStore.Inventories.Remove(invId))
                    Console.WriteLine("Inventory record removed.");
                else
                    Console.WriteLine("Inventory not found.");
            }
        }


        //Add patient method
        public void AddPat()
        {
            PatientBanMonitor patientBanMonitor = new PatientBanMonitor();      
            Console.Write("PatientID: ");
            string pid = Console.ReadLine();
            
            Console.Write("NationalID: ");
            string natID = Console.ReadLine();
            Console.Write("FirstName: ");
            string firstName = Console.ReadLine();
            Console.Write("LastName: ");
            string lastName = Console.ReadLine();
            Console.Write("Phone: ");
            string phone = Console.ReadLine();
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Street: ");
            string street = Console.ReadLine();
            Console.Write("Suburb: ");
            string suburb = Console.ReadLine();
            Console.Write("City: ");
            string city = Console.ReadLine();
            Console.Write("PostCode: ");
            string pc = Console.ReadLine();

            Console.Write("MedicalRecordNumber: ");
            string mrn = Console.ReadLine();
            Console.Write("IsCovered (true/false): ");
            bool covered; bool.TryParse(Console.ReadLine(), out covered);
            Console.Write("Coverage (plan name/notes): ");
            string coverage = Console.ReadLine();

            var patients = new Patient(pid, natID, firstName, lastName, phone, email, street, suburb, city, pc, mrn, covered, coverage);
            myDataStore.Patients[pid] = patients;
            Console.WriteLine("Patient registered.");
        }

        //Display Patient Info Method
        public void DispPatientInfo()
        {
            Console.Write("PatientID: ");
            string pid = Console.ReadLine();
            Patient myPatient;
            if (!myDataStore.Patients.TryGetValue(pid, out myPatient))
            {
                Console.WriteLine("Patient not found.");
                return;
            }

            DisplayPatient(myPatient);
        }


        //Create appointment Method
        public void CreateApp()
        {
            Console.Write("PatientID: ");
            string pid = Console.ReadLine();


            Console.Write("EmployeeID (who schedules): ");
            string eid = Console.ReadLine();
            Console.Write("Appointment Date (yyyy-MM-dd): ");
            DateTime date; DateTime.TryParse(Console.ReadLine(), out date);
            Console.Write("Appointment Time (HH:mm): ");
            TimeSpan time; TimeSpan.TryParse(Console.ReadLine(), out time);
            var dt = date.Date + time;

            Console.Write("Status (myEmployees.g., Scheduled): ");
            string status = Console.ReadLine();

            Patient myPatient;
            if (!myDataStore.Patients.TryGetValue(pid, out myPatient))
            {
                Console.WriteLine("Patient not found.");
                return;
            }

            myPatient.CreateAppointment(eid, dt, status);
            // Log encounter (very simplified)
            var enc = new Encounter(new Random().Next(100000, 999999), eid, DateTime.UtcNow, DateTime.UtcNow);
            Console.WriteLine($"Appointment created and encounter logged (ID {enc.EncounterID}).");
        }

        //Reschedule Appontment Method
        public void RescheduleApp()
        {
            Console.Write("PatientID: ");
            string myPatientID = Console.ReadLine();
            Console.Write("EmployeeID (who reschedules): ");
            string myEmpID = Console.ReadLine();
            Console.Write("New Date (yyyy-MM-dd): ");
            DateTime date; DateTime.TryParse(Console.ReadLine(), out date);
            Console.Write("New Time (HH:mm): ");
            TimeSpan time; TimeSpan.TryParse(Console.ReadLine(), out time);
            var dt = date.Date + time;

            Console.Write("New Status (myEmployees.g., Rescheduled): ");
            string status = Console.ReadLine();
            Console.Write("Reason: ");
            string reason = Console.ReadLine();

            Patient myPatients;
            if (!myDataStore.Patients.TryGetValue(myPatientID, out myPatients))
            {
                Console.WriteLine("Patient not found.");
                return;
            }

            try
            {
                myPatients.RescheduleAppointment(myEmpID, dt, status, reason);
                Console.WriteLine("Appointment rescheduled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        //Cancel Appointment Method
        public void CancelApp()
        {
            Console.Write("PatientID: ");
            string myPatientID = Console.ReadLine();
            Console.Write("EmployeeID (who cancels): ");
            string eid = Console.ReadLine();

            Patient myPatients;
            if (!myDataStore.Patients.TryGetValue(myPatientID, out myPatients))
            {
                Console.WriteLine("Patient not found.");
                return;
            }

            try
            {
                myPatients.CancelAppointment(eid, DateTime.UtcNow);
                Console.WriteLine("Appointment canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        
        //Generate Patient Contract Method
        public void Contract(string patientID)
        {
            Patient myPatients;
            if (!myDataStore.Patients.TryGetValue(patientID, out myPatients))
            {
                Console.WriteLine("Patient not found.");
                return;
            }

            // Extremely simplified "contract"
            Console.WriteLine("---- PATIENT SERVICE CONTRACT ----");
            Console.WriteLine($"Patient: {myPatients.FirstName} {myPatients.LastName} ({myPatients.PatientID})");
            Console.WriteLine($"Coverage: {(myPatients.IsCovered ? myPatients.Coverage : "None")}");
            Console.WriteLine($"Terms: Patient agrees to hospital policies and consents to treatment.");
            Console.WriteLine("----------------------------------");
        }

        //Ban Patient Method
        public void BanPatient(string patientID)
        {
            if (myDataStore.Patients.ContainsKey(patientID))
            {
                myDataStore.BannedPatientIDs.Add(patientID);
                Console.WriteLine($"Patient {patientID} has been banned.");
            }
            else
            {
                Console.WriteLine("Patient not found.");
            }
        }

        //Display Patient method
        public void DisplayInfo(string employeeID)
        {
            Employee myEmployees;
            if (myDataStore.Employees.TryGetValue(employeeID, out myEmployees))
            {
                Console.WriteLine($"Employee {myEmployees.EmployeeID} | Permissions: {myEmployees.Permissions} | Active: {myEmployees.Activity}");
                Console.WriteLine($"Type: {myEmployees.GetType().Name}");
            }
            else
            {
                Console.WriteLine("Employee not found.");
            }
        }

        //Remove Employee Method
        public void RemoveEmployee(string employeeID)
        {
            if (myDataStore.Employees.Remove(employeeID))
                Console.WriteLine("Employee removed.");
            else
                Console.WriteLine("Employee not found.");
        }

        //Add Employee Method
        public void AddEmployee(string employeeID)
        {
            // Provided for completeness; the console-driven AddEmp() handles prompting.
            if (myDataStore.Employees.ContainsKey(employeeID))
            {
                Console.WriteLine("Employee already exists.");
                return;
            }
            myDataStore.Employees[employeeID] = new Administrator(employeeID, "Admin", "Administration", "Administrator");
            Console.WriteLine("Default Administrator created.");
        }

        //Remove medicine Method
        public void RemoveMed(string id)
        {
            if (myDataStore.Medications.Remove(id))
                Console.WriteLine("Medication removed.");
            else
                Console.WriteLine("Medication not found.");
        }

        //Generate Order contract Method
        public void GenerateOrderContract()
        {
            Console.WriteLine("---- ORDER CONTRACT ----");
            Console.WriteLine("The hospital agrees to fulfill orders subject to stock availability and patient eligibility.");
            Console.WriteLine("------------------------");
        }

        //Patient Display Method
        private static void DisplayPatient(Patient myPatients)
        {
            Console.WriteLine($"PatientID: {myPatients.PatientID}");
            Console.WriteLine($"Name: {myPatients.FirstName} {myPatients.LastName}");
            Console.WriteLine($"NationalID: {myPatients.NationalID}");
            Console.WriteLine($"MRN: {myPatients.MedicalRecordNumber}");
            Console.WriteLine($"Coverage: {(myPatients.IsCovered ? myPatients.Coverage : "None")}");
            Console.WriteLine($"Active Appointment: {myPatients.ActiveAppointment}");
            Console.WriteLine($"Contact: {myPatients.PhoneNumber} | {myPatients.Email}");
            Console.WriteLine($"Address: {myPatients.Street}, {myPatients.Suburb}, {myPatients.City}, {myPatients.PostCode}");
        }
    }
}
