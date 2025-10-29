set shell := ["pwsh", "-c"]

registry_name := "dotnet-samples-registry"
registry_port := "5001"
registry_image := "ghcr.io/project-zot/zot"
registry_tag := "latest"

# List all available commands
default:
    just --list

# Install the sample Dockerfile app templates
install:
    dotnet new install ./src/Templates/ConsoleApp/ --force

# Remove all generated sample templates
clean-samples:
    get-childitem ./samples | foreach-object { remove-item -r -fo $_.fullname }

# Generate samples from templates
generate-samples: install
    dotnet new container-console --force                    -o ./samples/ConsoleApp
    dotnet new container-console --force --distroless       -o ./samples/ConsoleAppDistroless
    dotnet new container-console --force --self-contained   -o ./samples/ConsoleAppSelfContained
    dotnet new container-console --force --aot              -o ./samples/ConsoleAppNativeAot
    dotnet new container-console --force --distroless --aot -o ./samples/ConsoleAppDistrolessAot

# Build all samples and generate markdown report of images
generate-markdown:
    dotnet run --project src/Generator -- localhost:{{registry_port}}

# Generate all assets from scratch
generate-all: install clean-samples generate-samples start-registry generate-markdown
    @echo "Done. See images at https://localhost:{{registry_port}} or run stop-registry."

# Create a local container registry that images will be pushed to
@start-registry:
    docker run --rm -d -p {{registry_port}}:5000 --name {{registry_name}} {{registry_image}}

# Stop the local registry container
@stop-registry:
    docker stop {{registry_name}}
