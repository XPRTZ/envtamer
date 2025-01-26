# envtamer

Taming digital environment files chaos with elegant simplicity.

A command-line tool for managing environment variables across different projects and directories.

## About

This tool is written in three languages to learn polyglot programming. The reference language is C# and the other two languages are Python and Go. The tool is designed to be cross-platform and to provide a consistent user experience regardless of the underlying implementation.

## Installation

### C#

In the root of the repository, run the following command to build the C# project:
```dotnet build -c Release dotnet/envtamer.sln```

### Python

### Go

## Usage

envtamer provides the following commands:

### init

Initializes an empty database in the user's home folder.

```envtamer init```

This command creates an empty SQLite database file named `envtamer.db` in the `.envtamer` directory of the user's home folder. If the file already exists, the command will not overwrite it.

### push

Pushes the contents of a local .env file to the database.

```envtamer push [DIRECTORY_NAME] [-f|--filename <FILENAME>]```

- `DIRECTORY_NAME`: Optional. The directory containing the .env file. Defaults to the current working directory.
- `-f|--filename`: Optional. The name of the env file. Defaults to '.env'.

This command reads the specified .env file and stores its contents in the database, associated with the given directory.

### pull

Pulls environment variables from the database to a local .env file.

```envtamer pull [DIRECTORY_NAME] [-f|--filename <FILENAME>]```


- `DIRECTORY_NAME`: Required. The directory in the database to pull env variables from.
- `-f|--filename`: Optional. The name of the env file to create or update. Defaults to '.env'.

This command retrieves stored environment variables for the specified directory from the database and writes them to a local .env file. If the file already exists, it will prompt for confirmation before overwriting.

### list

Lists stored directories or environment variables.

```envtamer list [DIRECTORY_NAME]```


- `DIRECTORY_NAME`: Optional. The directory to list env variables for.

If no directory is specified, this command lists all directories stored in the database. If a directory is provided, it lists all environment variables stored for that directory.

## Examples

Initialize the database:

```envtamer init```

Push a .env file from the current directory:

```envtamer push```

Pull environment variables for a specific directory:

```envtamer pull /path/to/project```

List all stored directories:

```envtamer list```

List environment variables for a specific directory:

```envtamer list /path/to/project```


## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
