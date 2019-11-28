window.getChild = function (el, idx) {
    if (!el) {
        return;
    }
    return el.children[idx];
};
window.getClientHeight = function (el) {
    if (!el) {
        return 0;
    }
    return el.clientHeight || 0;
};
window.getClientWidth = function (el) {
    if (!el) {
        return 0;
    }
    return el.clientWidth || 0;
};
window.getOffsetLeft = function (el) {
    if (!el) {
        return 0;
    }
    return el.offsetLeft || 0;
};
window.setDisabled = function (el, disabled) {
    if (!el) {
        return;
    }
    el.disabled = disabled;
};
window.getOffsetTop = function (el) {
    if (!el) {
        return 0;
    }
    return el.offsetTop || 0;
};
window.getPaddingLeft = function (el) {
    if (!el) {
        return 0;
    }
    return parseInt(getComputedStyle(el, null)["padding-left"]);
};
window.getPaddingRight = function (el) {
    if (!el) {
        return 0;
    }
    return parseInt(getComputedStyle(el, null)["padding-right"]);
};
window.disableXScroll = function () {
    this.document.body.style.overflowX = "hidden";
}
window.disableYScroll = function () {
    this.document.body.style.overflowY = "hidden";
}
window.enableXScroll = function () {
    this.document.body.style.overflowX = "auto";
}
window.enableYScroll = function () {
    this.document.body.style.overflowY = "auto";
}
window.removeSelf = function (el) {
    if (!el) {
        return;
    }
    el.parentNode.removeChild(el);
};
window.documentAppendChild = function (el) {
    if (!el) {
        return;
    }
    document.body.append(el);
};
window.elementAppendChild = function (parent, child) {
    if (!parent || !child) {
        return 0;
    }
    parent.append(child);
};
window.getTopRelativeBody = function (el) {
    if (!el) {
        return 0;
    }
    var bodyRect = document.body.getBoundingClientRect(),
        elemRect = el.getBoundingClientRect(),
        offset = elemRect.top - bodyRect.top;
    return this.parseFloat(offset);
}
window.Remove = function (el) {
    if (!el) {
        return;
    }
    el.remove();
};
window.getLeft = function (el) {
    if (!el) {
        return 0;
    }
    return parseInt(getComputedStyle(el, null)["left"]);
};
window.getTop = function (el) {
    if (!el) {
        return 0;
    }
    return parseInt(getComputedStyle(el, null)["top"]);
};
window.getBoundingClientRect = function (el) {
    if (!el) {
        return {};
    }
    return el.getBoundingClientRect();
};
window.setTransform = function (el, value) {
    if (!el) {
        return;
    }
    el.style.transform = value;
};
window.setStyle = function (el, key, value) {
    if (!el) {
        return;
    }
    el.style[key] = value;
};
window.getMarginTop = function (el) {
    if (!el) {
        return "";
    }
    return el.style["margin-top"];
};
window.clearStyle = function (el, key) {
    if (!el) {
        return;
    }
    el.style[key] = "";
};
window.setTransitionAsync = function (el, value) {
    if (!el) {
        return;
    }
    el.style.transition = value;
};