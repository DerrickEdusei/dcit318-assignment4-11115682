using System;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalAppointmentApp.Forms
{
    public class MainForm : Form
    {
        private Button btnDoctors, btnBook, btnManage;

        public MainForm()
        {
            Text = "Medical Appointment Booking System";
            Width = 800;
            Height = 450;
            StartPosition = FormStartPosition.CenterScreen;

            btnDoctors = new Button { Text = "View Doctors", Width = 200, Height = 50, Location = new Point(50, 50) };
            btnBook    = new Button { Text = "Book Appointment", Width = 200, Height = 50, Location = new Point(50, 120) };
            btnManage  = new Button { Text = "Manage Appointments", Width = 200, Height = 50, Location = new Point(50, 190) };

            btnDoctors.Click += (s, e) => new DoctorListForm().ShowDialog(this);
            btnBook.Click    += (s, e) => new AppointmentForm().ShowDialog(this);
            btnManage.Click  += (s, e) => new ManageAppointmentsForm().ShowDialog(this);

            Controls.AddRange(new Control[] { btnDoctors, btnBook, btnManage });
        }
    }
}
