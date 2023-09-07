FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt update
RUN apt install nuget -y

RUN mkdir -p /nuget
COPY ./nuget/*.nupkg /nuget

# Copy code
WORKDIR /src
COPY ["TestOps.Service.Subscriber/src/TestOps.Service.Subscriber/TestOps.Service.Subscriber.csproj", "TestOps.Service.Subscriber/"]
COPY ["TestOps.Service.Subscriber/src/TestOps.Subscribers/TestOps.Subscribers.csproj", "TestOps.Subscribers/"]

COPY TestOps.Service.Subscriber/src/TestOps.Service.Subscriber/. TestOps.Service.Subscriber/
COPY TestOps.Service.Subscriber/src/TestOps.Subscribers/. TestOps.Subscribers/

# Copy nuget.config to the root directory so it's available in the same directory as the Dockerfile within the Docker build context
COPY TestOps.Service.Subscriber/NuGet.Config .

# Nuget
ARG DayforceFeedPwd
RUN sed -i "s|DF_FEED_PWD|$DayforceFeedPwd|" ./NuGet.Config
RUN dotnet restore "TestOps.Service.Subscriber/TestOps.Service.Subscriber.csproj"

# Build
WORKDIR "/src/TestOps.Service.Subscriber"
RUN dotnet build "TestOps.Service.Subscriber.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestOps.Service.Subscriber.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestOps.Service.Subscriber.dll"]