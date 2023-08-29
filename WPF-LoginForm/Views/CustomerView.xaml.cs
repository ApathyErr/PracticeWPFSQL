using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
// System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using WPF_LoginForm.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WPF_LoginForm.Views
{

    /// <summary>
    /// Interaction logic for CustomerView.xaml
    /// </summary>
    public partial class CustomerView : UserControl
    {
        private readonly string _connectionString = "Server=(local); Database=CRMBase; Integrated Security=true";
        SqlDataAdapter adapter;
        DataTable customersTable;

        public CustomerView()
        {
            InitializeComponent();

        }

        private void tbSearch_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (tbSearch.SelectionLength == 0)
            {
                tbSearch.SelectAll();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Customer.ClickOnChangeButton = 0;
            new AddCustomerWindow().ShowDialog();
        }

        public void GetCustomersFromDataBase()
        {
            string sql = "SELECT * FROM dbo.Customers";
            customersTable = new DataTable();
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(_connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);


                adapter.InsertCommand = new SqlCommand("sp_InsertCustomers", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                SqlParameter parameter = adapter.InsertCommand.Parameters.Add("@Id", SqlDbType.NVarChar, 29, "Id");
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Name", SqlDbType.NVarChar, 100, "Name"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar, 100, "email"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Telephone", SqlDbType.NChar, 10, "Telephone"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Region", SqlDbType.NVarChar, 100, "Region"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Subscribed", SqlDbType.Bit, 0, "Subscribed"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@Comment", SqlDbType.NVarChar, 0, "Comment"));

                parameter.Direction = ParameterDirection.Output;

                connection.Open();
                adapter.Fill(customersTable);
                customersGrid.ItemsSource = customersTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GetCustomersFromDataBase();
        }


        public void customersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            DataRowView rs = dg.SelectedItem as DataRowView;
            if (rs != null)
            {
                Customer.SelectedId = rs["ID"].ToString();
                Customer.SelectedName = rs["Name"].ToString();
                Customer.SelectedEmail = rs["email"].ToString();
                Customer.SelectedTelephone = rs["Telephone"].ToString();
                Customer.SelectedRegion = rs["Region"].ToString();
                Customer.SelectedSubscribed = Convert.ToBoolean(rs["Subscribed"]);
                Customer.SelectedComment = rs["Comment"].ToString();

            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrEmpty(Customer.SelectedId))
            //{
            //    // Do not perform delete operation if no customer is selected
            //    return;
            //}

            if ((customersGrid.SelectedItem == null)||(Customer.SelectedId =="")) return;

            if (MessageBox.Show("Вы действительно хотите удалить запись?", "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand())
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = "DELETE FROM Customers WHERE ID='" + Customer.SelectedId + "'";
                    command.ExecuteScalar();
                    GetCustomersFromDataBase();
                }
            }
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            if (customersGrid.SelectedItem == null)
            {
                MessageBox.Show("Для начала выделите запись, чтобы ее изменить");
                return;
            }   
            Customer.ClickOnChangeButton = 1;
            new AddCustomerWindow().ShowDialog();
        }
    }
}
