window.getChild = function (el, idx) {
    if (!el) {
        return;
    }
    return el.children[idx];
};
window.getClientHeight = function (el) {
    if (!el) {
        return this.document.body.clientHeight;
    }
    return el.clientHeight || 0;
};
window.submitForm = function (el, url) {
    if (!el) {
        return;
    }

    el.method = "post";
    el.action = url;
    el.submit();
};
window.upload = function (el) {
    if (!el) {
        return;
    }
    return el.children[1].click();
};

window.scanFiles = function (el) {
    if (!el) {
        return [];
    }
    let files = [];
    for (var i = 0; i < el.files.length; i++) {
        files.push(el.files.item(i).name);
    }
    return files;
};

window.uploadFile = function (el, fileName, url) {
    return new Promise((resolver, reject) => {
        let file = null;
        for (var i = 0; i < el.files.length; i++) {
            file = el.files[i];
            if (file.name == fileName) {
                break;
            }
        }
        let xhr = new XMLHttpRequest();
        xhr.open("POST", url);
        xhr.onreadystatechange = function () {
            if (this.readyState != 4) {
                return;
            }
            if (this.status < 200 || this.status >= 300) {
                return;
            }
            let response = JSON.parse(this.responseText);
            resolver([response.code.toString(), response.message]);
        };
        let formData = new this.FormData();
        formData.append("fileContent", file);
        xhr.send(formData);
    });
    //const temporaryFileReader = new FileReader();
    //return new Promise((resolve, reject) => {
    //    temporaryFileReader.onerror = () => {
    //        temporaryFileReader.abort();
    //        reject(new DOMException("Problem parsing input file."));
    //    };
    //    temporaryFileReader.addEventListener("load", function () {
    //        var data = [el.value, temporaryFileReader.result.split(',')[1]];
    //        resolve(data);
    //    }, false);
    //    temporaryFileReader.readAsDataURL(el.files[0]);
    //});
}
window.trigger = function (el, eventName) {
    var eventClass = "";

    // Different events have different event classes.
    // If this switch statement can't map an eventName to an eventClass,
    // the event firing is going to fail.
    switch (eventName) {
        case "click": // Dispatching of 'click' appears to not work correctly in Safari. Use 'mousedown' or 'mouseup' instead.
        case "mousedown":
        case "mouseup":
            eventClass = "MouseEvents";
            break;

        case "focus":
        case "change":
        case "blur":
        case "select":
            eventClass = "HTMLEvents";
            break;

        default:
            throw "fireEvent: Couldn't find an event class for event '" + eventName + "'.";
            break;
    }
    let event = this.document.createEvent(eventClass);
    event.initEvent(eventName);
    el.dispatchEvent(event);
}
window.getClientWidth = function (el) {
    if (!el) {
        return this.document.body.clientWidth;
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