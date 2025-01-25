namespace envtamer.Utils;

using System.Text.RegularExpressions;

public static class DirectorySanitizer
{
    private static readonly HashSet<string> ReservedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

    public static string SanitizeDirectoryName(string directoryPath)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
            return string.Empty;

        // Normalize path separators
        directoryPath = directoryPath.Replace('\\', '/');

        // Split the path into components
        var components = directoryPath.Split('/');

        for (int i = 0; i < components.Length; i++)
        {
            var component = components[i];

            // Remove invalid characters
            component = Regex.Replace(component, @"[^A-Za-z0-9_.-]", "_");

            // Replace multiple consecutive underscores
            component = Regex.Replace(component, @"_{2,}", "_");

            // Remove leading and trailing periods and spaces
            component = component.Trim('.', ' ');

            // Handle reserved names
            if (ReservedNames.Contains(component))
            {
                component = "_" + component;
            }

            // Convert to lowercase
            component = component.ToLowerInvariant();

            components[i] = component;
        }

        // Rejoin the path
        var sanitizedPath = string.Join("/", components);

        // Truncate if necessary
        if (sanitizedPath.Length > 255)
        {
            sanitizedPath = sanitizedPath.Substring(0, 255);
        }

        return sanitizedPath;
    }
}
