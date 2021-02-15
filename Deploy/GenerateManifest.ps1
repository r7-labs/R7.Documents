#!/usr/bin/pwsh

param ([string]$templateFilename, [string]$releaseVersion)

$versionStripped = $releaseVersion -replace "-.*$"
$versionObj = New-Object -TypeName System.Version -ArgumentList $versionStripped

$build = $versionObj.Build
if ($build -lt 0) {
	$build = 0
}

$version = $versionObj.Major.ToString("00") + "." + $versionObj.Minor.ToString("00") + "." + $build.ToString("00")

$template = Get-Content $templateFilename

$template = $template.replace("{{Version}}", $version)

$template | Set-Content $($templateFilename -replace "\.template")
