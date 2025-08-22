-- Drop and create database
IF DB_ID('MedicalDB') IS NOT NULL
BEGIN
    ALTER DATABASE MedicalDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MedicalDB;
END
GO

CREATE DATABASE MedicalDB;
GO

USE MedicalDB;
GO

-- Tables
CREATE TABLE Doctors (
    DoctorID INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Specialty VARCHAR(100) NOT NULL,
    Availability BIT NOT NULL DEFAULT(1)
);

CREATE TABLE Patients (
    PatientID INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE Appointments (
    AppointmentID INT IDENTITY(1,1) PRIMARY KEY,
    DoctorID INT NOT NULL FOREIGN KEY REFERENCES Doctors(DoctorID),
    PatientID INT NOT NULL FOREIGN KEY REFERENCES Patients(PatientID),
    AppointmentDate DATETIME NOT NULL,
    Notes VARCHAR(500) NULL
);

-- Sample Data
INSERT INTO Doctors (FullName, Specialty, Availability) VALUES
('Dr. Ama Boateng', 'Cardiology', 1),
('Dr. Kwesi Mensah', 'Dermatology', 1),
('Dr. Yaa Owusu', 'Pediatrics', 0),
('Dr. Kojo Asare', 'General Medicine', 1);

INSERT INTO Patients (FullName, Email) VALUES
('Emmanuel Dodoo', 'emmanuel@example.com'),
('Janice Owusu', 'janice@example.com'),
('Abena Adjei', 'abena@example.com');

-- Helpful index for conflicts
CREATE INDEX IX_Appointments_DoctorDate ON Appointments(DoctorID, AppointmentDate);
GO
