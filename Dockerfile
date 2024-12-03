FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GoOnlineTodo.Api/GoOnlineTodo.Api.csproj", "GoOnlineTodo.Api/"]
COPY ["GoOnlineTodo.DataService/GoOnlineTodo.DataService.csproj", "GoOnlineTodo.DataService/"]
COPY ["GoOnlineTodo.Entities/GoOnlineTodo.Entities.csproj", "GoOnlineTodo.Entities/"]
RUN dotnet restore "GoOnlineTodo.Api/GoOnlineTodo.Api.csproj"

COPY . .
WORKDIR "/src/GoOnlineTodo.Api"
RUN dotnet build "GoOnlineTodo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "GoOnlineTodo.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "GoOnlineTodo.Api.dll" ]