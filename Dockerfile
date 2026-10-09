# 1. 빌드 단계 (SDK 사용)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 프로젝트 파일 복사 및 라이브러리 복원
COPY ["jarvisServer.csproj", "./"]
RUN dotnet restore "jarvisServer.csproj"

# 전체 소스코드 복사 및 Release 빌드
COPY . .
RUN dotnet publish "jarvisServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. 실행 단계 (가벼운 Runtime 사용)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 클라우드 환경용 포트 설정 (8080 포트 개방)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "jarvisServer.dll"]