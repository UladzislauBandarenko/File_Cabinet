namespace FileCabinetApp.Models;

/// <summary>
/// Help messages for the application.
/// </summary>
public static class HelpMessages
{
    /// <summary>
    /// Help messages for the application.
    /// </summary>
    public static readonly IReadOnlyCollection<HelpMessage> Messages = new[]
    {
        new HelpMessage("help", "prints the help screen", "The 'help' command prints the help screen."),
        new HelpMessage("exit", "exits the application", "The 'exit' command exits the application."),
        new HelpMessage("stat", "prints the number of records", "The 'stat' command prints the number of records."),
        new HelpMessage("create", "creates a new record", "The 'create' command creates a new record."),
        new HelpMessage("export", "exports records to a file", "The 'export' command exports all records to a file. Usage: export [csv|xml] <filename>"),
        new HelpMessage("import", "imports records from a file", "The 'import' command imports records from a file. Usage: import [csv|xml] <filename>"),
        new HelpMessage("purge", "purges all records", "The 'purge' command purges all records."),
        new HelpMessage("insert", "inserts a new record", "The 'insert' command inserts a new record. Usage: insert (id, firstname, lastname, dateofbirth, height, weight, gender) values (<id>, <firstname>, <lastname>, <dateofbirth>, <height>, <weight>, <gender>)"),
        new HelpMessage("delete", "deletes records", "The 'delete' command deletes records. Usage: delete where <field>=<value>"),
        new HelpMessage("update", "updates records", "The 'update' command updates records. Usage: update set <field1>=<value1>, <field2>=<value2>, ... where <field3>=<value3>, <field4>=<value4>, ..."),
        new HelpMessage("select", "selects records", "The 'select' command selects records based on specified fields and conditions. Usage: select <field1>, <field2>, ... [where <condition1> and/or <condition2> ...]"),
    };
}