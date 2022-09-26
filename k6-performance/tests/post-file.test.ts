import { sleep, check } from "k6";
import { Options } from "k6/options";
import http, { StructuredRequestBody } from "k6/http";
import RampingVus from "../scenarios/ramping-vus.scenario";

const binFile = open("test.jpg", "b");

const stages: any[] = [
    { duration: "10s", target: 5 },
    { duration: "5s", target: 10 },
    { duration: "3s", target: 3 },
];

export let options: Options = {
    scenarios: {
        file: RampingVus({ exec: "file", gracefulRampDown: "5s", startVUs: 2, stages: stages }),
    },
};

export function setup() {
    return { endpoint: "https://httpbin.org/post" };
}

export function file(data: any) {
    const postData: StructuredRequestBody = { file: http.file(binFile) };
    const response = http.post(data.endpoint, postData);

    check(response, {
        "status is 200": (r) => r.status === 200,
    });

    sleep(1);
}
