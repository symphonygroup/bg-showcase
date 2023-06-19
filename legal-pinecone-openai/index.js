// create an express app that takes a post request with body and returns a json response, port should be read from env variable EXPRESS_PORT
// the body should be a json object with a key "question" and a string value, the response should be a json object with a key "answer" and a string value
// use module type "module" when importing libraries

import express from 'express';
import bodyParser from 'body-parser';
import cors from 'cors';
import { getSummary } from "./ai-summarization.js";
import { ensureIndex, updateIndex } from './index-management.js';
import { PineconeClient } from '@pinecone-database/pinecone';
import { DirectoryLoader } from "langchain/document_loaders/fs/directory";
import { TextLoader } from "langchain/document_loaders/fs/text";
import { PDFLoader } from "langchain/document_loaders/fs/pdf";

const app = express();
const port = process.env.EXPRESS_PORT || 3001;

const client = new PineconeClient();
await client.init({
    apiKey: process.env.PINECONE_API_KEY,
    environment: process.env.PINECONE_ENVIRONMENT,
});

app.use(bodyParser.json());
app.use(cors());

app.post('/answer', async (req, res) => {
    const { question, indexName } = req.body;
    const answer = await getAnswer(question, indexName);
    res.json({ answer });
});

app.post('/summarize', async (req, res) => {
    const text = req.body.text;
    const summary = await getSummary(text);
    res.json(summary);
});

app.post('/ensureIndex', async (req, res) => {
    const { indexName } = req.body;

    const existingIndexes = await client.listIndexes();

    if (existingIndexes.includes(indexName)) {
        console.log(`Index ${indexName} already exists.`);
        res.status(400).send(`Index ${indexName} already exists.`);
        return;
    }

    const vectorDimension = 1536;

    await ensureIndex(client, indexName, vectorDimension);

    res.status(201).send(`Index ${indexName} created.`);
});

app.post('/updateIndex', async (req, res) => {
    const { indexName } = req.body;

    const existingIndexes = await client.listIndexes();

    if (!existingIndexes.includes(indexName)) {
        console.log(`Index ${indexName} does not exist.`);
        res.status(400).send(`Index ${indexName} does not exist.`);
        return;
    }

    const loader = new DirectoryLoader("./documents", {
        ".txt": (path) => new TextLoader(path),
        ".pdf": (path) => new PDFLoader(path),
    });
    const docs = await loader.load();

    await updateIndex(client, indexName, docs);

    res.status(200).send(`Index ${indexName} updated.`);
});

app.listen(port, () => {
    console.log(`Document processing app listening at http://localhost:${port}`);
});

async function getAnswer(question, indexName) {
    const matches = await queryVector(client, indexName, question);
    const result = await queryModel(matches, question);
    const answer = result.text;
    return answer;
}