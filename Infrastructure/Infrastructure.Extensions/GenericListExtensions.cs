using System.Data;
using System.Reflection;

namespace Infrastructure.Extensions
{
    public class GenericListExtensions<T>
    {
        private static readonly Dictionary<string, Func<T, object>> PropertyGetter = new Dictionary<string, Func<T, object>>();

        public static DataTable ToDataTable(IEnumerable<T> lst)
        {
            DataTable table = new DataTable();

            var properties = typeof(T).GetProperties();
            bool NeedGetter = false;
            if (!PropertyGetter.IsNotNullOrEmpty())
            {
                NeedGetter = true;
            }
            foreach (var prop in properties)
            {
                
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                if (NeedGetter)
                {
                    PropertyGetter[prop.Name] = CreateGetter(prop);
                }
            }
            foreach (var item in lst)
            {
                var row = table.NewRow();

                foreach (var prop in properties)
                {

                    if (item.IsNotNullOrEmpty() && PropertyGetter.TryGetValue(prop.Name, out var getter))
                    {
                        try
                        {
                            row[prop.Name] = getter(item);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                    //row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }


        private static Func<T, object> CreateGetter(PropertyInfo prop)
        {
            return prop.PropertyType switch
            {
                Type t when t == typeof(string) => (item) => (string)prop.GetValue(item),
                Type t when t == typeof(int) || t == typeof(int?) => (item) => prop.GetValue(item),
                Type t when t == typeof(bool) || t == typeof(bool?) => (item) => prop.GetValue(item),
                Type t when t.IsEnum => (item) => prop.GetValue(item),
                _ => (item) => Convert.ToString(prop.GetValue(item))
            };
        }

    }
}
