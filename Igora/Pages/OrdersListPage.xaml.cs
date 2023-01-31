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
using System.Xaml;
using Xceed.Wpf.AvalonDock.Controls;

namespace Igora.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersListPage.xaml
    /// </summary>
    public partial class OrdersListPage : Page
    {
        Staff _currentStaff = null;
        List<Service> selectedServices = new List<Service>();
        RadioButton pressedStatus = null;

        public OrdersListPage( Staff staff)
        {
            _currentStaff = staff;
            InitializeComponent();
            (dgridOrders.Columns[1] as DataGridTextColumn).Binding.StringFormat = "dd-MM-yyyy HH:mm";
            (dgridOrders.Columns[7] as DataGridTextColumn).Binding.StringFormat = "dd-MM-yyyy HH:mm";
            ccbSearchServices.ItemsSource = IgoraEntities.GetContext().Services.ToList();
        }


        private void btnStatusChange_Click(object sender, RoutedEventArgs e)
        {
            Order prokatOrder = new Order();
           prokatOrder = ((sender as Button).DataContext as Order);
            prokatOrder.StatusId = 2;
            IgoraEntities.GetContext().Orders.AddOrUpdate(prokatOrder);
            try
            {
                IgoraEntities.GetContext().SaveChanges();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           UpdateData();
        }


        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void imgAddOrder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddOrderPage(null, _currentStaff));
        }

        private void dgridOrders_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {
            if (dgridOrders.SelectedItem != null)
            if (((e.Row.Item) as Order).StatusId==1)
                e.Row.DetailsVisibility= Visibility.Visible;
        }

        private void UpdateData()
        {
            try
            {
                    IgoraEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
            }
            catch { }
            var currentOrders= IgoraEntities.GetContext().Orders.OrderBy(p => p.StatusId).ToList();
                dgridOrders.ItemsSource = currentOrders;

            if(pressedStatus!=null)
                currentOrders=currentOrders.Where(p=>
                p.Status.Name.Equals(pressedStatus.Content.ToString())).ToList();

            currentOrders = currentOrders.Where(p => p.Id.ToString().Contains(tbSearch.Text)).ToList();

            if (selectedServices.Count>0)
            {
                List<Order> orders= new List<Order>();
                orders.AddRange(currentOrders);
                currentOrders.Clear();
                foreach (var service in selectedServices)
                    currentOrders.AddRange(orders.Where(p =>p.Services.Contains(service)));
                dgridOrders.ItemsSource = currentOrders.Distinct();
            }
                
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                UpdateData();
            }
            }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            selectedServices.Add((sender as CheckBox).DataContext as Service);
            UpdateData();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            selectedServices.Remove((sender as CheckBox).DataContext as Service);
            UpdateData();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
             pressedStatus = (RadioButton)sender;
            UpdateData();
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateData();
        }
    }
}
