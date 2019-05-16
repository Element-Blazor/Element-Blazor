window.getClientHeight = function (el) {
    return el.clientHeight || 0;
};
window.getClientWidth = function (el) {
    return el.clientWidth || 0;
};
window.getOffsetLeft = function (el) {
    return el.offsetLeft || 0;
};
window.getOffsetTop = function (el) {
    return el.offsetTop || 0;
};
window.getPaddingLeft = function (el) {
    return parseInt(getComputedStyle(el, null)["padding-left"]);
};
window.getPaddingRight = function (el) {
    return parseInt(getComputedStyle(el, null)["padding-right"]);
};
window.removeSelf = function (el) {
    el.parentNode.removeChild(el);
};
window.documentAppendChild = function (el) {
    document.body.append(el);
};
window.getLeft = function (el) {
    return parseInt(getComputedStyle(el, null)["left"]);
};
window.getTop = function (el) {
    return parseInt(getComputedStyle(el, null)["top"]);
};
window.getBoundingClientRect = function (el) {
    return el.getBoundingClientRect();
};
window.setTransform = function (el, value) {
    el.style.transform = value;
};
window.setTransitionAsync = function (el, value) {
    el.style.transition = value;
};