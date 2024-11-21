using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Saadiq_Jattiem_POE
{
    public partial class SubmitClaim : Window
    {
        public SubmitClaim()
        {
            InitializeComponent();
        }

        private string uploadedFilePath = null; // Store the uploaded file path
        private double totalAmount = 0; // Store calculated total amount

        // Event for uploading documents
        private void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Supported Documents|*.docx;*.xlsx;*.pdf"; // Only allow docx, xlsx, pdf
            if (openFileDialog.ShowDialog() == true)
            {
                uploadedFilePath = openFileDialog.FileName; // Store the path
                MessageBox.Show("Document uploaded successfully.");
            }
        }

        // Automatically calculate the total amount when the sessions or hourly rate change
        private void SessionsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotalAmount();
        }

        private void HourlyRateTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotalAmount();
        }

        // Method to calculate total amount based on sessions and hourly rate
        private void CalculateTotalAmount()
        {
            int numberOfSessions;
            double hourlyRate;

            // Validate the inputs
            if (int.TryParse(SessionsTextBox.Text, out numberOfSessions) &&
                double.TryParse(HourlyRateTextBox.Text, out hourlyRate))
            {
                totalAmount = numberOfSessions * hourlyRate;
                TotalAmountTextBlock.Text = totalAmount.ToString("C"); // Display total as currency
            }
            else
            {
                TotalAmountTextBlock.Text = string.Empty; // Clear if invalid input
            }
        }

        // Event for submitting the claim
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string classTaught = ClassTaughtTextBox.Text;
            int numberOfSessions;
            double hourlyRate;

            // Validate inputs
            if (!int.TryParse(SessionsTextBox.Text, out numberOfSessions))
            {
                MessageBox.Show("Please enter a valid number of sessions.");
                return;
            }

            if (!double.TryParse(HourlyRateTextBox.Text, out hourlyRate))
            {
                MessageBox.Show("Please enter a valid hourly rate.");
                return;
            }

            // Check if the total amount is calculated
            if (totalAmount == 0)
            {
                MessageBox.Show("Please enter valid values for sessions and hourly rate.");
                return;
            }

            // Ensure a document is uploaded
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                MessageBox.Show("Please upload a supporting document.");
                return;
            }

            // Save the claim to the database
            SaveClaimToDatabase(classTaught, numberOfSessions, hourlyRate, totalAmount, uploadedFilePath);

            // Clear the form after successful submission
            ClearForm();
        }

        // Method to save claim details to the database
        private void SaveClaimToDatabase(string classTaught, int sessions, double hourlyRate, double totalAmount, string documentPath)
        {
            string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";

            string query = @"INSERT INTO Claims (ClassTaught, NoOfSessions, HourlyRate, TotalAmount, SupportingDocumentPath)
                             VALUES (@ClassTaught, @NumberOfSessions, @HourlyRate, @TotalAmount, @DocumentPath)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassTaught", classTaught);
                command.Parameters.AddWithValue("@NumberOfSessions", sessions);
                command.Parameters.AddWithValue("@HourlyRate", hourlyRate);
                command.Parameters.AddWithValue("@TotalAmount", totalAmount);
                command.Parameters.AddWithValue("@DocumentPath", documentPath); // Store file path

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Claim submitted successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Event for clearing the form (Cancel)
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        // Helper method to clear the form
        private void ClearForm()
        {
            ClassTaughtTextBox.Clear();
            SessionsTextBox.Clear();
            HourlyRateTextBox.Clear();
            TotalAmountTextBlock.Text = string.Empty;
            uploadedFilePath = null;
            totalAmount = 0;
            MessageBox.Show("Form cleared.");
        }
    }
}

