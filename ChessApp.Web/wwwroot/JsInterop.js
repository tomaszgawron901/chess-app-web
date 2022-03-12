export function SetSelectAndCopy(component) {
    component.select();
    document.execCommand("copy");
}

export function SetSelect(component) {
    component.select();
}