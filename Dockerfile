FROM mcr.microsoft.com/dotnet/sdk:6.0 AS builder
WORKDIR /app
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
COPY --from=builder /app/out .
RUN mkdir -p content
ENTRYPOINT ["dotnet", "API.dll"]
