#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/AdminSkinCore.Api/AdminSkinCore.Api.csproj", "src/AdminSkinCore.Api/"]
RUN dotnet restore "src/AdminSkinCore.Api/AdminSkinCore.Api.csproj"
COPY . .
WORKDIR "/src/src/AdminSkinCore.Api"
RUN dotnet build "AdminSkinCore.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdminSkinCore.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

#���û���Ϊ������������Ŀ����ʱ��Ҫ�Լ�����һ�£�
ENV ASPNETCORE_ENVIRONMENT=Development

#����ʱ��Ϊ�й��Ϻ�
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

#����debianϵͳ  
RUN sed -i 's#http://deb.debian.org#https://mirrors.aliyun.com#g' /etc/apt/sources.list  
RUN apt-get update && apt-get install -y vim

ENTRYPOINT ["dotnet", "AdminSkinCore.Api.dll"]