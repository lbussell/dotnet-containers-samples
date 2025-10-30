# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
#if (!aot)
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
#else
FROM mcr.microsoft.com/dotnet/sdk:10.0-aot AS build
#endif
WORKDIR /source

# Build the app
COPY --link . .
RUN --mount=type=cache,target=/root/.nuget \
    --mount=type=cache,target=/source/bin \
    --mount=type=cache,target=/source/obj \
    #if (!aot)
    dotnet publish -o /app WebApi.csproj
    #else
    dotnet publish -o /app WebApi.csproj \
        && rm /app/*.dbg /app/*.Development.json
    #endif


# Runtime stage
#if (distroless && globalization)
FROM mcr.microsoft.com/dotnet/BASE_IMAGE_REPO:10.0-noble-chiseled-extra
#elseif (distroless)
FROM mcr.microsoft.com/dotnet/BASE_IMAGE_REPO:10.0-noble-chiseled
#else
FROM mcr.microsoft.com/dotnet/BASE_IMAGE_REPO:10.0
#endif
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./WebApi"]
