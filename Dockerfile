# 1. 빌드 단계 (SDK 사용)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 프로젝트 파일 복사 및 라이브러리 복원 (*.csproj 사용으로 대소문자 오차 방지)
COPY *.csproj ./
RUN dotnet restore

# 전체 소스코드 복사 및 Release 빌드
COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# 2. 실행 단계 (가벼운 Runtime 사용)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# 클라우드 환경용 포트 설정 (8080 포트 개방)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# 빌드 결과물 DLL 파일 지정 (프로젝트명 대소문자 맞춤)
ENTRYPOINT ["dotnet", "JarvisServer.dll"]