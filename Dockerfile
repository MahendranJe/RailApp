# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["MyAPP/MyAPP.csproj", "MyAPP/"]
RUN dotnet restore "MyAPP/MyAPP.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/MyAPP"
RUN dotnet build "MyAPP.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MyAPP.csproj" -c Release -o /app/publish

# Use the runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install SQL Server drivers (if needed)
# RUN apt-get update && apt-get install -y curl apt-transport-https \
#     && curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - \
#     && curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list > /etc/apt/sources.list.d/mssql-release.list \
#     && apt-get update \
#     && ACCEPT_EULA=Y apt-get install -y msodbcsql18 unixodbc-dev

# Copy published app
COPY --from=publish /app/publish .

# Create directory for uploaded images
RUN mkdir -p /app/wwwroot/images/timetables && chmod 777 /app/wwwroot/images/timetables

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "MyAPP.dll"]
