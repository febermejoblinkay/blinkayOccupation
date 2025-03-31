
az aks get-credentials --resource-group BLINKAY_USA --name aks-usa

kubectl apply -f .\blinkayoccupation-payments-worker-pro-usa.yaml --validate=false

pause