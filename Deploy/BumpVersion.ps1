#!/usr/bin/pwsh

param ([string]$version)

$mainProject = "R7.Documents"

$versionStripped = $version -replace "-.*$"
$projectFilesRegex = "Deploy\.csproj$|" + ($mainProject -replace "\.", "\.") + "(.*)\.csproj$"

$projectFilesRegex

$currentFolder = Get-Location

cd ".."
$solutionFile = Get-ChildItem | Where-Object {$_.Name -match "\.sln$"}
$solutionContent = Get-Content $solutionFile
$solutionContent = $solutionContent -replace "version = \d+.\d+.\d+", ("version = " + $version)
$solutionContent | Set-Content $solutionFile.FullName

cd ($mainProject + "/Properties")
$assemblyInfoFile = Get-ChildItem | Where-Object {$_.Name -match "^SolutionInfo.cs$"}
$assemblyInfoContent = Get-Content $assemblyInfoFile
$assemblyInfoContent = $assemblyInfoContent -replace "\[assembly: AssemblyVersion[^\]]+\]", ("[assembly: AssemblyVersion (""" + $versionStripped + """)]")
$assemblyInfoContent = $assemblyInfoContent -replace "\[assembly: AssemblyInformationalVersion[^\]]+\]", ("[assembly: AssemblyInformationalVersion (""" + $version + """)]")
$assemblyInfoContent | Set-Content $assemblyInfoFile.FullName

cd "../.."

$projectFiles = Get-ChildItem -Recurse | Where-Object {$_.Name -match $projectFilesRegex}
$projectFiles | ForEach-Object -Process {
    $projectFile = $_
    $projectFileContent = Get-Content $projectFile
    $projectFileContent = $projectFileContent -replace "<ReleaseVersion>[^<]+</ReleaseVersion>", ("<ReleaseVersion>" + $version + "</ReleaseVersion>")
    $projectFileContent | Set-Content $projectFile.FullName
}

cd $currentFolder
