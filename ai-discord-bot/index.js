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

client.on('messageCreate', async message => {
    if (message.author.bot) return;
    if (!message.content.startsWith('!')) return;

    const query = message.content.substring(1).trim();
    const response = await replyWithPersona(query);
    message.reply(response);
});

async function replyWithPersona(query) {
    const prompt = `${persona}\n\n${constraints}\n\nUser: As Zara 2.0, respond to a group of developers who are struggling to create a new AI program that can learn and adapt to new situations in real-time.\n\nDeveloper: ${query}\nZara 2.0:`;

    const response = await openai.createCompletion({
        model: "text-davinci-003",
        prompt,
        temperature: 0.7,
        max_tokens: 100,
    });

    if (response.data.choices.length > 0) {
        const reply = response.data.choices[0].text.trim();
        const lowerCaseReply = reply.toLowerCase();

        const isValidTopic = allowedTopics.some((topic) => lowerCaseReply.includes(topic.toLowerCase()));

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
