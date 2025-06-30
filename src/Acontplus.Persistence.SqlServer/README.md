# Acontplus.Persistence.SqlServer

A .NET library providing common infrastructure components for database access and data operations.

## Overview

Acontplus.Persistece.SqlServer is a utility library that provides common persistence components and database access functionality for .NET applications. It's built on .NET 8.0 and integrates with Entity Framework Core.

## Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package Acontplus.Persistence.SqlServer
```

Or via the NuGet Package Manager Console:

```powershell
Install-Package Acontplus.Persistence.SqlServer
```

## Dependencies

- FastMember
- Microsoft.Data.SqlClient
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer

## Features

- Database context management
- Repository pattern implementation
- SQL Server integration
- Data reader mapping utilities
- Parameter handling helpers

## Project Structure

- BaseContext.cs - Base database context implementation
- DbContextFactory.cs - Factory for creating database contexts
- Repository/ - Repository pattern implementations
  - AdoRepository.cs - ADO.NET based repository
  - AdoSqlServer.cs - SQL Server specific implementations
  - IAdoRepository.cs - Repository interfaces
- Utils/ - Utility classes for data operations

## License

This project is licensed under the MIT License.

## Author

Ivan Paz

## Company

Acontplus S.A.S.

## Repository

[GitHub Repository]https://github.com/Acontplus-S-A-S/acontplus-dotnet-libs)

## Tags

database;ado-net;data-access;sql;orm;micro-orm;query;crud

## Contributing
We welcome contributions! Please submit any issues or feature requests via our GitHub repository, or feel free to fork the project and submit pull requests.

## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Author

[Ivan Paz](https://linktr.ee/iferpaz7)

## Company

[Acontplus S.A.S.](https://acontplus.com.ec)