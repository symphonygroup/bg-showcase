#!/bin/bash
parent_path=$( cd "$(dirname "${BASH_SOURCE[0]}")" ; pwd -P )

kubectl apply -f $parent_path/../base/namespace.yaml
kubectl apply -f $parent_path/../base/dockerconfigjson.yaml

kubectl apply -f $parent_path/../base/db-data.yaml
kubectl apply -f $parent_path/../base/db.yaml

kubectl delete -f $parent_path/../base/api.yaml
kubectl apply -f $parent_path/../base/ui.yaml

kubectl apply -f $parent_path/../base/ingress.yaml

kubectl -n local rollout restart deployment/ui-deployment