using System.Collections.Generic;

namespace ForestBrush
{
    public static class Util
    {
        public static bool TryChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey oldKey, TKey newKey)
        {
            if (!dictionary.TryGetValue(oldKey, out TValue value) || dictionary.TryGetValue(newKey, out value))
                return false;
            dictionary.Remove(oldKey);
            dictionary.Add(newKey, value);
            return true;
        }
    }
}
