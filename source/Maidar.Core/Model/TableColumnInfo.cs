using MaiDarServer.Plugin.Models;

namespace MaiDar.Core.Model
{
	public class TableColumnInfo
	{
	    public TableColumnInfo()
		{
			ColumnName = string.Empty;
			DataType = DataType.NVarChar;
			Length = 0;
			IsPrimaryKey = false;
			IsIdentity = false;
		}

		public TableColumnInfo(string columnName, DataType dataType, int length, bool isPrimaryKey, bool isIdentity) 
		{
			ColumnName = columnName;
			DataType = dataType;
			Length = length;
			IsPrimaryKey = isPrimaryKey;
			IsIdentity = isIdentity;
		}

	    public string ColumnName { get; set; }

	    public DataType DataType { get; set; }

	    public int Length { get; set; }

	    public bool IsPrimaryKey { get; set; }

	    public bool IsIdentity { get; set; }
	}
}
