using Igora.Pages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Igora
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //  public Staff staff { get; set; }
        LogInWindow logInWindow = new LogInWindow();
        public MainWindow(Staff staff)
        {
            InitializeComponent();
            Manager.MainFrame = MainFrame;
            Manager.MainFrame.Navigate(new ProfilePage(staff));

        }
        SessionTimer sessionTimer = new SessionTimer();
        public MainWindow()
        {

            InitializeComponent();
            Manager.MainFrame = MainFrame;
            Manager.MainFrame.Navigate(new ProfilePage());

            //   Manager.MainFrame.Navigate(new ProfilePage(null));

        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
 
        }

        private async void SessionEnd()
        {
            await Task.Run(() =>
            {
                while(sessionTimer.TimeToEnd.TotalMilliseconds>1)
                {
                    if (sessionTimer.TimeToEnd.TotalMilliseconds == 300000)
                        MessageBox.Show("До конца сеанса осталось 5 минут!");
                }
            });
            logInWindow.Show();
            this.Close();
        }
        private void ImportEmp()
        {
            var fileData = File.ReadAllLines(@"C:\Users\Acer\Downloads\Коржина.7z_pass_123\Коржина\Коржина\Вариант 1\Сессия 1\Сотрудники_import\Сотрудники_import.txt");
            var images = Directory.GetFiles(@"C:\Users\Acer\Downloads\Коржина.7z_pass_123\Коржина\Коржина\Вариант 1\Сессия 1\Сотрудники_import\Staff");

            foreach (var line in fileData)
            {
                var data = line.Split('\t');
                string curRoleName = data[1].Replace("\"", "");
                var tempEmp = new Staff
                {
                    Id = int.Parse(data[0]),
                    RoleId = IgoraEntities.GetContext().Roles.FirstOrDefault(p => p.Name.Equals(curRoleName)).Id,
                    Surname = data[2].Replace("\"", ""),
                    Name = data[3].Replace("\"", ""),
                    Patronymic = data[4].Replace("\"", ""),
                    Login = data[5].Replace("\"", ""),
                    Password = data[6].Replace("\"", ""),
                   

                };
                var tempEntryHistory = new EntryHistory
                {
                    StaffId = IgoraEntities.GetContext().Staffs.FirstOrDefault(p => p.Id.Equals(tempEmp.Id)).Id,
                    EntryDate = DateTime.Parse(data[7]),
                    EntryType = (data[8].Replace("\"", "") == "Успешно" ? true : false)
                };
                try
                {
                    tempEmp.UserPic = File.ReadAllBytes(images.FirstOrDefault(p => p.Contains(tempEmp.Surname)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
               // IgoraEntities.GetContext().Staffs.AddOrUpdate(tempEmp);
                IgoraEntities.GetContext().EntryHistories.AddOrUpdate(tempEntryHistory);
                IgoraEntities.GetContext().SaveChanges();
            }
        }

            private void ImportOrders()
            {
                var fileData = File.ReadAllLines(@"C:\Users\Acer\Downloads\Коржина.7z_pass_123\Коржина\Коржина\Вариант 1\Сессия 1\Заказы_import.txt");

                foreach (var line in fileData)
                {
                    var data = line.Split('\t');
                    string curStatusName = data[4].Replace("\"", "");
                    var tempOrder = new Order
                    {
                        Id = int.Parse(data[0]),
                        StatusId = IgoraEntities.GetContext().Statuses.FirstOrDefault(p => p.Name.Equals(curStatusName)).Id,
                        CreateDate = DateTime.Parse(data[1]),
                        ClientId = int.Parse(data[2]),
                        HoursOfRent = int.Parse(data[6]),

                    };

                try
                {
                    tempOrder.DateOfClose = DateTime.Parse(data[5]);
                } catch
                {
                    tempOrder.DateOfClose = null;
                }

                foreach (var service in data[3].Replace("\"", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var currentService = IgoraEntities.GetContext().Services.ToList().FirstOrDefault(p => p.Id == int.Parse(service));
                    tempOrder.Services.Add(currentService);
                }

                IgoraEntities.GetContext().Orders.AddOrUpdate(tempOrder);
                    IgoraEntities.GetContext().SaveChanges();
                }
            }

        private void Window_Initialized(object sender, EventArgs e)
        {
            sessionTimer.StartTimer(DateTime.Now);
            TimerLog.DataContext = sessionTimer;
            SessionEnd();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            logInWindow.Show();
            this.Close();
        }
    }
}
