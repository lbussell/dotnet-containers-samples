# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM mcr.microsoft.com/dotnet/sdk:10.0-aot AS build
WORKDIR /source

# Copy source code and publish app
COPY --link . .
RUN dotnet publish -o /app


# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled-extra
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./ConsoleAppNativeAotDistrolessIcu"]
