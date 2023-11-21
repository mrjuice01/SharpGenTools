Param(
    [Parameter(Mandatory = $true)][string] $PackedConfiguration,
    [Parameter(Mandatory = $true)][string] $Project
)

$localPackagesFolder = "$Project/LocalPackages"
$restorePackagesFolder = "$Project/RestoredPackages"
$sdkPackages = "SharpGenTools.Sdk", "SharpGen.Runtime"

if (!(Test-Path -Path $localPackagesFolder)) {
    mkdir $localPackagesFolder -ErrorAction Stop
}

if (!(Test-Path -Path $restorePackagesFolder)) {
    mkdir $restorePackagesFolder -ErrorAction Stop
}

Remove-Item $localPackagesFolder/*.nupkg -ErrorAction Stop

try {
    foreach ($sdkPackage in $sdkPackages) {
        Copy-Item $sdkPackage/bin/$PackedConfiguration/*.nupkg $localPackagesFolder -ErrorAction Stop
        $restoreFolderName = $sdkPackage.ToLower()
        $restorePath = "$restorePackagesFolder/$restoreFolderName"
        if (Test-Path -Path $restorePath) {
            Remove-Item -Recurse -Force $restorePath -ErrorAction Stop
        }
    }
} catch {
    Write-Error $_.Exception.Message
    return $false
}

return $true