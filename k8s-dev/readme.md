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

Enable Minikube Ingress addon as we will deploy Ingress resource in order to access FE/BE from local computer.

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
You also need to add all hosts from `ingress.yaml` in your `/etc/hosts` file, e.g.

```text
127.0.0.1 local.demo.com
```

You can also start Minikube Dashboard with `minikube dashboard` in order to see resources.

## Setup Kubernetes Secret for GitHub Container Registry (GHCR)

In order to pull images from GHCR, we need to create access token and create it in `each namespace`. There is a post regarding this [here](https://dev.to/asizikov/using-github-container-registry-with-kubernetes-38fb), but here are the steps in case it is not available any more.

Go to GitHub and open your [Personal Access Tokens](https://github.com/settings/tokens). Click on Generate new token and select `package:read` scope.

Once you create a token you should take `username:123123adsfasdf123123` (where `username` is your GitHib username and `123123adsfasdf123123` is your token) and `base64` encode it

```bash
echo -n "username:123123adsfasdf123123" | base64
dXNlcm5hbWU6MTIzMTIzYWRzZmFzZGYxMjMxMjM=
```

Then create a `.dockerconfigjson` and also `base64` encode it.

```bash
echo -n  '{"auths":{"ghcr.io":{"auth":"dXNlcm5hbWU6MTIzMTIzYWRzZmFzZGYxMjMxMjM="}}}' | base64
eyJhdXRocyI6eyJnaGNyLmlvIjp7ImF1dGgiOiJkWE5sY201aGJXVTZNVEl6TVRJellXUnpabUZ6WkdZeE1qTXhNak09In19fQ==
```

Then store this value in `dockerconfigjson.yaml`. 

```yaml
kind: Secret
type: kubernetes.io/dockerconfigjson
apiVersion: v1
metadata:
  name: dockerconfigjson-github-com
  namespace: dev
data:
  .dockerconfigjson: eyJhdXRocyI6eyJnaGNyLmlvIjp7ImF1dGgiOiJkWE5sY201aGJXVTZNVEl6TVRJellXUnpabUZ6WkdZeE1qTXhNak09In19fQ==
```

> It is not recommended that you push this to GitHub.
> Bare in mind that you should run it for each namespace!

Now create this secret

```bash
kubectl create -f dockerconfigjson.yaml
secret/dockerconfigjson-github-com created
```

We will use it in manifests for `imagePullSecrets`.

As a simple alternative you can create a secret via kubectl cli. It will also do transformations for you:

```bash
kubectl create secret docker-registry dockerconfigjson-github-com --docker-server=https://ghcr.io --docker-username=mygithubusername --docker-password=mygithubreadtoken --docker-email=mygithubemail
```
