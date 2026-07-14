namespace UMT.Backend.Services
{
    public static class DotEnv
    {
        public static void Load(string path = ".env")
        {
            if (!File.Exists(path))
                return;

            foreach (var line in File.ReadAllLines(path))
            {
                var trimmed = line.Trim();

                // Skip comments / empty
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                    continue;

                var parts = trimmed.Split('=', 2);

                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}