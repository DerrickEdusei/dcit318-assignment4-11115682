using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using MedicalAppointmentApp.Data;

namespace MedicalAppointmentApp.Forms
{
    public class ManageAppointmentsForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Top, Height = 250, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private Button btnRefresh = new Button { Text = "Refresh", Left = 10, Top = 260, Width = 100 };
        private Button btnUpdate = new Button { Text = "Update Date", Left = 120, Top = 260, Width = 120 };
        private Button btnDelete = new Button { Text = "Delete", Left = 250, Top = 260, Width = 100 };
        private DateTimePicker dtpNew = new DateTimePicker { Left = 360, Top = 265, Width = 200, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm" };

        private DataSet ds = new DataSet();

        public ManageAppointmentsForm()
        {
            Text = "Manage Appointments";
            Width = 900;
            Height = 360;

            Controls.Add(grid);
            Controls.Add(btnRefresh);
            Controls.Add(btnUpdate);
            Controls.Add(btnDelete);
            Controls.Add(dtpNew);

            btnRefresh.Click += (s, e) => LoadAppointments();
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;

            Load += (s, e) => LoadAppointments();
        }

        private void LoadAppointments()
        {
            try
            {
                using (var conn = Db.GetConnection())
                using (var da = new SqlDataAdapter(
                    @"SELECT A.AppointmentID, D.FullName AS Doctor, P.FullName AS Patient, A.AppointmentDate, A.Notes, A.DoctorID, A.PatientID
                      FROM Appointments A
                      JOIN Doctors D ON A.DoctorID = D.DoctorID
                      JOIN Patients P ON A.PatientID = P.PatientID
                      ORDER BY A.AppointmentDate DESC", conn))
                {
                    ds.Reset();
                    da.Fill(ds, "Appointments");
                    grid.DataSource = ds.Tables["Appointments"];
                    grid.Columns["DoctorID"].Visible = false;
                    grid.Columns["PatientID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load appointments.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Select an appointment first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var id = Convert.ToInt32(grid.CurrentRow.Cells["AppointmentID"].Value);
            var newDate = dtpNew.Value;

            try
            {
                using (var conn = Db.GetConnection())
                using (var cmd = new SqlCommand("UPDATE Appointments SET AppointmentDate=@Date WHERE AppointmentID=@ID", conn))
                {
                    cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = newDate;
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAppointments();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null)
            {
                MessageBox.Show("Select an appointment first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var id = Convert.ToInt32(grid.CurrentRow.Cells["AppointmentID"].Value);
            if (MessageBox.Show("Delete this appointment?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using (var conn = Db.GetConnection())
                using (var cmd = new SqlCommand("DELETE FROM Appointments WHERE AppointmentID=@ID", conn))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadAppointments();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to delete.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
