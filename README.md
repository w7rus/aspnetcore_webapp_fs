```
docker build . -t aspnetcore_webapp_fs
docker run -d -p 127.0.0.1:5001:5000 -v "$(pwd)"/aspnetcore_webapp_fs_content:/app/content --env-file=.env --name aspnetcore_webapp_fs aspnetcore_webapp_fs
```