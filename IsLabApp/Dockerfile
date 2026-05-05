FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
# *.csproj ./
RUN dotnet restore "IsLabApp.csproj"
#RUN dotnet build -c Release -o /app/build
RUN dotnet publish "IsLabApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

#COPY . ./
#RUN dotnet publish -c  Release  -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

#ENV ASPNETCORE_URLS=http://0.0.0.0:8080
#EXPOSE 8080
ENTRYPOINT ["dotnet", "IsLabApp.dll"]
