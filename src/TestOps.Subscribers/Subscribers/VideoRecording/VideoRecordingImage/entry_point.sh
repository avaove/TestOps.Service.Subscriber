#!/usr/bin/env bash

echo "Starting job with the following parameters:"
echo "DISPLAY_NUM=${DISPLAY_NUM}"
echo "SE_SCREEN_WIDTH=${SE_SCREEN_WIDTH}"
echo "SE_SCREEN_HEIGHT=${SE_SCREEN_HEIGHT}"
echo "SE_FRAME_RATE=${SE_FRAME_RATE}"
echo "SE_CODEC=${SE_CODEC}"
echo "SE_PRESET=${SE_PRESET}"
echo "INPUT_IP=${INPUT_IP}"
echo "INPUT_SESSION_ID=${INPUT_SESSION_ID}"
echo "SE_AZ_BLOB_NAME=${SE_AZ_BLOB_NAME}"
echo "SE_AZ_ACCOUNT_NAME=${SE_AZ_ACCOUNT_NAME}"

/usr/bin/supervisord --configuration /etc/supervisord.conf &

# Wait for supervisor daemon to finish up before shutting down
wait ${SUPERVISOR_PID} 
