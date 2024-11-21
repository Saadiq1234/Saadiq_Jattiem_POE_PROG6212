using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Saadiq_Jattiem_POE
{
    /// <summary>
    /// Interaction logic for LecturerDashboard.xaml
    /// </summary>
    public partial class LecturerDashboard : Window
    {
        private string connectionString = "Data Source=labg9aeb3\\sqlexpress01;Initial Catalog=POE_2;Integrated Security=True;TrustServerCertificate=True;";
        private DispatcherTimer timer;

        public LecturerDashboard()
        {
            InitializeComponent();
            LoadClaimStatus();

            // Set up the timer to refresh claim status every 5 seconds
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            LoadClaimStatus(); // Reload the claim status every 5 seconds
        }

        private void SubmitClaim_Click(object sender, RoutedEventArgs e)
        {
            SubmitClaim submitClaim = new SubmitClaim();
            submitClaim.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SupportingDocs Docs = new SupportingDocs();
            Docs.Show();
        }

        // Event handler for the View Claims button
        private void ViewClaims_Click(object sender, RoutedEventArgs e)
        {
            ViewClaims viewClaims = new ViewClaims();
            viewClaims.Show();
            LoadClaimStatus(); // Load claims status when the button is clicked
        }

        // Method to load claim status into the ListView
        private void LoadClaimStatus()
        {
            string query = "SELECT ClassTaught, ClaimTotalAmount, ClaimStatus FROM Claims"; // Adjust the query as necessary

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                ClaimStatusListView.ItemsSource = dataTable.DefaultView; // Set the data source for the ListView
            }
        }
    }
}
