FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /app

COPY src/*.csproj ./src/
WORKDIR /app/src
RUN dotnet restore

WORKDIR /app/
COPY src/. ./src/
WORKDIR /app/src
RUN dotnet publish -c Release -r linux-musl-x64 --self-contained -o out /p:PublishTrimmed=true /p:TrimMode=Link

FROM alpine
WORKDIR /app
RUN apk update
ENV PYTHONUNBUFFERED=1
RUN apk add --update --no-cache python3 && ln -sf python3 /usr/bin/python
RUN apk add py3-lxml
RUN apk add py3-pycryptodome
RUN apk add py3-pip
RUN pip3 install streamlink

COPY --from=build /app/src/out ./
ENTRYPOINT ["./nhltv-fetcher"]
