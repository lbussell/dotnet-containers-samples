# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
<!--#if (!aot) -->
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:SDK_IMAGE_TAG AS build
ARG TARGETARCH
<!--#else -->
FROM mcr.microsoft.com/dotnet/sdk:SDK_IMAGE_TAG AS build
<!--#endif -->
WORKDIR /source

<!--#if (!aot) -->
# Copy project file and restore as distinct layers
COPY --link *.csproj .
RUN dotnet restore -a $TARGETARCH

<!--#endif -->
# Copy source code and publish app
COPY --link . .
<!--#if (!aot) -->
RUN dotnet publish -a $TARGETARCH --no-restore -o /app
<!--#else -->
RUN dotnet publish -o /app
<!--#endif -->


# Runtime stage
FROM mcr.microsoft.com/dotnet/BASE_IMAGE_REPO:BASE_IMAGE_TAG
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./WebApi"]
