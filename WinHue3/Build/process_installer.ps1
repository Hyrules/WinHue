$installer = $args[0];
$project = $args[1];
$path = Split-Path -Path $installer

$content = Get-Content -Path $installer -Encoding UTF8
$content = $content.Replace("[[[WINHUE3]]]",$project)
$utf8 = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllLines("$path\\installer.iss",$content,$utf8);
