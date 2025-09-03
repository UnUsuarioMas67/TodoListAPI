# TodoListAPI

RESTful API which allows users to register and manage their to-do list.

Solution for the roadmap.sh project: [Todo List API](https://roadmap.sh/projects/todo-list-api)

## Features

- Implements user registration.
- Generates JWT for user authentication
- CRUD operations for managing the to-do list
- Requires authorization to access the to-do list
- User and to-do list data is stored in a SQL database
- Implements pagination, filtering and sorting of the to-do list
- Implements rate limiting

## Requirements

- .NET 8 SDK or later
- Visual Studio 2022 or another IDE of choice
- SQL Server 2022

## Installation

### 1. Clone the repository

```bash
git clone https://github.com/UnUsuarioMas67/TodoListAPI
cd TodoListAPI
```

### 2. Setup Environment Variables

Add the following properties to `appsettings.Development.json`:

```
  "ConnectionStrings": {
    "TodoList": "YOUR SQL CONNECTION STRING"
  },
  "Jwt:SecretKey": "YOUR SECRET KEY"
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run the Application

```bash
cd TodoListAPI
dotnet run
```
