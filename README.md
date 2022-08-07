.env (example)
```
ASPNETCORE_URLS=http://0.0.0.0:5000
MiscOptions__ContentPath=content
MiscOptions__CorsAllowedOrigins__0=*
MiscOptions__IsFilePreviewsEnabled=true
MiscOptions__FilePreviewMaxLongEdgeLength=512
SeqOptions__Endpoint__Scheme=<SCHEME>
SeqOptions__Endpoint__Host=<DOMAIN>
SeqOptions__Endpoint__Port=<PORT>
SeqOptions__ApiKey=<SEQ_INGEST_API_KEY>
```

./update.sh (example)
```
#!/bin/sh

BASE_DIR=$(dirname $0)

cd ${BASE_DIR}/repository && git checkout master && git pull

GIT_COMMIT=$(git rev-parse --short HEAD)

sudo docker build \
        -t aspnetcore_webapp_fs:latest \
        -t aspnetcore_webapp_fs:${GIT_COMMIT} \
        .

sudo docker container rm \
        -f aspnetcore_webapp_fs

sudo docker run \
        --add-host host.docker.internal:host-gateway \
        --restart=unless-stopped \
        -d \
        -p 172.17.0.1:6000:5000 \
        -v /home/admin/apps/aspnetcore_webapp_fs/content:/app/content \
        --env-file=/home/admin/apps/aspnetcore_webapp_fs/.env \
        --name aspnetcore_webapp_fs \
        aspnetcore_webapp_fs:latest
```
