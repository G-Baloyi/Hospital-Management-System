using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRG281_Project
{
   /* 
    public delegate void StockAlert(string message);
    internal delegate void AppointmentHandler(Patient patient,string details);
    internal delegate void Prescription(Patient patient,  Medication med);
    public delegate void CompletedOrders(Encounter encounter );*/
      

    //Event to Display a warning if a banned patient is trying to create an appointment
    public sealed class BannedPatientDetectedEventArgs : EventArgs
    {
        public BannedPatientDetectedEventArgs(string patientId, DateTimeOffset detectedAt)
        {
            PatientId = patientId ?? throw new ArgumentNullException(nameof(patientId));    //If the event is called without an input it throws an error
            DetectedAt = detectedAt;
        }

        public string PatientId { get; }
        public DateTimeOffset DetectedAt { get; }   //At what time a banned patient tried do log in.
    }

    //Monitors if any banned patients try to do anything
    public class PatientBanMonitor
    {
        DataStore DataStore { get; set; }
        public event EventHandler<BannedPatientDetectedEventArgs> BannedPatientDetected;    //Runs the event

        public bool TryDetect(string patientId)     //Returs true if a banned patient is detected
        {
            if (string.IsNullOrWhiteSpace(patientId))   
                return false;

            //Checks in the BannedPatientIDs HashSet if the patientID appears in the list
            if (DataStore.BannedPatientIDs.Contains(patientId))
            {
                OnBannedPatientDetected(
                    new BannedPatientDetectedEventArgs(patientId, DateTimeOffset.UtcNow));
                return true;
            }

            return false;
        }

        protected virtual void OnBannedPatientDetected(BannedPatientDetectedEventArgs e)
        {
            BannedPatientDetected?.Invoke(this, e);
        }
    }
}

