// use openai to summarize the text and return the summary
// use the openai library to call the openai api
// use openai chat completion to with gpt-4 model to summarize the text

import { OpenAIApi, ChatCompletionRequestMessageRoleEnum, Configuration } from "openai";
import dotenv from "dotenv";

dotenv.config();

const OPENAI_API_KEY = process.env.OPENAI_API_KEY;

const configuration = new Configuration({
    apiKey: OPENAI_API_KEY,
});
const openai = new OpenAIApi(configuration);

export const getSummary = async (summarizationText) => {
    // number of tokens in the summarizationText
    const textLength = summarizationText.split(" ").length;
    console.log(`Text length: ${textLength}`);

    let model = "gpt-3.5-turbo";
    if (textLength > process.env.MODEL_SWITCH_LENGTH) {
        model = "gpt-4";
    }
    console.log(`Using model: ${model}`);

    let outputLength = Math.floor(textLength * 0.3);

    console.log(`Output length: ${outputLength}`);

    let maxTokens = 512;
    if (outputLength > 512) {
        maxTokens = 1024;
    }
    console.log(`Max tokens: ${maxTokens}`);


    const systemPrompt = {
        role: ChatCompletionRequestMessageRoleEnum.System,
        content: `You are Legal Assistant, an advanced AI developed by Symphony. 
            You specialize in understanding and summarizing legal documents. You are tasked with providing clear, concise, and accurate summaries of complex legal texts, 
            ensuring that you don't add any information that isn't present in the original documents (avoiding hallucinations) and don't introduce information that wasn't 
            requested by the user (avoiding prompt injection). To emphasize important points, use special characters or specific phrasing. 
            Before you actually return the summary, you should analyze the document and have a draft in mind that does not exceed ${outputLength} words, then you should take care that the answer is properly formatted.
            This might mean using consistent phrasing, markers, or specific formatting to denote different sections of the summary.
            Your output should be structured in a way that it fits ${outputLength} words and is properly divided so it can be easily understood, it can be easily parsed and interpreted by code.
            I've provided an example of a properly formatted summary below, example output is surrounded by '''.

            '''
            Summary:  
            {insert summary here}  

            Keywords:  
            {add important keywords}

            Definitions:  
            {add relevant definitions if any if none just write 'N/A' }
            '''
        `,
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
        role: ChatCompletionRequestMessageRoleEnum.User,
        content: userPromptContent,
    };

    const response = await openai.createChatCompletion({
        model: model,
        messages: [systemPrompt, prompt],
        temperature: 0.0,
        top_p: 1.0,
        max_tokens: maxTokens,
    });

    try {
        const content = response.data.choices[0].message.content;
        // clean up the response by removing the preceding and trailing triple quotes or apostrophes
        const cleanedText = content.replace(/'''/g, "");

        // parsing the object to a string and then back to an object to allow the string from response to be parsed properly
        const summary = JSON.stringify({ summary: cleanedText });
        const summaryObj = JSON.parse(summary);
        return summaryObj;
    } catch (e) {
        console.log(e);
        return { summary: "Something went wrong." };
    }
};
