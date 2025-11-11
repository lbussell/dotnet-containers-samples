# .NET Container Image Size Report

.NET offers a variety of deployment options for applications, which pair with container images that we offer. It's possible to produce very small container images. This document summarizes the available options to help you make the best choice for your apps and environment.

## Image Sizes

| Name                                 | OS     | Publish Type          | Distroless | Globalization | Compressed Size |
| ------------------------------------ | ------ | --------------------- | ---------- | ------------- | --------------: |
| ConsoleApp                           | Ubuntu | [Framework-dependent] | ✖️ No      | ✖️ No         |        80.30 MB |
| ConsoleAppIcu                        | Ubuntu | [Framework-dependent] | ✖️ No      | ✅ Yes         |        80.30 MB |
| ConsoleAppDistroless                 | Ubuntu | [Framework-dependent] | ✅ Yes      | ✖️ No         |        40.63 MB |
| ConsoleAppDistrolessIcu              | Ubuntu | [Framework-dependent] | ✅ Yes      | ✅ Yes         |        55.49 MB |
| ConsoleAppSelfContained              | Ubuntu | [Self-contained]      | ✖️ No      | ✖️ No         |        53.17 MB |
| ConsoleAppSelfContainedIcu           | Ubuntu | [Self-contained]      | ✖️ No      | ✅ Yes         |        53.29 MB |
| ConsoleAppSelfContainedDistroless    | Ubuntu | [Self-contained]      | ✅ Yes      | ✖️ No         |        13.50 MB |
| ConsoleAppSelfContainedDistrolessIcu | Ubuntu | [Self-contained]      | ✅ Yes      | ✅ Yes         |        28.48 MB |
| ConsoleAppNativeAot                  | Ubuntu | [Native AOT]          | ✖️ No      | ✖️ No         |        46.02 MB |
| ConsoleAppNativeAotIcu               | Ubuntu | [Native AOT]          | ✖️ No      | ✅ Yes         |        46.11 MB |
| ConsoleAppNativeAotDistroless        | Ubuntu | [Native AOT]          | ✅ Yes      | ✖️ No         |         6.35 MB |
| ConsoleAppNativeAotDistrolessIcu     | Ubuntu | [Native AOT]          | ✅ Yes      | ✅ Yes         |        21.31 MB |
| WebApi                               | Ubuntu | [Framework-dependent] | ✖️ No      | ✖️ No         |        92.48 MB |
| WebApiIcu                            | Ubuntu | [Framework-dependent] | ✖️ No      | ✅ Yes         |        92.48 MB |
| WebApiDistroless                     | Ubuntu | [Framework-dependent] | ✅ Yes      | ✖️ No         |        52.81 MB |
| WebApiDistrolessIcu                  | Ubuntu | [Framework-dependent] | ✅ Yes      | ✅ Yes         |        67.68 MB |
| WebApiSelfContained                  | Ubuntu | [Self-contained]      | ✖️ No      | ✖️ No         |        61.53 MB |
| WebApiSelfContainedIcu               | Ubuntu | [Self-contained]      | ✖️ No      | ✅ Yes         |        61.63 MB |
| WebApiSelfContainedDistroless        | Ubuntu | [Self-contained]      | ✅ Yes      | ✖️ No         |        21.86 MB |
| WebApiSelfContainedDistrolessIcu     | Ubuntu | [Self-contained]      | ✅ Yes      | ✅ Yes         |        36.82 MB |
| WebApiNativeAot                      | Ubuntu | [Native AOT]          | ✖️ No      | ✖️ No         |        51.27 MB |
| WebApiNativeAotIcu                   | Ubuntu | [Native AOT]          | ✖️ No      | ✅ Yes         |        51.36 MB |
| WebApiNativeAotDistroless            | Ubuntu | [Native AOT]          | ✅ Yes      | ✖️ No         |        11.60 MB |
| WebApiNativeAotDistrolessIcu         | Ubuntu | [Native AOT]          | ✅ Yes      | ✅ Yes         |        26.56 MB |
| ConsoleApp                           | Alpine | [Framework-dependent] | ✖️ No      | ✖️ No         |        39.75 MB |
| ConsoleAppIcu                        | Alpine | [Framework-dependent] | ✖️ No      | ✅ Yes         |        54.22 MB |
| ConsoleAppSelfContained              | Alpine | [Self-contained]      | ✖️ No      | ✖️ No         |        12.60 MB |
| ConsoleAppSelfContainedIcu           | Alpine | [Self-contained]      | ✖️ No      | ✅ Yes         |        27.18 MB |
| ConsoleAppNativeAot                  | Alpine | [Native AOT]          | ✖️ No      | ✖️ No         |         5.44 MB |
| ConsoleAppNativeAotIcu               | Alpine | [Native AOT]          | ✖️ No      | ✅ Yes         |        20.00 MB |
| WebApi                               | Alpine | [Framework-dependent] | ✖️ No      | ✖️ No         |        51.93 MB |
| WebApiIcu                            | Alpine | [Framework-dependent] | ✖️ No      | ✅ Yes         |        66.40 MB |
| WebApiSelfContained                  | Alpine | [Self-contained]      | ✖️ No      | ✖️ No         |        20.95 MB |
| WebApiSelfContainedIcu               | Alpine | [Self-contained]      | ✖️ No      | ✅ Yes         |        35.52 MB |
| WebApiNativeAot                      | Alpine | [Native AOT]          | ✖️ No      | ✖️ No         |        10.69 MB |
| WebApiNativeAotIcu                   | Alpine | [Native AOT]          | ✖️ No      | ✅ Yes         |        25.25 MB |

For more information on .NET image variants and AOT images, please see the following documentation:

- [.NET Image Variants](https://github.com/dotnet/dotnet-docker/blob/main/documentation/image-variants.md)
- [Announcement: .NET 10 AOT container images](https://github.com/dotnet/dotnet-docker/discussions/6312)
- [Announcement: New approach for differentiating .NET 8+ images](https://github.com/dotnet/dotnet-docker/discussions/4821)

Watch the [announcements page](https://github.com/dotnet/dotnet-docker/discussions/categories/announcements) for the latest information on new features and changes in .NET contanier images.

[Framework-dependent]: https://learn.microsoft.com/dotnet/core/deploying/#publish-framework-dependent
[Self-contained]: https://learn.microsoft.com/dotnet/core/deploying/#publish-self-contained
[Native AOT]: https://learn.microsoft.com/dotnet/core/deploying/native-aot/
