FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/Blazui.Admin.Docs/Blazui.Admin.Docs.csproj", "src/Blazui.Admin.Docs/"]
RUN dotnet restore "src/Blazui.Admin.Docs/Blazui.Admin.Docs.csproj"
COPY . .
WORKDIR "/src/src/Blazui.Admin.Docs"
RUN dotnet build "Blazui.Admin.Docs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Blazui.Admin.Docs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Blazui.Admin.Docs.dll"]
