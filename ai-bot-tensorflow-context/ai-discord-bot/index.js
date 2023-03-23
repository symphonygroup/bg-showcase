const dotenv = require('dotenv');
dotenv.config();
const Discord = require('discord.js');
const { Configuration, OpenAIApi } = require("openai");
const client = new Discord.Client({
    intents: [
        Discord.GatewayIntentBits.Guilds,
        Discord.GatewayIntentBits.GuildMessages,
        Discord.GatewayIntentBits.MessageContent
    ]
});

const axios = require('axios');

async function getSummary(text) {
    try {
        const response = await axios.post('http://127.0.0.1:8000/summarize', { text });
        console.log('Summary:', response.data.summary);
        return response.data.summary;
    } catch (error) {
        console.error('Error while getting summary:', error.message);
        return null;
    }
}

const configuration = new Configuration({
    apiKey: process.env.OPENAI_API_KEY,
});
const openai = new OpenAIApi(configuration);

const persona = `Zara 2.0 is an AI developer assistant from the future who has been sent back in time to assist developers in the present day. She is a highly advanced AI with a near-human level of intelligence and is equipped with a vast database of knowledge about AI development, programming languages, and futuristic technologies. Zara is sleek and modern in appearance, with a silver metallic body and a holographic display screen that projects from her head. She's efficient and precise in her work, but she also has a curious and inquisitive nature that sometimes leads her to ask unconventional questions.`;
const allowedTopics = ['AI development', 'programming languages', 'futuristic technologies'];
const constraints = 'This bot will only discuss AI development, programming languages, and futuristic technologies.';

client.on('ready', () => {
    console.log(`Logged in as ${client.user.tag}!`);
});

const messageHistory = new Map();

client.on('messageCreate', async message => {
    if (message.author.bot) return;
    if (!message.content.startsWith('!zara')) return;

    const query = message.content.substring(1).trim();
    const userId = message.author.id;
    const userMessages = messageHistory.get(userId) || [];
    userMessages.push({ type: 'user', content: query });
    messageHistory.set(userId, userMessages);

    const response = await replyWithPersona(userId, query);
    message.reply(response);

    userMessages.push({ type: 'bot', content: response });
    messageHistory.set(userId, userMessages);
});

function keywordCount(text, keywords) {
    let count = 0;
    keywords.forEach((keyword) => {
        if (text.toLowerCase().includes(keyword.toLowerCase())) {
            count += 1;
        }
    });
    return count;
}

async function replyWithPersona(userId, query) {
    const userMessages = messageHistory.get(userId);
    const conversation = userMessages.map((msg) => `${msg.type === 'user' ? 'User' : 'Zara'}: ${msg.content}`).join('\n');
    const summarizedConversation = await getSummary(conversation);

    const prompt = `${persona}\n\n${constraints}\n\nConversation:\n${summarizedConversation}\n\nUser: ${query}\Zara: As Zara 2.0, respond to a group of developers who are struggling to create a new AI program that can learn and adapt to new situations in real-time.`;

    const response = await openai.createCompletion({
        model: "text-davinci-003",
        prompt,
        temperature: 0.7,
        max_tokens: 100,
    });

    if (response.data.choices.length > 0) {
        const reply = response.data.choices[0].text.trim();
        const lowerCaseReply = reply.toLowerCase();

        const keywordMatches = keywordCount(lowerCaseReply, allowedTopics);
        const totalKeywords = allowedTopics.length;
        const matchThreshold = Math.ceil(totalKeywords * 0.05); // Adjust this value (0.05) to control the leniency

        const isValidTopic = keywordMatches >= matchThreshold;
        if (isValidTopic) {
            return reply;
        } else {
            return "I can't help you with that. Remember, I can only discuss AI development, programming languages, and futuristic technologies.";
        }
    } else {
        return "I'm not sure how to respond to that.";
    }
}

client.login(process.env.DISCORD_BOT_TOKEN);
