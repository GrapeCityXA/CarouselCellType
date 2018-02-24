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

namespace CarouselCellType
{
    /// <summary>
    /// Interaction logic for MyWindow.xaml
    /// </summary>
    public partial class MyWindow : Window
    {
        public MyWindow()
        {
            InitializeComponent();

            this.Owner = Application.Current.MainWindow;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private MyUserControl _dialogControl;
        public MyUserControl DialogControl
        {
            get
            {
                return this._dialogControl;
            }
            set
            {
                if (this._dialogControl != value)
                {
                    this._dialogControl = value;
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (DialogControl.Validate() == true)
            {
                DialogResult = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            var button = sender as Button;
            if (button == null)
            {
                return;
            }

            var win = GetWindow(button);
            if (win == null)
            {
                return;
            }

            win.Close();
        }
    }

    public interface IValidateValueDialog
    {
        bool Validate();
    }
    
    public class MyUserControl : UserControl, IValidateValueDialog
    {
        public virtual bool Validate()
        {
            return true;
        }
    }
}
