using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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
	}
}
