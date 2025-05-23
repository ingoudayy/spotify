# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage for static site
FROM nginx:alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 80
