FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ProjectManagement.sln ./
COPY Api/ProjectManagement.Api.csproj Api/
COPY Domain/ProjectManagement.Domain.csproj Domain/
COPY Application/ProjectManagement.Application.csproj Application/
COPY Infrastructure/ProjectManagement.Infrastructure.csproj Infrastructure/
RUN dotnet restore Api/ProjectManagement.Api.csproj

COPY . .
RUN dotnet publish Api/ProjectManagement.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ProjectManagement.Api.dll"]
