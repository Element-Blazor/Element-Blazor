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
window.execFocus = function (el) {
    if (!el) {
        return;
    }
    el.focus();
}
window.execBlur = function (el) {
    if (!el) {
        return;
    }
    el.blur();
}
window.execSelect = function (el) {
    if (!el || !el.select) {
        return;
    }
    el.select();
}
window.elementMentionGetSelection = function (el) {
    if (!el) {
        return [0, 0];
    }
    return [el.selectionStart || 0, el.selectionEnd || 0];
}
window.elementMentionSetSelection = function (el, start, end) {
    if (!el || !el.setSelectionRange) {
        return;
    }
    el.setSelectionRange(start || 0, end || 0);
}
window.scrollElementIntoViewById = function (id, options) {
    if (!id) {
        return;
    }
    var el = document.getElementById(id);
    if (!el) {
        return;
    }
    if (options) {
        try {
            el.scrollIntoView(JSON.parse(options));
            return;
        } catch (e) {
        }
    }
    el.scrollIntoView({ block: "nearest", inline: "nearest" });
}
window.elementConfigApplyNamespace = function (root, namespace, defaultNamespace) {
    if (!root || !namespace || !defaultNamespace || namespace === defaultNamespace) {
        return;
    }

    var previousNamespace = root.dataset.elementNamespace || defaultNamespace;
    root.dataset.elementNamespace = namespace;

    var rewriteElement = function (el) {
        if (!el || !el.classList || root.__elementApplyingNamespace) {
            return;
        }

        root.__elementApplyingNamespace = true;
        try {
            Array.from(el.classList).forEach(function (className) {
                var prefix = null;
                if (className.indexOf(defaultNamespace + "-") === 0) {
                    prefix = defaultNamespace + "-";
                } else if (previousNamespace && className.indexOf(previousNamespace + "-") === 0) {
                    prefix = previousNamespace + "-";
                }

                if (!prefix) {
                    return;
                }

                el.classList.remove(className);
                el.classList.add(namespace + "-" + className.substring(prefix.length));
            });
        } finally {
            root.__elementApplyingNamespace = false;
        }
    };

    var rewriteTree = function (el) {
        rewriteElement(el);
        if (!el || !el.querySelectorAll) {
            return;
        }
        el.querySelectorAll("[class]").forEach(rewriteElement);
    };

    rewriteTree(root);
    if (root.__elementNamespaceObserver) {
        root.__elementNamespaceObserver.disconnect();
    }

    root.__elementNamespaceObserver = new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            if (mutation.type === "attributes") {
                rewriteElement(mutation.target);
                return;
            }

            mutation.addedNodes.forEach(function (node) {
                if (node.nodeType === 1) {
                    rewriteTree(node);
                }
            });
        });
    });
    root.__elementNamespaceObserver.observe(root, {
        subtree: true,
        childList: true,
        attributes: true,
        attributeFilter: ["class"]
    });
};
window.upload = function (el) {
    if (!el) {
        return;
    }
    var input = el.querySelector && el.querySelector('input[type="file"]');
    if (!input || input.disabled) {
        return;
    }
    return input.click();
};

window.uploadDrop = function (event, el) {
    if (event && event.preventDefault) {
        event.preventDefault();
    }
    if (!el || !event || !event.dataTransfer) {
        return;
    }
    var input = el.querySelector && el.querySelector('input[type="file"]');
    if (!input || input.disabled) {
        return;
    }
    input.files = event.dataTransfer.files;
    input.dispatchEvent(new Event("change", { bubbles: true }));
};

var uploader, uploadUrl, uploadOptions;
window.clear = function (el) {
    if (!el) {
        return;
    }
    el.value = null;
};

async function executePasteUpload(event) {
    var items = event.clipboardData && event.clipboardData.items;
    var files = [];
    if (items && items.length) {
        // 检索剪切板items
        for (var i = items.length - 1; i >= 0; i--) {
            if (items[i].kind != "file") {
                continue;
            }
            file = items[i].getAsFile();
            files.push(file);
            break;
        }
    }

    let ids = await convertFiles(files).then(async fs => {
        return await uploader.invokeMethodAsync("previewFiles", fs);
    });
    for (var j = 0; j < files.length; j++) {
        var result = await new Promise(resolver => {
            _uploadFile(uploadUrl, files[j], resolver, uploadOptions);
        });
        await uploader.invokeMethodAsync("fileUploaded", result, ids[j]);
    }
    await uploader.invokeMethodAsync("filesUploaded");
};
window.registerPasteUpload = function (upload, url, options) {
    uploader = upload;
    uploadUrl = url;
    uploadOptions = options || {};
    document.addEventListener('paste', executePasteUpload);
};
window.unRegisterPasteUpload = function () {
    this.document.removeEventListener('paste', executePasteUpload);
};
function convertFiles(files) {
    let scanFile = function (file) {
        return new Promise((resolver) => {
            let infos = [file.name, file.size.toString()];
            if (file.type.indexOf("image") != -1) {
                let img = new Image();
                img.onload = function () {
                    infos.push(this.width.toString());
                    infos.push(this.height.toString());
                    infos.push(img.src);
                    resolver(infos);
                };
                img.src = URL.createObjectURL(file);
                return;
            }
            resolver(infos);
        });
    };
    let filePromises = [];
    for (var i = 0; i < files.length; i++) {
        let file = files.item ? files.item(i) : files[i];

        filePromises.push(new Promise((resolver) => {
            scanFile(file).then(infos => {
                resolver(infos);
            });
        }));
    }
    return Promise.all(filePromises);
};
window.scanFiles = function (el) {
    if (!el) {
        return [];
    }

    let files = this.convertFiles(el.files);
    return files;
};
async function _uploadFile(url, file, callback, options) {
    options = options || {};
    let xhr = new XMLHttpRequest();
    xhr.open((options.method || "POST").toUpperCase(), url);
    xhr.onreadystatechange = function () {
        if (this.readyState != 4) {
            return;
        }
        if (this.status < 200 || this.status >= 300) {
            callback(["1", this.statusText || "Upload failed.", "", ""]);
            return;
        }
        let response = JSON.parse(this.responseText);
        callback([response.code.toString(), response.message || "", response.id, response.url]);
    };
    xhr.onerror = function () {
        callback(["1", "Upload failed.", "", ""]);
    };
    xhr.withCredentials = options.withCredentials !== false;
    xhr.setRequestHeader("x-requested-with", "XMLHttpRequest");
    if (options.headers) {
        Object.keys(options.headers).forEach(function (key) {
            xhr.setRequestHeader(key, options.headers[key]);
        });
    }
    let formData = new this.FormData();
    if (options.data) {
        Object.keys(options.data).forEach(function (key) {
            formData.append(key, options.data[key]);
        });
    }
    formData.append(options.fileFieldName || "fileContent", file);
    xhr.send(formData);
};
window.uploadFile = function (el, fileName, url, options) {
    return new Promise((resolver, reject) => {
        let file = null;
        for (var i = 0; i < el.files.length; i++) {
            file = el.files[i];
            if (file.name == fileName) {
                break;
            }
        }
        _uploadFile(url, file, resolver, options);
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
};
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
};
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
};
window.disableYScroll = function () {
    this.document.body.style.overflowY = "hidden";
};
window.enableXScroll = function () {
    this.document.body.style.overflowX = "auto";
};
window.enableYScroll = function () {
    this.document.body.style.overflowY = "auto";
};
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
};
window.getLeftRelativeBody = function (el) {
    if (!el) {
        return 0;
    }
    var bodyRect = document.body.getBoundingClientRect(),
        elemRect = el.getBoundingClientRect(),
        offset = elemRect.left - bodyRect.left;
    return this.parseFloat(offset);
};
window.isScrollAtEnd = function (el) {
    if (!el) {
        return false;
    }
    return el.scrollTop + el.clientHeight >= el.scrollHeight - 1;
};
window.getScrollLeft = function (el) {
    if (!el) {
        return 0;
    }
    return el.scrollLeft || 0;
};
window.setScrollLeft = function (el, value) {
    if (!el) {
        return;
    }
    el.scrollLeft = value || 0;
};
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
window.getDomGuid = function (el) {
    for (var index in el.attributes) {
        var guid = el.attributes[index].name;
        if (!guid) {
            continue;
        }
        if (guid.indexOf("_bl_") == 0) {
            return guid;
        }
    }
};
window.onblazortransitionend = function (e) {
    var instance = window[getDomGuid(e.target)];
    if (!instance) {
        return;
    }
    console.log("end:");
    console.dir(instance)

    return instance.invokeMethodAsync('AnimationEnd', e.propertyName);
}
window.RegisterAnimationBegin = function (transitionRef, el) {
    window[getDomGuid(el)] = transitionRef;
    el.addEventListener('transitionend', onblazortransitionend);
}
window.elementScrollbarScrollTo = function (el, top, left) {
    if (!el) {
        return;
    }
    el.scrollTo({ top: top || 0, left: left || 0 });
};
window.elementScrollbarSetScrollTop = function (el, top) {
    if (!el) {
        return;
    }
    el.scrollTop = top || 0;
};
window.elementScrollbarSetScrollLeft = function (el, left) {
    if (!el) {
        return;
    }
    el.scrollLeft = left || 0;
};
window.elementScrollbarGetState = function (el) {
    if (!el) {
        return {
            scrollTop: 0,
            scrollLeft: 0,
            scrollHeight: 0,
            scrollWidth: 0,
            clientHeight: 0,
            clientWidth: 0
        };
    }
    return {
        scrollTop: el.scrollTop || 0,
        scrollLeft: el.scrollLeft || 0,
        scrollHeight: el.scrollHeight || 0,
        scrollWidth: el.scrollWidth || 0,
        clientHeight: el.clientHeight || 0,
        clientWidth: el.clientWidth || 0
    };
};
window.elementXBubbleListScrollToEnd = function (el, reverse, smooth, onlyWhenNearBottom, threshold) {
    if (!el) {
        return;
    }

    threshold = typeof threshold === "number" ? threshold : 80;
    var maxScrollTop = Math.max(0, (el.scrollHeight || 0) - (el.clientHeight || 0));
    var distance = reverse
        ? Math.abs(el.scrollTop || 0)
        : Math.abs(maxScrollTop - (el.scrollTop || 0));

    if (onlyWhenNearBottom && distance > threshold) {
        return;
    }

    if (el.scrollTo) {
        el.scrollTo({
            top: reverse ? 0 : maxScrollTop,
            behavior: smooth ? "smooth" : "auto"
        });
        return;
    }

    el.scrollTop = reverse ? 0 : maxScrollTop;
};
window.elementResolveScrollContainer = function (target) {
    if (!target) {
        return window;
    }
    if (typeof target === "string") {
        return document.querySelector(target) || window;
    }
    return target;
};
window.elementGetScrollTop = function (container) {
    if (!container || container === window || container === document || container === document.body || container === document.documentElement) {
        return window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
    }
    return container.scrollTop || 0;
};
window.elementSetScrollTop = function (container, top, behavior) {
    top = top || 0;
    behavior = behavior || "smooth";
    if (!container || container === window || container === document || container === document.body || container === document.documentElement) {
        window.scrollTo({ top: top, behavior: behavior });
        return;
    }
    if (container.scrollTo) {
        container.scrollTo({ top: top, behavior: behavior });
        return;
    }
    container.scrollTop = top;
};
window.elementGetContainerRect = function (container) {
    if (!container || container === window || container === document || container === document.body || container === document.documentElement) {
        return {
            top: 0,
            bottom: window.innerHeight || document.documentElement.clientHeight || 0,
            height: window.innerHeight || document.documentElement.clientHeight || 0
        };
    }
    return container.getBoundingClientRect();
};
window.elementAffixInit = function (root, dotnet, options) {
    if (!root) {
        return;
    }

    options = options || {};
    if (root.__elementAffixCleanup) {
        root.__elementAffixCleanup();
    }

    var content = root.firstElementChild || root;
    var container = elementResolveScrollContainer(options.target);
    var offset = parseInt(options.offset || 0);
    var position = options.position || "top";
    var zIndex = parseInt(options.zIndex || 100);
    var placeholderHeight = 0;

    var update = function () {
        var scrollTop = elementGetScrollTop(container);
        var rootRect = root.getBoundingClientRect();
        var containerRect = elementGetContainerRect(container);
        var fixed = position === "bottom"
            ? rootRect.bottom >= containerRect.bottom - offset
            : rootRect.top <= containerRect.top + offset;

        if (fixed) {
            placeholderHeight = placeholderHeight || root.offsetHeight || content.offsetHeight || 0;
            root.style.height = placeholderHeight + "px";
            content.style.position = "fixed";
            content.style.zIndex = zIndex;
            content.style.width = rootRect.width + "px";
            content.style.left = rootRect.left + "px";
            if (position === "bottom") {
                content.style.bottom = offset + "px";
                content.style.top = "";
            } else {
                content.style.top = offset + "px";
                content.style.bottom = "";
            }
        } else {
            root.style.height = "";
            content.style.position = "";
            content.style.zIndex = "";
            content.style.width = "";
            content.style.left = "";
            content.style.top = "";
            content.style.bottom = "";
        }

        if (dotnet) {
            dotnet.invokeMethodAsync("SetFixed", fixed, Math.round(scrollTop));
        }
    };

    var listenerTarget = container === window ? window : container;
    listenerTarget.addEventListener("scroll", update, { passive: true });
    window.addEventListener("resize", update);
    root.__elementAffixCleanup = function () {
        listenerTarget.removeEventListener("scroll", update);
        window.removeEventListener("resize", update);
    };
    update();
};
window.elementBacktopInit = function (root, dotnet, options) {
    options = options || {};
    var container = elementResolveScrollContainer(options.target);
    var visibilityHeight = parseInt(options.visibilityHeight || 200);
    if (root && root.__elementBacktopCleanup) {
        root.__elementBacktopCleanup();
    }

    var update = function () {
        var visible = elementGetScrollTop(container) >= visibilityHeight;
        if (dotnet) {
            dotnet.invokeMethodAsync("SetVisible", visible);
        }
    };

    var listenerTarget = container === window ? window : container;
    listenerTarget.addEventListener("scroll", update, { passive: true });
    if (root) {
        root.__elementBacktopCleanup = function () {
            listenerTarget.removeEventListener("scroll", update);
        };
    }
    update();
};
window.elementBacktopScrollTo = function (target) {
    elementSetScrollTop(elementResolveScrollContainer(target), 0, "smooth");
};
window.elementAnchorScrollTo = function (href, containerSelector, offset, duration) {
    if (!href || href.charAt(0) !== "#") {
        if (href) {
            location.href = href;
        }
        return;
    }

    var target = document.querySelector(href);
    if (!target) {
        return;
    }

    var container = elementResolveScrollContainer(containerSelector);
    var scrollTop = elementGetScrollTop(container);
    var containerRect = elementGetContainerRect(container);
    var targetRect = target.getBoundingClientRect();
    var nextTop = scrollTop + targetRect.top - containerRect.top - (parseInt(offset || 0));
    elementSetScrollTop(container, nextTop, parseInt(duration || 0) <= 0 ? "auto" : "smooth");
    if (history && history.replaceState) {
        history.replaceState(null, "", href);
    }
};
window.elementAnchorInit = function (root, dotnet, options) {
    if (!root) {
        return;
    }

    options = options || {};
    if (root.__elementAnchorCleanup) {
        root.__elementAnchorCleanup();
    }

    var container = elementResolveScrollContainer(options.container);
    var offset = parseInt(options.offset || 0);
    var bound = parseInt(options.bound || 15);
    var update = function () {
        var links = Array.prototype.slice.call(root.querySelectorAll("[data-anchor-href]"));
        var activeHref = "";
        var containerRect = elementGetContainerRect(container);
        links.forEach(function (link) {
            var href = link.getAttribute("data-anchor-href");
            if (!href || href.charAt(0) !== "#") {
                return;
            }
            var target = document.querySelector(href);
            if (!target) {
                return;
            }
            var top = target.getBoundingClientRect().top - containerRect.top;
            if (top <= offset + bound) {
                activeHref = href;
            }
        });
        if (dotnet && activeHref) {
            dotnet.invokeMethodAsync("SetActiveHref", activeHref);
        }
    };

    var listenerTarget = container === window ? window : container;
    listenerTarget.addEventListener("scroll", update, { passive: true });
    window.addEventListener("resize", update);
    root.__elementAnchorCleanup = function () {
        listenerTarget.removeEventListener("scroll", update);
        window.removeEventListener("resize", update);
    };
    update();
};
window.elementSplitterInit = function (root) {
    if (!root || root.dataset.elementSplitterReady === "true") {
        return;
    }

    root.dataset.elementSplitterReady = "true";
    var isVertical = root.classList.contains("el-splitter--vertical");
    var bars = root.querySelectorAll(":scope > .el-splitter__bar");

    bars.forEach(function (bar) {
        bar.addEventListener("dblclick", function () {
            if (bar.dataset.collapsible !== "true") {
                return;
            }

            var panel = bar.previousElementSibling;
            if (!panel || !panel.classList.contains("el-splitter-panel")) {
                return;
            }

            panel.style.flexBasis = "0px";
            panel.style[isVertical ? "height" : "width"] = "0px";
        });

        bar.addEventListener("mousedown", function (event) {
            var panel = bar.previousElementSibling;
            if (!panel || !panel.classList.contains("el-splitter-panel")) {
                return;
            }

            event.preventDefault();
            var start = isVertical ? event.clientY : event.clientX;
            var rect = panel.getBoundingClientRect();
            var startSize = isVertical ? rect.height : rect.width;

            var onMove = function (moveEvent) {
                var point = isVertical ? moveEvent.clientY : moveEvent.clientX;
                var nextSize = Math.max(0, startSize + point - start);
                panel.style.flexBasis = nextSize + "px";
                panel.style[isVertical ? "height" : "width"] = nextSize + "px";
            };

            var onUp = function () {
                document.removeEventListener("mousemove", onMove);
                document.removeEventListener("mouseup", onUp);
                document.body.style.userSelect = "";
                bar.classList.remove("is-active");
            };

            document.body.style.userSelect = "none";
            bar.classList.add("is-active");
            document.addEventListener("mousemove", onMove);
            document.addEventListener("mouseup", onUp);
        });
    });
};
