set windows-shell := ["pwsh", "-c"]

default:
    just --list

install:
    dotnet new install ./src/Templates/ConsoleApp/ --force

clean:
    {{ if os() == "windows" { "get-childitem ./samples | foreach-object { remove-item -r -fo $_.fullname }" } else { "rm -rf ./samples/*/" } }}

generate: install
    dotnet new container-console --force                    -o ./samples/ConsoleApp
    dotnet new container-console --force --distroless       -o ./samples/ConsoleAppDistroless
    dotnet new container-console --force --self-contained   -o ./samples/ConsoleAppSelfContained
    dotnet new container-console --force --aot              -o ./samples/ConsoleAppNativeAot
    dotnet new container-console --force --distroless --aot -o ./samples/ConsoleAppDistrolessAot
