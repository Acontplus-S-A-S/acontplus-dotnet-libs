using Acontplus.Reports.Exceptions;
using Acontplus.Reports.Security;
using Xunit;

namespace Acontplus.Reports.Tests.Security;

public class PathValidatorTests
{
    private readonly string _testBaseDirectory;

    public PathValidatorTests()
    {
        _testBaseDirectory = Path.Combine(Path.GetTempPath(), "ReportTests");
        Directory.CreateDirectory(_testBaseDirectory);
    }

    [Fact]
    public void ValidateAndResolvePath_WithValidPath_ReturnsFullPath()
    {
        // Arrange
        var requestedPath = "subfolder/report.rdlc";

        // Act
        var result = PathValidator.ValidateAndResolvePath(_testBaseDirectory, requestedPath);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith(_testBaseDirectory, result);
        Assert.EndsWith("report.rdlc", result);
    }

    [Fact]
    public void ValidateAndResolvePath_WithForwardSlashes_ReturnsFullPath()
    {
        // Arrange
        var requestedPath = "reports/invoices/template.rdlc";

        // Act
        var result = PathValidator.ValidateAndResolvePath(_testBaseDirectory, requestedPath);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("reports", result);
        Assert.Contains("invoices", result);
        Assert.EndsWith("template.rdlc", result);
    }

    [Fact]
    public void ValidateAndResolvePath_WithBackslashes_ReturnsFullPath()
    {
        // Arrange
        var requestedPath = @"reports\invoices\template.rdlc";

        // Act
        var result = PathValidator.ValidateAndResolvePath(_testBaseDirectory, requestedPath);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("reports", result);
        Assert.Contains("invoices", result);
        Assert.EndsWith("template.rdlc", result);
    }

    [Theory]
    [InlineData("../../../etc/passwd")]
    [InlineData("..\\..\\..\\windows\\system32\\config\\sam")]
    [InlineData("subfolder/../../sensitive.txt")]
    [InlineData("reports/../../../etc/passwd")]
    public void ValidateAndResolvePath_WithDirectoryTraversal_ThrowsException(string maliciousPath)
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidReportPathException>(() =>
            PathValidator.ValidateAndResolvePath(_testBaseDirectory, maliciousPath));

        Assert.Contains("relative directory references", exception.Message);
    }

    [Theory]
    [InlineData("report\0.rdlc")]
    [InlineData("sub\0folder/report.rdlc")]
    public void ValidateAndResolvePath_WithNullBytes_ThrowsException(string maliciousPath)
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidReportPathException>(() =>
            PathValidator.ValidateAndResolvePath(_testBaseDirectory, maliciousPath));

        Assert.Contains("null characters", exception.Message);
    }

    [Theory]
    [InlineData("C:\\Windows\\System32\\config.txt")]
    [InlineData("D:/sensitive/data.txt")]
    [InlineData("report:C:\\test.rdlc")]
    public void ValidateAndResolvePath_WithAbsolutePaths_ThrowsException(string maliciousPath)
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidReportPathException>(() =>
            PathValidator.ValidateAndResolvePath(_testBaseDirectory, maliciousPath));

        Assert.Contains("absolute references", exception.Message);
    }

    [Theory]
    [InlineData("\\\\server\\share\\file.txt")]
    [InlineData("\\\\malicious-server\\data\\report.rdlc")]
    public void ValidateAndResolvePath_WithUNCPaths_ThrowsException(string maliciousPath)
    {
        // Act & Assert
        var exception = Assert.Throws<InvalidReportPathException>(() =>
            PathValidator.ValidateAndResolvePath(_testBaseDirectory, maliciousPath));

        Assert.Contains("UNC paths", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ValidateAndResolvePath_WithNullOrEmptyPath_ThrowsException(string? invalidPath)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            PathValidator.ValidateAndResolvePath(_testBaseDirectory, invalidPath!));
    }

    [Fact]
    public void ValidateAndResolvePath_WithNullBaseDirectory_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            PathValidator.ValidateAndResolvePath(null!, "report.rdlc"));
    }

    [Fact]
    public void ValidateFileExtension_WithAllowedExtension_DoesNotThrow()
    {
        // Arrange
        var filePath = "report.rdlc";

        // Act & Assert
        var exception = Record.Exception(() =>
            PathValidator.ValidateFileExtension(filePath, ".rdlc", ".rdl"));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateFileExtension_WithDisallowedExtension_ThrowsException()
    {
        // Arrange
        var filePath = "malicious.exe";

        // Act & Assert
        var exception = Assert.Throws<InvalidReportPathException>(() =>
            PathValidator.ValidateFileExtension(filePath, ".rdlc", ".rdl"));

        Assert.Contains("not allowed", exception.Message);
        Assert.Contains(".exe", exception.Message);
    }

    [Fact]
    public void ValidateFileExtension_CaseInsensitive_DoesNotThrow()
    {
        // Arrange
        var filePath = "report.RDLC";

        // Act & Assert
        var exception = Record.Exception(() =>
            PathValidator.ValidateFileExtension(filePath, ".rdlc"));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateFileExtension_WithNoExtension_ThrowsException()
    {
        // Arrange
        var filePath = "report_without_extension";

        // Act & Assert
        var exception = Assert.Throws<InvalidReportPathException>(() =>
            PathValidator.ValidateFileExtension(filePath, ".rdlc", ".rdl"));

        Assert.Contains("no extension", exception.Message);
    }

    [Fact]
    public void ValidateFileExtension_WithEmptyAllowedList_DoesNotThrow()
    {
        // Arrange
        var filePath = "report.anything";

        // Act & Assert - Should not throw when no restrictions
        var exception = Record.Exception(() =>
            PathValidator.ValidateFileExtension(filePath));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidateFileExists_WithExistingFile_DoesNotThrow()
    {
        // Arrange
        var testFile = Path.Combine(_testBaseDirectory, "test.rdlc");
        File.WriteAllText(testFile, "test content");

        try
        {
            // Act & Assert
            var exception = Record.Exception(() =>
                PathValidator.ValidateFileExists(testFile));

            Assert.Null(exception);
        }
        finally
        {
            File.Delete(testFile);
        }
    }

    [Fact]
    public void ValidateFileExists_WithNonExistingFile_ThrowsException()
    {
        // Arrange
        var nonExistingFile = Path.Combine(_testBaseDirectory, "nonexistent.rdlc");

        // Act & Assert
        var exception = Assert.Throws<InvalidReportPathException>(() =>
            PathValidator.ValidateFileExists(nonExistingFile));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void ValidateAndResolvePath_PreventBreakout_ComplexScenario()
    {
        // Arrange - Try to break out using various techniques
        var maliciousPaths = new[]
        {
            "reports/./../../sensitive.txt",
            "reports/../../../etc/passwd",
            "./../../../windows/system32/config",
            "subfolder/.././../../../etc/passwd"
        };

        // Act & Assert
        foreach (var maliciousPath in maliciousPaths)
        {
            Assert.Throws<InvalidReportPathException>(() =>
                PathValidator.ValidateAndResolvePath(_testBaseDirectory, maliciousPath));
        }
    }

    [Theory]
    [InlineData("/leading/slash/report.rdlc")]
    [InlineData("\\leading\\backslash\\report.rdlc")]
    public void ValidateAndResolvePath_WithLeadingSlash_ResolvesCorrectly(string requestedPath)
    {
        // Act
        var result = PathValidator.ValidateAndResolvePath(_testBaseDirectory, requestedPath);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith(_testBaseDirectory, result);
        Assert.DoesNotContain("..", result);
    }
}
