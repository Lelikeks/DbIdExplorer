using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DbIdExplorer
{
	internal class DbManager
	{
		internal static List<TableItem> Search(string connectionString, Guid id)
		{
			using (var cn = new SqlConnection(connectionString))
			using (var cm = cn.CreateCommand())
			{
				cm.CommandText = @"set nocount on

declare @table sysname, @column sysname, @schema sysname, @sql nvarchar(1000), @count int
declare @result table (schemaName sysname, tableName sysname, columnName sysname, amount int)

declare cur cursor local for
select t1.name, t2.name, t3.name from sys.tables t1 join sys.columns t2 on t1.object_id = t2.object_id
join sys.schemas t3 on t1.schema_id = t3.schema_id where t2.system_type_id = '36' and t2.name != 'id'
open cur
fetch next from cur into @table, @column, @schema

while @@fetch_status = 0
begin
	set @count = 0
	set @sql = 'select @count = count(*) from ' + quotename(@schema) + '.' + quotename(@table) + ' where ' + quotename(@column) + ' = @id'
	print @sql
	exec sp_executesql @sql, N'@id uniqueidentifier, @count int out', @id, @count out
	
	if @count > 0
		insert @result values (@schema, @table, @column, @count)

	fetch next from cur into @table, @column, @schema
end

close cur
deallocate cur

select schemaName, tableName, columnName, amount from @result";

				cm.Parameters.AddWithValue("id", id);

				cn.Open();
				using (var dr = cm.ExecuteReader())
				{
					var result = new List<TableItem>();
					while (dr.Read())
					{
						var schemaName = dr.GetString(0);
						var tableName = dr.GetString(1);

						var tableItem = result.Find(t => t.Schema == schemaName && t.Name == tableName);
						if (tableItem == null)
						{
							tableItem = new TableItem { Schema = schemaName, Name = tableName };
							result.Add(tableItem);
						}

						tableItem.Columns.Add(new ColumnItem
						{
							Name = dr.GetString(2),
							RowsCount = dr.GetInt32(3)
						});
					}
					return result;
				}
			}
		}

		internal static DataTable GetData(string connectionString, TableItem table, Guid id)
		{
			var filters = table.Columns.Select(c => string.Format("[{0}] = '{1}'", c.Name, id)).Aggregate((o, n) => o + " or " + n);
			var sql = string.Format("select * from [{0}].[{1}] where {2}", table.Schema, table.Name, filters);

			using (var da = new SqlDataAdapter(sql, connectionString))
			{
				var result = new DataTable();
				da.Fill(result);
				return result;
			}
		}
	}
}
