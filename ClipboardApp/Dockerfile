FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_14.x | bash - \
    && apt-get install -y \
        nodejs \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /src
COPY ["ClipboardApp/ClipboardApp.csproj", "ClipboardApp/"]
RUN dotnet restore "ClipboardApp/ClipboardApp.csproj"
COPY . .
WORKDIR "/src/ClipboardApp"
RUN dotnet build "ClipboardApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClipboardApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClipboardApp.dll"]
