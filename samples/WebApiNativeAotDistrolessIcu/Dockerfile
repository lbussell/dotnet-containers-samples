# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM mcr.microsoft.com/dotnet/sdk:10.0-aot AS build
WORKDIR /source

# Build the app
COPY --link . .
RUN --mount=type=cache,target=/root/.nuget \
    --mount=type=cache,target=/source/bin \
    --mount=type=cache,target=/source/obj \
    dotnet publish -o /app WebApi.csproj \
        && rm /app/*.dbg /app/*.Development.json


# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime-deps:10.0-noble-chiseled-extra
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./WebApi"]
