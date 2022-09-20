import * as getTest from "./tests/get-request.test";
import * as postTest from "./tests/post-request.test";
import * as fileTest from "./tests/post-file.test";

export const options = {
    scenarios: {
        ...getTest.options.scenarios,
        ...postTest.options.scenarios,
        ...fileTest.options.scenarios
    },
    thresholds: {
        ...getTest.options.thresholds,
    },
};

export function setup() {
    const getRequestSetup = getTest.setup();
    const postRequestSetup = postTest.setup();
    const fileUploadSetup = fileTest.setup();

    return {
        getRequest: getRequestSetup,
        postRequest: postRequestSetup,
        fileRequest: fileUploadSetup
    };
}

export function get(data: any) {
    getTest.get(data.getRequest);
}

export function post(data: any) {
    postTest.post(data.postRequest);
}

export function file(data: any) {
    fileTest.file(data.fileRequest);
}
