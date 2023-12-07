#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ToolSC/ToolSC.csproj", "ToolSC/"]
RUN dotnet restore "ToolSC/ToolSC.csproj"
COPY . .
WORKDIR "/src/ToolSC"
RUN dotnet build "ToolSC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ToolSC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToolSC.dll"]

#docker build -t toolsc-image .

#docker run -p 8080:80 --name toolsc-container -d toolsc-image
