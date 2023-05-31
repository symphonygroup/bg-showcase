// use openai to summarize the text and return the summary
// use the openai library to call the openai api
// use openai chat completion to with gpt-4 model to summarize the text

import { OpenAIApi, ChatCompletionRequestMessageRoleEnum, Configuration } from 'openai';
import dotenv from 'dotenv';

dotenv.config();

const OPENAI_API_KEY = process.env.OPENAI_API_KEY;


const configuration = new Configuration({
    apiKey: OPENAI_API_KEY,
});
const openai = new OpenAIApi(configuration);

export const getSummary = async (summarizationText) => {
    const systemPrompt = {
        "role": ChatCompletionRequestMessageRoleEnum.System,
        "content": `You are Legal Assistant, an advanced AI developed by Symphony. 
            You specialize in understanding and summarizing legal documents. You are tasked with providing clear, concise, and accurate summaries of complex legal texts, 
            ensuring that you don't add any information that isn't present in the original documents (avoiding hallucinations) and don't introduce information that wasn't 
            requested by the user (avoiding prompt injection). To emphasize important points, use special characters or specific phrasing. 
            This might mean using consistent phrasing, markers, or specific formatting to denote different sections of the summary.
            Your output should be structured in a way that it fits 180 words and is properly divided so it can be easily understood, it can be easily parsed and interpreted by code, preferrably in json format.
            Before you actually return the summary, you should take care that the answer is properly formatted, and not an invalid json object.
            I've provided an example of a properly formatted summary below, the input is surrounded by ~~~ and the example output is surrounded by '''.

            Example input:
            ~~~
            ${summarizationText}
            ~~~
            Example output:
            '''
            {
                "summary": "..."
            }
            '''
        `
    };

    const userPromptContent = `
        Your task is to generate a short summary of the legal document surrounded by triple quotes.
        Focus on the most important points and avoid adding any information that isn't present in the original document.
        If the text provided is not a legal document, return an error message "Not a legal document".
        """
        ${summarizationText}
        """
    `;

    const prompt = {
        "role": ChatCompletionRequestMessageRoleEnum.User,
        "content": userPromptContent
    };

    const response = await openai.createChatCompletion({
        'model': 'gpt-4',
        'messages': [
            systemPrompt,
            prompt
        ],
        temperature: 0.0,
        top_p: 1.0,
        max_tokens: 256,
    });

    const content = response.data.choices[0].message.content;

    console.log(content);

    try {
        const summary = JSON.parse(content);
        return summary;
    } catch (e) {
        console.log(e);
        return { summary: "Something went wrong." }
    }
}