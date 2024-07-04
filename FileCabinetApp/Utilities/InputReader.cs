namespace FileCabinetApp.Utilities;

/// <summary>
/// Input reader.
/// </summary>
public static class InputReader
{
    /// <summary>
    /// Reads the input.
    /// </summary>
    /// <typeparam name="T">The type of the value to read.</typeparam>
    /// <param name="prompt">The prompt.</param>
    /// <param name="validator">The validator.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>The input.</returns>
    public static T ReadInput<T>(string prompt, Func<string, Tuple<bool, string>> validator, Func<string, T> converter)
    {
        if (converter is null)
        {
            throw new ArgumentNullException(nameof(converter));
        }

        if (validator is null)
        {
            throw new ArgumentNullException(nameof(validator));
        }

        string input;
        bool isValid;
        string errorMessage;

        do
        {
            Console.Write($"{prompt}: ");
            input = Console.ReadLine() ?? string.Empty;
            var validationResult = validator(input);
            isValid = validationResult.Item1;
            errorMessage = validationResult.Item2;

            if (!isValid)
            {
                Console.WriteLine($"Invalid input: {errorMessage}");
            }
        }
        while (!isValid);

        return converter(input);
    }
}