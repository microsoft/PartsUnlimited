docker image build . -t loekd/nanoserver:latest
docker image build . -t loekd/nanoserver:2.0

docker push loekd/nanoserver:latest
docker push loekd/nanoserver:2.0