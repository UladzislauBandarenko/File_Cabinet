namespace FileCabinetApp.Models;

/// <summary>
/// Help message.
/// </summary>
public class HelpMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelpMessage"/> class.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="shortDescription">The short description.</param>
    /// <param name="detailedDescription">The detailed description.</param>
    public HelpMessage(string command, string shortDescription, string detailedDescription)
    {
        this.Command = command;
        this.ShortDescription = shortDescription;
        this.DetailedDescription = detailedDescription;
    }

    /// <summary>
    /// Gets or sets the command.
    /// </summary>
    /// <value>
    /// The command.
    /// </value>
    public string Command { get; set; }

    /// <summary>
    /// Gets or sets the short description.
    /// </summary>
    /// <value>
    /// The short description.
    /// </value>
    public string ShortDescription { get; set; }

    /// <summary>
    /// Gets or sets the detailed description.
    /// </summary>
    /// <value>
    /// The detailed description.
    /// </value>
    public string DetailedDescription { get; set; }
}