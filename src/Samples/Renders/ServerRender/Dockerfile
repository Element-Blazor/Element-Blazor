#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["src/Samples/Blazui/Blazui.ServerRender/Element.ServerRender.csproj", "src/Samples/Blazui/Blazui.ServerRender/"]
COPY ["src/Components/Element.csproj", "src/Components/"]
COPY ["Element.Demo/Element.Demo.csproj", "Element.Demo/"]
COPY ["src/Markdown/Element.Markdown.csproj", "src/Markdown/"]
RUN dotnet restore "src/Samples/Blazui/Blazui.ServerRender/Element.ServerRender.csproj"
COPY . .
WORKDIR "/src/src/Samples/Blazui/Blazui.ServerRender"
RUN dotnet build "Element.ServerRender.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Element.ServerRender.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Element.ServerRender.dll"]