FROM registry.cn-shanghai.aliyuncs.com/wzyuchen/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM registry.cn-shanghai.aliyuncs.com/wzyuchen/sdk:5.00-buster AS build
COPY . .
RUN dotnet restore "src/Samples/Element/Element.ServerRender/Element.ServerRender.csproj"
RUN dotnet build "src/Samples/Element/Element.ServerRender/Element.ServerRender.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Samples/Element/Element.ServerRender/Element.ServerRender.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Element.ServerRender.dll"]