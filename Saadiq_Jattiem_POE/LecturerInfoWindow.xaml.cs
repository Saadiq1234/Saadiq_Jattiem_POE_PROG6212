using System;
using System.Data.SqlClient;
using System.Windows;

//this window will allow the user to view and edit the lecturer information
namespace Saadiq_Jattiem_POE
{
    public partial class LecturerInfoWindow : Window
    {
        //connection to database
        private readonly string connectionString = "Data Source=hp820g4\\SQLEXPRESS;Initial Catalog=POE;Integrated Security=True;";
        private readonly string firstName;
        private readonly string lastName;
        //connection to database and loading the lecturer data
        public LecturerInfoWindow(string firstName, string lastName)
        {
            InitializeComponent();
            this.firstName = firstName;
            this.lastName = lastName;
            LoadLecturerData();
        }
        //loads lecturer data and displays it in the textboxes
        private void LoadLecturerData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT FirstName, LastName, Email, PhoneNumber, AccountType FROM AccountUser  WHERE FirstName = @FirstName AND LastName = @LastName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            FirstNameTextBox.Text = reader["FirstName"].ToString();
                            LastNameTextBox.Text = reader["LastName"].ToString();
                            EmailTextBox.Text = reader["Email"].ToString();
                            PhoneNumberTextBox.Text = reader["PhoneNumber"].ToString();
                            AccountTypeTextBox.Text = reader["AccountType"].ToString();
                        }
                        else
                        {
                            MessageBox.Show("Lecturer not found.");
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading lecturer data: {ex.Message}");
            }
        }

        //saves changes into the database
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE AccountUser  SET FirstName = @FirstName, LastName = @LastName, Email = @Email, PhoneNumber = @PhoneNumber, AccountType = @AccountType WHERE FirstName = @OriginalFirstName AND LastName = @OriginalLastName";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FirstName", FirstNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@LastName", LastNameTextBox.Text);
                    cmd.Parameters.AddWithValue("@Email", EmailTextBox.Text);
                    cmd.Parameters.AddWithValue("@PhoneNumber", PhoneNumberTextBox.Text);
                    cmd.Parameters.AddWithValue("@AccountType", AccountTypeTextBox.Text); // Fixed the space here
                    cmd.Parameters.AddWithValue("@OriginalFirstName", firstName);
                    cmd.Parameters.AddWithValue("@OriginalLastName", lastName);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Lecturer information updated successfully.");
                    }
                    else
                    {
                        MessageBox.Show("No changes were made.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving lecturer data: {ex.Message}");
            }
        }

        //deletes information from the database
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this lecturer?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "DELETE FROM AccountUser  WHERE FirstName = @FirstName AND LastName = @LastName";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Lecturer deleted successfully.");
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Lecturer not found or could not be deleted.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting lecturer: {ex.Message}");
                }
            }
        }
    }
}