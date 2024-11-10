using System.Reflection;

namespace AsyncRestApi.Helpers
{
    internal class PropertyInfoComparer : IEqualityComparer<PropertyInfo>
    {
        public bool Equals(PropertyInfo? x, PropertyInfo? y)
        {
            return string.Equals(x?.Name, y?.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(PropertyInfo obj)
        {
            return obj.Name.ToLowerInvariant().GetHashCode();
        }
    }
}
