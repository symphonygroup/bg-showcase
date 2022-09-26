export default function RampingVus(params: any) {
    const { exec, tags, environmentVariables, stages, gracefulRampDown, startVUs } = params;

    let scenario : any = {
        executor: "ramping-vus",
        startVUs,
        stages,
        gracefulRampDown
    };

    if (exec) {
        scenario.exec = exec;
    }

    if (environmentVariables) {
        scenario.env = environmentVariables;
    }

    if (tags) {
        scenario.tags = tags;
    }

    return scenario;
}


// {
//     executor: 'ramping-vus',
//     startVUs: 0,
//     stages: [
//       { duration: '20s', target: 10 },
//       { duration: '10s', target: 0 },
//     ],
//     gracefulRampDown: '0s',
//   }