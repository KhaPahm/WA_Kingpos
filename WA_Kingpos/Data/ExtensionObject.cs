using System.Data;
using System.Globalization;
using System.Reflection;

namespace WA_Kingpos.Data
{
    public static class ExtensionObject
    {
        public static List<T> ToList<T>(this DataTable table)
        {
            var mapColumn = table.MappingModelProperty<T>();
            return table.Rows.Cast<DataRow>().Select(row => row.ToModel<T>(mapColumn)).ToList();

        }

        public static Dictionary<string, PropertyInfo> MappingModelProperty<T>(this DataTable table)
        {
            Type type = typeof(T);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite).ToList();
            Dictionary<string, PropertyInfo> ret = new();
            if (table != null)
            {
                foreach (DataColumn column in table.Columns)
                {
                    // Find the property that matches the column name (ignore case)
                    var property = properties.FirstOrDefault(p => p.Name.Equals(column.ColumnName, StringComparison.OrdinalIgnoreCase));
                    if (property != null)
                    {
                        ret[column.ColumnName] = property;
                    }
                }
            }
            return ret;
        }

        public static T ToModel<T>(this DataRow row, Dictionary<string, PropertyInfo> propertyMapping)
        {
            T obj = Activator.CreateInstance<T>();
            foreach (var prop in propertyMapping)
            {
                object value = row[prop.Key];
                if (value != null && value != DBNull.Value)
                {
                    try
                    {
                        prop.Value.SetValue(obj, value.ChangeType(prop.Value.PropertyType));
                    }
                    catch (Exception ex)
                    {
                        //new RowLog() { Action = $"{prop.Value.Name}, {value}, {prop.Value.PropertyType}", ErrMsg = ex.Message }.WriteLog();
                    }

                }
            }
            return obj;
        }
        public static object? ChangeType(this object? value, Type type)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                return Convert.ChangeType(value, Nullable.GetUnderlyingType(type)!);
            }
            return Convert.ChangeType(value, type);
        }

        public static T? ChangeType<T>(this object? value)
        {
            var type = typeof(T);
            if (value == null || value == DBNull.Value)
            {
                return default;
            }
            return (T?)ChangeType(value, type);
        }

        /// <summary>
        /// Chuyển object date thành string date, xóa time
        /// </summary>
        public static string? ToStrDate(this object? value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            return Convert.ToDateTime(value, CultureInfo.GetCultureInfo("en-GB")).ToString("dd/MM/yyyy");
        }


        public static T zeroOnNull<T>(this T? input) where T : struct
        {
            if (input == null || input.Equals(DBNull.Value))
            {
                return default;
            }
            else
            {
                return (T)input;
            }
        }

        public static string fmNum<T>(this T? value, string temp = "N0") where T : struct, IFormattable
        {
            if (!value.HasValue || value.Value.Equals(default(T)))
            {
                return "";
            }
            return value.Value.ToString(temp, null);
        }

        /// <summary>
        /// Khi cần Format ngày tháng thì truyền format="{0:dd/MM/yyyy}"
        /// </summary>
        public static string toSqlPar(this object? value, string format = "")
        {
            if (value == null || value == DBNull.Value)
            {
                return "null";
            }
            else if (!string.IsNullOrEmpty(format))
            {
                return $"N'{string.Format(format, value)}'";
            }
            else
            {
                return $"N'{value}'";
            }

        }

    }
}
