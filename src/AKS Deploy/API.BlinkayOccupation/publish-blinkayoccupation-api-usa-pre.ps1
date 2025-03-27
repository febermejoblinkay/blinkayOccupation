
az aks get-credentials --resource-group BLINKAY_USA --name aks-usa

kubectl apply -f .\blinkayoccupation-api-usa-pre.yaml --validate=false

pause