// create an express app that takes a post request with body and returns a json response, port should be read from env variable EXPRESS_PORT
// the body should be a json object with a key "question" and a string value, the response should be a json object with a key "answer" and a string value
// use module type "module" when importing libraries

import express from 'express';
import bodyParser from 'body-parser';
import cors from 'cors';
import { getSummary } from "./ai-summarization.js";

const app = express();
const port = process.env.EXPRESS_PORT || 3000;

app.use(bodyParser.json());
app.use(cors());

app.post('/answer', async (req, res) => {
    const question = req.body.question;
    const answer = await getAnswer(question);
    res.json({ answer });
});

app.post('/summarize', async (req, res) => {
    const text = req.body.text;
    const summary = await getSummary(text);
    res.json(summary);
});

app.listen(port, () => {
    console.log(`Document processing app listening at http://localhost:${port}`);
});

async function getAnswer(question) {
    return '42';
}