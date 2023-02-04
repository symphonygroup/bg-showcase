from fastapi import FastAPI
from mangum import Mangum
from main_router import router as mainRouter

app = FastAPI()

app.include_router(mainRouter)

handler = Mangum(app)