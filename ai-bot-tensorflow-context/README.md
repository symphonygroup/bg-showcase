# Zara Project Overview

The Zara Project is a unique combination of a Discord chatbot and a summarization API, designed to provide a seamless and engaging conversation experience to users within a Discord server. The project revolves around the persona of Zara 2.0, an AI developer assistant from the future who has been sent back in time to assist developers in the present day.

## Zara's persona

Zara 2.0 is a highly advanced AI with a near-human level of intelligence and is equipped with a vast database of knowledge about AI development, programming languages, and futuristic technologies. She is sleek and modern in appearance, with a silver metallic body and a holographic display screen that projects from her head. Efficient and precise in her work, Zara also has a curious and inquisitive nature that sometimes leads her to ask unconventional questions.

## Zara's tech
The chatbot leverages the powerful OpenAI GPT model to generate contextually relevant and accurate responses. To ensure that the conversation remains on-topic, the Zara 2.0 bot employs a summarization API to generate concise summaries of the user's messages and the bot's answers. The summaries are used as context for the GPT model, which helps maintain continuity and improve the quality of the conversation.

The Zara Project consists of two main components:

ai-discord-bot: This component is responsible for handling interactions between the user and the Zara 2.0 chatbot on the Discord platform. It listens for user messages, processes them, and sends them to the OpenAI GPT model for generating appropriate responses.

summarization-api: This component is a FastAPI-based service that takes the user's messages and the bot's answers as input and returns a summarized version of the conversation. It employs natural language processing (NLP) techniques and a combination of various summarization strategies to generate concise summaries while preserving the context.

The Zara Project offers an engaging, context-aware conversation experience for developers and technology enthusiasts, making it an excellent addition to Discord servers focused on AI development, programming languages, and futuristic technologies.

# Zara 2.0 Discord Bot Tutorial

This tutorial will guide you through the process of setting up and configuring the Zara 2.0 Discord bot along with the summarization API.

## Prerequisites

- Node.js installed on your system (download and install from [https://nodejs.org/](https://nodejs.org/))
- A Discord account (sign up at [https://discord.com/register](https://discord.com/register))

## Directory Structure

Your project folders should look like this:
- root
    - ai-discord-bot
    - summarization-api

## Step 1: Install dependencies

1. Navigate to the `ai-discord-bot` folder and run `npm install` to install the required packages.
2. Navigate to the `summarization-api` folder and run `pip install -r requirements.txt` to install the required Python packages.

## Step 2: Create a new Discord bot and invite it to your server

1. Go to the Discord Developer Portal at [https://discord.com/developers/applications](https://discord.com/developers/applications) and log in with your Discord account.
2. Click "New Application" in the top right corner and give your application a name.
3. In the "Bot" tab, click "Add Bot" and confirm the action.
4. Under "Token," click "Copy" to copy your bot's token. You will use this token in your code later.
5. In the "OAuth2" tab, scroll down to "Scopes" and check the "bot" box.
6. In "Bot Permissions," check the "Send Messages" and "Read Message History" boxes.
7. Click the generated OAuth2 URL and follow the steps to invite the bot to your server.

## Step 3: Create an OpenAI API key for GPT integration

1. Sign up for an OpenAI API key at [https://beta.openai.com/signup/](https://beta.openai.com/signup/).
2. Once you have access, go to the API key section and create a new API key.
3. Copy the API key. You will use this key in your code later.

## Step 4: Set up the Zara 2.0 Discord bot code

1. In the `ai-discord-bot` folder, edit the `.env` file.
2. Replace `YOUR_DISCORD_BOT_TOKEN` with the bot token you copied in Step 2.
3. Replace `YOUR_OPENAI_API_KEY` with the OpenAI API key you copied in Step 3.
4. Save the changes.

## Step 5: Start the Summarization API

1. Open a terminal or command prompt, navigate to the `summarization-api` folder, and run `uvicorn main:app --reload`.
2. The summarization API should now be running on `http://localhost:8000`.

## Step 6: Start the Zara 2.0 Discord bot

1. Open a new terminal or command prompt, navigate to the `ai-discord-bot` folder, and run `node .`.
2. If everything is set up correctly, you should see the message "Logged in as [Bot Name]!" in your terminal.

## Step 7: Interact with the Zara 2.0 Discord bot

1. Go to your Discord server where you invited the bot.
2. In any text channel, send a message starting with `!zara` followed by your question or message for Zara 2.0.
3. The bot will summarize the conversation, add the summarized text as context, and reply as Zara 2.0 based
