# ArgoCD

## Setup ArgoCD

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
