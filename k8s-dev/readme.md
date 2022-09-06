# Local Development and CI/CD

Purposes of this example are:

1. To setup local Kubernetes cluster with Minikube
2. [To setup local development environment with shell scripts](docs/shell.md)
3. [To setup local development environment with either shell scripts or DevSpaces](docs/devspace.md)
4. [To have an automated CI/CD pipeline for a UI & API applications with GitHub Actions and ArgoCD](docs/cd.md)

Step 1 is described in this document and is a prerequisite for all other steps.
Steps 2 and 3 are independent on each other and give you options for setup of local environment.
Step 4 is also independent and it is there to show you how some remote K8s cluster could work with GitHub Actions and ArgoCD by GitOps principles.

## Minikube setup, Ingress and tunneling

Install Docker Desktop with Kubernetes support or anything else required for Minikube

[Install Minikube](https://minikube.sigs.k8s.io/docs/start/)

Start Minikube. You can use different profiles if you want to have a separate one for this example

Enable Minikube Ingress addon as we will deploy Ingress resource in order to access FE/BE from local computer (it might be already enabled, but run this command anyways).

```bash
minikube addons enable ingress
```

In order to connect to your applications, you need to tunnel to minikube to Ingress (keep in separate terminal) with

```bash
sudo ssh -o UserKnownHostsFile=/dev/null -o StrictHostKeyChecking=no -N docker@127.0.0.1 -p PORT -i /Users/USERNAME/.minikube/machines/minikube/id_rsa -L 80:127.0.0.1:80
```

Where PORT can be retrieved with

```bash
docker port minikube | grep 22
```

`USERNAME` is your username on the system, e.g. `firstname.lastname`

You can also start Minikube Dashboard with `minikube dashboard` in order to see resources.

## Setup Kubernetes Secret for GitHub Container Registry (GHCR)

> This section is used in Steps 2-4 and should be applied when mentioned there.

In order to be able to pull images from GHCR.IO or some other image repository, you need to first make sure you create a namespace you need, e.g. `local`, `devspace`, `dev`, `qa`, with command

```bash
kubectl create namespace local
```

Then execute command to create K8s Secret for pulling from the image repository

```bash
kubectl create secret docker-registry dockerconfigjson-github-com \
--docker-server=https://ghcr.io \
--docker-username=GITHUB_USERNAME \
--docker-password=GITHUB_PAT \
--namespace=local
```

`GITHUB_USERNAME` is your GitHub username.

You need to create your GithHub Personal Access Token the following way. Go to GitHub and open your [Personal Access Tokens](https://github.com/settings/tokens). Click on Generate new token and select `package:read` scope. Remember the token somewhere and use it in previous command instead of `GITHUB_PAT`.

Make sure you use the same `namespace` that you previously created.
