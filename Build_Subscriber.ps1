param 
(
    $ShouldDeployACR='False'
)

$ACR_NAME="app541deploycr"

# Configs from ENV
$TPS_PAT_Dayforce_Encoded = [Environment]::GetEnvironmentVariable("TPS_PAT_Dayforce", "User")
$TPS_PAT_Dayforce = [System.Text.Encoding]::Unicode.GetString([System.Convert]::FromBase64String($TPS_PAT_Dayforce_Encoded))

if($ShouldDeployACR -ieq 'True')
{
    # Azure Build
    Write-Output "Building for Azure ACR " + $ACR_NAME
    az acr login --name $ACR_NAME
    az acr build -r $ACR_NAME -f .\Dockerfile ..\. --build-arg DayforceFeedPwd=$TPS_PAT_Dayforce -t tps_subscriber:dev_manual
}
else 
{
    # Docker Build
    Write-Output "Building Locally"
    docker build -f .\Dockerfile ..\. --build-arg DayforceFeedPwd=$TPS_PAT_Dayforce -t app541deploycr.azurecr.io/tps_subscriber:dev_manual
}

