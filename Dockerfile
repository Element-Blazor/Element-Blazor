FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
COPY . .
RUN dotnet restore "template/Samples/Renders/ServerRender/Element.ServerRender.csproj"
RUN dotnet build "template/Samples/Renders/ServerRender/Element.ServerRender.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "template/Samples/Renders/ServerRender/Element.ServerRender.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Element.ServerRender.dll"]
