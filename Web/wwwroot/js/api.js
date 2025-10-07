export async function getAllEntries() {
    return await fetchJson('api/entries/getAll', 'GET');
}

export async function getEntryById(id) {
    return await fetchJson('api/entries/getById', 'GET', { id: id });
}

export async function getEntryByHeight(height) {
    return await fetchJson('api/entries/getByHeight', 'GET', { height: height });
}

export async function getFilteredEntries(query) {
    return await fetchJson('api/entries/getFiltered', 'GET', { query: query });
}



export async function getQueue() {
    return await fetchJson('api/queue/get', 'GET');
}

export async function getProcessingQueueEntry() {
    return await fetchJson('api/queue/getProcessing', 'GET');
}

export async function addEntryToQueue(data, comment) {
    await fetch('api/queue/add?' + new URLSearchParams({
        data: data,
        comment: comment
    }), {
        method: 'POST'
    });
}

export async function removeEntryFromQueue(id) {
    await fetch('api/queue/remove?' + new URLSearchParams({
        id: id
    }), {
        method: 'POST'
    });
}

export async function clearQueue() {
    await fetch('api/queue/clear', {
        method: 'POST'
    });
}

async function fetchJson(url, method, params) {
    let result = await fetch(
        url + '?' + new URLSearchParams(params),
        { method: method }
    );
    return await result.json();
}