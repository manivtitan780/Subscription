#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["Subscription.Server/Subscription.Server.csproj", "Subscription.Server/"]
COPY ["Extensions/Extensions.csproj", "Extensions/"]
COPY ["Subscription.Model/Subscription.Model.csproj", "Subscription.Model/"]
RUN dotnet restore "./Subscription.Server/Subscription.Server.csproj"
COPY . .
WORKDIR "/src/Subscription.Server"
RUN dotnet build "./Subscription.Server.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./Subscription.Server.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Subscription.Server.dll"]