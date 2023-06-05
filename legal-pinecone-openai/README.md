# Node.js Express Server for AI Summarization and Answer Extraction

This project is a Node.js Express server that exposes endpoints for AI-driven text summarization and answer extraction from a given question. The server uses [OpenAI](https://openai.com/) and [Pinecone](https://www.pinecone.io/) as its primary dependencies.

## Getting Started

### Prerequisites

- Node.js
- An OpenAI API key
- A Pinecone API key

### Installation

1. Clone this repository:
``git clone <repository_url>``
2. Navigate into the cloned repository:
``cd <repository_name>``
3. Install the necessary dependencies:
``npm install``

### Environment Variables

The server requires several environment variables to function correctly. Create a `.env` file in the root directory and set the following variables:

``EXPRESS_PORT=<Your desired port number>``
``PINECONE_API_KEY=<Your Pinecone API key>``
``PINECONE_ENVIRONMENT=<Your Pinecone environment>``
``OPENAI_API_KEY=<Your OpenAI API key>``

### Running the Server

To start the server, run the following command:

``npm run start``

The server will start and listen on the port specified in your `.env` file.

## Usage

The server exposes three POST endpoints:

1. `/answer`: Accepts a JSON object with a key "question" and a string value, and returns a JSON object with a key "answer" and a string value. The answer is obtained using the OpenAI API.

2. `/summarize`: Accepts a JSON object with a key "text" and a string value, and returns a JSON object with the summarized text. The summarization is done using the OpenAI API.

3. `/ensureIndex`: Accepts a JSON object with a key "indexName" and a string value. It checks if an index with the provided name exists in the Pinecone environment. If the index doesn't exist, it creates one and populates it with documents present in the "./documents" directory. It supports text and PDF documents.

Please note that all endpoints require the request body to be in JSON format.

## Built With

- [Node.js](https://nodejs.org/)
- [Express](https://expressjs.com/)
- [OpenAI API](https://openai.com/)
- [Pinecone](https://www.pinecone.io/)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
