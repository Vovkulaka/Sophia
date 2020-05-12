#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Sophia.FrontApi/Sophia.FrontApi.csproj", "Sophia.FrontApi/"]
COPY ["DataProvider/DataProvider.csproj", "DataProvider/"]
COPY ["Configuration/Configuration.csproj", "Configuration/"]
COPY ["Logging/Logging.csproj", "Logging/"]
COPY ["MessageModelLib/MessageModelLib.csproj", "MessageModelLib/"]
RUN dotnet restore "Sophia.FrontApi/Sophia.FrontApi.csproj"
COPY . .
WORKDIR "/src/Sophia.FrontApi"
RUN dotnet build "Sophia.FrontApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sophia.FrontApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sophia.FrontApi.dll"]