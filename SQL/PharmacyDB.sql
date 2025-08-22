IF DB_ID('PharmacyDB') IS NOT NULL
BEGIN
    ALTER DATABASE PharmacyDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE PharmacyDB;
END
GO

CREATE DATABASE PharmacyDB;
GO

USE PharmacyDB;
GO

CREATE TABLE Medicines (
    MedicineID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(120) NOT NULL,
    Category VARCHAR(120) NOT NULL,
    Price DECIMAL(18,2) NOT NULL CHECK (Price >= 0),
    Quantity INT NOT NULL CHECK (Quantity >= 0)
);

CREATE TABLE Sales (
    SaleID INT IDENTITY(1,1) PRIMARY KEY,
    MedicineID INT NOT NULL FOREIGN KEY REFERENCES Medicines(MedicineID),
    QuantitySold INT NOT NULL CHECK (QuantitySold > 0),
    SaleDate DATETIME NOT NULL DEFAULT(GETDATE())
);
GO

-- Stored Procedures
CREATE OR ALTER PROCEDURE AddMedicine
    @Name VARCHAR(120),
    @Category VARCHAR(120),
    @Price DECIMAL(18,2),
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Medicines (Name, Category, Price, Quantity)
    VALUES (@Name, @Category, @Price, @Quantity);
END
GO

CREATE OR ALTER PROCEDURE SearchMedicine
    @SearchTerm VARCHAR(120)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MedicineID, Name, Category, Price, Quantity
    FROM Medicines
    WHERE Name LIKE '%' + @SearchTerm + '%'
       OR Category LIKE '%' + @SearchTerm + '%';
END
GO

CREATE OR ALTER PROCEDURE UpdateStock
    @MedicineID INT,
    @Quantity INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Medicines
    SET Quantity = @Quantity
    WHERE MedicineID = @MedicineID;
END
GO

CREATE OR ALTER PROCEDURE RecordSale
    @MedicineID INT,
    @QuantitySold INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentQty INT;
    SELECT @CurrentQty = Quantity FROM Medicines WHERE MedicineID = @MedicineID;

    IF @CurrentQty IS NULL
    BEGIN
        RAISERROR('Medicine not found.', 16, 1);
        RETURN;
    END

    IF @CurrentQty < @QuantitySold
    BEGIN
        RAISERROR('Insufficient stock.', 16, 1);
        RETURN;
    END

    INSERT INTO Sales (MedicineID, QuantitySold) VALUES (@MedicineID, @QuantitySold);
    UPDATE Medicines SET Quantity = Quantity - @QuantitySold WHERE MedicineID = @MedicineID;
END
GO

CREATE OR ALTER PROCEDURE GetAllMedicines
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MedicineID, Name, Category, Price, Quantity FROM Medicines ORDER BY Name;
END
GO

-- Sample Data
INSERT INTO Medicines (Name, Category, Price, Quantity) VALUES
('Paracetamol 500mg', 'Analgesic', 12.50, 200),
('Amoxicillin 250mg', 'Antibiotic', 25.00, 150),
('Cetirizine 10mg', 'Antihistamine', 18.75, 90);
GO
