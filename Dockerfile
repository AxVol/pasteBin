FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 7041

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["pasteBin/pasteBin.csproj", "pasteBin/"]
RUN dotnet restore "pasteBin/pasteBin.csproj"
COPY . .
WORKDIR "/src/pasteBin"
RUN dotnet build "pasteBin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "pasteBin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pasteBin.dll"]