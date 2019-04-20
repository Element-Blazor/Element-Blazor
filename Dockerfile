FROM registry.cn-shanghai.aliyuncs.com/wzyuchen/dotnetcoresdk:3.0.100-preview4 as builder
WORKDIR /app

COPY . ./
RUN cd /app/Blazui \
    dotnet restore --configfile=/app/nuget.config \
    dotnet publish Blazui.Server -c Release

FROM registry.cn-shanghai.aliyuncs.com/wzyuchen/dotnetcoreruntime:3.0.0-preview4 as production
WORKDIR /app
COPY --from=0 /app/Blazui/Blazui.Server/bin/Release/netcoreapp3.0/publish/ /app/
ENTRYPOINT [ "sh", "-c", "dotnet Blazui.Server.dll" ]