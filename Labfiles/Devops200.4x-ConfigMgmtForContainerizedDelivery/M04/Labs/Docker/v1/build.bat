docker image build . -t loekd/nanoserver:latest
docker image build . -t loekd/nanoserver:1.0

docker push loekd/nanoserver:latest
docker push loekd/nanoserver:1.0