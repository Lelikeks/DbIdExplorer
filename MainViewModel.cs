using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using DbIdExplorer.Annotations;
using System.Linq;

namespace DbIdExplorer
{
    internal class MainViewModel : INotifyPropertyChanged
	{
		private Guid? _id;
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

                if (!SavedConnectionStrings.Contains(_connectionString))
                {
                    SavedConnectionStrings.Insert(0, _connectionString);
                }
			}
		}

        public ObservableCollection<string> SavedConnectionStrings { get; } = new ObservableCollection<string>();

        public ObservableCollection<TableItem> Tables { get; set; }

		public Command SearchCommand { get; set; }

		public Command SearchThisCommand { get; set; }

		public TableItem SelectedTable
		{
			set
			{
				if (value == null || !Id.HasValue)
				{
					DataTable.Clear();
					return;
				}

                var dataTable = DbManager.GetData(ConnectionString, value, Id.Value);

                var num = 0;
                foreach (var column in value.Columns)
                {
                    dataTable.Columns[column.Name].SetOrdinal(num);
                    num++;
                }

                DataTable = dataTable;
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
			SearchThisCommand = new Command(OnSearchThis, () => SelectedCell is Guid && (Guid)SelectedCell != Id);
		}

		private void OnSearch()
		{
			if (!Id.HasValue) return;

			Tables.Clear();

			foreach (var table in DbManager.Search(ConnectionString, Id.Value).OrderBy(x => x.Name))
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
