using System.Collections.Generic;

namespace PRG281_Project
{
    public interface IPatientManager
    {
        void Contract(string patientID);
        void BanPatient(string patientID);
    }

    public interface IEmployeeManager
    {
        void DisplayInfo(string employeeID);
        void RemoveEmployee(string employeeID);
        void AddEmployee(string employeeID);
    }

    public interface IInventoryManager
    {
        void RemoveMed(string id);
        void AddMed();
    }

    public interface IOrderProcessor
    {
       void GenerateOrderContract();
    }
}


