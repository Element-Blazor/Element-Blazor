FROM registry.cn-shanghai.aliyuncs.com/wzyuchen/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM registry.cn-shanghai.aliyuncs.com/wzyuchen/sdk:3.0-buster AS build
COPY . .
RUN dotnet restore "src/Blazui.ServerRender/Blazui.ServerRender.csproj"
WORKDIR "/src/Blazui.ServerRender"
RUN dotnet build "Blazui.ServerRender.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blazui.ServerRender.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blazui.ServerRender.dll"]