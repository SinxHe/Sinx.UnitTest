FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sinx.Console.Web/Sinx.Console.Web.csproj", "Sinx.Console.Web/"]
RUN dotnet restore "Sinx.Console.Web/Sinx.Console.Web.csproj"
COPY . .
WORKDIR "/src/Sinx.Console.Web"
RUN dotnet build "Sinx.Console.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sinx.Console.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sinx.Console.Web.dll"]
