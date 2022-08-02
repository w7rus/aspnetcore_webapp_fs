FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-bullseye-slim AS base
WORKDIR /app

RUN apt update &&\
    apt-get install -y xz-utils libfontconfig1 curl && \
    curl https://johnvansickle.com/ffmpeg/releases/ffmpeg-release-amd64-static.tar.xz --output ffmpeg.tar.xz -s &&\
    tar -xf ffmpeg.tar.xz &&\
    rm -f ffmpeg.tar.xz &&\
    mv ffmpeg-5.0.1-amd64-static/ffmpeg ffmpeg-5.0.1-amd64-static/ffprobe /usr/local/bin &&\
    rm -rf ffmpeg-5.0.1-amd64-static

COPY --from=builder /app/out .
RUN mkdir -p content
ENTRYPOINT ["dotnet", "API.dll"]
