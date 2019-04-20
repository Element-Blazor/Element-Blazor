FROM microsoft/dotnet:2.2-sdk as builder
WORKDIR /app

COPY . ./
RUN dotnet restore --configfile=/app/nuget.config
RUN dotnet publish Blazui/Blazui.Client -c Release

FROM microsoft/dotnet:2.2-aspnetcore-runtime as production
WORKDIR /app
COPY --from=0 /app/Blazui/Blazui.Client/bin/Release/netcoreapp2.2/publish/ /app/
ENTRYPOINT [ "sh", "-c", "dotnet Blazui.Client.dll" ]