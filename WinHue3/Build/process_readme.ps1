$readme = $args[0];
$exe = $args[1];
$path = Split-Path -Path $readme
$version = (Get-Item $exe).VersionInfo.FileVersion
$content = Get-Content -Path $readme | Out-String
$content = $content.ToString().Replace("{release}",$version)
$content | Out-File -FilePath "$path\\readme.txt" -Force