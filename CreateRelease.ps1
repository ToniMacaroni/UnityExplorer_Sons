$destinationDir = "UnityExplorerRelease"

if (!(Test-Path -Path $destinationDir)) {
    New-Item -ItemType Directory -Path $destinationDir | Out-Null
}

Set-Location -Path src
dotnet build -p:Configuration=Release_ML_Cpp_net6 -p:Platform="Any CPU"
Set-Location -Path ..

New-Item -ItemType Directory -Path "$destinationDir\Mods" | Out-Null
New-Item -ItemType Directory -Path "$destinationDir\Mods\UnityExplorer" | Out-Null
New-Item -ItemType Directory -Path "$destinationDir\Libs" | Out-Null

Copy-Item -Path "Release\UnityExplorer.RedLoader\UnityExplorer.dll" -Destination "$destinationDir\Mods\"
Copy-Item -Path "Release\UnityExplorer.RedLoader\manifest.json" -Destination "$destinationDir\Mods\UnityExplorer\"
Copy-Item -Path "Release\UniverseLib.Il2Cpp.Interop\UniverseLib.IL2CPP.Interop.dll" -Destination "$destinationDir\Libs\"

Set-Location -Path $destinationDir
Get-ChildItem -Path . | Compress-Archive -DestinationPath "..\$destinationDir.zip"
Set-Location -Path ..
Remove-Item -Path $destinationDir -Recurse -Force

Move-Item -Path "$destinationDir.zip" -Destination "UnityExplorer.zip" -Force

Write-Host "Process completed successfully."