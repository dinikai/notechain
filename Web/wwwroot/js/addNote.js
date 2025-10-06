import * as api from './api.js';

async function submitButtonHandler() {
    await api.addEntryToQueue(contentInput.value, commentInput.value);
    isDirty = false;
    window.location = '/';
}

function fieldChangeHandler() {
    isDirty = true;
    submitButton.disabled = commentInput.value == '' || contentInput.value == '';
}

const commentInput = document.querySelector('#commentInput');
const contentInput = document.querySelector('#contentInput');
const submitButton = document.querySelector('#addButton');

// True if fields was been edited.
let isDirty;

// Add submit button listener.
submitButton.addEventListener('click', submitButtonHandler);

// Add field change listeners.
commentInput.addEventListener('input', fieldChangeHandler);
contentInput.addEventListener('input', fieldChangeHandler);

// Add page exit listener.
window.addEventListener('beforeunload', (e) => {
    if (isDirty) {
        e.preventDefault();
        e.returnValue = '';
    }
});