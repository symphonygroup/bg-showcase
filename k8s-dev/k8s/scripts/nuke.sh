#!/bin/bash
parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

kubectl delete -f $parent_path/../base/ingress.yaml

kubectl delete -f $parent_path/../base/db.yaml

kubectl delete -f $parent_path/../base/api.yaml
kubectl delete -f $parent_path/../base/ui.yaml

kubectl delete -f $parent_path/../base/db-data.yaml

kubectl apply -f $parent_path/../base/configmap.yaml

# We are not removing Secret for GHCR.IO and also not removing Namespace
#
# kubectl delete -f $parent_path/../base/dockerconfigjson.yaml
# kubectl delete -f $parent_path/../base/namespace.yaml