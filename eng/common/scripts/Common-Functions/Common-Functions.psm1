# Helper functions for retireving useful information from azure-sdk-for-* repo
# Example Use : Import-Module .\Common-Module.psm1 -ArgumentList 'JavaScript','C:\Git\azure-sdk-for-net'
Param
(
    [Parameter(Position=0, Mandatory=$true)]
    [ValidateSet("net","java","js","python")]
    [string]
    $Language,
    [Parameter(Position=1, Mandatory=$true)]
    [string]
    $RepoRoot
)

function Extract-PkgProps ($pkgPath, $pkgName)
{
    if ($Language -eq "net") { Extract-DotNetPkgProps -pkgPath $pkgPath -pkgName $pkgName }
    if ($Language -eq "java") { Extract-JavaPkgProps -pkgPath $pkgPath -pkgName $pkgName }
    if ($Language -eq "js") { Extract-JsPkgProps -pkgPath $pkgPath -pkgName $pkgName }
    if ($Language -eq "python") { Extract-PythonPkgProps -pkgPath $pkgPath -pkgName $pkgName }
}

function Extract-DotNetPkgProps ($pkgPath, $pkgName)
{
    $projectPath = Join-Path $pkgPath "src" "$pkgName.csproj"
    if (Test-Path $projectPath)
    {
        $projectData = New-Object -TypeName XML
        $projectData.load($projectPath)

        $pkgVersion = Select-XML -Xml $projectData -XPath '/Project/PropertyGroup/Version'
        $pkgReadMePath = Join-Path $pkgPath "README.md"
        $pkgReadMePath = If (Test-Path ($pkgReadMePath)) {  $pkgReadMePath } Else { $null }
        $pkgChangeLogPath = Join-Path $pkgPath "CHANGELOG.md"
        $pkgChangeLogPath = If (Test-Path ($pkgChangeLogPath)) {  $pkgChangeLogPath } Else { $null }
        
        return @{
            pkgName = $pkgName;
            pkgVersion = $pkgVersion;
            pkgDirPath = $pkgPath;
            pkgReadMePath = $pkgReadMePath;
            pkgChangeLogPath = $pkgChangeLogPath;
        }
    } else 
    {
        return $null
    }
}

function Extract-JsPkgProps ($pkgPath, $pkgName)
{
    $projectPath = Join-Path $pkgPath "package.json"
    if (Test-Path $projectPath)
    {
        $projectJson = Get-Content $projectPath | Out-String | ConvertFrom-Json
        $jsStylePkgName = $pkgName.replace("azure-", "@azure/")
        if ($projectJson.name -eq "$jsStylePkgName")
        {
            $pkgReadMePath = Join-Path $pkgPath "README.md"
            $pkgReadMePath = If (Test-Path ($pkgReadMePath)) {  $pkgReadMePath } Else { $null }
            $pkgChangeLogPath = Join-Path $pkgPath "CHANGELOG.md"
            $pkgChangeLogPath = If (Test-Path ($pkgChangeLogPath)) {  $pkgChangeLogPath } Else { $null }

            return @{
                pkgName = $projectJson.name;
                pkgVersion = $projectJson.version;
                pkgDirPath = $pkgPath;
                pkgReadMePath = $pkgReadMePath;
                pkgChangeLogPath = $pkgChangeLogPath;
            }
        }
    }
    return $null
}

function Extract-PythonPkgProps ($pkgPath, $pkgName)
{
    if (Test-Path (Join-Path $pkgPath "setup.py"))
    {
        $setupLocation = $pkgPath.Replace('\','/')
        pushd $RepoRoot
        $setupProps = python -c "import scripts.devops_tasks.common_tasks; scripts.devops_tasks.common_tasks.parse_setup('$setupLocation')"
        popd
        if ($setupProps[0] -eq $pkgName)
        {
            $pkgReadMePath = Join-Path $pkgPath "README.md"
            $pkgReadMePath = If (Test-Path ($pkgReadMePath)) {  $pkgReadMePath } Else { $null }
            $pkgChangeLogPath = Join-Path $pkgPath "CHANGELOG.md"
            $pkgChangeLogPath = If (Test-Path ($pkgChangeLogPath)) {  $pkgChangeLogPath } Else { $null }

            return @{
                pkgName = $setupProps[0];
                pkgVersion = $setupProps[1];
                pkgDirPath = $pkgPath;
                pkgReadMePath = $pkgReadMePath;
                pkgChangeLogPath = $pkgChangeLogPath;
            }
        }
    }
    return $null
}

function Extract-JavaPkgProps ($pkgPath, $pkgName)
{
    $projectPath = Join-Path $pkgPath "pom.xml"
    if (Test-Path $projectPath)
    {
        $projectData = New-Object -TypeName XML
        $projectData.load($projectPath)

        $projectPkgName = Select-XML -Xml $projectData -XPath '/project/artifactId'
        $pkgVersion = Select-XML -Xml $projectData -XPath '/project/version'
        $pkgReadMePath = Join-Path $pkgPath "README.md"
        $pkgReadMePath = If (Test-Path ($pkgReadMePath)) {  $pkgReadMePath } Else { $null }
        $pkgChangeLogPath = Join-Path $pkgPath "CHANGELOG.md"
        $pkgChangeLogPath = If (Test-Path ($pkgChangeLogPath)) {  $pkgChangeLogPath } Else { $null }
        
        if ($projectPkgName -eq $pkgName)
        {
            return @{
                pkgName = $pkgName;
                pkgVersion = $pkgVersion;
                pkgDirPath = $pkgPath;
                pkgReadMePath = $pkgReadMePath;
                pkgChangeLogPath = $pkgChangeLogPath;
            }
        }
    }
    return $null
}

# Takes package name and service Name
# Returns important properties of the package as related to the language repo
# Returns a HashTable with Keys @ { pkgName, pkgVersion, pkgDirPath, pkgReadMePath, pkgChangeLogPath }
function Get-PkgProperties {
    Param
    (
        [Parameter(Mandatory=$true)]
        [string]$pkgName,
        [Parameter(Mandatory=$true)]
        [string]$serviceName
    )

    $pkgDirectoryName = $null
    $pkgDirectoryPath = $null
    $serviceDirectoryPath = Join-Path $RepoRoot "sdk" $serviceName
    if (!(Test-Path $serviceDirectoryPath))
    {
        Write-Error "Service Directory $serviceName does not exist"
        exit 1
    }

    $directoriesPresent = Get-ChildItem $serviceDirectoryPath -Directory

    foreach ($directory in $directoriesPresent)
    {
        $dirName = $directory.Name
        Write-Host $dirName

        switch -Regex ($pkgName) {
            "$dirName" { $pkgDirectoryName = $dirName; Break} #Exact Match
            "$dirName$" { $pkgDirectoryName = $dirName; Break} #EndsWith
            "^$dirName?" { $pkgDirectoryName = $dirName; Break} #StartsWith
        }

        if ($pkgDirectoryName -ne $null)
        {
            $pkgDirectoryPath = Join-Path $serviceDirectoryPath $pkgDirectoryName
            $pkgProps = Extract-PkgProps -pkgPath $pkgDirectoryPath -pkgName $pkgName
            if ($pkgProps -ne $null)
            {
                return $pkgProps
            }
            $pkgDirectoryName = $null
        }
    }
    Write-Error "Package Directory for $pkgName Path not Found"
    exit 1
}

Export-ModuleMember -Function 'Get-PkgProperties'