For additional details, see [this article in the k6 docs](https://k6.io/docs/using-k6/modules)

## What is this?  
K6 Load testing example with TypeScript.

This means that you can write scripts using:
1. node module resolution
2. external node modules and getting them automatically bundled.
3. easily group tests

## Requirements  

Download and install latest [node.js](https://nodejs.org/en/download/), if you don't have it installed.

## How to use?

To install and configure K6 framework on your machine follow the [installation guide](https://k6.io/docs/getting-started/installation/).
```
# package installation step - only needs to be run for the initial setup and if packages are changed
npm install

# build step - only needs to be run for the initial setup and if tests are changed
npm run build

# run step - can only be run if the project is built.
npm run test

# alternative - run both build and run step at once
npm run start
```

If you wish to use docker execution:
```
# docker execution
docker run -v $(pwd)/build:/build loadimpact/k6 run /dist/index.js 
```