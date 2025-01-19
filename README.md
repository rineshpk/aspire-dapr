# Microservices Sample Project Using .NET Aspire and Dapr
> Read the blog post here: https://dev.to/rineshpk/simplifying-microservice-development-with-net-aspire-dapr-and-podman-3hp0

This is a sample .NET Microservice using Dapr and Aspire app host that orchestrates three services:
1.  **Service-A**: A minimal API service that publishes messages and do service-to-service invocations.
2.  **Service-B**: Another minimal API service that subscribes to messages and retrieves secrets from the Dapr secret store and interacts with a state store.
3.  **Blazor Frontend**: A web application that interacts with backend services.

.NET Aspire template projects

1.  **Aspire AppHost** – Responsible for orchestration.
2.  **ServiceDefaults** – Handles OpenTelemetry integration, health checks, and service discovery.

![Diagram](https://github.com/user-attachments/assets/ff1aaf5d-6642-442a-b6b6-89157eea41cf)


### Setting Up Your Development Environment

Getting started with .NET Aspire is simple. Follow these steps to set up your development environment:

#### 1. Install .NET Aspire SDK

-   To work with .NET Aspire, you need the following installed locally **.NET 8.0** or **.NET 9.0**.

![Visual Studio Installer](https://dev-to-uploads.s3.amazonaws.com/uploads/articles/7q8vws10fhp4ahs35sg4.png)


-   Visual Studio 2022 17.9 or higher includes the latest .NET Aspire SDK by default when you install the Web & Cloud workload.

#### 2. Install Podman Desktop

[Podman](https://podman-desktop.io/docs/installation/windows-install) is an open-source, daemonless alternative to Docker for building and running OCI containers.

If both Docker and Podman are installed on your system, .NET Aspire defaults to Docker. To switch to Podman, set the container runtime with the following command:

```powershell
[System.Environment]::SetEnvironmentVariable("DOTNET_ASPIRE_CONTAINER_RUNTIME", "podman", "User")
```

#### 3. Install Dapr CLI

To run applications with Dapr sidecars in .NET Aspire, install the **Dapr CLI (version 1.13 or later)**.

-   Follow the instructions here: [Install the Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/).
-   After installation, initialize Dapr for Podman with:
    
```bash
dapr init --container-runtime podman
```
For more details, see [Initialize Dapr with Podman](https://docs.dapr.io/operations/hosting/self-hosted/self-hosted-with-podman/).

### Build and Run
- Clone this repository.
- Execute dotnet run inside the AspireWithDapr.AppHost/ directory.
- Browse to https://localhost:17262 to open the .NET Aspire dashboard.
- Navigate to the service-a and publish an event.
- Navigate to WebUI and go to Data menu to invoke service-b
  
Navigate to the traces "Traces" menu in Aspire Dashboard to monitor the operations.
![Aspire Dashboard](https://github.com/user-attachments/assets/48b27d07-1225-411d-b006-0519c20caa2f)
![Traces full](https://github.com/user-attachments/assets/2e211730-a613-4588-8b3d-0d6734c6c1d4)
![Messaging Trace](https://github.com/user-attachments/assets/b4e473c0-ee07-4b9f-aa98-81639f06413d)



