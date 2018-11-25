$containerid = docker ps --filter "status=running" --filter "name=partsunlimitedwebsite" --format "{{.ID}}" -n1

$ipaddress = docker inspect --format="{{.NetworkSettings.Networks.nat.IPAddress}}" $containerid

Write-Output ("##vso[task.setvariable variable=ipadress:]$ipaddress")

Invoke-Webrequest $ipaddress -UseBasicParsing