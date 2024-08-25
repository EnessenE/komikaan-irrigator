FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["komikaan.Irrigator/komikaan.Irrigator.csproj", "komikaan.Irrigator/"]
RUN dotnet restore "komikaan.Irrigator/komikaan.Irrigator.csproj"
COPY . .
WORKDIR "/src/komikaan.Irrigator"
RUN dotnet build "komikaan.Irrigator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "komikaan.Irrigator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "komikaan.Irrigator.dll"]
