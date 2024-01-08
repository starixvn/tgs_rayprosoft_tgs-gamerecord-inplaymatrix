#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.InplayMatrix/Tgs.GameRecord.InplayMatrix.csproj", "Tgs.GameRecord.InplayMatrix/"]
COPY ["Tgs.GameRecord.InplayMatrix.Core/Tgs.GameRecord.InplayMatrix.Core.csproj", "Tgs.GameRecord.InplayMatrix.Core/"]
COPY ["Tgs.GameRecord.InplayMatrix.Common/Tgs.GameRecord.InplayMatrix.Common.csproj", "Tgs.GameRecord.InplayMatrix.Common/"]
RUN dotnet restore "./Tgs.GameRecord.InplayMatrix/./Tgs.GameRecord.InplayMatrix.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.InplayMatrix"
RUN dotnet build "./Tgs.GameRecord.InplayMatrix.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.InplayMatrix.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.InplayMatrix.dll"]