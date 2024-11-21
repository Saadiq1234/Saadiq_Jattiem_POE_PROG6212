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
using Microsoft.Data.Sql;
namespace Saadiq_Jattiem_POE
{
    /// <summary>
    /// Interaction logic for LecturerLogin.xaml
    /// </summary>
    public partial class LecturerLogin : Window
    {
        public LecturerLogin()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the user's input from the form fields
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // Validate that both fields are filled out
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.");
                return;
            }

            try
            {
                // SQL connection string to  database
                string connectionString = "Data Source=hp820g4\\SQLEXPRESS;Initial Catalog=POE;Integrated Security=True;";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to check if the email and password hash match an entry in the AccountUser table
                    string query = "SELECT COUNT(*) FROM AccountUser WHERE Email = @Email AND PasswordHash = @PasswordHash AND AccountType = 'Lecturer'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Use SQL parameters to prevent SQL injection attacks
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PasswordHash", password);  // Assuming password is stored as plain text (it should be hashed)

                        // Execute the query and get the number of matching entries
                        int count = (int)command.ExecuteScalar();

                        // Check if any entries were found
                        if (count > 0)
                        {
                            MessageBox.Show("Lecturer logged in successfully.");

                            // Open the Lecturer Dashboard window and close the login window
                            LecturerDashboard lecturerDashboard = new LecturerDashboard();
                            lecturerDashboard.Show();
                            this.Close();  // Close the login window
                        }
                        else
                        {
                            // No matching entries were found
                            MessageBox.Show("Invalid email or password.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the process
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
