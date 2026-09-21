using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CurrencyConverter_ex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SqlConnection sqlCon = new SqlConnection();    // Create Object for SqlConnection
        SqlCommand sqlCmd = new SqlCommand();          // Create Object for SqlCommand
        SqlDataAdapter sqlDa = new SqlDataAdapter();   // Create Object for SqlDataAdapter

        private int _currencyId = 0;
        private double _fromAmount = 0;
        private double _toAmount = 0;

        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
        }

        public void dbConnect()
        {
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString; //Database Connection String
            sqlCon = new SqlConnection(Conn);
            sqlCon.Open(); // Connection Open
        } // End


        private void BindCurrency()
        {
            dbConnect();
            // Create an object for DataTable
            DataTable dTable = new DataTable();
            // Fetch data from Currency_Master Database table
            sqlCmd = new SqlCommand("select Id, CurrencyName from Currency_Master", sqlCon);
            // Define type of command to use for fetch query
            sqlCmd.CommandType = CommandType.Text;

            // Assign command to data adapter
            sqlDa = new SqlDataAdapter(sqlCmd);
            // Fill data to DataTable
            sqlDa.Fill(dTable);

            // Create object for DataRow
            DataRow newRow = dTable.NewRow();
            // Assign default value to Id column
            newRow["Id"] = 0;
            // Assign default text to CurrencyName column
            newRow["CurrencyName"] = "-- SELECT--";

            // Insert the new row to DataTable at index 0
            dTable.Rows.InsertAt(newRow, 0);

            // Datatable is not null and row count is greater than 0
            if (dTable != null && dTable.Rows.Count > 0)
            {
                // Assign the data from the table to 'From Currency' ComboBox
                cmbFromCurrency.ItemsSource = dTable.DefaultView;

                // Assign te data from table to 'To Currency' ComboBox
                cmbToCurrency.ItemsSource = dTable.DefaultView;
            }
            sqlCon.Close(); // Connection Close

            cmbFromCurrency.DisplayMemberPath = "CurrencyName"; // Display text for ComboBox
            cmbFromCurrency.SelectedValuePath = "Id"; // Value for ComboBox
            cmbFromCurrency.SelectedIndex = 0; // Default selected index for ComboBox

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedIndex = 0;

        }

        private void convert_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Convert button clicked!");
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Clear button clicked!");
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Save button clicked!");
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cancel button clicked!");
        }

        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }
    }
}
