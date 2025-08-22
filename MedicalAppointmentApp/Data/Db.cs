using System.Configuration;
using System.Data.SqlClient;

namespace MedicalAppointmentApp.Data
{
    public static class Db
    {
        public static SqlConnection GetConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["MedicalDB"].ConnectionString;
            return new SqlConnection(cs);
        }
    }
}
