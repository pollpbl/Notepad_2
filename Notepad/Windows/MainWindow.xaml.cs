using Notepad.BusinessLogic.ViewModels.Main;
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
using Unity;
using Unity.Resolution;

namespace Notepad.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindowAccess
    {
        private MainWindowViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = Dependencies.Container.Instance.Resolve<MainWindowViewModel>(new ParameterOverride("access", this));
            DataContext = viewModel;
        }

        private void TextBoxBase_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            viewModel.IsSelected = editorTextBox.SelectionLength > 0;
        }

        public string GetSelectedText()
        {
            return editorTextBox.SelectedText;
        }

        public int GetSelectedIndex()
        {
            return editorTextBox.SelectionStart;
        }

        public int GetSelectionLength()
        {
            return editorTextBox.SelectionLength;
        }
    }
}
