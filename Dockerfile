
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY be.sln .          
COPY *.csproj ./        


RUN dotnet restore *.csproj


COPY . .


RUN dotnet build *.csproj -c Release -o /app


FROM build AS publish
RUN dotnet publish *.csproj -c Release -o /publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app


COPY --from=publish /publish .

ENTRYPOINT ["dotnet", "PGAdmin.dll"]
