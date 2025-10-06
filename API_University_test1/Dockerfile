# Use the .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

WORKDIR /src

# Copy the .csproj file from your project folder
COPY ["API_University_test1/API_University_test1.csproj", "API_University_test1/"]

# Restore dependencies
RUN dotnet restore "API_University_test1/API_University_test1.csproj"

# Copy all source code
COPY . .

# Publish the app to the /app/publish folder
RUN dotnet publish "API_University_test1/API_University_test1.csproj" -c Release -o /app/publish

# Use the smaller runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy the published output
COPY --from=build /app/publish .

# Create the SQLite data folder
RUN mkdir -p /app/App_Data

# Expose port 8080 (Render uses this)
EXPOSE 8080

# Run the app
ENTRYPOINT ["dotnet", "API_University_test1.dll"]
