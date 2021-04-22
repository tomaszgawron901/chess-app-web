// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function fitContentAfterWindowsSizeChange(element) {
    console.log();
    element.addEventListener('click', () => {
        console.log(element.width, element.height);
    });
}
