param(
    [string]$Destination = (Join-Path $PSScriptRoot '..\..\.fitnesse'),
    [string]$Version = ''
)

$ErrorActionPreference = 'Stop'
$base = 'https://repo1.maven.org/maven2/org/fitnesse/fitnesse'

if (-not $Version) {
    [xml]$information = (Invoke-WebRequest "$base/maven-metadata.xml" -UseBasicParsing).Content
    $Version = $information.metadata.versioning.release
}
if (-not $Version) {
    throw 'Could not determine the FitNesse version.'
}

New-Item -ItemType Directory -Force -Path $Destination | Out-Null
$jar = Join-Path $Destination "fitnesse-$Version-standalone.jar"
if (-not (Test-Path $jar)) {
    $archive = "$base/$Version/fitnesse-$Version-standalone.jar"
    $partial = "$jar.part"
    Remove-Item -Force -ErrorAction SilentlyContinue $partial
    try {
        Invoke-WebRequest $archive -OutFile $partial -UseBasicParsing
        $expected = (Invoke-WebRequest "$archive.sha256" -UseBasicParsing).Content.Trim().ToLowerInvariant()
        $sha = [System.Security.Cryptography.SHA256]::Create()
        try {
            $stream = [System.IO.File]::OpenRead($partial)
            try {
                $actual = ([System.BitConverter]::ToString($sha.ComputeHash($stream)) -replace '-', '').ToLowerInvariant()
            }
            finally {
                $stream.Dispose()
            }
        }
        finally {
            $sha.Dispose()
        }
        if ($expected -ne $actual) {
            throw "Checksum mismatch for $archive"
        }
        Move-Item -Force $partial $jar
    }
    catch {
        Remove-Item -Force -ErrorAction SilentlyContinue $partial
        throw
    }
}
Write-Output (Resolve-Path $jar).Path
