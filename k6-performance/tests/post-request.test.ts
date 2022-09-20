import { check, sleep } from "k6";
import http from "k6/http";
import { Options } from "k6/options";
import ConstantArrivalRate from "../scenarios/constant-arrival-rate.scenario";
import { randomIntBetween } from "../utils/randoms.util";

export const options: Options = {
    scenarios: {
        post: ConstantArrivalRate({ maxVUs: 50, duration: '10s', exec: 'post', rate: 1, timeUnit: '1s', preAllocatedVUs: 2 }),
    },
};

export function setup() {
    return { endpoint: "https://httpbin.org/status/400" };
}

export function post(data: any) {
    const res = http.post(data.endpoint);
    check(res, {
        "status is 400": () => res.status === 400,
    });
    sleep(randomIntBetween(1, 5));
}
