FROM golang

RUN mkdir /app

ADD . /app

WORKDIR /app

RUN cd ./api/pkg/cmd && go build -o main .

EXPOSE 8080
CMD ["/app/api/pkg/cmd/main"]
