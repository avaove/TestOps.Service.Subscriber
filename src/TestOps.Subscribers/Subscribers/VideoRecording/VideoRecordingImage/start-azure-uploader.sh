#!/usr/bin/env bash

SE_BLOB_STORAGE_KEY=${SE_BLOB_STORAGE_KEY}
SE_AZ_BLOB_NAME=${SE_AZ_BLOB_NAME}
SE_AZ_ACCOUNT_NAME=${SE_AZ_ACCOUNT_NAME}


echo "Uploading $1 to Azure blob"
export
az storage blob upload --account-name ${SE_AZ_ACCOUNT_NAME} --container-name ${SE_AZ_BLOB_NAME} --name $1 --file $2 --account-key ${SE_BLOB_STORAGE_KEY} --auth-mode key
echo "Finished uploading"