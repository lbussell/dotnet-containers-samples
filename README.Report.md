# .NET Container Image Size Report

.NET offers a variety of deployment options for applications, which pair with container images that we offer. It's possible to produce very small container images. This document summarizes the available options to help you make the best choice for your apps and environment.

## Image Sizes

| Name                              | Publish Type          | Distroless | Globalization | Compressed Size |
| --------------------------------- | --------------------- | ---------- | ------------- | --------------: |
| ConsoleApp                        | [Framework-dependent] | ✖️ No      | ✖️ No         |        80.30 MB |
| ConsoleAppDistroless              | [Framework-dependent] | ✅ Yes      | ✖️ No         |        40.63 MB |
| ConsoleAppSelfContained           | [Self-contained]      | ✖️ No      | ✖️ No         |        53.29 MB |
| ConsoleAppSelfContainedDistroless | [Self-contained]      | ✅ Yes      | ✖️ No         |        13.62 MB |
| ConsoleAppNativeAot               | [Native AOT]          | ✖️ No      | ✖️ No         |        48.18 MB |
| ConsoleAppDistrolessAot           | [Native AOT]          | ✅ Yes      | ✖️ No         |         8.51 MB |
| WebApi                            | [Framework-dependent] | ✖️ No      | ✖️ No         |        92.48 MB |

For more information on .NET image variants and AOT images, please see the following documentation:

- [.NET Image Variants](https://github.com/dotnet/dotnet-docker/blob/main/documentation/image-variants.md)
- [Announcement: .NET 10 AOT container images](https://github.com/dotnet/dotnet-docker/discussions/6312)
- [Announcement: New approach for differentiating .NET 8+ images](https://github.com/dotnet/dotnet-docker/discussions/4821)

Watch the [announcements page](https://github.com/dotnet/dotnet-docker/discussions/categories/announcements) for the latest information on new features and changes in .NET contanier images.

[Framework-dependent]: https://learn.microsoft.com/dotnet/core/deploying/#publish-framework-dependent
[Self-contained]: https://learn.microsoft.com/dotnet/core/deploying/#publish-self-contained
[Native AOT]: https://learn.microsoft.com/dotnet/core/deploying/native-aot/
