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

### Setup UI application on ArgoCD

