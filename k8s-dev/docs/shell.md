# Local development environment with shell scripts

If you dislike using additional tools for the setup of local development environment, you can use simple shell scripts to start all or just some of the services in the cluster.

You can find in `k8s/scripts` directory following scripts:

Script  | Purpose
---     | ---
all.sh  | Start all services in cluster
be-db.sh| Start backend and database
fe-db.sh| Start frontend and database
nuke.sh | Remove all resources except Secret and Namespace

All scripts contain `kubectl` commands to `apply` or `delete` some resources from `base` directory that contains base manifests.
It is up to you to do any port forwarding or similar actions.
Advantage is that you do not depend on external tools.

In order to start all services in Local namespace, run `all.sh`, give it a few minutes to start (check in dashboard or with `kubectl get deployments -n local`) and when all deployments are running, visit [http://local.demo.com](http://local.demo.com).
