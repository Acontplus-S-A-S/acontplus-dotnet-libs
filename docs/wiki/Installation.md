# Installation Guide

This page provides NuGet installation instructions for all Acontplus .NET libraries. Each library is distributed as a separate NuGet package and can be installed independently.

## 📦 NuGet Packages

Install the desired package(s) using either the NuGet Package Manager, .NET CLI, or by adding a PackageReference to your project file.

### NuGet Package Manager
```bash
Install-Package <PackageName>
```

### .NET CLI
```bash
dotnet add package <PackageName>
```

### PackageReference (in .csproj)
```xml
<ItemGroup>
  <PackageReference Include="<PackageName>" Version="x.y.z" />
</ItemGroup>
```

---

## Main Packages

| Library                        | NuGet Package Name                |
|--------------------------------|-----------------------------------|
| Acontplus.Core                 | Acontplus.Core                    |
| Acontplus.FactElect            | Acontplus.FactElect               |
| Acontplus.Notifications        | Acontplus.Notifications           |
| Acontplus.Reports              | Acontplus.Reports                 |
| Acontplus.Persistence.SqlServer| Acontplus.Persistence.SqlServer   |
| Acontplus.Services             | Acontplus.Services                |
| Acontplus.Utilities            | Acontplus.Utilities               |
| Acontplus.ApiDocumentation     | Acontplus.ApiDocumentation        |
| Acontplus.Logging              | Acontplus.Logging                 |
| Acontplus.Barcode              | Acontplus.Barcode                 |
| Acontplus.S3Application        | Acontplus.S3Application           |

---

For detailed usage and configuration, see the [Usage Guides](Home.md#📚-wiki-navigation). 