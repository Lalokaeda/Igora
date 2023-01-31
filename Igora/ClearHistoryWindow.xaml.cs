using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для ClearHistoryWindow.xaml
    /// </summary>
    public partial class ClearHistoryWindow : Window
    {
        public ClearHistoryWindow()
        {
            InitializeComponent();
            List<string> periods = new List<string>()
            { 
                "Последние 24 часа", 
                "Последние 7 дней", 
                "Последние 30 дней", 
                "Последний год", 
                "Все время"
            };
            cbPeriod.ItemsSource = periods;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            TimeSpan Day=TimeSpan.FromHours(24);
            TimeSpan Week = TimeSpan.FromDays(7)+TimeSpan.FromMinutes(1);
            TimeSpan Month = TimeSpan.FromDays(30) + TimeSpan.FromMinutes(1);
            TimeSpan Year = TimeSpan.FromDays(365) + TimeSpan.FromMinutes(1);
           
            switch (cbPeriod.SelectedIndex)
            {
                case 0:
                    ClearHistory(Day);
                    break;
                case 1:
                    ClearHistory(Week);
                    break; 
                case 2:
                    ClearHistory(Month);
                    break;
                case 3:
                    ClearHistory(Year);
                    break;
                case 4:
                    ClearHistory();
                    break;

            }
            this.DialogResult = true; 
            this.Close();
        }

        public void ClearHistory(TimeSpan days)
        {
            List < EntryHistory > entryHistories = new  List<EntryHistory>();
            entryHistories = IgoraEntities.GetContext().EntryHistories.ToList();
            foreach (EntryHistory history in entryHistories)
                if ((DateTime.Now - history.EntryDate).Duration() < days)
            IgoraEntities.GetContext().EntryHistories.Remove(history);
                IgoraEntities.GetContext().SaveChangesAsync();
                MessageBox.Show("Данные удалены успешно!");
        }

        public void ClearHistory()
        {
            try
            {
                IgoraEntities.GetContext().EntryHistories.RemoveRange(IgoraEntities.GetContext().EntryHistories.ToList());
                IgoraEntities.GetContext().SaveChangesAsync();
                MessageBox.Show("Данные удалены успешно!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCancelClear_Click(object sender, RoutedEventArgs e)
        {
            DialogResult= false;
            this.Close();
        }
    }
}
