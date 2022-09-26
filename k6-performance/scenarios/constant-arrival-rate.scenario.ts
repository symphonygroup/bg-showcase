export default function ConstantArrivalRate(params: any) {
    const { exec, tags, environmentVariables, duration, maxVUs, rate, timeUnit, preAllocatedVUs } = params;

    let scenario : any = {
        executor: "constant-arrival-rate",
        maxVUs,
        preAllocatedVUs,
        rate,
        timeUnit,
        duration: duration,
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