\# Docker Setup Notes — Part 1



\## Standalone Azurite container

docker network create coffeenchill-net

docker run -d --name azurite --network coffeenchill-net -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite



\## Build the Functions image

docker build -t coffeenchill-functions:v1.0 -f CoffeeNChillFunctions/Dockerfile .



\## Run the Functions container

docker run -d -p 7071:80 --network coffeenchill-net --name coffeenchill-functions -e AzureWebJobsStorage="DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://azurite:10000/devstoreaccount1;QueueEndpoint=http://azurite:10001/devstoreaccount1;TableEndpoint=http://azurite:10002/devstoreaccount1;" coffeenchill-functions:v1.0



\## Docker Hub images

\- https://hub.docker.com/r/deveshnaidust10473040/coffeenchill-functions

\- https://hub.docker.com/r/deveshnaidust10473040/coffeenchill-azurite



\## Push commands used

docker tag coffeenchill-functions:v1.0 deveshnaidust10473040/coffeenchill-functions:v1.0

docker push deveshnaidust10473040/coffeenchill-functions:v1.0

docker tag mcr.microsoft.com/azure-storage/azurite deveshnaidust10473040/coffeenchill-azurite:v1.0

docker push deveshnaidust10473040/coffeenchill-azurite:v1.0

