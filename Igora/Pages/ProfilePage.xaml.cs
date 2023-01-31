using Bytescout.BarCode;
using Igora.Properties;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Igora.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        Staff _currentStaff = null;
        public ProfilePage(Staff staff)
        {
            _currentStaff = staff;
            InitializeComponent();
            DataContext= _currentStaff;
            if (staff.Role.Equals(IgoraEntities.GetContext().Roles.Where(p => p.Name == "Администратор")))
            {
                btnCloseOrder.Visibility = Visibility.Collapsed;
            }
            if (staff.Role.Equals(IgoraEntities.GetContext().Roles.Where(p => p.Name == "Старший смены")))
            {
                btnLoginStory.Visibility = Visibility.Collapsed;
                btnReports.Visibility = Visibility.Collapsed;
            }
            if (staff.Role.Equals(IgoraEntities.GetContext().Roles.Where(p => p.Name == "Продавец")))
            {
                btnLoginStory.Visibility = Visibility.Collapsed;
                btnReports.Visibility = Visibility.Collapsed;
                btnCloseOrder.Visibility = Visibility.Collapsed;
            }
        }

        public ProfilePage()
        {
            InitializeComponent();

        }

        private void btnOrdersList_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new OrdersListPage(_currentStaff));
        }

        private void btnCloseOrder_Click(object sender, RoutedEventArgs e)
        {
            List<BarcodeViewModel> barcodesForClose = new List<BarcodeViewModel>();
            List <Order> orders= new List<Order>();
            orders = IgoraEntities.GetContext().Orders.ToList();
            BarcodeReader.Read();
            barcodesForClose = BarcodeReader.Decode();
            foreach (var barcode in barcodesForClose)
                foreach (var order in orders)
                    if (barcode.Value == order.Barcode.Value)
                    {
                        order.StatusId = 3;
                        order.DateOfClose = DateTime.Now.ToLocalTime();
                        IgoraEntities.GetContext().Orders.AddOrUpdate(order);

                        try
                        {
                            IgoraEntities.GetContext().SaveChangesAsync();
                            MessageBox.Show("Заказ " + order.Id.ToString() + ", Клиент: " 
                                            + order.Client.ShortName.ToString() + "Закрыт успешно!");
                        } catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message.ToString());
                        }
                    }
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLoginStory_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new EntryHistoryPage());
        }

        private void btnConsumableData_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
