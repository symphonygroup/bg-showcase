export default function ConstantVus(params: any) {
    const { exec, tags, environmentVariables, duration, vus } = params;

    let scenario : any = {
        executor: "constant-vus",
        vus: vus,
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
