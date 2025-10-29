# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
<!--#if (!stjSourceGen) -->
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:SDK_IMAGE_TAG AS build
ARG TARGETARCH
<!--#else -->
FROM mcr.microsoft.com/dotnet/sdk:SDK_IMAGE_TAG AS build
<!--#endif -->
WORKDIR /source

# Build the app
COPY --link . .
RUN --mount=type=cache,target=/root/.nuget \
    --mount=type=cache,target=/source/bin \
    --mount=type=cache,target=/source/obj \
    #if (!stjSourceGen)
    dotnet publish -o /app WebApi.csproj
    #else
    dotnet publish -o /app WebApi.csproj \
        && rm /app/*.dbg /app/*.Development.json
    #endif


# Runtime stage
FROM mcr.microsoft.com/dotnet/BASE_IMAGE_REPO:BASE_IMAGE_TAG
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./WebApi"]
