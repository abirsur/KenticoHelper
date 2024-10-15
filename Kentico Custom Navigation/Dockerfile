FROM python:3.12-slim

RUN apt-get update && apt-get install -y \
    fontconfig \
    libxrender1 \
    libxext6 \
    && apt-get clean

COPY ./mscorefonts/*.deb /tmp/
RUN dpkg -i /tmp/*.deb || apt-get -f install -y
RUN fc-cache -f -v
