param()

# Function to increment version
function Increment-Version {
    param([string]$version, [string]$type)

    $parts = $version -split '\.'
    if ($parts.Length -ne 3) {
        throw "Invalid version format: $version"
    }

    $major = [int]$parts[0]
    $minor = [int]$parts[1]
    $patch = [int]$parts[2]

    if ($type -eq 'minor') {
        $minor++
    } elseif ($type -eq 'major') {
        $major++
        $minor = 0
        $patch = 0
    } else {
        throw "Invalid increment type: $type"
    }

    "$major.$minor.$patch"
}

# Get all .csproj files recursively, excluding application and test projects
$csprojs = Get-ChildItem -Recurse -Filter *.csproj | Where-Object {
    $_.FullName -notmatch '(?i)App|Test|Tests'
}

Write-Host "Found $($csprojs.Count) NuGet package projects to check."

foreach ($csproj in $csprojs) {
    Write-Host "Processing project: $($csproj.FullName)"

    # Get the relative path of the project directory
    $projectPath = $csproj.Directory.FullName.Replace("$PWD\", '').Replace('\', '/')

    # Check for code changes in the project directory (excluding .csproj)
    $statusOutput = & git status --porcelain 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Git not available or not a git repository. Skipping $($csproj.FullName)"
        continue
    }

    $changes = $statusOutput | Where-Object {
        $line = $_.Trim()
        if ($line.Length -lt 4) { return $false }
        $filePath = $line.Substring(3)
        ($filePath.StartsWith($projectPath) -and $filePath -match '\.(cs|vb)$' -and $filePath -notmatch '\.csproj$') -or $filePath -eq 'Directory.Packages.props'
    }

    if ($changes.Count -eq 0) {
        Write-Host "No code changes detected in project. Skipping version update."
        continue
    }

    Write-Host "Code changes detected: $($changes.Count) files changed."

    # Determine increment type based on latest commit message
    $commitMsg = & git log -1 --oneline 2>$null
    $incrementType = if ($commitMsg -match '(?i)breaking') { 'major' } else { 'minor' }
    Write-Host "Determined increment type: $incrementType (based on commit message: $commitMsg)"

    # Read and update the .csproj file
    $content = Get-Content $csproj.FullName -Raw
    $versionPattern = '<Version>(\d+\.\d+\.\d+)</Version>'

    if ($content -match $versionPattern) {
        $currentVersion = $matches[1]
        $newVersion = Increment-Version $currentVersion $incrementType
        $newContent = $content -replace $versionPattern, "<Version>$newVersion</Version>"
        Set-Content $csproj.FullName $newContent
        Write-Host "Successfully updated version from $currentVersion to $newVersion ($incrementType increment)."
    } else {
        Write-Host "Warning: Version tag not found or invalid format in $($csproj.FullName). Skipping update."
    }
}

Write-Host "Version upgrade process completed."
