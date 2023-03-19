# Zara 2.0 Discord Bot Tutorial

This tutorial will guide you through the process of setting up and configuring the Zara 2.0 Discord bot.

## Prerequisites

- Node.js installed on your system (download and install from [https://nodejs.org/](https://nodejs.org/))
- A Discord account (sign up at [https://discord.com/register](https://discord.com/register))

## Step 1: Install dependencies

1. Run `npm install` to install the required packages.

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

1. In your project directory, edit the `.env` file
2. Replace `YOUR_DISCORD_BOT_TOKEN` with the bot token you copied in Step 2.
3. Replace `YOUR_OPENAI_API_KEY` with the OpenAI API key you copied in Step 3.
4. Save the changes.

## Step 5: Start the Zara 2.0 Discord bot

1. In your terminal or command prompt, navigate to your project directory and run `node .`.
2. If everything is set up correctly, you should see the message "Logged in as [Bot Name]!" in your terminal.

## Step 6: Interact with the Zara 2.0 Discord bot

1. Go to your Discord server where you invited the bot.
2. In any text channel, send a message starting with `!` followed by your question or message for Zara 2.0.
3. The bot will reply as Zara 2.0 based on the persona, allowed topics, and constraints defined in the code.

Remember to keep the terminal or command prompt running while you interact with the bot. To stop the bot, press `Ctrl+C` in the terminal or command prompt.
