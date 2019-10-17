using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace WPF_MySQL_Demo
{
    /// <summary>
    /// Interaction logic for LocationEdit.xaml
    /// </summary>
    public partial class LocationEdit : Window
    {
        public LocationEdit()
        {
            InitializeComponent();
            HideErrors();
            Initialize();

        }

        private string server;
        private string database;
        private string db_user;
        private string password;
        private int port;

        private string dsnString;

        private MySqlConnection connection;


        private void Initialize()
        {
            server = "localhost";
            database = "nmt_demo";
            db_user = "nmt_demo_user";
            password = "Password1";
            port = 3306;

            dsnString = "SERVER=" + server + ";"
                + "PORT=" + port + ";"
                + "DATABASE=" + database + ";"
                + "UID=" + db_user + ";"
                + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(dsnString);

        }

        private void HideErrors()
        {
            LocationErrorTextBlock.Visibility = Visibility.Hidden;
            CodeErrorTextBlock.Visibility = Visibility.Hidden;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            HideErrors();
            string name = LocationTextBox.Text;
            string code = CodeTextBox.Text;
            bool validName = ValidateName(name);
            bool validCode = ValidateCode(code);
            bool locationFound = FindLocationInDB(name);
            bool codeFound = FindCodeInDB(code);

            if (!validName)
            {
                // TODO: Display error to the user
                LocationErrorTextBlock.Text = "Invalid Name (min 4, max 128 chars)";
                LocationErrorTextBlock.Visibility = Visibility.Visible;
            }
            else if (locationFound)
            {
                // TODO: Display error to the user
                LocationErrorTextBlock.Text = "Location already exists";
                LocationErrorTextBlock.Visibility = Visibility.Visible;
            }

            if (!validCode)
            {
                // TODO: Display error to the user
                CodeErrorTextBlock.Text = "Invalid Code (exactly 2 letters required)";
                CodeErrorTextBlock.Visibility = Visibility.Visible;
            }
            else if (codeFound)
            {
                // TODO: Display error to the user
                CodeErrorTextBlock.Text = "Code already exists";
                CodeErrorTextBlock.Visibility = Visibility.Visible;
            }

            if (validCode && validName)
            {
                UpdateLocationInDB(name, code);
                this.DialogResult = true;
                this.Close();
            }

        }


        private bool UpdateLocationInDB(String theName, string theCode)
        {
            string addLocationSQL = "INSERT INTO locations(location_name, two_letter_code) "
                + " VALUES ( '" + theName + "', '" + theCode + "')";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmdSel = new MySqlCommand(addLocationSQL, connection);
                cmdSel.ExecuteNonQuery();
                return true;
            }
            return false;
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.

                switch(ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server. Contact Administrator");
                        break;
                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }


        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }


        private bool FindLocationInDB(string theName)
        {
            string nameSQL = "SELECT * FROM locations "
            + "WHERE location_name LIKE '%" + theName + "%'";
            using (MySqlCommand cmdSel = new MySqlCommand(nameSQL,
            connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.Fill(dt);
                int numRows = dt.Rows.Count;
                return numRows > 0;
            }
            // return 0;
        }


        private bool FindCodeInDB(string theCode)
        {
            // TODO: Add code to check if Location Code exists, return true/false
            string codeSQL = "SELECT * FROM locations " + "WHERE two_letter_code LIKE '%" + theCode + "%'";

            using (MySqlCommand cmdSel = new MySqlCommand(codeSQL,connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.Fill(dt);
                int numRows = dt.Rows.Count;
                return numRows > 0;
            }
            // return 0;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private bool ValidateName(string theName)
        {
            if (theName.Length < 4 || theName.Length > 128)
            {
                return false;
            }
            return true;
        }
        private bool ValidateCode(string theCode)
        {
            if (theCode.Length != 2)
            {
                return false;
            }
            return true;
        }

    }
}
