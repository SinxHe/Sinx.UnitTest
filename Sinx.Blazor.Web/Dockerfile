FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Sinx.Blazor.Web/Sinx.Blazor.Web.csproj", "Sinx.Blazor.Web/"]
RUN dotnet restore "Sinx.Blazor.Web/Sinx.Blazor.Web.csproj"
COPY . .
WORKDIR "/src/Sinx.Blazor.Web"
RUN dotnet build "Sinx.Blazor.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sinx.Blazor.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sinx.Blazor.Web.dll"]
