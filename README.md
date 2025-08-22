# DCIT 318 – Assignment 4 (WinForms + SQL Server)

This solution contains **two WinForms projects** and **SQL scripts** for the databases.

## Projects
1. **MedicalAppointmentApp** – Question 1 (ADO.NET with readers/adapters, appointments UI)
2. **PharmacyInventoryApp** – Question 2 (Stored Procedures; inventory UI)

## Databases (SQL Server)
Scripts are in the `SQL/` folder.
- `MedicalDB.sql`
- `PharmacyDB.sql`

> **Default connection** is LocalDB: `(localdb)\MSSQLLocalDB`.
> You can change the connection strings in each project's `App.config` if needed.

## Setup Steps
1. Open **SQL Server Management Studio** (SSMS) and run the scripts in `SQL/` (in order).
2. Open the solution: `DCIT318_Assignment4_Solution.sln` in Visual Studio.
3. Set each project as **Startup Project** (one at a time) and press **F5** to run.
4. For **MedicalAppointmentApp**:
   - Use **Doctors** and **Patients** sample data provided by the script.
   - Book, search/filter and manage appointments.
5. For **PharmacyInventoryApp**:
   - Add medicines, search by term, update stock, record sales, and view all medicines—**all via stored procedures**.

## Notes for Grading
- Uses **SqlConnection, SqlCommand, SqlDataReader, SqlDataAdapter, DataSet** appropriately.
- Uses **parameters** and **CommandType.StoredProcedure** where required.
- UI uses **TextBox, ComboBox, Button, DateTimePicker, DataGridView** and common events.
- All operations wrapped in **try/catch** with user-friendly **MessageBox** messages.
- Connections closed in **finally** blocks (or `using` statements).

Good luck!
