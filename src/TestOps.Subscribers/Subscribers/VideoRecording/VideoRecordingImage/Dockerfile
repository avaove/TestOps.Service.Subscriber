FROM jrottenberg/ffmpeg:4.3.1-ubuntu2004
LABEL authors="Selenium <selenium-developers@googlegroups.com>"

#================================================
# Customize sources for apt-get
#================================================
RUN  echo "deb http://archive.ubuntu.com/ubuntu focal main universe\n" > /etc/apt/sources.list \
  && echo "deb http://archive.ubuntu.com/ubuntu focal-updates main universe\n" >> /etc/apt/sources.list \
  && echo "deb http://security.ubuntu.com/ubuntu focal-security main universe\n" >> /etc/apt/sources.list

# No interactive frontend during docker build
ENV DEBIAN_FRONTEND=noninteractive \
    DEBCONF_NONINTERACTIVE_SEEN=true

#========================
# Miscellaneous packages
# Includes minimal runtime used for executing non GUI Java programs
#========================
RUN apt-get -qqy update \
  && apt-get -qqy --no-install-recommends install \
    ca-certificates \
    sudo \
    jq \
    curl \
    supervisor \
    gnupg2  

#========================
# Azure installation command
#========================
# Packages required to run the Azure CLI installation
RUN	apt-get update && apt install azure-cli -y

#========================
# Supervisor
#========================
RUN apt-get -qqy update \
  && rm -rf /var/lib/apt/lists/* /var/cache/apt/*

#======================================
# Add Supervisor configuration files
#======================================
COPY supervisord.conf /etc
COPY entry_point.sh video.sh start-azure-uploader.sh /opt/bin/
RUN cd /opt/bin 

RUN  mkdir -p /var/run/supervisor /var/log/supervisor /videos

ENTRYPOINT ["/opt/bin/entry_point.sh"]
CMD ["/opt/bin/entry_point.sh"]

ENV DISPLAY_NUM 99
ENV SE_FRAME_RATE 3
ENV SE_CODEC libx264
ENV SE_PRESET "-preset ultrafast"
ENV INPUT_IP=
ENV INPUT_SESSION_ID=
ENV SE_BLOB_STORAGE_KEY=
ENV SE_AZ_BLOB_NAME=
ENV SE_AZ_ACCOUNT_NAME=

EXPOSE 9000