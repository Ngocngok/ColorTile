Add-Type -AssemblyName System.IO.Compression.FileSystem

$docxPath = "E:\Huhu\COLOR TILES\Assets\Docs\COLOR TILES – Single Player.docx"
$outputPath = "E:\Huhu\COLOR TILES\Assets\Docs\game_design.txt"

$zip = [System.IO.Compression.ZipFile]::OpenRead($docxPath)
$entry = $zip.Entries | Where-Object { $_.Name -eq "document.xml" }
$stream = $entry.Open()
$reader = New-Object System.IO.StreamReader($stream)
$content = $reader.ReadToEnd()
$reader.Close()
$stream.Close()
$zip.Dispose()

$textContent = $content -replace '<[^>]+>', ' ' -replace '\s+', ' '
$textContent | Out-File -FilePath $outputPath -Encoding UTF8

Write-Host "Content extracted successfully"

