using System.Reflection;

namespace Infrastructure.Extensions;
public static class ClassExtensions
{
    private static readonly Dictionary<Type, List<FieldInfo>> ResolvedDependencies = new Dictionary<Type, List<FieldInfo>>();

    public static void ResolveClassDependencies(this object obj,string TargetResolver, IHttpContextAccessor _httpContextAccessor = null, Type type = null, List<FieldInfo> DepencyProps = null)
    {
        IHttpContextAccessor httpContextAccessor = _httpContextAccessor ?? new HttpContextAccessor();
        var resolvedProps = new List<FieldInfo>();

        if (TargetResolver == type.Name)
        {
            var RequiredServicess = httpContextAccessor.HttpContext.RequestServices;

            if (DepencyProps is null)
            {
                DepencyProps = type.GetRuntimeFields().Where(x => x.IsInitOnly).ToList();
            }
            if (DepencyProps.IsNotNullOrEmpty())
            {
                foreach (var item in DepencyProps)
                {
                    if (!item.GetValue(obj).IsNotNullOrEmpty())
                    {
                        item.SetValue(obj, RequiredServicess.GetService(item.FieldType));
                    }
                    resolvedProps.Add(item);
                }
            }
        }
        else
        {
            if (type.BaseType.IsNotNullOrEmpty())
            {
                obj.ResolveClassDependencies(TargetResolver, httpContextAccessor, type.BaseType, DepencyProps);
            }
        }

        

        
    }

}
