import * as api from './api.js';
import * as domHelper from './domHelper.js';
import * as modal from './modal.js';

// Stores current queue interval to real-time elapsed time update.
let queueTimer;

/**
 * Clears all entries from list and fills it from array.
 * @param {Array} entries
 */
function updateEntriesDom(entries) {
    let list = document.querySelector('#notesList');

    domHelper.clearChildren(list);

    entries.forEach(e => {
        let entry = domHelper.unpackTemplate(document.querySelector('#noteTemplate'));


        entry.querySelector('.list-note').href = `/note/${e.height}`;
        entry.querySelector('.list-note-height').innerHTML = `#${e.height}`;
        entry.querySelector('.list-note-preview').innerHTML = e.comment;

        list.appendChild(entry);
    });
}

/**
 * Clear queue and fills it from array.
 * @param {Array} queue
 */
function updateQueueDom(queue) {
    let queueElement = document.querySelector('#queue');

    domHelper.clearChildren(queueElement);

    clearInterval(queueTimer);

    let i = 0;
    queue.forEach(e => {
        let entry = domHelper.unpackTemplate(document.querySelector('#queueEntryTemplate'));

        entry.querySelector('.queue-entry-timestamp').innerHTML = formatQueueTimestamp(new Date(e.timestamp));
        entry.querySelector('.queue-entry-preview').innerHTML = e.comment;

        entry.querySelector('.queue-entry-remove').addEventListener('click', () => {
            queueEntryRemoveModal.show({ id: e.id });
        });

        if (i == 0) {
            const entryElapsedElement = entry.querySelector('.queue-entry-elapsed');
            const entryRateElement = entry.querySelector('.queue-entry-rate');
            async function updateTimerHandler() {
                let processingEntry;
                try {
                    processingEntry = await api.getProcessingQueueEntry();
                } catch {
                    updateEntriesDom(await api.getAllEntries());
                    updateQueueDom(await api.getQueue());
                    return;
                }

                entryRateElement.innerHTML = formatHashrate(processingEntry.hashesPerSecond);

                let formatted = formatElapsed(new Date(e.timestamp));
                entryElapsedElement.innerHTML = formatted;
            }
            queueTimer = setInterval(updateTimerHandler, 1000);
            updateTimerHandler();
        }

        queueElement.appendChild(entry);

        i++;
    });

    clearQueueButton.disabled = queue.length == 0;

    if (queue.length == 0) {
        let stub = domHelper.unpackTemplate(document.querySelector('#queueStub'));
        queueElement.appendChild(stub);
    }
}

/**
 * Formats encoded date-time string to human-readable timestamp.
 * @param {Date} timestamp
 * @returns {string}
 */
function formatQueueTimestamp(date) {
    const options = {
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
    };
    return date.toLocaleString("en-US", options);
}

/**
 * Formats Date object to human-readable elapsed time format.
 * @param {Date} date
 * @returns {string}
 */
function formatElapsed(date) {
    const ms = Date.now() - date.getTime();
    const seconds = Math.floor(ms / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (days > 0) return `${days}d ${hours % 24}h`;
    if (hours > 0) return `${hours}h ${minutes % 60}m`;
    if (minutes > 0) return `${minutes}m ${seconds % 60}s`;
    return `${seconds}s`;
}

/**
 * Formats hashrate to units.
 * @param {number} hps Hashes per second.
 * @returns {string}
 */
function formatHashrate(hps) {
    const units = ['H/s', 'kH/s', 'MH/s', 'GH/s', 'TH/s', 'PH/s'];
    let i = 0;

    while (hps >= 1000 && i < units.length - 1) {
        hps /= 1000;
        i++;
    }

    return `${hps.toFixed(2)} ${units[i]}`;
}

async function filterInputHandler() {
    if (filterInput.value == '') {
        let entries = await api.getAllEntries();
        updateEntriesDom(entries);
    } else {
        let filtered = await api.getFilteredEntries(filterInput.value);
        updateEntriesDom(filtered);
    }
}

// Init modal windows.
const queueEntryRemoveModal = new modal.Modal('removeQueueEntryModal');
queueEntryRemoveModal.mapAction('cancel', async params => {
    queueEntryRemoveModal.hide();
    await api.removeEntryFromQueue(params.id);
    updateQueueDom(await api.getQueue());
});

const queueClearModal = new modal.Modal('clearQueueModal');
queueClearModal.mapAction('clear', async () => {
    queueClearModal.hide();
    await api.clearQueue();
    updateQueueDom(await api.getQueue());
});

// Add filter input handler.
let filterInput = document.querySelector('#filterInput');
filterInput.addEventListener('input', filterInputHandler);

// Add a handler for the clear queue button.
let clearQueueButton = document.querySelector('#clearQueueButton');
clearQueueButton.addEventListener('click', () => queueClearModal.show());

// Update notes and queue.
updateEntriesDom(await api.getAllEntries());
updateQueueDom(await api.getQueue());