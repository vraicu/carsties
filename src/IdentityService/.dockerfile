FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /app
EXPOSE 80

# copy all .csjproj files and restore as distinct layers
# use of the same COPY command for every dockerfile
# in the project to take advantage of docker caching
COPY carsties.sln carsties.sln
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY src/Contracts/Contracts.csproj src/Contracts/Contracts.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj
COPY tests/AuctionService.Tests/AuctionService.Tests.csproj tests/AuctionService.Tests/AuctionService.Tests.csproj
COPY tests/AuctionService.IntegrationTests/AuctionService.IntegrationTests.csproj tests/AuctionService.IntegrationTests/AuctionService.IntegrationTests.csproj
COPY src/BiddingService/BiddingService.csproj src/BiddingService/BiddingService.csproj
COPY src/NotificationService/NotificationService.csproj src/NotificationService/NotificationService.csproj
COPY tests/AuctionService.PlaywrightTests/AuctionService.PlaywrightTests.csproj tests/AuctionService.PlaywrightTests/AuctionService.PlaywrightTests.csproj

# restore package deps
RUN dotnet restore carsties.sln

# copy the app folders over 
COPY src/IdentityService src/IdentityService

WORKDIR /app/src/IdentityService
RUN dotnet publish -c Release -o /app/src/out 

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet", "IdentityService.dll" ]


