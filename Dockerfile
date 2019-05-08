FROM registry.cn-shanghai.aliyuncs.com/wzyuchen/dotnetcoresdk:3.0.100-preview5 as builder
WORKDIR /app

COPY . ./
#RUN dotnet restore /app/Blazui/Blazui.Server --configfile=/app/nuget.config
RUN dotnet publish /app/Blazui/Blazui.Server -c Release

FROM registry.cn-shanghai.aliyuncs.com/wzyuchen/dotnetcoreaspnetruntime:3.0.0-preview5 as production
WORKDIR /app
COPY --from=0 /app/Blazui/Blazui.Server/bin/Release/netcoreapp3.0/publish/ /app/
ENTRYPOINT [ "sh", "-c", "dotnet Blazui.Server.dll" ]