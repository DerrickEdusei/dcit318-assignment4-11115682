using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using MedicalAppointmentApp.Data;

namespace MedicalAppointmentApp.Forms
{
    public class DoctorListForm : Form
    {
        private DataGridView grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

        public DoctorListForm()
        {
            Text = "Doctors";
            Width = 700;
            Height = 400;
            Controls.Add(grid);
            Load += DoctorListForm_Load;
        }

        private void DoctorListForm_Load(object sender, EventArgs e)
        {
            try
            {
                using (var conn = Db.GetConnection())
                using (var cmd = new SqlCommand("SELECT DoctorID, FullName, Specialty, Availability FROM Doctors ORDER BY FullName", conn))
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        var table = new System.Data.DataTable();
                        table.Load(reader);
                        grid.DataSource = table;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load doctors.\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
