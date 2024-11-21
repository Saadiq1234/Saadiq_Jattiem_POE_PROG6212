using System;
using System.Data.SqlClient;
using System.Windows;

namespace Saadiq_Jattiem_POE
{
    //Logic behind allowing users to update their claims information
    public partial class EditClaim : Window
    {
        //connect to the database
        public string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        public int claimID;
        public decimal sessionCost = 105; // Example cost per session

        public EditClaim(int claimID)
        {
            InitializeComponent();
            this.claimID = claimID;
            LoadClaimDetails();
        }

        //updated the information in the claims so that the total amount is updated and displayed based on the total sessions
        public void LoadClaimDetails()
        {
            string query = "SELECT ClassTaught, NumberOfSessions, TotalAmount FROM Claims WHERE ClaimID = @ClaimID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClaimID", claimID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        ClassTaughtTextBox.Text = reader["ClassTaught"].ToString();
                        NumberOfSessionsTextBox.Text = reader["NumberOfSessions"].ToString();
                        TotalAmountTextBox.Text = reader["TotalAmount"].ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        //method to capture the amount of sessions the user has
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
        //once the save button is clicked it will save data and update the total amount for that course
        public void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string classTaught = ClassTaughtTextBox.Text;
            int numberOfSessions;
            decimal totalAmount;

            if (int.TryParse(NumberOfSessionsTextBox.Text, out numberOfSessions) &&
                decimal.TryParse(TotalAmountTextBox.Text, out totalAmount))
            {
                UpdateClaim(classTaught, numberOfSessions, totalAmount);
                MessageBox.Show("Claim updated successfully!");
                Close();
            }
            else
            {
                MessageBox.Show("Invalid input for number of sessions or total amount.");
            }
        }
        //this method will update the claim in the database
        public void UpdateClaim(string classTaught, int numberOfSessions, decimal totalAmount)
        {
            string query = "UPDATE Claims SET ClassTaught = @ClassTaught, NumberOfSessions = @NumberOfSessions, TotalAmount = @TotalAmount WHERE ClaimID = @ClaimID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassTaught", classTaught);
                command.Parameters.AddWithValue("@NumberOfSessions", numberOfSessions);
                command.Parameters.AddWithValue("@TotalAmount", totalAmount);
                command.Parameters.AddWithValue("@ClaimID", claimID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}