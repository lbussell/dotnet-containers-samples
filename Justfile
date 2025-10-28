set windows-shell := ["pwsh", "-c"]

# List all available commands
default:
    just --list

# Install the sample Dockerfile app templates
install:
    dotnet new install ./src/Templates/ConsoleApp/ --force

# Remove all generated sample templates
clean-samples:
    {{ if os() == "windows" { "get-childitem ./samples | foreach-object { remove-item -r -fo $_.fullname }" } else { "rm -rf ./samples/*/" } }}

# Generate samples from templates
generate-samples: install
    dotnet new container-console --force                    -o ./samples/ConsoleApp
    dotnet new container-console --force --distroless       -o ./samples/ConsoleAppDistroless
    dotnet new container-console --force --self-contained   -o ./samples/ConsoleAppSelfContained
    dotnet new container-console --force --aot              -o ./samples/ConsoleAppNativeAot
    dotnet new container-console --force --distroless --aot -o ./samples/ConsoleAppDistrolessAot

# Build all samples and generate markdown report of images
generate-markdown:
    dotnet run --project src/Generator

# Generate all assets from scratch
generate-all: install clean-samples generate-samples generate-markdown
