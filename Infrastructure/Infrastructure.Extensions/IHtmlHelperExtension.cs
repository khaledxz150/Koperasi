using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Html;



namespace Infrastructure.Extensions;
public static class IHtmlHelperExtension
{

    private static readonly ConcurrentDictionary<PropertyInfo, Delegate> AttributeDelegates = new();


    /// <summary>
    /// Retrieves custom attributes using a cached delegate.
    /// </summary>
    /// <param name="propertyInfo">The property info.</param>
    /// <returns>An array of attributes.</returns>
    public static TValue GetAttributeValue<TAttribute, TValue>(
        PropertyInfo propertyInfo,
        Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
    {
        if (propertyInfo == null)
            throw new ArgumentNullException(nameof(propertyInfo));

        if (valueSelector == null)
            throw new ArgumentNullException(nameof(valueSelector));

        // Get or create the delegate to retrieve the attribute
        var attributeDelegate = AttributeDelegates.GetOrAdd(propertyInfo, prop =>
        {
            // Create a delegate to retrieve the specific attribute
            var attributeMethod = typeof(PropertyInfo).GetMethod(nameof(PropertyInfo.GetCustomAttributes), new[] { typeof(bool) });
            var target = Expression.Constant(prop, typeof(PropertyInfo));
            var call = Expression.Call(target, attributeMethod, Expression.Constant(true));
            var cast = Expression.TypeAs(call, typeof(object[]));
            var lambda = Expression.Lambda<Func<object[]>>(cast);
            return lambda.Compile();
        });

        // Invoke the delegate to get all attributes
        var attributes = ((Func<object[]>)attributeDelegate).Invoke();

        // Find the target attribute and extract the desired value
        var targetAttribute = attributes.OfType<TAttribute>().FirstOrDefault();
        return targetAttribute != null ? valueSelector(targetAttribute) : default;
    }


    public static List<TValue> GetAttributeValues<TAttribute, TValue>(
        PropertyInfo propertyInfo,
        Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
    {
        if (propertyInfo == null)
            throw new ArgumentNullException(nameof(propertyInfo));

        if (valueSelector == null)
            throw new ArgumentNullException(nameof(valueSelector));

        // Get or create the delegate to retrieve the attribute
        var attributeDelegate = AttributeDelegates.GetOrAdd(propertyInfo, prop =>
        {
            // Create a delegate to retrieve the specific attribute
            var attributeMethod = typeof(PropertyInfo).GetMethod(nameof(PropertyInfo.GetCustomAttributes), new[] { typeof(bool) });
            var target = Expression.Constant(prop, typeof(PropertyInfo));
            var call = Expression.Call(target, attributeMethod, Expression.Constant(true));
            var cast = Expression.TypeAs(call, typeof(object[]));
            var lambda = Expression.Lambda<Func<object[]>>(cast);
            return lambda.Compile();
        });

        // Invoke the delegate to get all attributes
        var attributes = ((Func<object[]>)attributeDelegate).Invoke();

        // Find all target attributes and extract the desired values
        var targetAttributes = attributes.OfType<TAttribute>().ToList();
        var values = targetAttributes.Select(valueSelector).ToList();

        return values;
    }

    /// <summary>
    /// Gets the string representation of an IHtmlContent.
    /// </summary>
    /// <param name="htmlContent">The HTML content.</param>
    /// <returns>The string representation.</returns>
    public static string GetString(this IHtmlContent htmlContent)
    {
        if (htmlContent is not null)
        {
            using (var writer = new StringWriter())
            {
                htmlContent.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                return writer.ToString();
            }
        }

        return "";
    }

    /// <summary>
    /// Converts a dictionary of attributes into an HTML attributes string.
    /// </summary>
    /// <param name="attributes">The dictionary containing attribute key-value pairs.</param>
    /// <returns>A string representing the HTML attributes.</returns>
    public static string ConvertAttributesToHtml(Dictionary<string, object> attributes)
    {
        if (!(attributes is not null))
            return string.Empty;

        var stringBuilder = new StringBuilder();

        foreach (var attribute in attributes)
        {
            if (attribute.Value != null)
            {
                // Escape attribute values for HTML safety
                var escapedValue = System.Net.WebUtility.HtmlEncode(attribute.Value.ToString());
                stringBuilder.Append($" {attribute.Key}=\"{escapedValue}\"");
            }
        }

        return stringBuilder.ToString();
    }
}
