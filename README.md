# Hospital Record Management System (C# Console Application)

## Overview
This project modernizes hospital operations by providing a secure, efficient, and accessible record management system. Developed in C#, the console application leverages Object-Oriented Programming (OOP) principles, custom threads, events, interfaces, and exception handling to meet academic and practical learning outcomes. The system simplifies hospital record management, improves data accuracy, and ensures secure access to patient and employee information. Extra features like file persistence and background saving exceed basic requirements.

## Team Members
- Jandre Neethling 
- Mufunwa Muofhe 
- Goitsemang Baloyi 

## Features
- Employee Management: Maintain staff details, roles, permissions, and activity status
- Patient Management: Store patient demographics, encrypted medical records, and appointment tracking
- Medication & Inventory: Track stock levels, dispensing rules, and automated low-stock alerts
- Encounters & Prescriptions: Manage patient visits, doctor notes, and secure prescription creation
- Audit Logging: Track all operations with employee IDs, timestamps, and action details
- Input Validation: Validate national IDs, emails, phone numbers, and medical data integrity
- Data Encryption: Encrypt sensitive patient information at rest and during transmission
- Role-Based Access Control (RBAC): Restrict access based on employee permissions and activity status
- Custom Threads: Background tasks for inventory monitoring, encounter processing, reminders, and system maintenance
- Events & Delegates: Trigger notifications for low stock, appointments, prescriptions, and order completion
- Exception Handling: Custom exceptions for invalid data, unauthorized actions, inventory issues, and scheduling conflicts

## Technologies Used
- Language: C# (.NET Console Application)
- Paradigm: Object-Oriented Programming (inheritance, polymorphism, abstraction, interfaces)
- Data Persistence: JSON file storage with background autosave
- Security: AES encryption, RBAC, audit logging
- Concurrency: Custom threads for background tasks
- Validation: Regex-based input validation

## Project Structure
/Models (People, Employee, Doctor, Nurse, Pharmacist, Administrator, Patient, Encounter, Prescription, Medication, Inventory)  
/Interfaces (IPatientManager, IEmployeeManager, IInventoryManager, IOrderProcessor, INotificationService, IIDGenerate)  
/Services (AuditService, EncryptionService, ValidationService, AuthService, PersistenceService)  
/Threads (InventoryMonitorThread, EncounterProcessingThread, AppointmentReminderThread, SystemMaintenanceThread)  
/Exceptions (InvalidPatientDataException, InsufficientInventoryException, UnauthorizedEmployeeActionException, InvalidMedicationException, AppointmentConflictException)

## Security Highlights
- Audit Logging for compliance and monitoring
- Encryption of patient medical records
- RBAC to enforce role-specific permissions
- Prescription Security: Only licensed doctors can issue prescriptions
- Inventory Access Control: Restrict dispensing based on employee roles

## Exception Handling
- InvalidPatientDataException
- InsufficientInventoryException
- UnauthorizedEmployeeActionException
- InvalidMedicationException
- AppointmentConflictException

## Learning Outcomes
- Apply OOP principles (inheritance, polymorphism, abstraction, interfaces)
- Implement custom threads and events/delegates
- Design secure systems with encryption and RBAC
- Handle errors gracefully with custom exception classes
- Build maintainable, extensible software with layered architecture

## Conclusion
This system demonstrates how modern software practices can transform hospital record management. By combining OOP design, concurrency, security, and persistence, the project delivers a robust and extensible solution for healthcare operations.

## How to Run
1. Clone the repository:  
   `git clone https://github.com/g-baloyi/hospital-management-system.git`
2. Open in Visual Studio or VS Code
3. Build and run the project:  
   `dotnet run`
4. Login with test credentials and explore role-based menus

## License
This project is for academic purposes. Feel free to adapt and extend for learning or demonstration.

