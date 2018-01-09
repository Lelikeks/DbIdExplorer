using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using DbIdExplorer.Annotations;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;

namespace DbIdExplorer
{
	internal class MainViewModel : INotifyPropertyChanged
	{
		private Guid? _id;
		private Guid _loadedGuid;
		private DataTable _dataTable;
		private object _selectedCell;
		private string _connectionString;

		public Guid? Id
		{
			get { return _id; }
			set
			{
				_id = value;
				OnPropertyChanged();
				SearchCommand.OnCanExecuteChanged();
			}
		}

		public object SelectedCell
		{
			get { return _selectedCell; }
			set
			{
				_selectedCell = value;
				OnPropertyChanged();
				SearchThisCommand.OnCanExecuteChanged();
			}
		}

		public string ConnectionString
		{
			get { return _connectionString; }
			set
			{
				_connectionString = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<TableItem> Tables { get; set; }

		public Command SearchCommand { get; set; }

		public Command SearchThisCommand { get; set; }

		public TableItem SelectedTable
		{
			set
			{
				if (value == null) return;
				DataTable = DbManager.GetData(ConnectionString, value, _loadedGuid);
			}
		}

		public DataTable DataTable
		{
			get { return _dataTable; }
			set
			{
				_dataTable = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		internal MainViewModel()
		{
			Tables = new ObservableCollection<TableItem>();

			SearchCommand = new Command(OnSearch, () => Id != null);
			SearchThisCommand = new Command(OnSearchThis, () => SelectedCell is Guid && (Guid)SelectedCell != _loadedGuid);
		}

		private void OnSearch()
		{
			if (Id == null) return;

			Tables.Clear();
			_loadedGuid = Id.Value;

			foreach (var table in DbManager.Search(ConnectionString, _loadedGuid).OrderBy(x => x.Name))
			{
				Tables.Add(table);
			}
		}

		private void OnSearchThis()
		{
			Id = (Guid) SelectedCell;
			SearchCommand.Execute(null);
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
