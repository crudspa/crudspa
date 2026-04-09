function logConsoleError(message, error) {
    if (typeof console === "undefined" || !console || typeof console.error !== "function")
        return;

    if (typeof error === "undefined")
        console.error(message);
    else
        console.error(message, error);
}

function logInteropError(name, error) {
    logConsoleError("An error occurred in " + name + ".", error);
}

function getElementById(id) {
    if (!id)
        return null;

    return document.getElementById(id);
}

function isFiniteNumber(value) {
    return typeof value === "number" && isFinite(value);
}

function clearElementTimer(element, key) {
    if (!element || !element[key])
        return;

    clearTimeout(element[key]);
    element[key] = null;
}

function attachPromiseCatch(promise, handler) {
    if (promise && typeof promise.catch === "function")
        promise.catch(handler);
}

function invokeDotNet(reference, methodName) {
    if (!reference || typeof reference.invokeMethodAsync !== "function")
        return;

    var args = Array.prototype.slice.call(arguments, 2);

    try {
        var promise = reference.invokeMethodAsync.apply(reference, [methodName].concat(args));
        attachPromiseCatch(promise, function (error) {
            logConsoleError("Error invoking " + methodName + ".", error);
        });
    } catch (error) {
        logConsoleError("Error invoking " + methodName + ".", error);
    }
}

function canUseSmoothScroll() {
    return !!document
        && !!document.documentElement
        && !!document.documentElement.style
        && "scrollBehavior" in document.documentElement.style;
}

function getWindowScrollX() {
    if (typeof window.scrollX === "number")
        return window.scrollX;

    return window.pageXOffset || 0;
}

function getWindowScrollY() {
    if (typeof window.scrollY === "number")
        return window.scrollY;

    return window.pageYOffset || 0;
}

function decodeCookieValue(value) {
    if (typeof value !== "string")
        return value;

    try {
        return decodeURIComponent(value);
    } catch (error) {
        return value;
    }
}

// #region Browser features

function getCookieValue(key) {
    try {
        if (!key)
            return null;

        var encodedKey = encodeURIComponent(String(key)) + "=";
        var allCookies = document.cookie || "";

        if (!allCookies)
            return null;

        var items = allCookies.split(";");

        for (var i = 0; i < items.length; i++) {
            var item = items[i];

            while (item.charAt(0) === " ")
                item = item.substring(1);

            if (item.indexOf(encodedKey) === 0)
                return decodeCookieValue(item.substring(encodedKey.length));
        }

        return null;
    } catch (ex) {
        logInteropError("getCookieValue", ex);
        return null;
    }
}

function setCookieValue(key, value, expiresUtc) {
    try {
        if (!key)
            return;

        var cookie = encodeURIComponent(String(key))
            + "="
            + encodeURIComponent(value == null ? "" : String(value))
            + "; path=/; secure; samesite=lax;";

        if (expiresUtc)
            cookie += " expires=" + expiresUtc + ";";

        document.cookie = cookie;
    } catch (ex) {
        logInteropError("setCookieValue", ex);
    }
}

function fallbackCopyToClipboard(text, originalError) {
    try {
        var textArea = document.createElement("textarea");
        var activeElement = document.activeElement;

        textArea.value = text;
        textArea.setAttribute("readonly", "readonly");
        textArea.style.position = "fixed";
        textArea.style.top = "-1000px";
        textArea.style.left = "-1000px";
        textArea.style.opacity = "0";
        textArea.style.pointerEvents = "none";

        document.body.appendChild(textArea);
        textArea.focus();
        textArea.select();

        if (typeof document.execCommand === "function")
            document.execCommand("copy");

        document.body.removeChild(textArea);

        if (activeElement && typeof activeElement.focus === "function")
            activeElement.focus();

        if (originalError)
            logConsoleError("navigator.clipboard.writeText() failed. Falling back to execCommand().", originalError);
    } catch (fallbackError) {
        if (originalError)
            logConsoleError("navigator.clipboard.writeText() failed.", originalError);

        logConsoleError("Fallback: Unable to copy.", fallbackError);
    }
}

function copyToClipboard(text) {
    try {
        var value = text == null ? "" : String(text);

        if (typeof navigator !== "undefined"
            && navigator.clipboard
            && typeof navigator.clipboard.writeText === "function") {
            try {
                var writePromise = navigator.clipboard.writeText(value);

                if (writePromise && typeof writePromise.then === "function") {
                    writePromise.then(function () { }, function (error) {
                        fallbackCopyToClipboard(value, error);
                    });
                    return;
                }

                return;
            } catch (clipboardError) {
                fallbackCopyToClipboard(value, clipboardError);
                return;
            }
        }

        fallbackCopyToClipboard(value);
    } catch (ex) {
        logInteropError("copyToClipboard", ex);
    }
}

// #endregion

// #region Scrolling

function scrollToTop() {
    try {
        if (canUseSmoothScroll())
            window.scrollTo({ top: 0, behavior: "smooth" });
        else
            window.scrollTo(0, 0);
    } catch (ex) {
        logInteropError("scrollToTop", ex);
    }
}

function scrollToBottom() {
    try {
        var bodyHeight = document.body ? document.body.scrollHeight : 0;
        var rootHeight = document.documentElement ? document.documentElement.scrollHeight : 0;
        var scrollHeight = Math.max(bodyHeight, rootHeight);

        if (canUseSmoothScroll())
            window.scrollTo({ top: scrollHeight, behavior: "smooth" });
        else
            window.scrollTo(0, scrollHeight);
    } catch (ex) {
        logInteropError("scrollToBottom", ex);
    }
}

function scrollToId(id) {
    try {
        var element = getElementById(id);

        if (!element)
            return;

        if (canUseSmoothScroll())
            element.scrollIntoView({ behavior: "smooth" });
        else {
            var rect = element.getBoundingClientRect();
            window.scrollTo(rect.left + getWindowScrollX(), rect.top + getWindowScrollY());
        }
    } catch (ex) {
        logInteropError("scrollToId", ex);
    }
}

// #endregion

// #region Audio

function playSound(key) {
    try {
        var audioElement = getElementById("sound-" + key);

        if (!audioElement || typeof audioElement.play !== "function")
            return;

        audioElement.currentTime = 0;

        var playPromise = audioElement.play();
        attachPromiseCatch(playPromise, function (error) {
            logConsoleError("Sound play() failed.", error);
        });
    } catch (ex) {
        logInteropError("playSound", ex);
    }
}

function addAudioListeners(id, reference) {
    try {
        var audioElement = getElementById(id);

        if (!audioElement)
            return;

        removeAudioListeners(id);

        audioElement.__apRestarting = false;
        audioElement.__apStopping = false;
        audioElement.__apManualStop = false;
        audioElement.__apShouldRecover = !audioElement.paused && !audioElement.ended;
        audioElement.__apSession = audioElement.__apSession || 0;
        audioElement.__apRecoverAttempts = 0;
        audioElement.__apReference = reference;

        function queueRecovery(reason, delayMs) {
            if (!audioElement.__apShouldRecover)
                return;

            if (audioElement.__apManualStop || audioElement.__apStopping || audioElement.__apRestarting || audioElement.ended)
                return;

            if ((audioElement.__apRecoverAttempts | 0) >= 2)
                return;

            clearElementTimer(audioElement, "__apRecoverTimer");

            var token = audioElement.__apSession | 0;

            audioElement.__apRecoverTimer = window.setTimeout(function () {
                if ((audioElement.__apSession | 0) !== token)
                    return;

                if (audioElement.__apManualStop || audioElement.__apStopping || audioElement.__apRestarting || audioElement.ended)
                    return;

                audioElement.__apRecoverAttempts = (audioElement.__apRecoverAttempts | 0) + 1;

                try {
                    if (audioElement.readyState === 0 || audioElement.networkState === 3 || audioElement.error)
                        audioElement.load();
                } catch (loadError) {
                    logConsoleError("Audio recovery load() failed.", loadError);
                }

                var recoverPromise = audioElement.play();
                attachPromiseCatch(recoverPromise, function (error) {
                    logConsoleError("Audio recovery play() failed. Reason=" + reason + ".", error);
                });
            }, delayMs);
        }

        function playHandler() {
            audioElement.__apShouldRecover = true;
            audioElement.__apManualStop = false;
            audioElement.__apStopping = false;
            invokeDotNet(reference, "HandleAudioPlaying");
        }

        function playingHandler() {
            clearElementTimer(audioElement, "__apRecoverTimer");
            clearElementTimer(audioElement, "__apStallTimer");
            audioElement.__apShouldRecover = true;
            audioElement.__apManualStop = false;
            audioElement.__apStopping = false;
            audioElement.__apRecoverAttempts = 0;
            invokeDotNet(reference, "HandleAudioPlaying");
        }

        function pauseHandler() {
            if (audioElement.__apRestarting)
                return;

            var stopping = audioElement.__apStopping === true;
            var shouldRecover = audioElement.__apShouldRecover === true;
            var likelyBufferingPause =
                shouldRecover
                && !audioElement.__apManualStop
                && !stopping
                && !audioElement.ended
                && !audioElement.error
                && audioElement.readyState < 3
                && (audioElement.networkState === 2 || audioElement.networkState === 3);

            if (likelyBufferingPause) {
                queueRecovery("pause-buffering", 250);
                return;
            }

            audioElement.__apShouldRecover = false;

            var currentTime = isFiniteNumber(audioElement.currentTime) ? audioElement.currentTime : 0;
            var duration = isFiniteNumber(audioElement.duration) ? audioElement.duration : 0;
            var token = audioElement.__apSession | 0;

            invokeDotNet(reference, "HandleAudioPaused", currentTime, duration, token);
        }

        function endedHandler() {
            clearElementTimer(audioElement, "__apRecoverTimer");
            clearElementTimer(audioElement, "__apStallTimer");
            audioElement.__apShouldRecover = false;
            audioElement.__apStopping = false;
            invokeDotNet(reference, "HandleAudioEnded", audioElement.__apSession | 0);
        }

        function errorHandler() {
            clearElementTimer(audioElement, "__apRecoverTimer");
            clearElementTimer(audioElement, "__apStallTimer");

            var err = audioElement.error;
            var code = err ? err.code : 0;
            var message = err && typeof err.message === "string" ? err.message : "";

            invokeDotNet(reference, "HandleAudioError", code, message);
            logConsoleError("Audio element error.", { code: code, message: message });

            if (audioElement.__apShouldRecover && !audioElement.__apStopping && !audioElement.ended && code !== 4)
                queueRecovery("media-error", 350);
        }

        function waitingHandler() {
            if (!audioElement.__apShouldRecover)
                return;

            if (audioElement.__apManualStop || audioElement.__apStopping || audioElement.ended)
                return;

            clearElementTimer(audioElement, "__apStallTimer");

            var token = audioElement.__apSession | 0;

            audioElement.__apStallTimer = window.setTimeout(function () {
                if ((audioElement.__apSession | 0) !== token)
                    return;

                if (audioElement.__apManualStop || audioElement.__apStopping || audioElement.ended)
                    return;

                queueRecovery("waiting-timeout", 0);
            }, 1250);
        }

        function stalledHandler() {
            if (!audioElement.__apShouldRecover)
                return;

            if (audioElement.__apManualStop || audioElement.__apStopping || audioElement.ended)
                return;

            queueRecovery("stalled", 300);
        }

        function suspendHandler() {
            if (!audioElement.__apShouldRecover)
                return;

            if (audioElement.__apManualStop || audioElement.__apStopping || audioElement.ended)
                return;

            if (audioElement.readyState < 3)
                queueRecovery("suspend", 300);
        }

        function canplayHandler() {
            clearElementTimer(audioElement, "__apStallTimer");
        }

        audioElement.__apHandlers = {
            playHandler: playHandler,
            playingHandler: playingHandler,
            pauseHandler: pauseHandler,
            endedHandler: endedHandler,
            errorHandler: errorHandler,
            waitingHandler: waitingHandler,
            stalledHandler: stalledHandler,
            suspendHandler: suspendHandler,
            canplayHandler: canplayHandler
        };

        audioElement.addEventListener("play", playHandler);
        audioElement.addEventListener("playing", playingHandler);
        audioElement.addEventListener("pause", pauseHandler);
        audioElement.addEventListener("ended", endedHandler);
        audioElement.addEventListener("error", errorHandler);
        audioElement.addEventListener("waiting", waitingHandler);
        audioElement.addEventListener("stalled", stalledHandler);
        audioElement.addEventListener("suspend", suspendHandler);
        audioElement.addEventListener("canplay", canplayHandler);
    } catch (ex) {
        logInteropError("addAudioListeners", ex);
    }
}

function removeAudioListeners(id) {
    try {
        var audioElement = getElementById(id);

        if (!audioElement || !audioElement.__apHandlers)
            return;

        var handlers = audioElement.__apHandlers;

        audioElement.removeEventListener("play", handlers.playHandler);
        audioElement.removeEventListener("playing", handlers.playingHandler);
        audioElement.removeEventListener("pause", handlers.pauseHandler);
        audioElement.removeEventListener("ended", handlers.endedHandler);
        audioElement.removeEventListener("error", handlers.errorHandler);
        audioElement.removeEventListener("waiting", handlers.waitingHandler);
        audioElement.removeEventListener("stalled", handlers.stalledHandler);
        audioElement.removeEventListener("suspend", handlers.suspendHandler);
        audioElement.removeEventListener("canplay", handlers.canplayHandler);

        clearElementTimer(audioElement, "__apRecoverTimer");
        clearElementTimer(audioElement, "__apStallTimer");

        delete audioElement.__apHandlers;
        delete audioElement.__apManualStop;
        delete audioElement.__apShouldRecover;
        delete audioElement.__apRestarting;
        delete audioElement.__apStopping;
        delete audioElement.__apSession;
        delete audioElement.__apRecoverAttempts;
        delete audioElement.__apReference;
    } catch (ex) {
        logInteropError("removeAudioListeners", ex);
    }
}

function playAudio(id) {
    try {
        var audioElement = getElementById(id);

        if (!audioElement)
            return;

        audioElement.__apSession = (audioElement.__apSession | 0) + 1;
        audioElement.__apManualStop = false;
        audioElement.__apShouldRecover = true;
        audioElement.__apStopping = false;
        audioElement.__apRecoverAttempts = 0;
        audioElement.preload = "auto";

        clearElementTimer(audioElement, "__apRecoverTimer");
        clearElementTimer(audioElement, "__apStallTimer");

        if (!audioElement.paused || (audioElement.currentTime || 0) > 0) {
            audioElement.__apRestarting = true;
            audioElement.pause();
            audioElement.currentTime = 0;
            window.setTimeout(function () {
                audioElement.__apRestarting = false;
            }, 0);
        }

        var playPromise = audioElement.play();
        attachPromiseCatch(playPromise, function (error) {
            var name = error && typeof error.name === "string" ? error.name : "";
            var message = error && typeof error.message === "string" ? error.message : "";

            if (audioElement.__apReference)
                invokeDotNet(audioElement.__apReference, "HandleAudioError", 0, name && message ? name + ": " + message : (message || name || "play() failed"));

            if (name === "NotAllowedError")
                audioElement.__apShouldRecover = false;

            if (name !== "NotAllowedError"
                && audioElement.__apShouldRecover
                && !audioElement.__apManualStop
                && !audioElement.__apStopping
                && !audioElement.ended) {
                audioElement.__apRecoverTimer = window.setTimeout(function () {
                    if (!audioElement.__apShouldRecover || audioElement.__apManualStop || audioElement.__apStopping || audioElement.ended)
                        return;

                    try {
                        if (audioElement.readyState === 0 || audioElement.networkState === 3)
                            audioElement.load();
                    } catch (loadError) {
                        logConsoleError("Audio retry load() failed.", loadError);
                    }

                    var retryPromise = audioElement.play();
                    attachPromiseCatch(retryPromise, function (retryError) {
                        logConsoleError("Audio retry play() failed.", retryError);
                    });
                }, 300);
            }

            logConsoleError("Audio play() failed.", error);
        });
    } catch (ex) {
        logInteropError("playAudio", ex);
    }
}

function stopAudio(id) {
    try {
        var audioElement = getElementById(id);

        if (!audioElement)
            return;

        audioElement.__apManualStop = true;
        audioElement.__apShouldRecover = false;
        audioElement.__apStopping = true;

        clearElementTimer(audioElement, "__apRecoverTimer");
        clearElementTimer(audioElement, "__apStallTimer");

        audioElement.pause();
        audioElement.currentTime = 0;

        window.setTimeout(function () {
            audioElement.__apStopping = false;
        }, 0);
    } catch (ex) {
        logInteropError("stopAudio", ex);
    }
}

// #endregion

// #region Video

function clearVideo(videoElement, reset) {
    if (!videoElement)
        return;

    var handlers = videoElement.__vpHandlers;

    if (handlers) {
        videoElement.removeEventListener("play", handlers.playHandler);
        videoElement.removeEventListener("pause", handlers.pauseHandler);
        videoElement.removeEventListener("ended", handlers.endedHandler);
        videoElement.removeEventListener("error", handlers.errorHandler);
    }

    delete videoElement.__vpHandlers;
    delete videoElement.__vpReference;
    delete videoElement.__vpRestarting;
    delete videoElement.__vpStopping;

    if (!reset)
        return;

    try {
        videoElement.pause();
        videoElement.currentTime = 0;
    } catch (error) {
        logConsoleError("Video reset failed.", error);
    }
}

function initializeVideo(id, reference) {
    try {
        var videoElement = getElementById(id);

        if (!videoElement)
            return;

        clearVideo(videoElement, false);

        videoElement.__vpReference = reference;
        videoElement.__vpRestarting = false;
        videoElement.__vpStopping = false;

        function playHandler() {
            videoElement.__vpStopping = false;
            invokeDotNet(reference, "HandleVideoPlayed");
        }

        function pauseHandler() {
            if (videoElement.__vpRestarting || videoElement.__vpStopping)
                return;

            invokeDotNet(reference, "HandleVideoStopped");
        }

        function endedHandler() {
            videoElement.__vpStopping = false;
            invokeDotNet(reference, "HandleVideoEnded");
        }

        function errorHandler() {
            var err = videoElement.error;
            var code = err ? err.code : 0;
            var message = err && typeof err.message === "string" ? err.message : "";
            var detail = code > 0
                ? "Video error code " + code + (message ? ": " + message : "")
                : (message || "Video element error");

            invokeDotNet(reference, "HandleVideoError", detail);
            logConsoleError("Video element error.", { code: code, message: message });
        }

        videoElement.__vpHandlers = {
            playHandler: playHandler,
            pauseHandler: pauseHandler,
            endedHandler: endedHandler,
            errorHandler: errorHandler
        };

        videoElement.addEventListener("play", playHandler);
        videoElement.addEventListener("pause", pauseHandler);
        videoElement.addEventListener("ended", endedHandler);
        videoElement.addEventListener("error", errorHandler);
    } catch (ex) {
        logInteropError("initializeVideo", ex);
    }
}

function disposeVideo(id) {
    try {
        var videoElement = getElementById(id);

        if (!videoElement)
            return;

        clearVideo(videoElement, true);
    } catch (ex) {
        logInteropError("disposeVideo", ex);
    }
}

function playVideo(id) {
    try {
        var videoElement = getElementById(id);

        if (!videoElement)
            return;

        videoElement.__vpStopping = false;

        if (!videoElement.paused || videoElement.currentTime !== 0) {
            videoElement.__vpRestarting = true;
            videoElement.pause();
            videoElement.currentTime = 0;
            window.setTimeout(function () {
                videoElement.__vpRestarting = false;
            }, 0);
        }

        var playPromise = videoElement.play();
        attachPromiseCatch(playPromise, function (error) {
            var name = error && typeof error.name === "string" ? error.name : "";
            var message = error && typeof error.message === "string" ? error.message : "";
            var detail = name && message ? name + ": " + message : (message || name || "Video play() failed");

            if (videoElement.__vpReference)
                invokeDotNet(videoElement.__vpReference, "HandleVideoError", detail);

            logConsoleError("Video play() failed.", error);
        });
    } catch (ex) {
        logInteropError("playVideo", ex);
    }
}

function stopVideo(id) {
    try {
        var videoElement = getElementById(id);

        if (!videoElement)
            return;

        videoElement.__vpStopping = true;
        videoElement.pause();
        videoElement.currentTime = 0;

        window.setTimeout(function () {
            videoElement.__vpStopping = false;
        }, 0);
    } catch (ex) {
        logInteropError("stopVideo", ex);
    }
}

// #endregion

// #region Line drawing

var resizeListeners = {};

function addResizedListener(id, reference) {
    try {
        if (!id || !reference)
            return;

        removeResizedListener(id);

        var listener = {
            timer: null,
            handler: null
        };

        listener.handler = function () {
            if (listener.timer)
                clearTimeout(listener.timer);

            listener.timer = window.setTimeout(function () {
                listener.timer = null;
                invokeDotNet(reference, "HandleWindowResized");
            }, 50);
        };

        resizeListeners[id] = listener;
        window.addEventListener("resize", listener.handler);
    } catch (ex) {
        logInteropError("addResizedListener", ex);
    }
}

function removeResizedListener(id) {
    try {
        var listener = resizeListeners[id];

        if (!listener)
            return;

        window.removeEventListener("resize", listener.handler);

        if (listener.timer) {
            clearTimeout(listener.timer);
            listener.timer = null;
        }

        delete resizeListeners[id];
    } catch (ex) {
        logInteropError("removeResizedListener", ex);
    }
}

function drawLine(sourceId, targetId) {
    try {
        var sourceElement = getElementById(sourceId);
        var targetElement = getElementById(targetId);
        var line = getElementById("line-" + targetId);

        if (!sourceElement || !targetElement || !line || !line.ownerSVGElement)
            return;

        var svgRect = line.ownerSVGElement.getBoundingClientRect();
        var sourceRect = sourceElement.getBoundingClientRect();
        var targetRect = targetElement.getBoundingClientRect();

        line.setAttribute("x1", String(sourceRect.left + (sourceRect.width / 2) - svgRect.left));
        line.setAttribute("y1", String(sourceRect.top + sourceRect.height - svgRect.top));
        line.setAttribute("x2", String(targetRect.left + (targetRect.width / 2) - svgRect.left));
        line.setAttribute("y2", String(targetRect.top - svgRect.top));
    } catch (ex) {
        logInteropError("drawLine", ex);
    }
}

// #endregion

// #region Link interception

function findClosestAnchor(startNode, container) {
    var current = startNode;

    while (current && current !== container) {
        if (current.tagName && String(current.tagName).toUpperCase() === "A")
            return current;

        current = current.parentNode;
    }

    if (container && container.tagName && String(container.tagName).toUpperCase() === "A")
        return container;

    return null;
}

function initializeLinkInterceptor(id, dotNetReference) {
    try {
        var element = getElementById(id);

        if (!element || !dotNetReference)
            return;

        disposeLinkInterceptor(id);

        function onLinkClick(event) {
            var anchor = findClosestAnchor(event.target, element);

            if (anchor && anchor.href)
                invokeDotNet(dotNetReference, "HandleLinkClicked", anchor.href);
        }

        element.addEventListener("click", onLinkClick);
        element._linkInterceptor = { onLinkClick: onLinkClick };
    } catch (ex) {
        logInteropError("initializeLinkInterceptor", ex);
    }
}

function disposeLinkInterceptor(id) {
    try {
        var element = getElementById(id);

        if (element && element._linkInterceptor) {
            element.removeEventListener("click", element._linkInterceptor.onLinkClick);
            delete element._linkInterceptor;
        }
    } catch (ex) {
        logInteropError("disposeLinkInterceptor", ex);
    }
}

// #endregion

// #region Object URLs

function getObjectUrlFromInput(inputId) {
    try {
        var element = getElementById(inputId);

        if (!element || !element.files || !element.files.length)
            return null;

        return URL.createObjectURL(element.files[0]);
    } catch (ex) {
        logInteropError("getObjectUrlFromInput", ex);
        return null;
    }
}

function revokeObjectUrl(url) {
    try {
        if (url)
            URL.revokeObjectURL(url);
    } catch (ex) {
        logInteropError("revokeObjectUrl", ex);
    }
}

// #endregion