#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Sophia.Uploader/Sophia.Uploader.csproj", "Sophia.Uploader/"]
COPY ["DataProvider/DataProvider.csproj", "DataProvider/"]
COPY ["MessageModel/MessageModel.csproj", "MessageModel/"]
RUN dotnet restore "Sophia.Uploader/Sophia.Uploader.csproj"
COPY . .
WORKDIR "/src/Sophia.Uploader"
RUN dotnet build "Sophia.Uploader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sophia.Uploader.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sophia.Uploader.dll"]