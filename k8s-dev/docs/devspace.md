# Working with DevSpace as local development environment

[DevSpace](https://devspace.sh) is an open-source developer tool for Kubernetes that lets you develop and deploy cloud-native software faster. We will should how it can be used for development in a local K8s environment.

This setup will allow us to develop API on local machine while that API will be continuously build and deployed (on any file change) in local K8s cluster. We will also allow port forwarding from local machine toward that API. DevSpace also allows reverse port forwarding from the K8s cluster to our local machine.

## Setup explained

First you will need to install DevSpace as explained [here](https://devspace.sh/docs/getting-started/installation?x0=3).

```bash
brew install devspace
```

We already initialized and setup DevSpace for API project, but you can do it on your own and explore the steps. Our setup is:

- For a `dotnet` application and uses `kustomize` in order to reuse our K8s manifests
- We decided to create a new overlay called `devspace`, so it doesn't clash with the `local` namespace that is setup for `shell` scripts
- We are building from Docker file and use `ghcr.io` repository
- We forward from port `8082` on local machine to API (for testing the API inside cluster or for UI access as you will see later)

You can check `devspace.yaml` and familiarize with the settings.

## Specific setup

Since we need access to `ghcr.io`, we need to do some additional steps before following the regular DevSpace commands for starting the development environment.

Create `devspace` namespace

```bash
kubectl create namespace devspace
```

Create GHCR.IO Secret in `devspace` namespace as explained in the main [readme.md](../readme.md) of `k8s-dev` project.

In order to access the UI from local machine, add the following line in `/etc/hosts` file - `127.0.0.1 devspace.demo.com`

## Start development environment

Go to the directory where the API source is located. It should have a `devspace.yaml` in it.

Make sure DevSpaces uses the right cluster and namespace with

```bash
devspace use context
devspace use namespace devspace
```

Start dev container

```bash
devspace dev
```

If you check deployments in a cluster, you will see that the *regular* API deployment (`devspace-api-deployment`) has 0 pods running, but now we see there is `devspace-api-deployment-devspace` deployment running. If you try to access it with [http://localhost:8082/](http://localhost:8082/) it will fail. That is because you haven't started your API.

In order to start dotnet application, you should either run `dotnet run` or `dotnet watch`. The later will hot reload the application on each file change. If you now visit [http://localhost:8082/](http://localhost:8082/) it will work.

Try to change version value in `Program.cs` to see that it works. Remember, your application is working in a cluster!

## Cleanup

If you want all services to work in cluster with no DevSpace running, you can do it easily. First, stop the application (Control+C). Then exit DevSpaces with `exit`. Now, when you are in the regular terminal, type `devspace reset pods`.

What DevSpace did is that it returned the regular `devspace-api-deployment`. You can test it by visiting [http://devspace.demo.com](http://devspace.demo.com). If you check Developers Tools, you will see UI has a `config.json` that defines API URL and page is hitting that URL.

## Connect UI with DevSpace API

This DevSpace setup is for the backend developers. They will usually use just the DB and UI is less of their concern, but it a good practice to check the UI that all is good if you developed something that can break the frontend.

Let's start again DevSpaces with `devspace dev` and start the API. If we try to access the UI, we will see that UI can't access the API as our regular `devspace-api-deployment` is down to 0 pods. Ok, but we have API exposed on port `8082`, so if we could point UI to hit that URI, that would work.

Luckily, we have in our `devspace` overlay a `configmap.yaml` where we can override URL for API to `http://localhost:8082`. You can just change that file. In order to apply, we could use `kubectl` command, but that would interfere in how DevSpace works and only DevSpace should use that overlay. That is why we need to:

1. Stop API
2. Exit DevSpace
3. Run DevSpace again with `devspace dev`
4. Start API

Since DevSpace will apply `configmap.yaml` overlay, it needs a minute-two to figure out there is a change and restart the pods with new value. Check you frontend app and you will see that `config.json` now contains API endpoint value that points to port `8082` on localhost.
