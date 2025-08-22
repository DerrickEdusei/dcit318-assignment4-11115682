using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using MedicalAppointmentApp.Data;

namespace MedicalAppointmentApp.Forms
{
    public class AppointmentForm : Form
    {
        private ComboBox cboDoctor = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 250, Left = 20, Top = 30 };
        private ComboBox cboPatient = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 250, Left = 20, Top = 90 };
        private DateTimePicker dtp = new DateTimePicker { Width = 250, Left = 20, Top = 150, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" };
        private TextBox txtNotes = new TextBox { Width = 400, Left = 20, Top = 210 };
        private Button btnBook = new Button { Text = "Book Appointment", Width = 200, Left = 20, Top = 260 };

        public AppointmentForm()
        {
            Text = "Book Appointment";
            Width = 500;
            Height = 360;
            Controls.AddRange(new Control[] {
                new Label { Text = "Doctor", Left = 20, Top = 10, AutoSize = true },
                cboDoctor,
                new Label { Text = "Patient", Left = 20, Top = 70, AutoSize = true },
                cboPatient,
                new Label { Text = "Date & Time", Left = 20, Top = 130, AutoSize = true },
                dtp,
                new Label { Text = "Notes", Left = 20, Top = 190, AutoSize = true },
                txtNotes,
                btnBook
            });

            Load += AppointmentForm_Load;
            btnBook.Click += BtnBook_Click;
        }

        private void AppointmentForm_Load(object sender, EventArgs e)
        {
            LoadDoctors();
            LoadPatients();
        }

        private void LoadDoctors()
        {
            try
            {
                using (var conn = Db.GetConnection())
                using (var cmd = new SqlCommand("SELECT DoctorID, FullName FROM Doctors WHERE Availability = 1 ORDER BY FullName", conn))
                {
                    conn.Open();
                    var table = new DataTable();
                    table.Load(cmd.ExecuteReader());
                    cboDoctor.DataSource = table;
                    cboDoctor.DisplayMember = "FullName";
                    cboDoctor.ValueMember = "DoctorID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load doctors.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPatients()
        {
            try
            {
                using (var conn = Db.GetConnection())
                using (var cmd = new SqlCommand("SELECT PatientID, FullName FROM Patients ORDER BY FullName", conn))
                {
                    conn.Open();
                    var table = new DataTable();
                    table.Load(cmd.ExecuteReader());
                    cboPatient.DataSource = table;
                    cboPatient.DisplayMember = "FullName";
                    cboPatient.ValueMember = "PatientID";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load patients.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnBook_Click(object sender, EventArgs e)
        {
            if (cboDoctor.SelectedValue == null || cboPatient.SelectedValue == null)
            {
                MessageBox.Show("Please select both a doctor and a patient.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dtp.Value < DateTime.Now)
            {
                MessageBox.Show("Appointment time cannot be in the past.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    // Check if the doctor is still available
                    using (var checkAvail = new SqlCommand("SELECT Availability FROM Doctors WHERE DoctorID = @DoctorID", conn))
                    {
                        checkAvail.Parameters.AddWithValue("@DoctorID", cboDoctor.SelectedValue);
                        var avail = Convert.ToBoolean(checkAvail.ExecuteScalar() ?? false);
                        if (!avail)
                        {
                            MessageBox.Show("Selected doctor is not available.", "Not Available", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // Simple conflict check: exact datetime match for same doctor
                    using (var checkConflict = new SqlCommand(
                        "SELECT COUNT(*) FROM Appointments WHERE DoctorID=@DoctorID AND CONVERT(VARCHAR(16), AppointmentDate, 120)=CONVERT(VARCHAR(16), @Date, 120)", conn))
                    {
                        checkConflict.Parameters.AddWithValue("@DoctorID", cboDoctor.SelectedValue);
                        checkConflict.Parameters.AddWithValue("@Date", dtp.Value);
                        int conflicts = (int)checkConflict.ExecuteScalar();
                        if (conflicts > 0)
                        {
                            MessageBox.Show("There is already an appointment for this doctor at the selected time.", "Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    using (var insert = new SqlCommand(
                        "INSERT INTO Appointments (DoctorID, PatientID, AppointmentDate, Notes) VALUES (@DoctorID, @PatientID, @AppointmentDate, @Notes)",
                        conn))
                    {
                        insert.Parameters.Add("@DoctorID", System.Data.SqlDbType.Int).Value = cboDoctor.SelectedValue;
                        insert.Parameters.Add("@PatientID", System.Data.SqlDbType.Int).Value = cboPatient.SelectedValue;
                        insert.Parameters.Add("@AppointmentDate", System.Data.SqlDbType.DateTime).Value = dtp.Value;
                        insert.Parameters.Add("@Notes", System.Data.SqlDbType.VarChar, 500).Value = (object)txtNotes.Text ?? DBNull.Value;

                        int rows = insert.ExecuteNonQuery();
                        if (rows > 0)
                            MessageBox.Show("Appointment booked successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("No rows inserted.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to book appointment.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
