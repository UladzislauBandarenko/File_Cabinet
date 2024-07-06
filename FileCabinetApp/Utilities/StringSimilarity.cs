using System.Collections.ObjectModel;

namespace FileCabinetApp.Utilities;

/// <summary>
/// Provides methods for calculating the Levenshtein distance between two strings and finding the most similar commands.
/// </summary>
public static class StringSimilarity
{
    /// <summary>
    /// Calculates the Levenshtein distance between two strings.
    /// </summary>
    /// <param name="s">The first string.</param>
    /// <param name="t">The second string.</param>
    /// <returns>The Levenshtein distance between the two strings.</returns>
    public static int LevenshteinDistance(string s, string t)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        if (t == null)
        {
            throw new ArgumentNullException(nameof(t));
        }

        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0)
        {
            return m;
        }

        if (m == 0)
        {
            return n;
        }

        for (int i = 0; i <= n; d[i, 0] = i++)
        {
        }

        for (int j = 0; j <= m; d[0, j] = j++)
        {
        }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }

    /// <summary>
    /// Finds the most similar commands to the input string.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="commands">The commands to compare against.</param>
    /// <param name="maxDistance">The maximum distance between the input string and the commands.</param>
    /// <returns>The most similar commands.</returns>
    public static ReadOnlyCollection<string> FindSimilarCommands(string input, IEnumerable<string> commands, int maxDistance = 3)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (commands == null)
        {
            throw new ArgumentNullException(nameof(commands));
        }

        var similarCommands = new List<string>();
        foreach (var command in commands)
        {
            int distance = LevenshteinDistance(input.ToLowerInvariant(), command.ToLowerInvariant());
            if (distance <= maxDistance)
            {
                similarCommands.Add(command);
            }
        }

        return similarCommands.OrderBy(c => LevenshteinDistance(input.ToLowerInvariant(), c.ToLowerInvariant())).ToList().AsReadOnly();
    }
}