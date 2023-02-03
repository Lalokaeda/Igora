using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

namespace Igora
{
    /// <summary>
    /// Логика взаимодействия для AddClientWindow.xaml
    /// </summary>
    public partial class AddClientWindow : Window
    {
        private Client _currentClient = new Client();
        public AddClientWindow(Client selectedClient)
        {

            InitializeComponent();
            if (selectedClient ==null)
            { 
                _currentClient = new Client();
            _currentClient.GeneratePassword();
            _currentClient.BirthDate= DateTime.Now;
            DataContext= _currentClient;
            }
        }

        public AddClientWindow()
        {

            InitializeComponent();

                _currentClient = new Client();
                _currentClient.GeneratePassword();
                _currentClient.BirthDate = DateTime.Now;
                DataContext = _currentClient;
        }

        private void btnSaveClient_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (String.IsNullOrEmpty(_currentClient.Surname))
                errors.AppendLine("Введите фамилию");
            if (String.IsNullOrEmpty(_currentClient.Name))
                errors.AppendLine("Введите имя");
            if (String.IsNullOrEmpty(_currentClient.Passport))
                errors.AppendLine("Введите паспортные данные");
            if (_currentClient.Passport.Length<11)
                errors.AppendLine("Некорректные паспортные данные");
            if (String.IsNullOrEmpty(_currentClient.Address))
                errors.AppendLine("Введите адрес");
            if (_currentClient.PhoneNum.Equals("+7"))
                errors.AppendLine("Введите номер телефона");
            if (_currentClient.PhoneNum.Length <12)
                errors.AppendLine("Номер телефона некорректный");
            if (String.IsNullOrEmpty(_currentClient.Email))
                errors.AppendLine("Введите электрнную почту");
            if (!(_currentClient.Email.Contains("@")))
                errors.AppendLine("Электронная почта некорректна");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            IgoraEntities.GetContext().Clients.AddOrUpdate(_currentClient);

            try
            {
                IgoraEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена успешно");
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            
        }

        private void btnCancelAddClient_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
