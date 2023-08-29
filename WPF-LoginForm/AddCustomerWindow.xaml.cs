using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Net;
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
using WPF_LoginForm.ViewModels;

namespace WPF_LoginForm
{
    /// <summary>
    /// Логика взаимодействия для AddCustomerWindow.xaml
    /// </summary>
    public partial class AddCustomerWindow : Window
    {
        private readonly string _connectionString = "Server=(local); Database=CRMBase; Integrated Security=true";
        
        public AddCustomerWindow()
        {
            InitializeComponent();
            if (Customer.ClickOnChangeButton == 1)
            {
                tbFIO.Text = Customer.SelectedName;
                tbEmail.Text = Customer.SelectedEmail;
                tbTelephone.Text = Customer.SelectedTelephone;
                tbRegion.Text = Customer.SelectedRegion;
                tbSubscribed.IsChecked = Convert.ToBoolean(Customer.SelectedSubscribed);
                tbComment.Text = Customer.SelectedComment;
            }
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {

            // ИДЕЯ: сделать try catch на ловлю любых ошибок при execute. И, если есть - выводить сообщение в окне добавления инфы и предлагать изменить
            // Тестим реалиазацию задумки сверху
            StringBuilder errors = new StringBuilder();
            if (String.IsNullOrWhiteSpace(tbFIO.Text))
                errors.AppendLine("Укажите ФИО");
            if (String.IsNullOrWhiteSpace(tbTelephone.Text))
                errors.AppendLine("Укажите номер телефона");
            if (tbFIO.Text.Length > 100)
                errors.AppendLine("Вы превысили лимит вводимых символов для номера ФИО");
            if (tbTelephone.Text.Length > 10)
                errors.AppendLine("Вы превысили лимит вводимых символов для номера телефона");
            if (tbFIO.Text.Length > 100)
                errors.AppendLine("Вы превысили лимит вводимых символов для номера Региона");

            if (errors.Length > 0 )
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            //Генерация id
            Random rnd = new Random();
            int num;
            string id = "";
            string idPool = "abcdefghijklmnopqrstuvwxyz";
            idPool += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            idPool += "0123456789";
            //idPool += "!@#$%^&*()№;:?-_=+{}[]'|><.,~";
            for (int i = 0; i < 25; i++)
            {
                num = rnd.Next(idPool.Length);
                if ((i % 5 == 0) && (i != 0))
                {
                    id += "-";
                }
                id += idPool[num];
            }
            Console.WriteLine(id);
            //
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                // INSERT INTO dbo.Customers ("Name", [e-mail], Telephone, Region, Subscribed, Comment) VALUES ('Sergay', '123@mail.ru', '+82273', 'RegionSome', 1, 'Comment))');
                if (Customer.ClickOnChangeButton == 0)
                {
                    command.CommandText = String.Format("INSERT INTO dbo.Customers (ID, \"Name\", email, Telephone, Region, Subscribed, Comment) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}');", id, tbFIO.Text, tbEmail.Text, tbTelephone.Text, tbRegion.Text, Convert.ToInt32(tbSubscribed.IsChecked), tbComment.Text);

                }
                if (Customer.ClickOnChangeButton == 1)
                {
                    command.CommandText = String.Format("UPDATE Customers SET ID='{0}', Name='{1}', email='{2}', Telephone='{3}', Region='{4}', Subscribed='{5}', Comment='{6}' WHERE ID='{7}'",
            Convert.ToString(id),
            tbFIO.Text,
            tbEmail.Text,
            tbTelephone.Text,
            tbRegion.Text,
            Convert.ToInt32(tbSubscribed.IsChecked),
            tbComment.Text,
            Customer.SelectedId);
                }
                command.ExecuteScalar();
            }
            id = "";
            this.Close();

        }
    }
}


