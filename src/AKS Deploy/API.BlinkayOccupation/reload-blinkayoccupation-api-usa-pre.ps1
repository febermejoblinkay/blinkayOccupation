
az aks get-credentials --resource-group BLINKAY_USA --name aks-usa

kubectl rollout restart deployment/blinkayoccupation-api-pre

pause