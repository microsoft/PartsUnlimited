FROM microsoft/aspnetcore:2.0-nanoserver-sac2016 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY PartsUnlimited.sln ./
COPY src/PartsUnlimitedWebsite/PartsUnlimitedWebsite.csproj src/PartsUnlimitedWebsite/
COPY src/PartsUnlimited.Models/PartsUnlimited.Models.csproj src/PartsUnlimited.Models/
RUN dotnet restore -nowarn:msb3202,nu1503
RUN npm rebuild node-sass
COPY . .
#WORKDIR /src/PartsUnlimitedWebsite
#RUN npm rebuild node-sass
WORKDIR /src/src/PartsUnlimitedWebsite
#RUN npm rebuild node-sass
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN npm rebuild node-sass
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PartsUnlimitedWebsite.dll"]
