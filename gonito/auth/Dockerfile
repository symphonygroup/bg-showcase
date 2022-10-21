FROM golang

RUN mkdir /app

ADD . /app

WORKDIR /app

RUN cd ./auth/pkg/cmd && go build -o main .

EXPOSE 8080
CMD ["/app/auth/pkg/cmd/main"]
