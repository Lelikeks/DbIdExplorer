using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DbIdExplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void DataGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
		{
			if (!e.AddedCells.Any()) return;

			var index = ((DataGrid) sender).CurrentCell.Column.DisplayIndex;
			((MainViewModel) DataContext).SelectedCell = ((DataRowView) e.AddedCells.First().Item)[index];
		}

		private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			ViewModel.ConnectionString = Properties.Settings.Default.ConnectionString;
		}

		private void MainWindow_OnClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.ConnectionString = ViewModel.ConnectionString;
			Properties.Settings.Default.Save();
		}

		private MainViewModel ViewModel
		{
			get { return (MainViewModel) DataContext; }
		}

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var textBlock = new FrameworkElementFactory(typeof(TextBlock));
            textBlock.SetValue(TextBlock.TextProperty, new Binding
            {
                Path = new PropertyPath(e.PropertyName)
            });

            var style = new Style();
            var trigger = new Trigger
            {
                Property = TextBlock.TextProperty,
                Value = ViewModel.Id?.ToString()
            };
            trigger.Setters.Add(new Setter(TextBlock.BackgroundProperty, new SolidColorBrush(Color.FromRgb(240, 240, 240))));

            style.Triggers.Add(trigger);
            textBlock.SetValue(TextBlock.StyleProperty, style);

            var cell = new DataTemplate
            {
                VisualTree = textBlock
            };

            e.Column = new DataGridTemplateColumn
            {
                Header = e.PropertyName,
                CellTemplate = cell,
                ClipboardContentBinding = new Binding
                {
                    Path = new PropertyPath(e.PropertyName)
                }
            };
        }
    }
}
