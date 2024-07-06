# File Cabinet Application

## Overview
The File Cabinet Application is a robust, console-based record management system designed to handle personal information efficiently. It offers a comprehensive set of features for creating, reading, updating, and deleting records, with support for various storage mechanisms and flexible validation rules. This application demonstrates the implementation of several design patterns and SOLID principles, resulting in a highly modular and extensible architecture.

## How to Use

1. Run the application with desired command-line arguments:
   - `--validation-rules` or `-v`: Specify validation rules (default or custom)
   - `--storage` or `-s`: Specify storage type (memory or file)
   - `--use-stopwatch`: Enable performance measurement
   - `--use-logger`: Enable operation logging

2. Use the following commands in the application:
   - `create`: Create a new record
   - `update`: Update existing records
   - `delete`: Delete records
   - `insert`: Insert a new record with a specific ID
   - `select`: Query records
   - `export`: Export records to CSV or XML
   - `import`: Import records from CSV or XML
   - `purge`: Remove deleted records (file storage only)
   - `stat`: Display record statistics
   - `help`: Show available commands
   - `exit`: Exit the application

## Features

### 1. Record Management
- **Create**: Add new personal records with details such as first name, last name, date of birth, age, salary, and gender.
- **Read**: Retrieve and display records based on various criteria.
- **Update**: Modify existing records, with support for updating multiple fields simultaneously.
- **Delete**: Remove records from the system, with options for both soft and hard deletion.

### 2. Storage Flexibility
- **In-Memory Storage**: Fast, volatile storage suitable for temporary data or testing.
- **File-Based Storage**: Persistent storage using a custom binary file format, allowing data to be preserved between sessions.

### 3. Validation System
- **Default Validation Rules**: Pre-configured rules for basic data integrity.
- **Custom Validation Rules**: Ability to define and apply custom validation logic for each field.
- **Composite Validators**: Combine multiple validation rules for complex data validation scenarios.

### 4. Data Import/Export
- **CSV Format**: Support for importing and exporting records in CSV format for easy data exchange.
- **XML Format**: Ability to import and export records in XML format, providing a more structured data representation.

### 5. Command-Line Interface
- **Interactive Console**: User-friendly command-line interface for interacting with the application.
- **Multiple Commands**: Supports a wide range of commands for various operations (e.g., create, update, delete, select, export, import).

### 6. Performance and Logging
- **Execution Time Measurement**: Optional stopwatch functionality to measure and display the execution time of operations.
- **Operation Logging**: Capability to log all service operations for auditing and debugging purposes.

## Architecture

### Core Components

1. **Program Class**
   - Serves as the entry point of the application.
   - Parses command-line arguments to configure the application.
   - Initializes the necessary services and command handlers.
   - Manages the main application loop for processing user commands.

2. **IFileCabinetService Interface**
   - Defines the contract for all record management operations.
   - Ensures consistency across different storage implementations.

3. **FileCabinetMemoryService Class**
   - Implements IFileCabinetService for in-memory storage.
   - Uses collections (List and Dictionary) to store and index records.
   - Provides fast access and modification of records.

4. **FileCabinetFilesystemService Class**
   - Implements IFileCabinetService for file-based storage.
   - Manages records in a custom binary file format.
   - Handles file I/O operations for persistent storage.

5. **Command Handlers**
   - Implements the Chain of Responsibility pattern.
   - Each handler (e.g., CreateCommandHandler, UpdateCommandHandler) is responsible for processing specific commands.
   - Allows easy addition of new commands without modifying existing code.

6. **Validator System**
   - Consists of various validator classes (e.g., FirstNameValidator, AgeValidator).
   - Uses the Composite pattern to combine multiple validators.
   - Employs the Builder pattern (ValidatorBuilder) for constructing complex validator objects.

7. **ServiceMeter and ServiceLogger Classes**
   - Implement the Decorator pattern to add performance measurement and logging capabilities to the IFileCabinetService.

### Data Flow

1. **User Input**: The user enters a command through the console interface.

2. **Command Processing**:
   - The input is passed through the chain of command handlers.
   - Each handler checks if it can process the command; if not, it passes to the next handler.

3. **Service Interaction**:
   - The appropriate handler interacts with the FileCabinetService (either memory or file-based).
   - If enabled, the ServiceMeter and ServiceLogger decorators intercept these calls to measure performance and log operations.

4. **Data Validation**:
   - For operations that modify data, the input is validated using the configured validators.
   - Composite validators ensure that all validation rules are applied.

5. **Storage Operation**:
   - The FileCabinetService performs the requested operation on the records.
   - For file-based storage, this involves reading from or writing to the file system.

6. **Result Presentation**:
   - The result of the operation is returned through the command handler chain.
   - Finally, the result is displayed to the user via the console.

## Design Patterns

1. **Chain of Responsibility**
   - Used in: Command handling system
   - Implementation: Each command handler (e.g., CreateCommandHandler, UpdateCommandHandler) can either handle a command or pass it to the next handler.
   - Benefit: Allows for easy addition of new commands and decouples command sending from command handling.

2. **Composite**
   - Used in: Validation system
   - Implementation: CompositeValidator class allows combining multiple IValidator<T> objects.
   - Benefit: Enables complex validation scenarios by composing simple validators.

3. **Builder**
   - Used in: ValidatorBuilder class
   - Implementation: Provides methods to add different types of validators and build the final IRecordValidator.
   - Benefit: Simplifies the construction of complex validator objects.

4. **Decorator**
   - Used in: ServiceMeter and ServiceLogger classes
   - Implementation: These classes wrap an IFileCabinetService instance, adding extra functionality.
   - Benefit: Allows adding performance measurement and logging without modifying the original service classes.

5. **Strategy**
   - Used in: Storage implementation selection
   - Implementation: Different storage strategies (memory and file) can be selected at runtime.
   - Benefit: Enables switching between storage implementations without changing client code.

6. **Factory Method**
   - Used in: CreateFileCabinetService method in Program class
   - Implementation: Creates the appropriate IFileCabinetService implementation based on configuration.
   - Benefit: Centralizes the creation logic for different service implementations.

7. **Singleton**
   - Used in: Config class
   - Implementation: Provides a single point of access for application configuration.
   - Benefit: Ensures consistent configuration across the application.

## Key Classes and Their Roles

1. **Program**
   - Role: Application entry point and overall coordinator
   - Responsibilities:
     - Parse command-line arguments
     - Initialize services and command handlers
     - Manage the main application loop

2. **FileCabinetRecord**
   - Role: Data model for a single record
   - Properties: Id, FirstName, LastName, DateOfBirth, Age, Salary, Gender

3. **IFileCabinetService**
   - Role: Core interface for record management
   - Key Methods: CreateRecord, GetRecords, FindBy*, UpdateRecords, DeleteRecords

4. **FileCabinetMemoryService**
   - Role: In-memory implementation of IFileCabinetService
   - Key Features:
     - Uses List<FileCabinetRecord> for storage
     - Implements indexing for efficient querying

5. **FileCabinetFilesystemService**
   - Role: File-based implementation of IFileCabinetService
   - Key Features:
     - Manages records in a binary file
     - Implements record status (active/inactive) for soft deletion

6. **CommandHandlerBase**
   - Role: Base class for all command handlers
   - Key Feature: Implements the SetNext method for building the handler chain

7. **ValidatorBuilder**
   - Role: Constructs validator objects based on configuration
   - Key Methods: AddFirstNameValidator, AddLastNameValidator, etc.

8. **ServiceMeter**
   - Role: Measures and reports execution time of service operations
   - Key Feature: Wraps IFileCabinetService methods with stopwatch functionality

9. **ServiceLogger**
   - Role: Logs all service operations
   - Key Feature: Wraps IFileCabinetService methods with logging functionality
