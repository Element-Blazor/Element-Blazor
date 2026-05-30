(function () {
    const sessions = new Map();

    window.elementXBubbleListScrollToEnd = function (element, reverse, smooth, onlyWhenNearBottom, threshold) {
        if (!element) {
            return;
        }

        const distance = getDistanceFromEnd(element, reverse);
        if (onlyWhenNearBottom && distance > (threshold || 80)) {
            return;
        }

        const behavior = smooth ? "smooth" : "auto";
        if (reverse) {
            element.scrollTo({ top: 0, behavior });
            return;
        }

        element.scrollTo({ top: element.scrollHeight, behavior });
    };

    window.elementXBubbleListIsUserPinned = function (element, reverse, threshold) {
        if (!element) {
            return false;
        }

        return getDistanceFromEnd(element, reverse) > (threshold || 80);
    };

    window.elementXRecordIsSupported = function () {
        return !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia && window.MediaRecorder);
    };

    window.elementXRecordStart = async function (options) {
        if (!window.elementXRecordIsSupported()) {
            return {
                supported: false,
                started: false,
                state: "unsupported",
                message: "Browser audio recording is not available."
            };
        }

        const sessionId = options && options.sessionId ? options.sessionId : createSessionId();
        const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
        const recorderOptions = {};
        if (options && options.mimeType) {
            recorderOptions.mimeType = options.mimeType;
        }
        if (options && options.audioBitsPerSecond) {
            recorderOptions.audioBitsPerSecond = options.audioBitsPerSecond;
        }

        const recorder = new MediaRecorder(stream, recorderOptions);
        const chunks = [];
        recorder.ondataavailable = event => {
            if (event.data && event.data.size > 0) {
                chunks.push(event.data);
            }
        };

        sessions.set(sessionId, { recorder, stream, chunks, mimeType: recorder.mimeType || recorderOptions.mimeType || "audio/webm" });
        recorder.start();

        return {
            supported: true,
            started: true,
            sessionId,
            state: "recording",
            mimeType: recorder.mimeType || recorderOptions.mimeType || "audio/webm"
        };
    };

    window.elementXRecordStop = function (sessionId) {
        const session = resolveSession(sessionId);
        if (!session) {
            return {
                supported: window.elementXRecordIsSupported(),
                started: false,
                state: "idle",
                message: "Recording session was not found."
            };
        }

        return new Promise(resolve => {
            session.recorder.onstop = async function () {
                stopTracks(session.stream);
                sessions.delete(session.sessionId);
                const blob = new Blob(session.chunks, { type: session.mimeType });
                resolve({
                    supported: true,
                    started: false,
                    sessionId: session.sessionId,
                    state: "stopped",
                    mimeType: blob.type,
                    fileName: "recording.webm",
                    size: blob.size,
                    base64: await blobToBase64(blob)
                });
            };
            session.recorder.stop();
        });
    };

    window.elementXRecordCancel = function (sessionId) {
        const session = resolveSession(sessionId);
        if (!session) {
            return {
                supported: window.elementXRecordIsSupported(),
                started: false,
                state: "idle"
            };
        }

        stopTracks(session.stream);
        if (session.recorder.state !== "inactive") {
            session.recorder.stop();
        }
        sessions.delete(session.sessionId);
        return {
            supported: true,
            started: false,
            sessionId: session.sessionId,
            state: "canceled"
        };
    };

    function resolveSession(sessionId) {
        if (sessionId && sessions.has(sessionId)) {
            const session = sessions.get(sessionId);
            session.sessionId = sessionId;
            return session;
        }

        const first = sessions.entries().next();
        if (first.done) {
            return null;
        }

        first.value[1].sessionId = first.value[0];
        return first.value[1];
    }

    function stopTracks(stream) {
        if (!stream) {
            return;
        }

        stream.getTracks().forEach(track => track.stop());
    }

    function getDistanceFromEnd(element, reverse) {
        if (reverse) {
            return Math.abs(element.scrollTop);
        }

        return element.scrollHeight - element.scrollTop - element.clientHeight;
    }

    function createSessionId() {
        if (window.crypto && window.crypto.randomUUID) {
            return window.crypto.randomUUID();
        }

        return "record-" + Date.now().toString(36) + Math.random().toString(36).slice(2);
    }

    function blobToBase64(blob) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = () => resolve(String(reader.result || ""));
            reader.onerror = reject;
            reader.readAsDataURL(blob);
        });
    }
})();
