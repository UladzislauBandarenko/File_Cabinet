namespace FileCabinetApp.Utilities;

public static class InputReader
{
    public static T ReadInput<T>(string prompt, Func<string, Tuple<bool, string>> validator, Func<string, T> converter)
    {
        string input;
        bool isValid;
        string errorMessage;

        do
        {
            Console.Write($"{prompt}: ");
            input = Console.ReadLine();
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