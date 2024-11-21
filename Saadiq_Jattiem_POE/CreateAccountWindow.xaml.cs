using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Saadiq_Jattiem_POE
{
    /// <summary>
    /// This window allows users to create their account and store it in the database.
    /// </summary>
    public partial class CreateAccountWindow : Window
    {
        public CreateAccountWindow()
        {
            InitializeComponent();
        }

        // This method is called when the Create Account button is clicked.
        public void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve values from input fields
            string role = (RoleComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string phoneNumber = PhoneNumberTextBox.Text.Trim();

            // Validate inputs
            if (string.IsNullOrWhiteSpace(role) || string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                MessageBox.Show("Please fill out all fields.");
                return;
            }

            if (!int.TryParse(phoneNumber, out int phoneNumberInt))
            {
                MessageBox.Show("Phone Number must contain only digits.");
                return;
            }

            try
            {
                // Hash the password using SHA256
                string passwordHash = HashPassword(password);

                // Database connection string
                string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE_2;Integrated Security=True;TrustServerCertificate=True;";


                // Insert user data into the database
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "INSERT INTO AccountUser (FirstName, LastName, Email, PhoneNumber, PasswordHash, AccountType) " +
                                   "VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @PasswordHash, @AccountType)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumberInt); // Cast to INT
                        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@AccountType", role);

                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }

                MessageBox.Show("Account successfully created.");
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show("Database error: " + sqlEx.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        // Function to hash passwords using SHA256
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
