import { sleep, check } from "k6";
import { Options } from "k6/options";
import http from "k6/http";
import { BasicHeader } from "../utils/basic-header.util";
import ConstantVus from "../scenarios/constant-vus.scenario";

export const options: Options = {
    scenarios: {
        get: ConstantVus({ vus: 50, duration: '10s', exec: 'get' })
    },
    thresholds: {
        'http_req_duration{scenario:get}': ['p(95)<500'],
    }
};

export function setup() {
    return { endpoint: 'https://test-api.k6.io' };
}

export function get(data: any) {
    const res = http.get(data.endpoint, BasicHeader());
    check(res, {
        "status is 200": () => res.status === 200,
    });
    sleep(1);
};
