FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY FCG.PaymentsAPI/*.csproj FCG.PaymentsAPI/
RUN dotnet restore FCG.PaymentsAPI/FCG.PaymentsAPI.csproj
COPY . .
RUN dotnet publish FCG.PaymentsAPI/FCG.PaymentsAPI.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
RUN mkdir -p Logs
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "FCG.PaymentsAPI.dll"]