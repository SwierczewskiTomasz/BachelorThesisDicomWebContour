export const orthancURL = "http://localhost:1337/localhost:8042/";
export const apiURL = "https://localhost:5001/";

export async function getBuilder<TResult>(
    baseUrl: string,
    resourceUrl: string
): Promise<TResult> {
    const makeRequest = async () => {
        const url = `${baseUrl}${resourceUrl}`;

        const requestInit: RequestInit = {
            method: "GET",
            headers: new Headers({})
        };

        const result = await fetch(url, requestInit);

        if (result.ok) {
            return result;
        }
        else {
            return undefined;
        }
    };

    let response = await makeRequest();
    return response.json();
}

export async function postBuilder<TResult>(
    baseUrl: string,
    resourceUrl: string,
    requestBody: any
): Promise<TResult> {
    const makeRequest = async () => {
        const url = `${baseUrl}${resourceUrl}`;

        const requestInit: RequestInit = {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json"
            },
            body: JSON.stringify(requestBody)
        };

        const result = await fetch(url, requestInit);

        if (result.ok) {
            return result;
        }
        else {
            return undefined;
        }
    };

    let response = await makeRequest();
    return response.json();
}

export async function deleteBuilder<TResult>(
    baseUrl: string,
    resourceUrl: string
): Promise<TResult> {
    const makeRequest = async () => {
        const url = `${baseUrl}${resourceUrl}`;

        const requestInit: RequestInit = {
            method: "DELETE",
            headers: new Headers({})
        };

        return await fetch(url, requestInit);
    };

    let response = await makeRequest();
    return response.json();
}
