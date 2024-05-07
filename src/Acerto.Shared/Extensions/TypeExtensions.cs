using System.Reflection;

namespace Acerto.Shared.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Assembly> InternalLoadedAssemblies { get; } =
            new List<Assembly>(
                Assembly
                .GetEntryAssembly()?
                .GetReferencedAssemblies()
                .Where(a => a.FullName.StartsWith("Acerto"))
                .Select(Assembly.Load) ?? [])
                .Append(Assembly.GetEntryAssembly()!);
        public static IEnumerable<Type> InternalLocadedTypes { get; } =
            InternalLoadedAssemblies.
            SelectMany(x => x.DefinedTypes);

        public static bool IsConcrete(this Type type) => !type.IsInterface && !type.IsAbstract;

        public static bool IsClosedTypeOf(this Type type, Type openGeneric)
        {
            var typeInfo = type.GetTypeInfo();

            return typeInfo.IsGenericType && !typeInfo.ContainsGenericParameters &&
                type.GetGenericTypeDefinition() == openGeneric;
        }

        public static bool IsOpenGeneric(this Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return typeInfo.IsGenericTypeDefinition || typeInfo.ContainsGenericParameters;
        }

        public static IEnumerable<Type> GetInterfacesFromTypesOf(this Type type, Type[] interfaceTypes)
        {
            return type.GetInterfaces()
                .Where(t => t.IsGenericType && interfaceTypes.Any(i => i.IsAssignableFrom(t.GetGenericTypeDefinition())));
        }

        public static long ToUnixEpochDate(this DateTime date)
           => (long)Math.Round((date.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds);
    }
}
