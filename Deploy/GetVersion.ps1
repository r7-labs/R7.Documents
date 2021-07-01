#!/usr/bin/pwsh

$mainProject = "R7.Documents"

$currentFolder = Get-Location

cd ("../" + $mainProject + "/Properties")
$assemblyInfoFile = Get-ChildItem | Where-Object {$_.Name -match "^SolutionInfo.cs$"}
$assemblyInfoContent = Get-Content $assemblyInfoFile

$pattern = [Regex]::new("AssemblyInformationalVersion\s*\(`"(?<Version>[^`"]+)")
$match = $pattern.Match($assemblyInfoContent)
if (($match.Groups.Values).Count -gt 0) {
    $match.Groups["Version"].Value
}
else {
    $pattern = [Regex]::new("AssemblyVersion\s*\(`"(?<Version>[^`"]+)")
    $match = $pattern.Match($assemblyInfoContent)
    if (($match.Groups.Values).Count -gt 0) {
        $match.Groups["Version"].Value
    }
}

cd $currentFolder

