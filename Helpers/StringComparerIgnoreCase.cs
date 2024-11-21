namespace ObjectScouter.Helpers
{
    internal class StringComparerIgnoreCase : IEqualityComparer<string?>
    {
        public bool Equals(string? x, string? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return string.Equals(x, y, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return obj?.ToLowerInvariant().GetHashCode() ?? 0;
        }
    }
}
