using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;

namespace Infrastructure.Extensions;
public static class ListExtensions
{
    public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] this IEnumerable<T> list)
    {
        return !(list is null) && list.Any();
    }

    public static bool IsNotNullOrEmpty<T>([NotNullWhen(true)] this ICollection<T> list)
    {
        return !(list is null) && list.Any();
    }
    /// <summary>
    /// Checks if the collection is null or empty. If not, returns the collection as is; 
    /// otherwise, returns a new list with a default instance of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="list">The collection to check.</param>
    /// <returns>
    /// The original collection if it's not null or empty; otherwise, a new list 
    /// with a single default instance of <typeparamref name="T"/>.
    /// </returns>
    public static List<T> IsNotNullOrEmptyOrDefualt<T>([NotNullWhen(true)] this ICollection<T> list)
    {
        return !(list is null) && list.Any() ? list.ToList() : new List<T>() { Activator.CreateInstance<T>() };
    }


    public static DataTable ToIDDataTable(this long[] lstIDs)
    {
        DataTable table = new DataTable();
        var IDColumnName = "ID";
        table.Columns.Add(IDColumnName, typeof(long));
        object lockObject = new object();
        Parallel.ForEach(lstIDs, id =>
        {
            lock (lockObject)
            {
                var row = table.NewRow()[IDColumnName] = id;
                table.Rows.Add(row);
            }
        });

        return table;
    }

    public static DataTable ToIDDataTable(this int[] lstIDs)
    {
        DataTable table = new DataTable();
        var IDColumnName = "ID";
        table.Columns.Add(IDColumnName, typeof(long));
        object lockObject = new object();
        Parallel.ForEach(lstIDs, id =>
        {
            lock (lockObject)
            {
                var row = table.NewRow()[IDColumnName] = id;
                table.Rows.Add(row);
            }
        });

        return table;
    }

    public static DataTable ToIDDataTable(this List<int> lstIDs)
    {
        DataTable table = new DataTable();
        var IDColumnName = "ID";
        table.Columns.Add(IDColumnName, typeof(long));
        object lockObject = new object();
        Parallel.ForEach(lstIDs, id =>
        {
            lock (lockObject)
            {
                var row = table.NewRow()[IDColumnName] = id;
                table.Rows.Add(row);
            }
        });

        return table;
    }

    public static DataTable ToDataTable<T>(this List<T> lstItems)
    {
        var table = new DataTable();

        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
        {
            if (prop.Name is "Parser" or "Descriptor")
                continue;

            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        var lockObject = new object();

        //Parallel.ForEach(lstItems, item =>
        //{
        //    var row = table.NewRow();

        //    foreach (var prop in properties)
        //    {
        //        var value = prop.GetValue(item, null);
        //        row[prop.Name] = value ?? DBNull.Value;
        //    }

        //    lock (lockObject)
        //    {
        //        table.Rows.Add(row);
        //    }
        //});

        foreach (var item in lstItems)
        {
            var row = table.NewRow();

            foreach (var prop in properties)
            {
                if (prop.Name is "Parser" or "Descriptor")
                    continue;

                var value = prop.GetValue(item, null);
                row[prop.Name] = value ?? DBNull.Value;
            }

            table.Rows.Add(row);
        }


        return table;
    }

    public static string ConvertPropertyInfoWithCustomAttributeToJSON<T>(this IEnumerable<PropertyInfo> obj, Dictionary<int, string> localizer, string TrueProp = "ConditionTrueLocalizaionKey", string FalseProp = "ConditionFalseLocalizaionKey", string TrueClassProp = "ConditionTrueClass", string FalseClassProp = "ConditionFalseClass") where T : Attribute
    {
        if (obj.IsNotNullOrEmpty())
        {
            var people = new List<object>();
            foreach (var prop in obj)
            {
                var Attribute = prop.GetCustomAttributes(typeof(T), true).FirstOrDefault();
                if ((bool)Attribute.GetPropValue("IsBool"))
                {
                    people.Add(
                        new
                        {
                            PropertyName = prop.Name,
                            TrueDescription = localizer[(int)Attribute.GetPropValue(TrueProp)],
                            FalseDescription = localizer[(int)Attribute.GetPropValue(FalseProp)],
                            Attributes = Attribute,
                            TrueClass = localizer[(int)Attribute.GetPropValue(TrueClassProp)],
                            FalseClass = localizer[(int)Attribute.GetPropValue(FalseClassProp)]
                        });
                }
                else
                {
                    people.Add(new { PropertyName = prop.Name, TrueDescription = "", FalseDescriptions = "", Attributes = Attribute });
                }
            }

            var jsonString = JsonSerializer.Serialize(people);

            return jsonString;

        }

        return null;
    }

}
