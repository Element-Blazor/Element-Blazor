window.getClientHeight = function (el) {
    return el.clientHeight || 0;
};
window.getClientWidth = function (el) {
    return el.clientWidth || 0;
};
window.getOffsetLeft = function (el) {
    return el.offsetLeft || 0;
};
window.getPaddingLeft = function (el) {
    return parseInt(getComputedStyle(el, null)["padding-left"]);
};
window.getPaddingRight = function (el) {
    return parseInt(getComputedStyle(el, null)["padding-right"]);
};