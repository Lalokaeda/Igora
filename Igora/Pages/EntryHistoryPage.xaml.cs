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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Igora.Pages
{
    /// <summary>
    /// Логика взаимодействия для EntryHistoryPage.xaml
    /// </summary>
    public partial class EntryHistoryPage : Page
    {
        public EntryHistoryPage()
        {
            InitializeComponent();
            (dgridEntryHistory.Columns[1] as DataGridTextColumn).Binding.StringFormat = "dd-MM-yyyy HH:mm";
            UpdateData();
        }

        private void btnClearHistory_Click(object sender, RoutedEventArgs e)
        {
            ClearHistoryWindow clearHistoryWindow = new ClearHistoryWindow();
            clearHistoryWindow.ShowDialog();
            if (clearHistoryWindow.DialogResult == true)
                UpdateData();

        }

        private void Page_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateData();
        }

        public void UpdateData()
        {
            IgoraEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
            dgridEntryHistory.ItemsSource = IgoraEntities.GetContext().EntryHistories.ToList().OrderByDescending(p=>p.EntryDate);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }
    }
}
