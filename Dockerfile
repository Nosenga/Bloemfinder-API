FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the project file
COPY ["BloemFinder.API/BloemFinder.API.csproj", "BloemFinder.API/"]
RUN dotnet restore "BloemFinder.API/BloemFinder.API.csproj"

# Copy everything else
COPY . .

# Publish the app
WORKDIR "/src/BloemFinder.API"
RUN dotnet publish "BloemFinder.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "BloemFinder.API.dll"]