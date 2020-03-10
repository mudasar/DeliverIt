docker build -t deliverit .
docker run -it --rm -p 5000:80 --name deliverit_web deliverit 