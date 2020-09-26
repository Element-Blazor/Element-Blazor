FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/Element.Admin.Docs/Element.Admin.Docs.csproj", "src/Element.Admin.Docs/"]
RUN dotnet restore "src/Element.Admin.Docs/Element.Admin.Docs.csproj"
COPY . .
WORKDIR "/src/src/Element.Admin.Docs"
RUN dotnet build "Element.Admin.Docs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Element.Admin.Docs.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Element.Admin.Docs.dll"]
