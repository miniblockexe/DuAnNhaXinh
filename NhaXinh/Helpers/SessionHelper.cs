using System.Text.Json;

namespace NhaXinh.Helpers
{
    public static class SessionHelper
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            session.SetString(key, json);
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            var json = session.GetString(key);

            if (string.IsNullOrEmpty(json))
                return default;

            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return default;
            }
        }

        public static bool HasKey(this ISession session, string key)
        {
            return session.Keys.Contains(key);
        }

        public static void Remove(this ISession session, string key)
        {
            session.Remove(key);
        }
    }
}
