set shell := ["pwsh", "-c"]

default:
    just --list

install:
    dotnet new install ./src/Templates/ConsoleApp/ --force

clean:
    get-childitem ./samples | foreach-object { remove-item -r -fo $_.FullName }

generate: install
    dotnet new container-console --force                  -o ./samples/ConsoleApp
    dotnet new container-console --force --self-contained -o ./samples/ConsoleAppSelfContained
