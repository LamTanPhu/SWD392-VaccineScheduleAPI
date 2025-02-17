# Use the official .NET runtime as the base image (updated to .NET 8.0)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET SDK to build the app (updated to .NET 8.0)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["VaccineScheduleAPI/VaccineScheduleAPI.csproj", "VaccineScheduleAPI/"]
RUN dotnet restore "VaccineScheduleAPI/VaccineScheduleAPI.csproj"
COPY . . 
WORKDIR "/src/VaccineScheduleAPI"
RUN dotnet build "VaccineScheduleAPI.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "VaccineScheduleAPI.csproj" -c Release -o /app/publish

# Create the final image to run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "VaccineScheduleAPI.dll"]
