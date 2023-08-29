using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_LoginForm.ViewModels
{

    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Region { get; set; }
        public bool Subscribed { get; set; }
        public string Comment { get; set; }
        public static string SelectedId { get; set; }
        public static string SelectedName { get; set; }
        public static string SelectedEmail { get; set; }
        public static string SelectedTelephone { get; set; }
        public static string SelectedRegion { get; set; }
        public static bool SelectedSubscribed { get; set; }
        public static string SelectedComment { get; set; }
        public static int ClickOnChangeButton { get; set; }
    }


    public class CustomerRepository
    {
        private readonly string connectionString;

        public CustomerRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Customer> GetCustomers()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM Customer";

                SqlCommand command = new SqlCommand(sql, connection);

                SqlDataReader reader = command.ExecuteReader();

                List<Customer> customers = new List<Customer>();

                while (reader.Read())
                {
                    Customer customer = new Customer
                    {
                        ID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Email = reader.GetString(2),
                        Telephone = reader.GetString(3),
                        Region = reader.GetString(4),
                        Subscribed = reader.GetBoolean(5),
                        Comment = reader.GetString(6),
                    };

                    customers.Add(customer);
                }

                return customers;
            }
        }

        // Почему тут UPDATE??
        public void SaveCustomer(Customer customer)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "UPDATE Customer SET Name=@Name, Email=@Email, Telephone=@Telephone, Region=@Region, Subscribed=@Subscribed, Comment=@Comment WHERE ID=@ID";

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ID", customer.ID);
                command.Parameters.AddWithValue("@Name", customer.Name);
                command.Parameters.AddWithValue("@Email", customer.Email);
                command.Parameters.AddWithValue("@Telephone", customer.Telephone);
                command.Parameters.AddWithValue("@Region", customer.Region);
                command.Parameters.AddWithValue("@Subscribed", customer.Subscribed);
                command.Parameters.AddWithValue("@Comment", customer.Comment);

                command.ExecuteNonQuery();
            }
        }
    }
    public class CustomerViewModel: ViewModelBase
    {
    }
}
