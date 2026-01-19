FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Schematics.sln ./
COPY src/Schematics.API/Schematics.API.csproj src/Schematics.API/

RUN dotnet restore Schematics.sln

COPY . .

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/Logs


EXPOSE 8080

ENTRYPOINT ["dotnet", "Schematics.API.dll"]