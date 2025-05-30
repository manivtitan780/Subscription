# PowerShell Script: Reset-TFConnection.ps1

# Set your Azure DevOps server URL and collection
$serverUrl = "https://maniv.visualstudio.com"
$collectionUrl = "$serverUrl/DefaultCollection"

# Remove the stored connection
Write-Host "Removing stored TF.exe connection for $serverUrl..."
& 'C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\TF.exe' settings connections /remove:$serverUrl

# Trigger a fresh login by listing workspaces
Write-Host "Triggering fresh login..."
& 'C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\TF.exe' workspaces /collection:$collectionUrl

Write-Host "You can now run checkin, get, or other tf.exe commands as the new user."
