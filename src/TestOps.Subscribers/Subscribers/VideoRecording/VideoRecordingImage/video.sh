#!/bin/sh

normalize_session_id() {
    local input=$1
    local normalized=$(echo "${input}" | tr -d '-' | tr "[:upper:]" "[:lower:]")
    echo "${normalized}"
}

DISPLAY_NUM=${DISPLAY_NUM}
FRAME_RATE=${FRAME_RATE:-$SE_FRAME_RATE}
CODEC=${CODEC:-$SE_CODEC}
PRESET=${PRESET:-$SE_PRESET}
INPUT_IP=${INPUT_IP}
INPUT_SESSION_ID=${INPUT_SESSION_ID}
SESSION_ID=$(normalize_session_id "${INPUT_SESSION_ID}") 

node_status_url="http://$INPUT_IP:5555/status"
recording_started="false"
video_file_name=""
video_location="videos"
error_connection_refused="Connection refused"
error_connection_timed_out="Connection refused"

# Check if connection is made successfully, kill job if connection errors occur
status_response=$(curl --request GET "$node_status_url")
if [ "$status_response" = *"$error_connection_refused"* -o "$status_response" = *"$error_connection_timed_out"* ];
then
	echo "Connection to IP address refused or connection timed out"
	exit 0
fi

# Get active session ID from Grid status reponse
status_response=$(curl -s --request GET "$node_status_url")
session_id=$(echo $status_response | jq -r '.[]?.node?.slots | .[0]?.session?.sessionId')
session_id=$(normalize_session_id "$session_id") 

# Check session ID matches input session ID
if [ "$session_id" != "$SESSION_ID" ];
then
	echo "Session ${SESSION_ID} has ended already ended, recording will not take place"
	exit 0
fi


while true;
do
	status_response=$(curl -s --request GET "$node_status_url")
	session_id=$(echo $status_response | jq -r '.[]?.node?.slots | .[0]?.session?.sessionId')
	session_id=$(normalize_session_id "$session_id") 

	# Session ID is active, start recording
	if [ "$session_id" != "null" -a "$session_id" != "" ] && [ "$recording_started" = "false" ];
	then
		echo "Starting to record video for session ${session_id}"
		video_file_name="$session_id.mp4"
		video_location="${VIDEO_LOCATION}/${video_file_name}"
		exec ffmpeg -nostdin -y -f x11grab -r ${FRAME_RATE} -i ${INPUT_IP}:${DISPLAY_NUM}.0 -codec:v ${CODEC} ${PRESET} -pix_fmt yuv420p $video_location &
		recording_started="true"
		echo "Video recording started"
	# Session ID is not active, stop recording
	elif [ "$session_id" = "null" -o "$session_id" = "" ] && [ "$recording_started" = "true" ];
	then
		echo "Stopping to record video"
		pkill --signal INT ffmpeg
	    /opt/bin/start-azure-uploader.sh ${video_file_name} ${video_location} 
		recording_started="false"
		echo "Video recording stopped"
		echo "Killing job"
		exit 0
	elif [ "$recording_started" = "true" ];
	then
		echo "Video recording in progress"
	else
		echo "Session ${SESSION_ID} has ended already ended, recording will not take place"
		exit 0
	fi
	sleep 1
done