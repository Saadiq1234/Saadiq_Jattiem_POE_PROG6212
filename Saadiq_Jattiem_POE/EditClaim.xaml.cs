using System;
using System.Data.SqlClient;
using System.Windows;

namespace Saadiq_Jattiem_POE
{
    public partial class EditClaim : Window
    {
        // Database connection string
        private readonly string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE_2;Integrated Security=True;TrustServerCertificate=True;";
        private readonly int claimID;
        private readonly decimal sessionCost = 105; // Example cost per session

        public EditClaim(int claimID)
        {
            InitializeComponent();
            this.claimID = claimID;
            LoadClaimDetails();
        }

        // Load claim details into the form
        public void LoadClaimDetails()
        {
            string query = "SELECT ClassTaught, NoOfSessions FROM Claims WHERE ClaimsID = @ClaimsID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClaimsID", claimID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        ClassTaughtTextBox.Text = reader["ClassTaught"].ToString();
                        NumberOfSessionsTextBox.Text = reader["NoOfSessions"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading claim details: " + ex.Message);
                }
            }
        }

        // Update the total amount whenever the number of sessions changes
        public void NumberOfSessionsTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (int.TryParse(NumberOfSessionsTextBox.Text, out int numberOfSessions))
            {
                decimal totalAmount = numberOfSessions * sessionCost;
                TotalAmountTextBox.Text = totalAmount.ToString("F2"); // Format to 2 decimal places
            }
            else
            {
                TotalAmountTextBox.Text = "0.00"; // Reset if input is invalid
            }
        }

        // Save button click handler
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string classTaught = ClassTaughtTextBox.Text;
            if (int.TryParse(NumberOfSessionsTextBox.Text, out int numberOfSessions))
            {
                UpdateClaim(classTaught, numberOfSessions);
                MessageBox.Show("Claim updated successfully!");
                Close();
            }
            else
            {
                MessageBox.Show("Invalid input for number of sessions.");
            }
        }

        // Update the claim in the database
        public void UpdateClaim(string classTaught, int numberOfSessions)
        {
            string query = "UPDATE Claims SET ClassTaught = @ClassTaught, NoOfSessions = @NoOfSessions WHERE ClaimsID = @ClaimsID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassTaught", classTaught);
                command.Parameters.AddWithValue("@NoOfSessions", numberOfSessions);
                command.Parameters.AddWithValue("@ClaimsID", claimID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating claim: " + ex.Message);
                }
            }
        }
    }
}
