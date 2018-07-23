# Taken from psake https://github.com/psake/psake

<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

if(Test-Path .\src\artifacts) { Remove-Item .\src\artifacts -Force -Recurse }


$revision = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1 }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
$revision = "{0:D2}" -f [convert]::ToInt32($revision, 10)

exec { & dotnet build WebOptimizer.AngularTemplateCache.sln -c Release -/property:Version=1.0.$revision }

Push-Location -Path .\test\

try {
    exec { & dotnet test -c Release --no-build --no-restore }
} finally {
    Pop-Location
}

Pop-Location

exec { & dotnet pack .\src\WebOptimizer.AngularTemplateCache.csproj -c Release -o .\artifacts --include-symbols --no-build /p:PackageVersion=1.0.$revision }

