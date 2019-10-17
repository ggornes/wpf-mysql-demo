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
    /// Interaction logic for LocationAdd.xaml
    /// </summary>
    public partial class LocationAdd : Window


    {

        /// <summary>
        /// 
        /// </summary>
        private string dsnString = "server=localhost;" +
            "user=nmt_demo_user;" +
            "database=nmt_demo;" +
            "port=3306;" +
            "password=Password1";


        private MySqlConnection connection;

        public LocationAdd()
        {
            InitializeComponent();
            HideErrors();
            GetConnection();
        }

        private void HideErrors()
        {
            LocationErrorTextBlock.Visibility = Visibility.Hidden;
            CodeErrorTextBlock.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// Validate the inputs and insert data to the database if valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            HideErrors();
            string name = NameTextbox.Text;
            string code = CodeTextbox.Text;
            bool validName = ValidateName(name);
            bool validCode = ValidateCode(code);
            bool locationFound = FindLocationInDB(name);
            bool codeFound = FindCodeInDB(code);

            if (!validName)
            {
                // display error to the user
                LocationErrorTextBlock.Text = "InvalidName (min 4, max 128 chars)";
                LocationErrorTextBlock.Visibility = Visibility.Visible;
            }  else if (locationFound)
            {
                LocationErrorTextBlock.Text = "Location already exists";
                LocationErrorTextBlock.Visibility = Visibility.Visible;
            }

            if (!validCode)
            {
                // display error to the user
                CodeErrorTextBlock.Text = "InvalidName (exactly 2 letters required)";
                CodeErrorTextBlock.Visibility = Visibility.Visible;
            } else if (codeFound)
            {
                CodeErrorTextBlock.Text = "Code already exists";
                CodeErrorTextBlock.Visibility = Visibility.Visible;
            }

            if (validCode && validName)
            {
                // Add location to the DB
                AddLocationToDB(name, code);
                this.Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool ValidateName(string theName)
        {
            if(theName.Length < 4 || theName.Length > 128)
            {
                return false;
            }
            else
            {
                return true;
            }
            
        }

        private bool ValidateCode(string theCode)
        {
            if(theCode.Length != 2)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        private void GetConnection()
        {
            using (connection = new MySqlConnection(dsnString))
            {
                connection.Open();
            }
        }

        private void CloseConnection()
        {
            connection.Close();
        }

        private bool FindLocationInDB( string theName)
        {
            string nameSQL = "SELECT * FROM locations" +
                " WHERE location_name LIKE '%" + theName + "%'";

            using (MySqlCommand cmdSel = new MySqlCommand(nameSQL, connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.Fill(dt);
                int numRows = dt.Rows.Count;
                return numRows>0;

            }

            //return 0;
        }

        private bool FindCodeInDB(string theCode)
        {
            string codeSQL = "SELECT * FROM locations" +
                " WHERE two_letter_code LIKE '%" + theCode + "%'";

            using (MySqlCommand cmdSel = new MySqlCommand(codeSQL, connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.Fill(dt);
                int numRows = dt.Rows.Count;
                return numRows > 0;

            }
        }

        private bool AddLocationToDB(string theName, string theCode)
        {
            string addLocationSQL = "INSERT INTO locations(location_name, two_letter_code) " 
                + " VALUES ('"+ theName +"', '"+ theCode +"')";

            using (MySqlCommand cmdSel = new MySqlCommand(addLocationSQL, connection))
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter da = new MySqlDataAdapter(cmdSel);
                da.Fill(dt);
                int numRows = dt.Rows.Count;
                MessageBox.Show(numRows.ToString());
                return numRows == 0;

            }
        }
    }
}
