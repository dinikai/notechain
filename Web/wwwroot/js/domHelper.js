/**
 * Removes all children elements.
 */
export function clearChildren(element) {
    while (element.childElementCount > 0)
        element.removeChild(element.firstChild);
}

/**
 * Unpacks the given template and returns copy of its content.
 * @param {Node} template Template to be copied.
 * @returns {Node}
 */
export function unpackTemplate(template) {
    return template.content.cloneNode(true);
}