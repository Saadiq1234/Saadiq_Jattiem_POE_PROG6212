using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace Saadiq_Jattiem_POE
{
    public partial class HRView : Window
    {//connect to database
        private readonly string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

        public HRView()
        {
            InitializeComponent();
            LoadClaimsData();
        }
        //load and display claims in the database
        private void LoadClaimsData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ClaimID, ClassTaught, NumberOfSessions, HourlyRate, TotalAmount, ClaimStatus FROM Claims";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    ClaimsDataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading claims data: {ex.Message}");
            }
        }
        //generates report for approved claims and displays it in a message box
        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Example: Generating a report for approved claims
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Claims WHERE ClaimStatus = 'Approved'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    string report = "Approved Claims Report\n\n";
                    while (reader.Read())
                    {
                        report += $"Claim ID: {reader["ClaimID"]}, Class Taught: {reader["ClassTaught"]}, " +
                                  $"Number of Sessions: {reader["NumberOfSessions"]}, Hourly Rate: {reader["HourlyRate"]}, " +
                                  $"Total Amount: {reader["TotalAmount"]}, Status: {reader["ClaimStatus"]}\n";
                    }

                    // Show the report in a message box
                    MessageBox.Show(report, "Report");

                    // Ask user where to save the report
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv",
                        DefaultExt = "txt",
                        AddExtension = true
                    };
                    //saves report to a file
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllText(saveFileDialog.FileName, report);
                        MessageBox.Show("Report saved successfully!", "Success");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}");
            }
        }
        //searches for a lecturer and allows for their information to be updated or deleted
        private void SearchLecturerButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Please enter both first and last names to search for a lecturer.");
                return;
            }

            LecturerInfoWindow lecturerInfoWindow = new LecturerInfoWindow(firstName, lastName);
            lecturerInfoWindow.ShowDialog();
        }
        //change claim to approved
        private void ApproveClaimButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int claimId = Convert.ToInt32(selectedRow["ClaimID"]);
                UpdateClaimStatus(claimId, "Approved");
            }
            else
            {
                MessageBox.Show("Please select a claim to approve.");
            }
        }
        //delete a claim
        private void DeleteClaimButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int claimId = Convert.ToInt32(selectedRow["ClaimID"]);
                if (MessageBox.Show("Are you sure you want to delete this claim?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    DeleteClaim(claimId);
                }
            }
            else
            {
                MessageBox.Show("Please select a claim to delete.");
            }
        }
        //updates the status of a claim
        private void UpdateClaimStatus(int claimId, string newStatus)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Claims SET ClaimStatus = @ClaimStatus WHERE ClaimID = @ClaimID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ClaimStatus", newStatus);
                    cmd.Parameters.AddWithValue("@ClaimID", claimId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Claim status updated successfully.");
                        LoadClaimsData(); // Refresh the data grid
                    }
                    else
                    {
                        MessageBox.Show("Failed to update claim status.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating claim status: {ex.Message}");
            }
        }

        private void DeleteClaim(int claimId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Claims WHERE ClaimID = @ClaimID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ClaimID", claimId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Claim deleted successfully.");
                        LoadClaimsData(); // Refresh the data grid
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete claim.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting claim: {ex.Message}");
            }
        }
        //Automation to automatically approve of claims set to PENDING
        private async Task AutoApprovePendingClaimsAsync()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "UPDATE Claims SET ClaimStatus = 'Approved' WHERE ClaimStatus = 'Pending' ";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"{rowsAffected} claims auto-approved.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during auto-approval: {ex.Message}");
            }
        }
        //CHANGES CLAIM STATUS TO PROCESSING IF IT IS WAITING
        private async void AutoUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    string query = "UPDATE Claims SET ClaimStatus = 'PROCESSING' WHERE ClaimStatus = 'WAITING'";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"{rowsAffected} claims updated to 'PROCESSING'.");
                        LoadClaimsData(); // Refresh the data grid
                    }
                    else
                    {
                        MessageBox.Show("No claims were updated. There may be no claims with the status 'WAITING'.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating claim statuses: {ex.Message}");
            }
        }
        //REDIRECTS BACK TO HOME PAGE
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(); // Create a new instance of MainWindow
            mainWindow.Show(); // Show the MainWindow
            this.Close(); // Close the HRView window
        }
    }
}
