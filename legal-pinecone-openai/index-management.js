import { OpenAIEmbeddings } from "langchain/embeddings/openai";
import { RecursiveCharacterTextSplitter } from "langchain/text_splitter";

export const ensureIndex = async (client, indexName, vectorDimension) => {
    console.log(`Creating index ${indexName}...`);

    await client.createIndex({
        createRequest: {
            name: indexName,
            dimension: vectorDimension,
            metric: "cosine"
        }
    });

    console.log(`Index ${indexName} created.`);

    await new Promise(resolve => setTimeout(resolve, 60000));
};

export const updateIndex = async (client, indexName, documents) => {
    console.log(`Updating database index ${indexName}...`);

    const index = client.Index(indexName);

    console.log(`Adding ${documents.length} documents to index ${indexName}...`);

    for (const document of documents) {
        const txtPath = document.metadata.source;
        const text = doc.pageContent;

        const textSplitter = new RecursiveCharacterTextSplitter({
            chunkSize: 1000,
        });

        const chunks = await textSplitter.createDocuments([text]);

        console.log(`Text split into ${chunks.length} chunks.`);

        const embeddings = await new OpenAIEmbeddings().embedDocuments(
            chunks.map((chunk) => chunk.pageContent.replace(/\n/g, " "))
        );

        console.log(`Embeddings generated for ${chunks.length} chunks.`);

        const batchSize = 100;

        let batch = [];

        for (let i = 0; i < chunks.length; i++) {
            const chunk = chunks[i];

            const vector = {
                id: `${txtPath}_${i}`,
                values: embeddings[i],
                metadata: {
                    ...chunk.metadata,
                    loc: JSON.stringify(chunk.metadata.loc),
                    pageContent: chunk.pageContent,
                    txtPath: txtPath,
                },
            };

            batch.push(vector);

            if (batch.length === batchSize || i === chunks.length - 1) {
                await index.upsert({
                    upsertRequest: {
                        vectors: batch,
                    },
                });

                batch = [];
            }
        }

        console.log(`Added ${chunks.length} chunks to index ${indexName}.`);
    };
};