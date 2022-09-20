export function BasicHeader(token: string = '') {
    const params : any = {
        headers: {
            "Content-Type": "application/json",
        }
    };

    if (token != '') {
        params.headers.Authorization = `Bearer  ${token}`;
    }

    return params;
}
