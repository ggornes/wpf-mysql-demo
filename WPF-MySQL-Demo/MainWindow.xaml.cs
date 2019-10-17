using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_MySQL_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Components of this code have been modified from examples on:
    /// https://blogs.msdn.microsoft.com/benwilli/2016/06/30/asynchronousinfinite‐loops‐instead‐of‐timers/
    /// http://www.java2s.com/Tutorial/CSharp/0470__Windows‐Presentation‐Foundation/IsKeyCapsLockToggled.htm
    /// https://stackoverflow.com/questions/577411/how‐can‐i‐find‐the‐stateof‐numlock‐capslock‐and‐scrolllock‐in‐net
    /// 
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor for MainWindow
        /// 
        /// Calls the Initialise Component task, plus starts the background scanning of
        /// the keyboard and fills the table with data
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            ScanStatusKeysInBackground();

            FillTable();

        } // end MainWindows Constructor


        /// <summary>
        /// Checks the status of the num, scroll and caps lock keys
        /// it then changes the status bar CAPS, NUM and SCRL indicators when toggled.
        /// </summary>
        public void CheckStatusKeys()
        {
            var isNumLockToggled = Keyboard.IsKeyToggled(Key.NumLock);
            var isCapsLockToggled = Keyboard.IsKeyToggled(Key.CapsLock);
            var isScrollLockToggled = Keyboard.IsKeyToggled(Key.Scroll);

            if (isNumLockToggled)
            {
                NumLockStatus.Foreground = Brushes.Red;
            }
            else
            {
                NumLockStatus.Foreground = Brushes.Gray;
            }

            // Long  version

            /*
            if (isCapsLockToggled)
            {
                CapsLockStatus.Foreground = Brushes.Red;
            }
            else
            {
                CapsLockStatus.Foreground = Brushes.Gray;
            }

            if (isScrollLockToggled)
            {
                ScrollLockStatus.Foreground = Brushes.Red;
            }
            else
            {
                ScrollLockStatus.Foreground = Brushes.Gray;
            }
            */

            // short version

            CapsLockStatus.Foreground = isCapsLockToggled ? Brushes.Red : Brushes.Gray;

            ScrollLockStatus.Foreground = isScrollLockToggled ? Brushes.Red : Brushes.Gray;


        } // End CheckKeyStatus


        /// <summary>
        /// This creates a background thread that continuosly scans the keyboard
        /// The scans are delayed by 0.1 seconds between updates to the status indicators
        /// </summary>
        /// <returns></returns>
        private async Task ScanStatusKeysInBackground()
        {
            while (true)
            {
                CheckStatusKeys();
                await Task.Delay(100);
            }
        } // End scanStatus


        /// <summary>
        /// Update the table when the filter textbox is changed
        /// Calls the FillTable routine as needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FillTable(FilterTextBox.Text);
        } // End of FilterTexbox_TextChanged



        /// <summary>
        /// Update the DataGrid with new results when the application starts and when the user
        /// types in the text to filter by
        /// </summary>
        /// <param name="searchTerm"></param>
        private async void FillTable(string searchTerm = "")
        {
            // Data Source String (Data Source Name)
            string dnsString = "server=localhost;" +
                "user=nmt_demo_user;" +
                "database=nmt_demo;" +
                "port=3306;" +
                "password=Password1";

            MessageTextBlock.Background = this.FindResource(SystemColors.ControlLightLightBrushKey) as Brush;
            MessageTextBlock.Foreground = Brushes.Black;

            try
            {
                UpdateStatus(500, "Processing...");

                // MessageTextBlock.Text = "Configuring database connection";
                // await Task.Delay(250);

                // Perform database operations

                /*
                string sql = "SELECT * FROM locations";
                if (!searchTerm.Equals(""))
                {
                    sql += " WHERE location_name LIKE '%" + searchTerm + "%'";
                }
                MessageTextBox.Text = sql;
                */

                using (MySqlConnection connection = new MySqlConnection(dnsString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM locations";
                    if (!searchTerm.Equals(""))
                    {
                        sql += " WHERE location_name LIKE '%" + searchTerm + "%'";
                    }

                    // MessageTextBlock.Text = "Processing...";

                    // await Task.Delay(1250); // for demonstration purposes only

                    using (MySqlCommand cmdSel = new MySqlCommand(sql, connection))
                    {
                        DataTable dt = new DataTable();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                        da.Fill(dt);
                        LocationDataGrid.DataContext = dt;
                    }
                    connection.Close();
                    //MessageTextBlock.Text = "Ready";
                    UpdateStatus(500, "Ready...");
                }

            }

            catch (Exception ex)
            {
                Debug.Write("Error: " + ex.ToString() + "\n");
                MessageTextBlock.Text = "Unable to connect to database...";
                MessageTextBlock.Background = Brushes.Red;
                MessageTextBlock.Foreground = Brushes.White;
            }
        } // End fill table

        

        private void LocationAddMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LocationAdd locationWindow = new LocationAdd();
            // Reset the controls to blank
            locationWindow.NameTextbox.Text = "";
            locationWindow.CodeTextbox.Text = "";
            locationWindow.Owner = this;

            locationWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            if (locationWindow.ShowDialog() == true)
            {
                UpdateStatus(500, "Location Saved");
                FillTable();
                UpdateStatus(50, "Ready...");
            }

            locationWindow.Close();
        }

        private void LocationDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid locationGrid = (DataGrid)sender;
            DataRowView locationSelected = locationGrid.SelectedItem as DataRowView;
            MySqlDataReader reader = null;

            string dnsString = "server=localhost;" +
                "user=nmt_demo_user;" +
                "database=nmt_demo;" +
                "port=3306;" +
                "password=Password1";


            if (locationSelected != null)
            {
                String locationID = locationSelected[0].ToString();


                // get the details from the database
                using (MySqlConnection connection = new MySqlConnection(dnsString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM locations WHERE id = '" + locationID + "'";
                    MessageBox.Show(sql);

                    using (MySqlCommand cmdSel = new MySqlCommand(sql, connection))
                    {
                        // DataTable dt = new DataTable();
                        //MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                        //da.Fill(dt);

                        reader = cmdSel.ExecuteReader();

                        while (reader.Read())
                        {
                            MessageBox.Show(reader.GetString("location_name"));
                        }

                        
                    }





                    connection.Close();
                    //MessageTextBlock.Text = "Ready";
                    UpdateStatus(500, "Ready...");
                }




                LocationEdit locationWindow = new LocationEdit();
                // fill the LocationTextBox with the location title
                // fill the CodeTextBox with the location code
                locationWindow.LocationTextBox.Text = "???";
                locationWindow.CodeTextBox.Text = "???";
                locationWindow.Owner = this;
                locationWindow.WindowStartupLocation =
                WindowStartupLocation.CenterOwner;
                if (locationWindow.ShowDialog() == true)
                {
                    FillTable();
                    UpdateStatus(2500, "Location updated");
                }
            }

        
        }


        private async void UpdateStatus(int delayPeriod, string MessageToShow = "")
        {
            MessageTextBlock.Text = MessageToShow;

            await Task.Delay(delayPeriod); // for demonstration purposes only

        }

        

        private void AddCountryButton_Click(object sender, RoutedEventArgs e)
        {
            Window addLocation = new LocationAdd();
            addLocation.ShowDialog();
            FillTable();
        }

        private void LocationDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid locationGrid = (DataGrid)sender;
            DataRowView locationSelected = locationGrid.SelectedItem as DataRowView;

            if (locationSelected != null)
            {
                String locationID = locationSelected[0].ToString();
                SelectedRowLabel.Content = locationID;
            }
        }
    } // End Class MainWindow
} // End NameSpace
