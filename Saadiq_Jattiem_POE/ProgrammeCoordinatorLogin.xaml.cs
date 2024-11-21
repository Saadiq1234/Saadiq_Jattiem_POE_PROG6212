using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Saadiq_Jattiem_POE
{
    /// <summary>
    /// Handles login for Programme Coordinators and redirects them to the dashboard upon successful login.
    /// </summary>
    public partial class ProgrammeCoordinatorLogin : Window
    {
        public ProgrammeCoordinatorLogin()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the Sign-In button click event.
        /// </summary>
        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve email and password from user input
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // Validate input
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Hash the entered password using the same hashing method as account creation
                string hashedPassword = HashPassword(password);

                // Connection string to the database
                string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE_2;Integrated Security=True;TrustServerCertificate=True;";

                // Open database connection
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL query to validate Programme Coordinator login
                    string query = @"
                        SELECT COUNT(*) 
                        FROM AccountUser 
                        WHERE Email = @Email 
                          AND PasswordHash = @PasswordHash 
                          AND AccountType = 'Programme Coordinator/Academic Manager'";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters for security
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                        // Execute the query and check the result
                        int count = (int)command.ExecuteScalar();

                        if (count > 0)
                        {
                            MessageBox.Show("Programme Coordinator logged in successfully.", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                            // Redirect to ProgrammeCoordinatorDashboard and close the current window
                            ProgrammeCoordinatorDashboard dashboard = new ProgrammeCoordinatorDashboard();
                            dashboard.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid email or password. Please try again.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database connection error: {sqlEx.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Hashes a password using SHA256.
        /// </summary>
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
