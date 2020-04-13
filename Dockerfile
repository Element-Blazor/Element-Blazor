FROM registry.cn-shanghai.aliyuncs.com/wzyuchen/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM registry.cn-shanghai.aliyuncs.com/wzyuchen/sdk:3.1-buster AS build
COPY . .
RUN dotnet restore "src/Samples/Blazui/Blazui.ServerRender/Blazui.ServerRender.csproj"
RUN dotnet build "src/Samples/Blazui/Blazui.ServerRender/Blazui.ServerRender.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Samples/Blazui/Blazui.ServerRender/Blazui.ServerRender.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blazui.ServerRender.dll"]