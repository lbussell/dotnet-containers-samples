set shell := ["pwsh", "-c"]

default:
    just --list

install:
    dotnet new install ./src/Templates/ConsoleApp/ --force

clean:
    get-childitem ./samples | foreach-object { remove-item -r -fo $_.FullName }

generate: clean install
    dotnet new container-console -o ./samples/ConsoleApp --force
