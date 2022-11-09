FROM golang

RUN mkdir /app

ADD . /app

WORKDIR /app

RUN cd ./oauth/pkg/cmd && go build -o main .

EXPOSE 8080
CMD ["/app/oauth/pkg/cmd/main"]
