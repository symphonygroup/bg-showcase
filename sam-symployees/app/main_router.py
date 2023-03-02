from fastapi import APIRouter

router = APIRouter(
    prefix="/api"
)


@router.get("/")
def func1():
    return {"Hello": "/"}


@router.get("/test")
def func2():
    return {"Hello": "test"}


@router.get("/ping/ping")
def func3():
    return {"Hello": "ping ping"}
