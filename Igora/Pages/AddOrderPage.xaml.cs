using Microsoft.Win32;
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
    /// Логика взаимодействия для AddOrderPage.xaml
    /// </summary>
    public partial class AddOrderPage : Page
    {
        private Order _currentOrder = new Order();
        List<ComboBox> cbsServices = new List<ComboBox>();
        List<Service> servicesList = new List<Service>();
        public AddOrderPage( Order selectedOrder, Staff staff)
        {
            if (selectedOrder != null)
                _currentOrder = selectedOrder;
            else
            {
                _currentOrder.Status = IgoraEntities.GetContext().Statuses.Where(p => p.Name == "Новая").FirstOrDefault();
                _currentOrder.Staff = staff;
            }
            InitializeComponent();
            UpdateData();
            cbStatus.ItemsSource = IgoraEntities.GetContext().Statuses.ToList();
            cbsServices.Add(cbServices);
            DataContext = _currentOrder;
            tbCreaeteDate.Text = DateTime.Now.ToString("dd MMMM yyyy HH:mm");
            _currentOrder.CreateDate = DateTime.UtcNow.ToLocalTime();
        }

        private void btnAddService_Click(object sender, RoutedEventArgs e)
        {
            btnDelService.Visibility = Visibility.Visible;
            ComboBox cBService= new ComboBox();
                foreach(var i in cbsServices)
                { 
                Service service = (Service)i.SelectedItem;
                if(service != null)
              servicesList.Remove(IgoraEntities.GetContext().Services.ToList().FirstOrDefault(p => p.Name.Equals(service.Name)));
            }
            cBService.SelectionChanged += new SelectionChangedEventHandler(cbServices_SelectionChanged);
            cBService.ItemsSource = servicesList.Where(p=>p.InStock==true);
           cbsServices.Add(cBService);
            cBService.DisplayMemberPath="Name";
            stackPanelServices.Children.Add(cBService);
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            AddClientWindow addClientWindow= new AddClientWindow(null);
            addClientWindow.ShowDialog();
            if (addClientWindow.DialogResult==true)
                UpdateData();

        }


        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {

            foreach (var i in cbsServices)
            {
                Service service = (Service)i.SelectedItem;
                if (service != null)
                    _currentOrder.Services.Add(IgoraEntities.GetContext().Services.FirstOrDefault(p => p.Name.Equals(service.Name)));
            }
            tbPrice.Text = _currentOrder.Price.ToString();
        }

        private void btnCancelAddOrder_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void btnSaveNewOrder_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if(_currentOrder.Id == 0)
                errors.AppendLine("Введите номер заказа");
            if(_currentOrder.Services.Count==0)
                errors.AppendLine("Выберите услугу");
            if (_currentOrder.Client==null)
                errors.AppendLine("Выберите клиента");
            if (_currentOrder.HoursOfRent==0) 
                errors.AppendLine("Введите количество часов проката");
            if(_currentOrder.Price==0)
                errors.AppendLine("Рассчитайте стоимость");
            if (IgoraEntities.GetContext().Orders.Where(p => p.Id == _currentOrder.Id).Any())
                errors.AppendLine("Заказ с таким номером уже существует");

                if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            IgoraEntities.GetContext().Orders.AddOrUpdate(_currentOrder);

            try
            {
                IgoraEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена успешно");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            { 
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnBarcode_Click(object sender, RoutedEventArgs e)
        {
            
                try
                {
                if (tbOrderNum.Text.Trim().Length > 0 && int.Parse(tbOrderNum.Text.Trim()) > 0)
                {
                    Barcode.Value = _currentOrder.Barcode.Value;
                    barCodeLine.Visibility = Visibility.Visible;
                    Barcode.Visibility = Visibility.Visible;
                    if (SaveControl("PDF | *.pdf"))
                    {
                        tbOrderNum.IsEnabled = false;
                        btnBarcode.IsEnabled = false;
                        btnSaveNewOrder.IsEnabled = true;
                    }
                }
                } catch (Exception ex) 
                {
                    MessageBox.Show("Номер заказа может содержать в себе только цифры!");
                }
        }

        private bool SaveControl(string filter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = filter;
            if (saveFileDialog.ShowDialog() == true)
            {
                Barcode.SaveImage(saveFileDialog.FileName);
                return true;
            }
            else return false;
        }

        private void UpdateData()
        {
            try
            {

                IgoraEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());

                cbClients.ItemsSource = IgoraEntities.GetContext().Clients.ToList().OrderBy(p => p.Surname);
                servicesList = IgoraEntities.GetContext().Services.ToList();
                cbServices.ItemsSource = servicesList.Where(p => p.InStock == true);
            }
            catch (Exception ex) { }
        }

        private void cbServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ComboBox).IsEnabled= false;
            btnAddService.Visibility= Visibility.Visible;
        }

        private void btnDelService_Click(object sender, RoutedEventArgs e)
        {
            servicesList.Add(cbsServices[cbsServices.Count - 1].SelectedItem as Service);
            stackPanelServices.Children.Remove(cbsServices[cbsServices.Count - 1]);
            cbsServices.RemoveAt(cbsServices.Count-1);
            cbsServices[cbsServices.Count - 1].IsEnabled = true;
            if(cbsServices.Count == 1)
            {
                btnDelService.Visibility = Visibility.Collapsed;
                btnAddService.Visibility = Visibility.Collapsed;
            }
                
        }
    }
}
