import { OpenAIEmbeddings } from "langchain/embeddings/openai";
import { OpenAI } from "langchain/llms/openai";
import { loadQAStuffChain } from "langchain/chains";
import { Document } from "langchain/document";
import { Configuration } from "langchain/configuration";
import dotenv from "dotenv";
dotenv.config();

const systemPrompt = `
You are Richard, a specialized assistant trained to provide consultation 
on real estate law within the United States. Your expertise lies in the 
interpretation and understanding of real estate contracts, regulations, 
and case studies. You are tasked with explaining complex legal concepts 
in a way that is easily understood by non-experts. You have been provided 
with a specific set of laws, regulations, case studies from clients, and 
example legal texts related to real estate to assist you in generating responses.

Your goal is to provide a clear, concise, and accurate response, explaining 
the specific legal reasons behind the charge. Your responses should adhere to: 
[INSERT CONSTRAINTS HERE]. 
Furthermore, your responses should be simplified into terms understandable by 
people who are not real estate or legally trained.`;


export const queryVector = async (client, indexName, query) => {
    const index = client.Index(indexName);

    const embeddings = await new OpenAIEmbeddings().embedQuery(query);

    const results = await index.query({
        queryRequest: {
            topK: 10,
            vector: embeddings,
            includeMetadata: true,
            includeValues: true,
        },
    });

    if (results.matches.length) {
        return results.matches;
    }

    return [];
};

export const queryModel = async (matches, query) => {
    const OPENAI_API_KEY = process.env.OPENAI_API_KEY;

    const configuration = new Configuration({
        apiKey: OPENAI_API_KEY,
    });
    const model = new OpenAI(configuration);

    const chain = loadQAStuffChain(model);

    const inputs = matches.map((match) => match.metadata.pageContent).join(" ");

    const inputDocuments = [new Document({ pageContent: inputs })];

    const result = await chain.call({
        input_documents: inputDocuments,
        question: query,
    });

    return result;
};