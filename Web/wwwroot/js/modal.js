export class Modal {
    /**
     * HTML element associated with modal window.
     * @type {HTMLElement}
     */
    #element;

    /**
     * Modal window wrapper.
     * @type {HTMLElement}
     */
    #wrapper;

    /**
     * Parameters updated when the modal window is shown.
     */
    #params;

    /**
     * Gets the modal window visiblity state.
     * @returns {boolean}
     */
    get visible() {
        return this.#wrapper.classList.contains('modal-wrapper-visible');
    }

    /**
     * Creates a new modal window object.
     * @param {string} id HTML element ID associated with modal window.
     */
    constructor(id) {
        this.#element = document.getElementById(id);

        this.#wrap();
        this.#setupButtons();
    }

    /**
     * Wraps the modal element in wrapper.
     */
    #wrap() {
        let wrapper = document.createElement('div');
        wrapper.classList.add('modal-wrapper');

        this.#element.parentElement.insertBefore(wrapper, this.#element);
        wrapper.appendChild(this.#element);

        this.#wrapper = wrapper;
    }

    /**
     * Adds event listeners to buttons if they exist.
     */
    #setupButtons() {
        let closeButton = this.#element.querySelector('.modal-close-button');
        if (closeButton)
            closeButton.addEventListener('click', () => this.hide());
    }

    /**
     * Maps action to the button.
     * @param {string} name Action name.
     * @param {()} handler Action handler.
     */
    mapAction(name, handler) {
        let buttons = this.#element.querySelector('.modal-buttons');
        Array.from(buttons.children).forEach(button => {
            if (button.classList.contains('close-modal-button'))
                return;

            if (button.dataset.action != name)
                return;

            button.addEventListener('click', () => handler(this.#params));
        });
    }

    /**
     * Shows the modal window.
     * @param params Parameters object.
     */
    show(params) {
        this.#params = params;
        this.#wrapper.classList.add('modal-wrapper-visible');
    }

    /**
     * Hides the modal window.
     */
    hide() {
        this.#wrapper.classList.remove('modal-wrapper-visible');
    }
}