export const orthancURL = `http://localhost:1337/localhost:8042/`;

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

        return await fetch(url, requestInit);
    };

    let response = await makeRequest();
    return response.json();
}
