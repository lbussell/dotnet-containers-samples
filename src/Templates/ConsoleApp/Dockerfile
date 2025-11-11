# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
#if (!aot)
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG TARGETARCH
#else
FROM mcr.microsoft.com/dotnet/sdk:10.0-aot AS build
#endif
WORKDIR /source

#if (!aot)
# Copy project file and restore as distinct layers
COPY --link *.csproj .
RUN dotnet restore -a $TARGETARCH

#endif
# Copy source code and publish app
COPY --link . .
#if (!aot)
RUN dotnet publish -a $TARGETARCH --no-restore -o /app
#else
RUN dotnet publish -o /app \
    && rm /app/*.dbg
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
ENTRYPOINT ["./ConsoleApp"]
