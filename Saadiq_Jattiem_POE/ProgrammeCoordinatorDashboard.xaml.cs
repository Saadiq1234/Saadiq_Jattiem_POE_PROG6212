using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace Saadiq_Jattiem_POE
{
    public partial class ProgrammeCoordinatorDashboard : Window
    {
        public ProgrammeCoordinatorDashboard()
        {
            InitializeComponent();
            LoadClaims();
        }

        // Loads and displays the claims in the list view
        private void LoadClaims()
        {
            List<Claim> claims = GetClaimsFromDatabase();
            if (claims != null && claims.Any())
            {
                ClaimsListView.ItemsSource = claims;
            }
            else
            {
                MessageBox.Show("No claims found.");
            }
        }

        // Retrieves the claims from the database
        private List<Claim> GetClaimsFromDatabase()
        {
            List<Claim> claims = new List<Claim>();
            string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE_2;Integrated Security=True;TrustServerCertificate=True;";
            string query = "SELECT ClaimsID, ClassTaught, ClaimTotalAmount, ClaimStatus, NoOfSessions FROM Claims";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Safely retrieve data, checking for null values
                        int claimsID = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        string classTaught = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                        decimal totalAmount = reader.IsDBNull(2) ? 0.0m : reader.GetDecimal(2);
                        string claimStatus = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                        int numberOfSessions = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);

                        claims.Add(new Claim
                        {
                            ClaimsID = claimsID,
                            ClassTaught = classTaught,
                            TotalAmount = totalAmount,
                            ClaimStatus = claimStatus,
                            NumberOfSessions = numberOfSessions
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error retrieving claims: " + ex.Message);
                }
            }

            return claims.Count > 0 ? claims : null;
        }


        // Updates the claim status in the database
        private void UpdateClaimStatus(int claimsID, string newStatus)
        {
            string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE_2;Integrated Security=True;TrustServerCertificate=True;";
            string query = "UPDATE Claims SET ClaimStatus = @ClaimStatus WHERE ClaimsID = @ClaimsID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClaimStatus", newStatus);
                command.Parameters.AddWithValue("@ClaimsID", claimsID);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    LoadClaims(); // Reload claims after update
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Validates the claim
        private bool ValidateClaim(Claim claim)
        {
            const decimal minHourlyRate = 100.00m;
            const decimal maxHourlyRate = 200.00m;
            decimal hourlyRate = claim.TotalAmount / claim.NumberOfSessions;

            // Ensure hourly rate is between $100 and $200
            return hourlyRate >= minHourlyRate && hourlyRate <= maxHourlyRate;
        }

        private void ApproveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsListView.SelectedItem is Claim selectedClaim)
            {
                if (ValidateClaim(selectedClaim))
                {
                    UpdateClaimStatus(selectedClaim.ClaimsID, "Approved");
                }
                else
                {
                    MessageBox.Show("Claim is invalid.");
                }
            }
            else
            {
                MessageBox.Show("No claim selected.");
            }
        }

        // Rejects the claim
        private void RejectButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsListView.SelectedItem is Claim selectedClaim)
            {
                UpdateClaimStatus(selectedClaim.ClaimsID, "Rejected");
            }
            else
            {
                MessageBox.Show("No claim selected.");
            }
        }

        // Changes claim status to pending
        private void PendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsListView.SelectedItem is Claim selectedClaim)
            {
                UpdateClaimStatus(selectedClaim.ClaimsID, "Pending");
            }
            else
            {
                MessageBox.Show("No claim selected.");
            }
        }

        // Runs the automation process for claim approval/rejection
        private void RunAutomationButton_Click(object sender, RoutedEventArgs e)
        {
            List<Claim> claims = GetClaimsFromDatabase();

            if (claims != null && claims.Any())
            {
                foreach (var claim in claims)
                {
                    // Automatically approve claims with 5 or more sessions and valid criteria
                    if (claim.ClaimStatus == "WAITING" && claim.NumberOfSessions >= 5 && ValidateClaim(claim))
                    {
                        UpdateClaimStatus(claim.ClaimsID, "Approved");
                    }
                    // Change "WAITING" claims to "PENDING"
                    else if (claim.ClaimStatus == "WAITING")
                    {
                        UpdateClaimStatus(claim.ClaimsID, "Pending");
                    }
                }

                MessageBox.Show("Automation process completed!");
            }
            else
            {
                MessageBox.Show("No claims available for automation.");
            }
        }

        // Download supporting documents for the selected claim
        private void DownloadDocument_Click(object sender, RoutedEventArgs e)
        {
            if (ClaimsListView.SelectedItem is Claim selectedClaim)
            {
                List<SupportingDocument> documents = GetSupportingDocuments(selectedClaim.ClaimsID);

                if (documents == null || !documents.Any())
                {
                    MessageBox.Show("No supporting documents found for this claim.");
                    return;
                }

                foreach (var doc in documents)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        FileName = doc.DocName,
                        Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        try
                        {
                            File.Copy(doc.FilePath, saveFileDialog.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error downloading document: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No claim selected.");
            }
        }

        // Retrieves the supporting documents for a given claim
        private List<SupportingDocument> GetSupportingDocuments(int claimsID)
        {
            List<SupportingDocument> documents = new List<SupportingDocument>();
            string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE_2;Integrated Security=True;TrustServerCertificate=True;";
            string query = "SELECT DocID, DocName, FilePath FROM SupportingDocuments WHERE ClaimsID = @ClaimsID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClaimsID", claimsID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        documents.Add(new SupportingDocument
                        {
                            DocID = reader.GetInt32(0),
                            DocName = reader.GetString(1),
                            FilePath = reader.GetString(2)
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            return documents.Count > 0 ? documents : null;
        }

    }

    // Claim model class to match the database schema
    public class Claim
    {
        public int ClaimsID { get; set; }
        public string ClassTaught { get; set; }
        public decimal TotalAmount { get; set; }
        public string ClaimStatus { get; set; }
        public int NumberOfSessions { get; set; }
    }

    // Supporting document model class
    public class SupportingDocument
    {
        public int DocID { get; set; }
        public string DocName { get; set; }
        public string FilePath { get; set; }
    }
}
