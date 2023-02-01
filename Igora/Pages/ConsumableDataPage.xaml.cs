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
    /// Логика взаимодействия для ConsumableDataPage.xaml
    /// </summary>
    public partial class ConsumableDataPage : Page
    {
        public ConsumableDataPage()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(Visibility == Visibility.Visible)
            {
                try
                {
                    IgoraEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                }
                catch { }
                var consData = IgoraEntities.GetContext().Services.ToList();
                dgridConsData.ItemsSource = consData;
            }
          
        }
    }
}
