FROM mcr.microsoft.com/dotnet/core/sdk:3.0.100-preview4 as builder
WORKDIR /app

COPY . ./
RUN cd /app/Blazui \
    dotnet restore --configfile=/app/nuget.config \
    dotnet publish Blazui.Server -c Release

FROM mcr.microsoft.com/dotnet/core/runtime:3.0.0-preview4 as production
WORKDIR /app
COPY --from=0 /app/Blazui/Blazui.Server/bin/Release/netcoreapp3.0/publish/ /app/
ENTRYPOINT [ "sh", "-c", "dotnet Blazui.Server.dll" ]