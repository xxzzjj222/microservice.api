#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MicroService.Api.OrderService/MicroService.Api.OrderService.csproj", "src/MicroService.Api.OrderService/"]
RUN dotnet restore "src/MicroService.Api.OrderService/MicroService.Api.OrderService.csproj"
COPY . .
WORKDIR "/src/src/MicroService.Api.OrderService"
RUN dotnet build "MicroService.Api.OrderService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroService.Api.OrderService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroService.Api.OrderService.dll"]