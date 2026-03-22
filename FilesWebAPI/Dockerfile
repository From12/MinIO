# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["FilesWebAPI/FilesWebAPI.csproj", "FilesWebAPI/"]
COPY ["Files.Application/Files.Application.csproj", "Files.Application/"]
COPY ["Files.Domain/Files.Domain.csproj", "Files.Domain/"]
COPY ["Files.Persistence/Files.Persistence.csproj", "Files.Persistence/"]
COPY ["MinIO/MinIOStorage.csproj", "MinIO/"]

# Restore dependencies
RUN dotnet restore "FilesWebAPI/FilesWebAPI.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/FilesWebAPI"
RUN dotnet build "FilesWebAPI.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FilesWebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Expose port
EXPOSE 8080
EXPOSE 8081

# Copy published application
COPY --from=publish /app/publish .

# Set entry point
ENTRYPOINT ["dotnet", "FilesWebAPI.dll"]