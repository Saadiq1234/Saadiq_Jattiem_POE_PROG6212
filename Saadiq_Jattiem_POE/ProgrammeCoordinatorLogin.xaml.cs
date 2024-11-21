using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Saadiq_Jattiem_POE;

namespace Saadiq_Jattiem_POE
{
    /// <summary>
    /// When coordinators are approved for login they will be taken to coordinator dashboard
    /// </summary>
    public partial class ProgrammeCoordinatorLogin : Window
    {
        public ProgrammeCoordinatorLogin()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Get email and password from the form
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // Validate input
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.");
                return;
            }

            try
            {
                // Connection string to the database
                string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Open connection
                    connection.Open();

                    // SQL query to check if the user exists and is a Programme Coordinator
                    string query = "SELECT COUNT(*) FROM AccountUser WHERE Email = @Email AND PasswordHash = @PasswordHash AND AccountType = 'Programme Coordinator/Academic Manager'";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters for the email and password
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PasswordHash", password);  // Password should be hashed in a real app

                        // Execute the query
                        int count = (int)command.ExecuteScalar();

                        // If the credentials match a Programme Coordinator, redirect to the dashboard
                        if (count > 0)
                        {
                            MessageBox.Show("Programme Coordinator logged in successfully.");

                            // Redirect to ProgrammeCoordinatorDashboard window and close the login window
                            ProgrammeCoordinatorDashboard coordinatorDashboard = new ProgrammeCoordinatorDashboard();
                            coordinatorDashboard.Show();
                            this.Close();  // Close the login window
                        }
                        else
                        {
                            MessageBox.Show("Invalid email or password.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
