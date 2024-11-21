using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

namespace Saadiq_Jattiem_POE
{
    /// <summary>
    /// Users can upload more documents or upload a document if they forgot as long as they know the name of the class they taught
    /// </summary>
    public partial class SupportingDocs : Window
    {
        private string uploadedFilePath = null; // Store the uploaded file path

        public SupportingDocs()
        {
            InitializeComponent();
        }

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

        // Event for submitting the document
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string classTaught = ClassTaughtTextBox.Text;

            // Ensure a document is uploaded
            if (string.IsNullOrEmpty(uploadedFilePath))
            {
                MessageBox.Show("Please upload a supporting document.");
                return;
            }

            // Check if the claim exists by class name
            int claimID = GetClaimIDByClass(classTaught);
            if (claimID == -1)
            {
                MessageBox.Show("No claim found for the given class.");
                return;
            }

            // Save the document path to the database and update the Claims table
            SaveSupportingDocument(claimID, uploadedFilePath);

            // Clear the form after successful submission
            ClearForm();
        }

        // Method to get the claim ID based on the class name
        private int GetClaimIDByClass(string classTaught)
        {
            string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE_2;Integrated Security=True;";
            string query = "SELECT ClaimsID FROM Claims WHERE ClassTaught = @ClassTaught";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassTaught", classTaught);

                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToInt32(result); // Return the ClaimID
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            return -1; // Return -1 if claim not found
        }

        // Method to save the supporting document to the database and update Claims table
        private void SaveSupportingDocument(int claimID, string documentPath)
        {
            string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE_2;Integrated Security=True;TrustServerCertificate=True;";

            // Insert into SupportingDocuments table
            string insertQuery = @"INSERT INTO SupportingDocuments (ClaimsID, DocName, FilePath, SubmissionDate)
                                   VALUES (@ClaimID, @DocName, @FilePath, @SubmissionDate)";

            // Update the Claims table to reflect the new document path
            string updateQuery = @"UPDATE Claims
                                   SET SupportingDocumentPath = @FilePath
                                   WHERE ClaimsID = @ClaimID";  // Corrected ClaimsID column name

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // First, insert the document into SupportingDocuments
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@ClaimID", claimID);  // Corrected ClaimsID
                    insertCommand.Parameters.AddWithValue("@DocName", Path.GetFileName(documentPath));
                    insertCommand.Parameters.AddWithValue("@FilePath", documentPath);
                    insertCommand.Parameters.AddWithValue("@SubmissionDate", DateTime.Now);
                    insertCommand.ExecuteNonQuery();

                    // Then, update the Claims table to save the document path
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@ClaimID", claimID);  // Corrected ClaimsID
                    updateCommand.Parameters.AddWithValue("@FilePath", documentPath);
                    updateCommand.ExecuteNonQuery();

                    MessageBox.Show("Supporting document submitted successfully!");
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
            uploadedFilePath = null;
            MessageBox.Show("Form cleared.");
        }
    }
}
