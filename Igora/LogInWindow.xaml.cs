using System;
using System.Collections;
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
    /// Логика взаимодействия для LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        private int _countEnties = 0;
        public LogInWindow()
        {
            InitializeComponent();
        }

        private void ShowPass_Checked(object sender, RoutedEventArgs e)
        {
            tbPassword.Text = pbPassword.Password;
            tbPassword.Visibility= Visibility.Visible;
            pbPassword.Visibility= Visibility.Collapsed;
        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {
            _countEnties++;
            string login = tbLogin.Text.Trim();
            string password;
            if (ShowPass.IsChecked ==true)
            password = tbPassword.Text.Trim();
            else
                password = pbPassword.Password.Trim();
            Staff staff = null;
            StringBuilder errors = new StringBuilder();
            if (login.Length < 3)
                errors.AppendLine("Логин должен содержать минимум 3 символа!");
            if(password.Length < 3) 
                errors.AppendLine("Пароль должен содержать минимум 3 символа!");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            Captcha captcha = new Captcha();
            if (_countEnties > 4)
            {
                captcha.ShowDialog();
            }
            if (captcha.DialogResult == false)
                return;

                using (IgoraEntities context = new IgoraEntities())
                {
                    staff = context.Staffs.Where(p => p.Login == login && p.Password == password).FirstOrDefault();
                    if (staff != null)
                        staff.Role = context.Roles.Where(p => p.Id == staff.RoleId).FirstOrDefault();
                }

                if (staff == null)
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    AddEntryHistory(login);
                    return;
                }

            if (staff != null)
                {
                List<EntryHistory> entryHistory = new List<EntryHistory>();
                entryHistory = IgoraEntities.GetContext().EntryHistories.Where(p => p.StaffId == staff.Id && p.EntryType == true).OrderBy(p => p.EntryDate).ToList();
                DateTime LastEntry = entryHistory.LastOrDefault().EntryDate;
                DateTime NewEntry = LastEntry.AddMinutes(13);
                if ((DateTime.Now -LastEntry).Duration() > TimeSpan.FromMinutes(13))
                {
                    MainWindow mainWindow = new MainWindow(staff);
                    mainWindow.Show();
                    this.Close();
                    AddEntryHistory(staff, true);
                }
                else
                {
                    MessageBox.Show("Вы можете начать сессию только через: " + Math.Round((NewEntry - DateTime.Now).TotalMinutes, 2).ToString() + " мин.");
                    AddEntryHistory(staff, false);
                }
                    
            }
                else
                if(login != "")
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
          

           
            
        }

        private void ShowPass_Unchecked(object sender, RoutedEventArgs e)
        {
            pbPassword.Password = tbPassword.Text;
            pbPassword.Visibility= Visibility.Visible;
            tbPassword.Visibility= Visibility.Collapsed;
        }

        private void AddEntryHistory(string login)
        {
            EntryHistory entryHistorystaff = null;
                entryHistorystaff = new EntryHistory
                {
                    EntryDate = DateTime.Now.ToLocalTime(),
                    StaffId = IgoraEntities.GetContext().Staffs.Where(p => p.Login == login).FirstOrDefault().Id,
                    EntryType = false

                };
                if (entryHistorystaff.StaffId != 0)
                {
                    IgoraEntities.GetContext().EntryHistories.AddOrUpdate(entryHistorystaff);
                    try
                    {

                        IgoraEntities.GetContext().SaveChanges();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message.ToString()); }
                }
            
            
        }

        private void AddEntryHistory(Staff staff, bool entryType)
        {
            EntryHistory entryHistorystaff = null;

                entryHistorystaff = new EntryHistory
                {
                    EntryDate = DateTime.Now.ToLocalTime(),
                    StaffId = staff.Id,
                    EntryType = entryType

                };
                    IgoraEntities.GetContext().EntryHistories.AddOrUpdate(entryHistorystaff);
                    try
                    {

                        IgoraEntities.GetContext().SaveChanges();
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message.ToString()); }
                
        }
    }
}
