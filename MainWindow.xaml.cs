using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
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
            GetData();
            lblCurrency.Content = "0";
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
            //Declare ConvertedValue variable with double data type to store converted currency value
            double ConvertedValue;

            try
            {
               
                //Check amount textbox is Null or Blank
                if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
                {
                    //If amount Textbox is Null or Blank then show dialog box
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    //Set focus to amount textbox
                    txtCurrency.Focus();
                    return;
                }
                //If From currency selected value is null or default text as --SELECT--
                else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
                {
                    //Open Dialog box
                    MessageBox.Show("Please select 'From Currency'", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbFromCurrency.Focus();
                    return;
                }
                else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
                {
                    MessageBox.Show("Please select 'To Currency'", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    cmbToCurrency.Focus();
                    return;
                }

                if (cmbFromCurrency.SelectedValue == cmbToCurrency.SelectedValue) //Check if From and To Combobox Selected Same Value
                {
                    //Amount textbox value is set in ConvertedValue. The double.parse is used to change Datatype from String To Double. 
                    //Textbox text has string, and ConvertedValue is double.
                    ConvertedValue = double.Parse(txtCurrency.Text);

                    //Show the label converted currency name and converted currency amount. The ToString("N3") is used for Placing 000 after the dot(.)
                    lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
                }
                else
                {

                    //Calculation for currency converter is From currency value Multiplied(*) with amount textbox value and then that total is divided(/) with To currency value.
                    ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.SelectedValue.ToString());

                    //Show the label converted currency name and converted currency amount.
                    lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }// End

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // Clear and reset ComboBoxes in currency converter section.
            try
            {
                txtCurrency.Text = string.Empty;
                if (cmbFromCurrency.Items.Count > 0)
                    cmbFromCurrency.SelectedIndex = 0;

                if (cmbToCurrency.Items.Count > 0)
                    cmbToCurrency.SelectedIndex = 0;

                lblCurrency.Content = "";
                txtCurrency.Focus();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
           Regex regex = new Regex("[^0-9]+"); // Regular expression to allow only numbers
            e.Handled = regex.IsMatch(e.Text); // If input is not a number, mark the event as handled
        }// End

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // If textamount is null or empty
                if(txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {
                    if(_currencyId > 0) // Code for update button.
                    {
                        if (MessageBox.Show("Update information?", "Information", MessageBoxButton.YesNo, 
                            MessageBoxImage.Question) == MessageBoxResult.Yes) // Show confirmation message
                        {
                            // If confirmation message is yes, run code.
                            dbConnect();
                            DataTable dTable = new DataTable();
                            sqlCmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", sqlCon); // Update the datatable row
                            sqlCmd.CommandType = CommandType.Text;
                            sqlCmd.Parameters.AddWithValue("@Id", _currencyId);
                            sqlCmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                            sqlCmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text.Trim());
                            sqlCmd.ExecuteNonQuery(); // Execute the query
                            sqlCon.Close(); // Close the connection

                            MessageBox.Show("Information updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        
                    }
                    else // Save new currency button code
                    {
                        if (MessageBox.Show("Save information?", "Information", MessageBoxButton.YesNo, 
                            MessageBoxImage.Question) == MessageBoxResult.Yes) // Show confirmation message
                        {
                            // If confirmation message is yes, run code.
                            dbConnect();
                            sqlCmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", sqlCon); // Save the datatable row
                            sqlCmd.CommandType = CommandType.Text;
                            sqlCmd.Parameters.AddWithValue("@Amount", txtAmount.Text.Trim());
                            sqlCmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text.Trim());
                            sqlCmd.ExecuteNonQuery(); // Execute the query
                            sqlCon.Close(); // Close the connection

                            MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    // Clear user input data
                    ClearMaster();
                }
                    

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }// End

        private void ClearMaster()
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                _currencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }// End

        private void GetData() // Bind data to DataGrid view.
        {
            dbConnect();
            DataTable dTable = new DataTable();
            sqlCmd = new SqlCommand("SELECT * FROM Currency_Master", sqlCon);
            sqlCmd.CommandType = CommandType.Text;
            sqlDa = new SqlDataAdapter(sqlCmd);
            sqlDa.Fill(dTable);

            if (dTable != null && dTable.Rows.Count > 0)
                dgvCurrency.ItemsSource = dTable.DefaultView;
            
            else
                dgvCurrency.ItemsSource = null;

            sqlCon.Close();

        }// End

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }// End

        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid dGrid = (DataGrid)sender;      // Create object for DataGrid
                DataRowView selectedRow = dGrid.SelectedItem as DataRowView;   // Create object for DataRowView

                if (selectedRow != null) // selected row is NOT null
                {
                    if (dgvCurrency.Items.Count > 0) // DataGrid view HAS rows
                    {
                        if (dGrid.SelectedCells.Count > 0) // selected row HAS cells
                        {
                            _currencyId = Int32.Parse(selectedRow["Id"].ToString()); // Get selected row Id column

                            if (dGrid.SelectedCells[0].Column.DisplayIndex == 0) // If selected cell is in first column (Edit icon button cell)
                            {
                                txtAmount.Text = selectedRow["Amount"].ToString(); // Get selected row Amount column
                                txtCurrencyName.Text = selectedRow["CurrencyName"].ToString(); // Get selected row CurrencyName column
                                btnSave.Content = "Update"; // Change button text to 'Update'
                            }
                            if (dGrid.SelectedCells[0].Column.DisplayIndex == 1) // If selected cell is in second column (Delete icon button cell)
                            {
                                if (MessageBox.Show("Delete information?", "Information", MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) == MessageBoxResult.Yes) // Show confirmation message
                                {
                                    dbConnect();
                                    sqlCmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", sqlCon); // Delete the datatable row
                                    sqlCmd.CommandType = CommandType.Text;
                                    sqlCmd.Parameters.AddWithValue("@Id", _currencyId);
                                    sqlCmd.ExecuteNonQuery(); // Execute the query
                                    sqlCon.Close(); // Close the connection

                                    MessageBox.Show("Information deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
