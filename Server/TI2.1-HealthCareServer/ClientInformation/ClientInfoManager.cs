using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using TI2._1_HealthCareServer.ClientInformation;


/// <summary>
/// This class handles all the information about the connected clients with the server.
/// The connected bikes, the logged in patients, which patients are on which bike,
/// which docter is connected etc.
/// </summary>
public static class ClientInfoManager
{
    private static List<Bike> _bikes = new List<Bike>();
    private static List<Patient> _patients = new List<Patient>();

    /// <summary>
    /// Add new bike object to the list of connected bikes aka clients
    /// </summary>
    /// <param name="stream">SslStream that belongs to the right bike aka client</param>
    public static void AddBike(SslStream stream)
    {
        _bikes.Add(new Bike(stream));
    }

    /// <summary>
    /// Removes a bike from the list
    /// </summary>
    /// <param name="stream">SslStream which belongs to the bike</param>
    public static void RemoveBike(SslStream stream)
    {
        var bike = _bikes.Find(b => b.Stream == stream);
        _bikes.Remove(bike);
    }

    /// <summary>
    /// Add a new patient to the list of patients
    /// </summary>
    /// <param name="patientId">Patientid of the patient</param>
    /// <param name="name">Name of the patient</param>
    public static void AddPatient(string patientId, string name)
    {
        _patients.Add(new Patient(patientId, name));
    }

    /// <summary>
    /// Removes a patient from the list
    /// </summary>
    /// <param name="patientId">Patientid of the patient</param>
    public static void RemovePatient(string patientId)
    {
        var patient = _patients.Find(b => b.PatientId == patientId);
        _patients.Remove(patient);
    }

    /// <summary>
    /// Pairs or unpairs a patient to a bike
    /// </summary>
    /// <param name="stream">Which bike</param>
    /// <param name="patientId">Patientid of the patient</param>
    /// <param name="add">Add or remove patient from bike</param>
    public static void SetPatientForBike(SslStream stream, string patientId, bool add)
    {
        var bike = _bikes.Find(b => b.Stream == stream);
        if (add)
        {
            if (bike != null)
            {
                bike.HasPatient = true;
                bike.PatientId = patientId;
            }
        }
        else
        {
            if (bike != null)
            {
                bike.HasPatient = false;
                bike.PatientId = null;
            }
        }
    }

    /// <summary>
    /// Get a list of all bikes that have patients on them
    /// </summary>
    /// <returns>List of bike objects</returns>
    public static List<Bike> GetBikesWithPatients()
    {
        return _bikes.FindAll(b => b.HasPatient);
    }

    /// <summary>
    /// Find a bike with a certain patient on it
    /// </summary>
    /// <param name="patientId">Patientid of the patient</param>
    /// <returns>SslStream of the bike</returns>
    public static SslStream FindBikeWithPatientId(string patientId)
    {
        var bike = _bikes.Find(b => b.PatientId == patientId);
        Console.WriteLine("Found this bike: ");
        Console.WriteLine(bike);
        return bike.Stream;
    }

    /// <summary>
    /// Get a list of patient objects that are on a bike
    /// </summary>
    /// <returns>List with patient objects</returns>
    public static List<Patient> GetPatientsOnBikes()
    {
        var patientsOnBikes = new List<Patient>();
        foreach (var bike in _bikes)
        {
            if (bike.HasPatient)
            {
                var patient = _patients.Find(p => p.PatientId == bike.PatientId);
                if (patient != null)
                {
                    patientsOnBikes.Add(patient);
                }
            }
        }

        return patientsOnBikes;
    }

    /// <summary>
    /// Sets if a patient has a session running or not
    /// </summary>
    /// <param name="patientId">Patientid of the patient</param>
    /// <param name="sessionStarted">If the session has started or not</param>
    public static void SetSessionStartedStatusForPatient(string patientId, bool sessionStarted)
    {
        var patient = _patients.Find(p => p.PatientId == patientId);
        if (patient != null)
        {
            patient.SessionStarted = sessionStarted;
        }
    }

    /// <summary>
    /// Get all patients that have running sessions
    /// </summary>
    /// <returns>List of patient objects</returns>
    public static List<Patient> GetPatientsWithActiveSessions()
    {
        return _patients.FindAll(p => p.SessionStarted);
    }

    /// <summary>
    /// Get a name of the patient
    /// </summary>
    /// <param name="patientId">Patientid of the patient</param>
    /// <returns></returns>
    public static string GetPatientNameById(string patientId)
    {
        var patient = _patients.Find(p => p.PatientId == patientId);
        if (patient != null)
        {
            return patient.Name;
        }

        return string.Empty;
    }
}