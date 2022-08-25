# Minikube, ArgoCD & GitHub CI/CD

Purpose of this example is to have an automated CI/CD pipeline for a UI & API applications.

## Minikube & manifests

Install Docker Desktop with Kubernetes support or anything else required for Minikube

[Install Minikube](https://minikube.sigs.k8s.io/docs/start/)

Start Minikube. You can use different profiles if you want to have a separate one for this example

Go to `k8s` directory and apply all manifests

```bash
kubectl apply -f .
```

You might need to first create a namespace for `dev` (apply namespace.yaml), so other manifests can run properly.

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
127.0.0.1 dev.demo.com
```

Now you can go to [http://dev.demo.com/](http://dev.demo.com/) and check the application.

## ArgoCD

### Setup ArgoCD

Setup ArgoCD as explained here [Install ArgoCD](https://argo-cd.readthedocs.io/en/stable/getting_started/#1-install-argo-cd)

```bash
kubectl create namespace argocd
kubectl apply -n argocd -f https://raw.githubusercontent.com/argoproj/argo-cd/stable/manifests/install.yaml
```

Simplest way to access to ArgoCD is by port forwarding

```bash
kubectl port-forward svc/argocd-server -n argocd 8080:443
```

Get secret for `admin` user with

```bash
kubectl -n argocd get secret argocd-initial-admin-secret -o jsonpath="{.data.password}" | base64 -d; echo
```

Then open it by accessing URL [https://127.0.0.1:8080/](https://127.0.0.1:8080/) with `admin` username and `secret` you retrieved in previous command.

## Setup Kubernetes Secret for GitHub Container Registry (GHCR)

In order for ArgoCD to pull images from GHCR, we need to create access token and create it in `each namespace`. There is a post regarding this [here](https://dev.to/asizikov/using-github-container-registry-with-kubernetes-38fb), but here are the steps in case it is not available any more.

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

## Setup Development Applications (environments) on ArgoCD

In `Setup ArgoCD` section we opened ArgoCD UI. Here is the official manual for [creating application though UI](https://argo-cd.readthedocs.io/en/stable/getting_started/#creating-apps-via-ui). You can also do it through CLI. We used the following parameters:

Parameter | value | value for QA (if different)
--- | --- | ---
Application Name | k8s-dev | k8s-qa
Project | default |
Sync Policy | Automated |
Repository URL | https://github.com/symphonygroup/bg-showcase.git |
Revision | HEAD |
Path | k8s-dev/k8s/overlays/dev | k8s-dev/k8s/overlays/qa
Cluster | kubernetes.default.svc |
Namespace | dev | qa

Create apps for both DEV and QA. Sync could be also Manual.

## Trigger CI/CD workflows

- Pull the `main` branch code.
- Make a change in UI and/or API projects (change index.html and Program.cs).
- Commit and push changes.
- Check in GitHub Actions that workflows were triggered.
- Manualy Refresh DEV application in ArgoCD and Sync then or wait for Automatic Sync (depends on your setup)
- Approve GitHub Actions workflows (need approval for QA)
- Manualy Refresh QA application in ArgoCD and Sync then or wait for Automatic Sync (depends on your setup)

> Since both code and K8s/Kustomize manifests are in the same repository, GHA will commit changes to that same repository when changing image tags. Recommended practice is to have a separate repository for manifests. Both UI and API are in the same repository, but you might want to have them in separate ones.
