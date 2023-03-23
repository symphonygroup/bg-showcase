from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from transformers import pipeline

app = FastAPI()
summarization_pipeline = pipeline("summarization")


class TextToSummarize(BaseModel):
    text: str


@app.post("/summarize")
def summarize(text_to_summarize: TextToSummarize):
    text = text_to_summarize.text
    if not text:
        raise HTTPException(status_code=400, detail="Text not provided")

    summary = summarization_pipeline(text, max_length=100)
    return {"summary": summary[0]["summary_text"]}


if __name__ == "__main__":
    import uvicorn

    uvicorn.run("summarization_api_fastapi:app", host="127.0.0.1", port=8000)
