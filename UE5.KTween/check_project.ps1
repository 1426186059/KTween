$projDir = "d:\OpenSource\KTween\UE5.KTween\UE5_KTween"
$files = @("UE5_KTween.uproject", "Source\UE5_KTween.Target.cs", "Source\UE5_KTween\UE5_KTween.Build.cs", "Source\UE5_KTween\UE5_KTween.cpp", "Config\DefaultEngine.ini")

Write-Host "=== File encoding check ==="
foreach ($rel in $files) {
    $full = Join-Path $projDir $rel
    if (-not (Test-Path $full)) { Write-Host "NOT FOUND: $rel"; continue }
    $bytes = [System.IO.File]::ReadAllBytes($full)
    $hasBOM = ($bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF)
    $bomStr = if ($hasBOM) { "UTF8 BOM" } else { "NO BOM" }
    $content = [System.Text.Encoding]::UTF8.GetString($bytes)
    $lines = $content -split "`n"
    Write-Host "$rel : $bomStr, $($lines.Length) lines, $($bytes.Length) bytes"
    if ($lines.Length -le 1) { Write-Host "  WARNING: Only $($lines.Length) line(s)! May be corrupted." }
}

Write-Host "=== Directory structure ==="
$src = Join-Path $projDir "Source"
if (Test-Path $src) {
    Get-ChildItem $src -Recurse -File | ForEach-Object { Write-Host "  $($_.FullName)" }
} else { Write-Host "Source directory not found!" }
