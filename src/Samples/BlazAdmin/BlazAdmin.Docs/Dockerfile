FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/BlazAdmin.Docs/BlazAdmin.Docs.csproj", "src/BlazAdmin.Docs/"]
RUN dotnet restore "src/BlazAdmin.Docs/BlazAdmin.Docs.csproj"
COPY . .
WORKDIR "/src/src/BlazAdmin.Docs"
RUN dotnet build "BlazAdmin.Docs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazAdmin.Docs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazAdmin.Docs.dll"]