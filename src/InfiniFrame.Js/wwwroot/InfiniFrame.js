/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./TypeScript/Contracts/EnvelopeProtocol.ts"
/*!**************************************************!*\
  !*** ./TypeScript/Contracts/EnvelopeProtocol.ts ***!
  \**************************************************/
(__unused_webpack_module, exports) {


Object.defineProperty(exports, "__esModule", ({ value: true }));


/***/ },

/***/ "./TypeScript/Contracts/IInfiniFrame.ts"
/*!**********************************************!*\
  !*** ./TypeScript/Contracts/IInfiniFrame.ts ***!
  \**********************************************/
(__unused_webpack_module, exports) {


Object.defineProperty(exports, "__esModule", ({ value: true }));


/***/ },

/***/ "./TypeScript/Contracts/IInfiniFrameHostMessaging.ts"
/*!***********************************************************!*\
  !*** ./TypeScript/Contracts/IInfiniFrameHostMessaging.ts ***!
  \***********************************************************/
(__unused_webpack_module, exports) {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.ReceiveFromHostMessageIds = exports.SendToHostMessageIds = void 0;
const infiniframe = "__infiniframe";
exports.SendToHostMessageIds = {
    titleChange: `${infiniframe}:title:change`,
    fullscreenEnter: `${infiniframe}:fullscreen:enter`,
    fullscreenExit: `${infiniframe}:fullscreen:exit`,
    openExternalLink: `${infiniframe}:open:external`,
    windowClose: `${infiniframe}:window:close`,
    ready: `${infiniframe}:ready`,
    getMessageRequest: `${infiniframe}:get:request`,
};
exports.ReceiveFromHostMessageIds = {
    registerOpenExternal: `${infiniframe}:register:open:external`,
    registerFullscreenChange: `${infiniframe}:register:fullscreen:change`,
    registerTitleChange: `${infiniframe}:register:title:change`,
    registerWindowClose: `${infiniframe}:register:window:close`,
    getMessageResponse: `${infiniframe}:get:response`,
};


/***/ },

/***/ "./TypeScript/Contracts/IInfiniFrameUtils.ts"
/*!***************************************************!*\
  !*** ./TypeScript/Contracts/IInfiniFrameUtils.ts ***!
  \***************************************************/
(__unused_webpack_module, exports) {


Object.defineProperty(exports, "__esModule", ({ value: true }));


/***/ },

/***/ "./TypeScript/Contracts/IInfiniFrameWindow.ts"
/*!****************************************************!*\
  !*** ./TypeScript/Contracts/IInfiniFrameWindow.ts ***!
  \****************************************************/
(__unused_webpack_module, exports) {


Object.defineProperty(exports, "__esModule", ({ value: true }));


/***/ },

/***/ "./TypeScript/Contracts/global.ts"
/*!****************************************!*\
  !*** ./TypeScript/Contracts/global.ts ***!
  \****************************************/
(__unused_webpack_module, exports) {


Object.defineProperty(exports, "__esModule", ({ value: true }));


/***/ },

/***/ "./TypeScript/Contracts/index.ts"
/*!***************************************!*\
  !*** ./TypeScript/Contracts/index.ts ***!
  \***************************************/
(__unused_webpack_module, exports, __webpack_require__) {


var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __exportStar = (this && this.__exportStar) || function(m, exports) {
    for (var p in m) if (p !== "default" && !Object.prototype.hasOwnProperty.call(exports, p)) __createBinding(exports, m, p);
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
__exportStar(__webpack_require__(/*! ./EnvelopeProtocol */ "./TypeScript/Contracts/EnvelopeProtocol.ts"), exports);
__exportStar(__webpack_require__(/*! ./global */ "./TypeScript/Contracts/global.ts"), exports);
__exportStar(__webpack_require__(/*! ./IInfiniFrame */ "./TypeScript/Contracts/IInfiniFrame.ts"), exports);
__exportStar(__webpack_require__(/*! ./IInfiniFrameHostMessaging */ "./TypeScript/Contracts/IInfiniFrameHostMessaging.ts"), exports);
__exportStar(__webpack_require__(/*! ./IInfiniFrameUtils */ "./TypeScript/Contracts/IInfiniFrameUtils.ts"), exports);
__exportStar(__webpack_require__(/*! ./IInfiniFrameWindow */ "./TypeScript/Contracts/IInfiniFrameWindow.ts"), exports);


/***/ },

/***/ "./TypeScript/Index.ts"
/*!*****************************!*\
  !*** ./TypeScript/Index.ts ***!
  \*****************************/
(__unused_webpack_module, exports, __webpack_require__) {


var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
const InfiniFrame_1 = __importDefault(__webpack_require__(/*! ./InfiniFrame */ "./TypeScript/InfiniFrame.ts"));
const HostBridge_1 = __webpack_require__(/*! ./Interop/NativeHost/HostBridge */ "./TypeScript/Interop/NativeHost/HostBridge.ts");
(0, HostBridge_1.installHostBridge)();
window.infiniframe = new InfiniFrame_1.default();


/***/ },

/***/ "./TypeScript/InfiniFrame.ts"
/*!***********************************!*\
  !*** ./TypeScript/InfiniFrame.ts ***!
  \***********************************/
(__unused_webpack_module, exports, __webpack_require__) {


var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.InfiniFrame = void 0;
const InfiniFrameHostMessaging_1 = __importDefault(__webpack_require__(/*! ./InfiniFrameHostMessaging */ "./TypeScript/InfiniFrameHostMessaging.ts"));
const InfiniFrameUtils_1 = __webpack_require__(/*! ./InfiniFrameUtils */ "./TypeScript/InfiniFrameUtils.ts");
const InfiniFrameWindow_1 = __webpack_require__(/*! ./InfiniFrameWindow */ "./TypeScript/InfiniFrameWindow.ts");
class InfiniFrame {
    constructor() {
        this.messaging = new InfiniFrameHostMessaging_1.default();
        this.window = new InfiniFrameWindow_1.InfiniFrameWindow();
        this.utils = new InfiniFrameUtils_1.InfiniFrameUtils();
    }
}
exports.InfiniFrame = InfiniFrame;
exports["default"] = InfiniFrame;


/***/ },

/***/ "./TypeScript/InfiniFrameHostMessaging.ts"
/*!************************************************!*\
  !*** ./TypeScript/InfiniFrameHostMessaging.ts ***!
  \************************************************/
(__unused_webpack_module, exports, __webpack_require__) {


var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
const Contracts_1 = __webpack_require__(/*! ./Contracts */ "./TypeScript/Contracts/index.ts");
const InteropEnvelopeProtocol_1 = __webpack_require__(/*! ./Interop/EnvelopeProtocol/InteropEnvelopeProtocol */ "./TypeScript/Interop/EnvelopeProtocol/InteropEnvelopeProtocol.ts");
const BlankTargetHandler_1 = __webpack_require__(/*! ./Utils/BlankTargetHandler */ "./TypeScript/Utils/BlankTargetHandler.ts");
const Observers_1 = __webpack_require__(/*! ./Utils/Observers */ "./TypeScript/Utils/Observers.ts");
class InfiniFrameHostMessaging {
    constructor() {
        this.messageHandlers = new Map();
        this.openExternalRegistered = false;
        this.fullscreenRegistered = false;
        this.titleRegistered = false;
        this.windowCloseRegistered = false;
        this.readyHandshakeAttempts = 0;
        this.readyHandshakeAcknowledged = false;
        this.readyHandshakeRetryTimer = null;
        this.assignWebMessageReceiver();
        this.assignMessageReceivedHandler(Contracts_1.ReceiveFromHostMessageIds.registerOpenExternal, _ => {
            this.markReadyHandshakeAcknowledged();
            this.registerOpenExternal();
        });
        this.assignMessageReceivedHandler(Contracts_1.ReceiveFromHostMessageIds.registerFullscreenChange, _ => {
            this.markReadyHandshakeAcknowledged();
            this.registerFullscreenChange();
        });
        this.assignMessageReceivedHandler(Contracts_1.ReceiveFromHostMessageIds.registerTitleChange, _ => {
            this.markReadyHandshakeAcknowledged();
            this.registerTitleChange();
        });
        this.assignMessageReceivedHandler(Contracts_1.ReceiveFromHostMessageIds.registerWindowClose, _ => {
            this.markReadyHandshakeAcknowledged();
            this.registerWindowClose();
        });
        this.sendReadyHandshakeWithRetry();
    }
    sendMessageToHost(id, data) {
        var _a, _b;
        const envelope = (0, InteropEnvelopeProtocol_1.createEnvelope)(id, data);
        if ((_b = (_a = window.__infiniframe) === null || _a === void 0 ? void 0 : _a.host) === null || _b === void 0 ? void 0 : _b.postData) {
            window.__infiniframe.host.postData(envelope);
        }
        else {
            console.warn("Message to host failed. Host bridge API is not initialized.");
            return;
        }
    }
    getMessageFromHostAsync(message) {
        return __awaiter(this, void 0, void 0, function* () {
            var _a;
            const host = (_a = window.__infiniframe) === null || _a === void 0 ? void 0 : _a.host;
            if (!(host === null || host === void 0 ? void 0 : host.getData))
                throw new Error("Message to host failed. Host getData API is not initialized.");
            const envelope = typeof message === "string"
                ? (0, InteropEnvelopeProtocol_1.createEnvelope)(message)
                : message;
            return yield host.getData(envelope);
        });
    }
    assignMessageReceivedHandler(messageId, callback) {
        this.messageHandlers.set(messageId, callback);
    }
    unregisterMessageReceivedHandler(messageId) {
        this.messageHandlers.delete(messageId);
    }
    assignWebMessageReceiver() {
        var _a, _b;
        if ((_b = (_a = window.__infiniframe) === null || _a === void 0 ? void 0 : _a.host) === null || _b === void 0 ? void 0 : _b.receiveCallback) {
            window.__infiniframe.host.receiveCallback((message) => {
                this.handleInteropMessage(message);
            });
        }
        else {
            console.warn("Web message receiver failed. Host bridge API is not initialized.");
            return;
        }
    }
    handleInteropMessage(message) {
        if (typeof message !== 'string')
            return false;
        if (!message)
            return false;
        const parsedMessage = (0, InteropEnvelopeProtocol_1.parseIncomingMessage)(message);
        if ("error" in parsedMessage)
            return false;
        if (parsedMessage.messageId.startsWith(InfiniFrameHostMessaging.BlazorWebViewMessagePrefix)) {
            return true;
        }
        const handler = this.messageHandlers.get(parsedMessage.messageId);
        if (!handler) {
            console.warn('No handler registered for message:', parsedMessage);
            return false;
        }
        handler(parsedMessage.payload);
        return true;
    }
    registerOpenExternal() {
        if (this.openExternalRegistered)
            return;
        this.openExternalRegistered = true;
        document.addEventListener("click", BlankTargetHandler_1.blankTargetHandler, { capture: true });
    }
    registerFullscreenChange() {
        if (this.fullscreenRegistered)
            return;
        this.fullscreenRegistered = true;
        document.addEventListener("fullscreenchange", (_) => {
            if (document.fullscreenElement)
                this.sendMessageToHost(Contracts_1.SendToHostMessageIds.fullscreenEnter);
            else
                this.sendMessageToHost(Contracts_1.SendToHostMessageIds.fullscreenExit);
        });
        document.addEventListener("keydown", (e) => __awaiter(this, void 0, void 0, function* () {
            if (e.key !== "F11")
                return;
            if (document.fullscreenElement)
                yield document.exitFullscreen();
            else
                yield document.body.requestFullscreen();
        }));
    }
    registerTitleChange() {
        if (this.titleRegistered)
            return;
        this.titleRegistered = true;
        const titleTarget = (0, Observers_1.getTitleObserverTarget)();
        if (titleTarget) {
            (0, Observers_1.getTitleObserver)().observe(titleTarget, { childList: true });
            return;
        }
        const headTarget = document.head || document.documentElement;
        if (!headTarget)
            return;
        const headObserver = new MutationObserver(() => {
            const target = (0, Observers_1.getTitleObserverTarget)();
            if (!target)
                return;
            headObserver.disconnect();
            (0, Observers_1.getTitleObserver)().observe(target, { childList: true });
        });
        headObserver.observe(headTarget, { childList: true, subtree: true });
    }
    registerWindowClose() {
        if (this.windowCloseRegistered)
            return;
        this.windowCloseRegistered = true;
        window.close = () => {
            this.sendMessageToHost(Contracts_1.SendToHostMessageIds.windowClose);
        };
    }
    sendReadyHandshakeWithRetry() {
        this.sendReadyHandshake();
        this.readyHandshakeRetryTimer = window.setInterval(() => {
            if (this.readyHandshakeAcknowledged || this.readyHandshakeAttempts >= InfiniFrameHostMessaging.MaxReadyHandshakeAttempts) {
                this.stopReadyHandshakeRetry();
                return;
            }
            this.sendReadyHandshake();
        }, InfiniFrameHostMessaging.ReadyHandshakeRetryIntervalMs);
    }
    sendReadyHandshake() {
        this.readyHandshakeAttempts++;
        this.sendMessageToHost(Contracts_1.SendToHostMessageIds.ready);
    }
    markReadyHandshakeAcknowledged() {
        if (this.readyHandshakeAcknowledged)
            return;
        this.readyHandshakeAcknowledged = true;
        this.stopReadyHandshakeRetry();
    }
    stopReadyHandshakeRetry() {
        if (this.readyHandshakeRetryTimer === null)
            return;
        window.clearInterval(this.readyHandshakeRetryTimer);
        this.readyHandshakeRetryTimer = null;
    }
}
InfiniFrameHostMessaging.BlazorWebViewMessagePrefix = "__bwv:";
InfiniFrameHostMessaging.ReadyHandshakeRetryIntervalMs = 1000;
InfiniFrameHostMessaging.MaxReadyHandshakeAttempts = 20;
exports["default"] = InfiniFrameHostMessaging;


/***/ },

/***/ "./TypeScript/InfiniFrameUtils.ts"
/*!****************************************!*\
  !*** ./TypeScript/InfiniFrameUtils.ts ***!
  \****************************************/
(__unused_webpack_module, exports) {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.InfiniFrameUtils = void 0;
class InfiniFrameUtils {
    setPointerCapture(element, pointerId) {
        if (element === null)
            return;
        if (pointerId === null)
            return;
        if (element.hasPointerCapture(pointerId))
            return;
        element.setPointerCapture(pointerId);
    }
    releasePointerCapture(element, pointerId) {
        if (element === null)
            return;
        if (pointerId === null)
            return;
        if (!element.hasPointerCapture(pointerId))
            return;
        element.releasePointerCapture(pointerId);
    }
}
exports.InfiniFrameUtils = InfiniFrameUtils;


/***/ },

/***/ "./TypeScript/InfiniFrameWindow.ts"
/*!*****************************************!*\
  !*** ./TypeScript/InfiniFrameWindow.ts ***!
  \*****************************************/
(__unused_webpack_module, exports, __webpack_require__) {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.InfiniFrameWindow = void 0;
const Contracts_1 = __webpack_require__(/*! ./Contracts */ "./TypeScript/Contracts/index.ts");
class InfiniFrameWindow {
    setTitle(title) {
        window.infiniframe.messaging.sendMessageToHost(Contracts_1.SendToHostMessageIds.titleChange, title);
    }
}
exports.InfiniFrameWindow = InfiniFrameWindow;


/***/ },

/***/ "./TypeScript/Interop/EnvelopeProtocol/InteropEnvelopeProtocol.ts"
/*!************************************************************************!*\
  !*** ./TypeScript/Interop/EnvelopeProtocol/InteropEnvelopeProtocol.ts ***!
  \************************************************************************/
(__unused_webpack_module, exports) {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.InteropMessageMaxSizeBytes = exports.InteropEnvelopeVersion = void 0;
exports.createEnvelope = createEnvelope;
exports.createEnvelopeMessage = createEnvelopeMessage;
exports.parseIncomingMessage = parseIncomingMessage;
exports.InteropEnvelopeVersion = 1;
exports.InteropMessageMaxSizeBytes = 1024 * 1024;
function createEnvelope(id, data, channel) {
    if (!id || id.trim().length === 0) {
        throw new Error("Envelope 'id' is required.");
    }
    return {
        id,
        data,
        version: exports.InteropEnvelopeVersion,
        channel
    };
}
function createEnvelopeMessage(id, data, channel) {
    const envelope = createEnvelope(id, data, channel);
    return JSON.stringify(envelope);
}
function parseIncomingMessage(message) {
    if (!message || message.trim().length === 0) {
        return { error: "Message is empty." };
    }
    if (getUtf8ByteCount(message) > exports.InteropMessageMaxSizeBytes) {
        return { error: `Message exceeds max size of ${exports.InteropMessageMaxSizeBytes} bytes.` };
    }
    try {
        const parsed = JSON.parse(message);
        if (!isObject(parsed)) {
            return { error: "Envelope root must be a JSON object." };
        }
        if (typeof parsed.id !== "string" || parsed.id.trim().length === 0) {
            return { error: "Envelope 'id' is required and must be a string." };
        }
        if (typeof parsed.version !== "number" || !Number.isInteger(parsed.version)) {
            return { error: "Envelope 'version' is required and must be an integer." };
        }
        if (parsed.version !== exports.InteropEnvelopeVersion) {
            return { error: `Unsupported envelope version '${parsed.version}'.` };
        }
        const payload = convertDataToPayload(parsed.data);
        return {
            messageId: parsed.id,
            payload
        };
    }
    catch (_a) {
        return { error: "Envelope JSON is malformed." };
    }
}
function convertDataToPayload(data) {
    if (data === null || data === undefined) {
        return undefined;
    }
    if (typeof data === "string") {
        return data;
    }
    return JSON.stringify(data);
}
function getUtf8ByteCount(message) {
    return new TextEncoder().encode(message).length;
}
function isObject(value) {
    return typeof value === "object" && value !== null;
}


/***/ },

/***/ "./TypeScript/Interop/NativeHost/HostBridge.ts"
/*!*****************************************************!*\
  !*** ./TypeScript/Interop/NativeHost/HostBridge.ts ***!
  \*****************************************************/
(__unused_webpack_module, exports, __webpack_require__) {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.installHostBridge = installHostBridge;
const InteropEnvelopeProtocol_1 = __webpack_require__(/*! ../EnvelopeProtocol/InteropEnvelopeProtocol */ "./TypeScript/Interop/EnvelopeProtocol/InteropEnvelopeProtocol.ts");
const GetMessageRequestId = "__infiniframe:get:request";
const GetMessageResponseId = "__infiniframe:get:response";
const GetMessageTimeoutMs = 10000;
const receiveCallbacks = new Set();
let receiveBridgeAttached = false;
function installHostBridge() {
    var _a, _b;
    const root = (_a = window.__infiniframe) !== null && _a !== void 0 ? _a : {};
    const host = ((_b = root.host) !== null && _b !== void 0 ? _b : {});
    const existingPostData = host.postData;
    const existingReceiveCallback = host.receiveCallback;
    const existingGetData = host.getData;
    host.postData = (envelope) => {
        dispatchEnvelopeToHost(envelope, existingPostData);
    };
    host.receiveCallback = (callback) => {
        registerWebMessageReceiver(callback, existingReceiveCallback);
    };
    host.getData = (message) => {
        return requestMessageFromHost(message, host, existingGetData, existingReceiveCallback);
    };
    root.host = host;
    window.__infiniframe = root;
}
function dispatchEnvelopeToHost(envelope, existingPostData) {
    if (typeof envelope === "string") {
        const rawMessage = envelope.trim();
        if (rawMessage.length === 0) {
            console.warn("Ignoring empty host bridge payload.");
            return;
        }
        if (existingPostData) {
            try {
                existingPostData(rawMessage);
                return;
            }
            catch (error) {
                console.warn("Existing InfiniFrame host bridge failed. Falling back to platform adapters.", error);
            }
        }
        sendViaPlatformTransport(rawMessage);
        return;
    }
    const normalized = normalizeEnvelope(envelope);
    if (!normalized) {
        return;
    }
    const serializedEnvelope = JSON.stringify(normalized);
    if (existingPostData) {
        try {
            existingPostData(serializedEnvelope);
            return;
        }
        catch (error) {
            try {
                existingPostData(normalized);
                return;
            }
            catch (_a) {
                console.warn("Existing InfiniFrame host bridge failed. Falling back to platform adapters.", error);
            }
        }
    }
    sendViaPlatformTransport(serializedEnvelope);
}
function requestMessageFromHost(message, host, existingGetData, existingReceiveCallback) {
    const normalizedMessage = normalizeGetMessageInput(message);
    if (!normalizedMessage) {
        return Promise.reject(new Error("Host getData payload is invalid."));
    }
    if (existingGetData) {
        try {
            const existingResult = existingGetData(normalizedMessage);
            if (existingResult && typeof existingResult.then === "function") {
                return existingResult;
            }
            return Promise.resolve(String(existingResult !== null && existingResult !== void 0 ? existingResult : ""));
        }
        catch (error) {
            console.warn("Existing InfiniFrame getData bridge failed. Falling back to request/response transport.", error);
        }
    }
    const requestId = createRequestId();
    return new Promise((resolve, reject) => {
        var _a;
        const timeout = window.setTimeout(() => {
            unregisterWebMessageReceiver(responseCallback);
            reject(new Error("Timed out waiting for getData response from host."));
        }, GetMessageTimeoutMs);
        const responseCallback = (rawMessage) => {
            var _a, _b;
            const parsed = (0, InteropEnvelopeProtocol_1.parseIncomingMessage)(rawMessage);
            if ("error" in parsed || parsed.messageId !== GetMessageResponseId || !parsed.payload) {
                return;
            }
            let payload;
            try {
                payload = JSON.parse(parsed.payload);
            }
            catch (_c) {
                return;
            }
            if (!isGetMessageResponsePayload(payload) || payload.requestId !== requestId) {
                return;
            }
            window.clearTimeout(timeout);
            unregisterWebMessageReceiver(responseCallback);
            if (payload.success) {
                resolve((_a = payload.data) !== null && _a !== void 0 ? _a : "");
                return;
            }
            reject(new Error((_b = payload.error) !== null && _b !== void 0 ? _b : "Host getData failed."));
        };
        registerWebMessageReceiver(responseCallback, existingReceiveCallback);
        (_a = host.postData) === null || _a === void 0 ? void 0 : _a.call(host, {
            id: GetMessageRequestId,
            data: {
                requestId,
                message: normalizedMessage
            },
            version: InteropEnvelopeProtocol_1.InteropEnvelopeVersion
        });
    });
}
function normalizeGetMessageInput(message) {
    if (typeof message === "string") {
        const trimmed = message.trim();
        if (trimmed.length === 0) {
            return null;
        }
        return trimmed;
    }
    const normalizedEnvelope = normalizeEnvelope(message);
    if (!normalizedEnvelope) {
        return null;
    }
    return normalizedEnvelope;
}
function createRequestId() {
    return `if_req_${Date.now().toString(36)}_${Math.random().toString(36).slice(2, 10)}`;
}
function normalizeEnvelope(envelope) {
    if (!envelope || typeof envelope !== "object") {
        console.warn("Host bridge payload must be an envelope object.");
        return null;
    }
    if (typeof envelope.id !== "string" || envelope.id.trim().length === 0) {
        console.warn("Host bridge envelope requires a non-empty 'id'.");
        return null;
    }
    const version = Number.isInteger(envelope.version)
        ? envelope.version
        : InteropEnvelopeProtocol_1.InteropEnvelopeVersion;
    const normalized = {
        id: envelope.id,
        data: envelope.data,
        version
    };
    if (envelope.channel !== undefined && typeof envelope.channel === "string" && envelope.channel.trim().length > 0) {
        normalized.channel = envelope.channel;
    }
    return normalized;
}
function sendViaPlatformTransport(message) {
    var _a;
    if ((_a = window.chrome) === null || _a === void 0 ? void 0 : _a.webview) {
        window.chrome.webview.postMessage(message);
        return;
    }
    console.warn("Message to host failed. No supported host transport was found.");
}
function registerWebMessageReceiver(callback, existingReceiveCallback) {
    receiveCallbacks.add(callback);
    attachReceiveBridgeOnce(existingReceiveCallback);
}
function unregisterWebMessageReceiver(callback) {
    receiveCallbacks.delete(callback);
}
function attachReceiveBridgeOnce(existingReceiveCallback) {
    var _a;
    if (receiveBridgeAttached) {
        return;
    }
    const dispatch = (message) => {
        for (const callback of receiveCallbacks) {
            callback(message);
        }
    };
    if (existingReceiveCallback) {
        try {
            existingReceiveCallback(dispatch);
            receiveBridgeAttached = true;
            return;
        }
        catch (error) {
            console.warn("Existing InfiniFrame host receive bridge failed. Falling back to platform adapters.", error);
        }
    }
    if ((_a = window.chrome) === null || _a === void 0 ? void 0 : _a.webview) {
        window.chrome.webview.addEventListener("message", (event) => {
            dispatch(event.data);
        });
        receiveBridgeAttached = true;
        return;
    }
    console.warn("Receive message registration failed. No supported host receive transport was found.");
}
function isObject(value) {
    return typeof value === "object" && value !== null;
}
function isGetMessageResponsePayload(value) {
    return isObject(value)
        && typeof value.requestId === "string"
        && typeof value.success === "boolean"
        && (value.data === undefined || typeof value.data === "string")
        && (value.error === undefined || typeof value.error === "string");
}


/***/ },

/***/ "./TypeScript/Utils/BlankTargetHandler.ts"
/*!************************************************!*\
  !*** ./TypeScript/Utils/BlankTargetHandler.ts ***!
  \************************************************/
(__unused_webpack_module, exports, __webpack_require__) {


var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.blankTargetHandler = blankTargetHandler;
const Contracts_1 = __webpack_require__(/*! ../Contracts */ "./TypeScript/Contracts/index.ts");
function isExternalLink(url) {
    try {
        return new URL(url, location.href).hostname !== location.hostname;
    }
    catch (_a) {
        return false;
    }
}
function blankTargetHandler(e) {
    return __awaiter(this, void 0, void 0, function* () {
        var _a;
        let el = e.target;
        while (el && el !== document.body) {
            if (((_a = el.tagName) === null || _a === void 0 ? void 0 : _a.toLowerCase()) !== "a") {
                el = el.parentElement;
                continue;
            }
            const anchor = el;
            if (!anchor.href) {
                el = el.parentElement;
                continue;
            }
            const target = anchor.getAttribute("target");
            const shouldHandle = target === "_blank" || anchor.hasAttribute("data-external") || isExternalLink(anchor.href);
            if (!shouldHandle) {
                el = el.parentElement;
                continue;
            }
            e.preventDefault();
            window.infiniframe.messaging.sendMessageToHost(Contracts_1.SendToHostMessageIds.openExternalLink, anchor.href);
            return;
        }
    });
}


/***/ },

/***/ "./TypeScript/Utils/Observers.ts"
/*!***************************************!*\
  !*** ./TypeScript/Utils/Observers.ts ***!
  \***************************************/
(__unused_webpack_module, exports, __webpack_require__) {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.getTitleObserverTarget = getTitleObserverTarget;
exports.getTitleObserver = getTitleObserver;
const Contracts_1 = __webpack_require__(/*! ../Contracts */ "./TypeScript/Contracts/index.ts");
function getTitleObserverTarget() {
    return document.querySelector('title');
}
function getTitleObserver() {
    return new MutationObserver((mutations, _) => {
        mutations.forEach((mutation) => {
            if (mutation.type !== "childList")
                return;
            window.infiniframe.messaging.sendMessageToHost(Contracts_1.SendToHostMessageIds.titleChange, document.title);
        });
    });
}


/***/ }

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		if (!(moduleId in __webpack_modules__)) {
/******/ 			delete __webpack_module_cache__[moduleId];
/******/ 			var e = new Error("Cannot find module '" + moduleId + "'");
/******/ 			e.code = 'MODULE_NOT_FOUND';
/******/ 			throw e;
/******/ 		}
/******/ 		__webpack_modules__[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
/******/ 	
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	// This entry module is referenced by other modules so it can't be inlined
/******/ 	var __webpack_exports__ = __webpack_require__("./TypeScript/Index.ts");
/******/ 	
/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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Q0E7VUFDQTtVQUNBOztVQUVBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7O1VBRUE7VUFDQTtVQUNBOzs7O1VFNUJBO1VBQ0E7VUFDQTtVQUNBIiwic291cmNlcyI6WyJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS5qcy1idWlsZC8uL1R5cGVTY3JpcHQvQ29udHJhY3RzL0lJbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS5qcy1idWlsZC8uL1R5cGVTY3JpcHQvQ29udHJhY3RzL2luZGV4LnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvLi9UeXBlU2NyaXB0L0luZGV4LnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvLi9UeXBlU2NyaXB0L0luZmluaUZyYW1lLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvLi9UeXBlU2NyaXB0L0luZmluaUZyYW1lSG9zdE1lc3NhZ2luZy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLmpzLWJ1aWxkLy4vVHlwZVNjcmlwdC9JbmZpbmlGcmFtZVV0aWxzLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvLi9UeXBlU2NyaXB0L0luZmluaUZyYW1lV2luZG93LnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvLi9UeXBlU2NyaXB0L0ludGVyb3AvRW52ZWxvcGVQcm90b2NvbC9JbnRlcm9wRW52ZWxvcGVQcm90b2NvbC50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLmpzLWJ1aWxkLy4vVHlwZVNjcmlwdC9JbnRlcm9wL05hdGl2ZUhvc3QvSG9zdEJyaWRnZS50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLmpzLWJ1aWxkLy4vVHlwZVNjcmlwdC9VdGlscy9CbGFua1RhcmdldEhhbmRsZXIudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS5qcy1idWlsZC8uL1R5cGVTY3JpcHQvVXRpbHMvT2JzZXJ2ZXJzLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvd2VicGFjay9ib290c3RyYXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS5qcy1idWlsZC93ZWJwYWNrL2JlZm9yZS1zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvd2VicGFjay9zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvd2VicGFjay9hZnRlci1zdGFydHVwIl0sInNvdXJjZXNDb250ZW50IjpbIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxuLy8gSW1wb3J0c1xuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXG5pbXBvcnQge0ludGVyb3BFbnZlbG9wZVYxfSBmcm9tIFwiLi9FbnZlbG9wZVByb3RvY29sXCI7XG5cbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxuLy8gQ29kZVxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXG5jb25zdCBpbmZpbmlmcmFtZTogc3RyaW5nID0gXCJfX2luZmluaWZyYW1lXCI7XG5cbmV4cG9ydCBjb25zdCBTZW5kVG9Ib3N0TWVzc2FnZUlkcyA9IHtcbiAgICB0aXRsZUNoYW5nZTogYCR7aW5maW5pZnJhbWV9OnRpdGxlOmNoYW5nZWAsXG4gICAgZnVsbHNjcmVlbkVudGVyOiBgJHtpbmZpbmlmcmFtZX06ZnVsbHNjcmVlbjplbnRlcmAsXG4gICAgZnVsbHNjcmVlbkV4aXQ6IGAke2luZmluaWZyYW1lfTpmdWxsc2NyZWVuOmV4aXRgLFxuICAgIG9wZW5FeHRlcm5hbExpbms6IGAke2luZmluaWZyYW1lfTpvcGVuOmV4dGVybmFsYCxcbiAgICB3aW5kb3dDbG9zZTogYCR7aW5maW5pZnJhbWV9OndpbmRvdzpjbG9zZWAsXG4gICAgcmVhZHk6IGAke2luZmluaWZyYW1lfTpyZWFkeWAsXG4gICAgZ2V0TWVzc2FnZVJlcXVlc3Q6IGAke2luZmluaWZyYW1lfTpnZXQ6cmVxdWVzdGAsXG59XG5cbmV4cG9ydCBjb25zdCBSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzID0ge1xuICAgIHJlZ2lzdGVyT3BlbkV4dGVybmFsOiBgJHtpbmZpbmlmcmFtZX06cmVnaXN0ZXI6b3BlbjpleHRlcm5hbGAsXG4gICAgcmVnaXN0ZXJGdWxsc2NyZWVuQ2hhbmdlOiBgJHtpbmZpbmlmcmFtZX06cmVnaXN0ZXI6ZnVsbHNjcmVlbjpjaGFuZ2VgLFxuICAgIHJlZ2lzdGVyVGl0bGVDaGFuZ2U6IGAke2luZmluaWZyYW1lfTpyZWdpc3Rlcjp0aXRsZTpjaGFuZ2VgLFxuICAgIHJlZ2lzdGVyV2luZG93Q2xvc2U6IGAke2luZmluaWZyYW1lfTpyZWdpc3Rlcjp3aW5kb3c6Y2xvc2VgLFxuICAgIGdldE1lc3NhZ2VSZXNwb25zZTogYCR7aW5maW5pZnJhbWV9OmdldDpyZXNwb25zZWAsXG59XG5cbmV4cG9ydCB0eXBlIFNlbmRUb0hvc3RNZXNzYWdlSWQgPSB0eXBlb2YgU2VuZFRvSG9zdE1lc3NhZ2VJZHNba2V5b2YgdHlwZW9mIFNlbmRUb0hvc3RNZXNzYWdlSWRzXTtcbmV4cG9ydCB0eXBlIE1lc3NhZ2VDYWxsYmFjayA9IChkYXRhPzogc3RyaW5nKSA9PiB2b2lkO1xuXG5leHBvcnQgaW50ZXJmYWNlIElJbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmcge1xuICAgIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkIHwgc3RyaW5nLCBkYXRhPzogdW5rbm93bik6IHZvaWQ7XG4gICAgZ2V0TWVzc2FnZUZyb21Ib3N0QXN5bmMobWVzc2FnZTogSW50ZXJvcEVudmVsb3BlVjEgfCBzdHJpbmcpOiBQcm9taXNlPHN0cmluZz47XG5cbiAgICBhc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nLCBjYWxsYmFjazogTWVzc2FnZUNhbGxiYWNrKTogdm9pZDtcblxuICAgIHVucmVnaXN0ZXJNZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nKTogdm9pZDtcbn1cbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBFeHBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgKiBmcm9tIFwiLi9FbnZlbG9wZVByb3RvY29sXCI7XHJcbmV4cG9ydCAqIGZyb20gXCIuL2dsb2JhbFwiO1xyXG5leHBvcnQgKiBmcm9tIFwiLi9JSW5maW5pRnJhbWVcIjtcclxuZXhwb3J0ICogZnJvbSBcIi4vSUluZmluaUZyYW1lSG9zdE1lc3NhZ2luZ1wiO1xyXG5leHBvcnQgKiBmcm9tIFwiLi9JSW5maW5pRnJhbWVVdGlsc1wiO1xyXG5leHBvcnQgKiBmcm9tIFwiLi9JSW5maW5pRnJhbWVXaW5kb3dcIjsiLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IEluZmluaUZyYW1lIGZyb20gXCIuL0luZmluaUZyYW1lXCI7XHJcbmltcG9ydCB7aW5zdGFsbEhvc3RCcmlkZ2V9IGZyb20gXCIuL0ludGVyb3AvTmF0aXZlSG9zdC9Ib3N0QnJpZGdlXCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQge307XHJcbmluc3RhbGxIb3N0QnJpZGdlKCk7XHJcblxyXG53aW5kb3cuaW5maW5pZnJhbWUgPSBuZXcgSW5maW5pRnJhbWUoKTtcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7SUluZmluaUZyYW1lLCBJSW5maW5pRnJhbWVIb3N0TWVzc2FnaW5nLCBJSW5maW5pRnJhbWVVdGlscywgSUluZmluaUZyYW1lV2luZG93fSBmcm9tIFwiLi9Db250cmFjdHNcIjtcclxuaW1wb3J0IEluZmluaUZyYW1lSG9zdE1lc3NhZ2luZyBmcm9tIFwiLi9JbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmdcIjtcclxuaW1wb3J0IHtJbmZpbmlGcmFtZVV0aWxzfSBmcm9tIFwiLi9JbmZpbmlGcmFtZVV0aWxzXCI7XHJcbmltcG9ydCB7SW5maW5pRnJhbWVXaW5kb3d9IGZyb20gXCIuL0luZmluaUZyYW1lV2luZG93XCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY2xhc3MgSW5maW5pRnJhbWUgaW1wbGVtZW50cyBJSW5maW5pRnJhbWUge1xyXG4gICAgbWVzc2FnaW5nOiBJSW5maW5pRnJhbWVIb3N0TWVzc2FnaW5nID0gbmV3IEluZmluaUZyYW1lSG9zdE1lc3NhZ2luZygpO1xyXG4gICAgd2luZG93OiBJSW5maW5pRnJhbWVXaW5kb3cgPSBuZXcgSW5maW5pRnJhbWVXaW5kb3coKTtcclxuICAgIFxyXG4gICAgdXRpbHM6IElJbmZpbmlGcmFtZVV0aWxzID0gbmV3IEluZmluaUZyYW1lVXRpbHMoKSAgICBcclxufVxyXG5cclxuZXhwb3J0IGRlZmF1bHQgSW5maW5pRnJhbWVcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7XHJcbiAgICBJSW5maW5pRnJhbWVIb3N0TWVzc2FnaW5nLFxyXG4gICAgSW50ZXJvcEVudmVsb3BlVjEsXHJcbiAgICBNZXNzYWdlQ2FsbGJhY2ssXHJcbiAgICBSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLFxyXG4gICAgU2VuZFRvSG9zdE1lc3NhZ2VJZCxcclxuICAgIFNlbmRUb0hvc3RNZXNzYWdlSWRzXHJcbn0gZnJvbSBcIi4vQ29udHJhY3RzXCI7XHJcbmltcG9ydCB7Y3JlYXRlRW52ZWxvcGUsIHBhcnNlSW5jb21pbmdNZXNzYWdlfSBmcm9tIFwiLi9JbnRlcm9wL0VudmVsb3BlUHJvdG9jb2wvSW50ZXJvcEVudmVsb3BlUHJvdG9jb2xcIjtcclxuaW1wb3J0IHtibGFua1RhcmdldEhhbmRsZXJ9IGZyb20gXCIuL1V0aWxzL0JsYW5rVGFyZ2V0SGFuZGxlclwiO1xyXG5pbXBvcnQge2dldFRpdGxlT2JzZXJ2ZXIsIGdldFRpdGxlT2JzZXJ2ZXJUYXJnZXR9IGZyb20gXCIuL1V0aWxzL09ic2VydmVyc1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmNsYXNzIEluZmluaUZyYW1lSG9zdE1lc3NhZ2luZyBpbXBsZW1lbnRzIElJbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmcge1xyXG4gICAgcHJpdmF0ZSBzdGF0aWMgcmVhZG9ubHkgQmxhem9yV2ViVmlld01lc3NhZ2VQcmVmaXggPSBcIl9fYnd2OlwiO1xyXG4gICAgcHJpdmF0ZSBzdGF0aWMgcmVhZG9ubHkgUmVhZHlIYW5kc2hha2VSZXRyeUludGVydmFsTXMgPSAxMDAwO1xyXG4gICAgcHJpdmF0ZSBzdGF0aWMgcmVhZG9ubHkgTWF4UmVhZHlIYW5kc2hha2VBdHRlbXB0cyA9IDIwO1xyXG4gICAgcHJpdmF0ZSBtZXNzYWdlSGFuZGxlcnM6IE1hcDxzdHJpbmcsIE1lc3NhZ2VDYWxsYmFjaz4gPSBuZXcgTWFwKCk7XHJcbiAgICBwcml2YXRlIG9wZW5FeHRlcm5hbFJlZ2lzdGVyZWQgPSBmYWxzZTtcclxuICAgIHByaXZhdGUgZnVsbHNjcmVlblJlZ2lzdGVyZWQgPSBmYWxzZTtcclxuICAgIHByaXZhdGUgdGl0bGVSZWdpc3RlcmVkID0gZmFsc2U7XHJcbiAgICBwcml2YXRlIHdpbmRvd0Nsb3NlUmVnaXN0ZXJlZCA9IGZhbHNlO1xyXG4gICAgcHJpdmF0ZSByZWFkeUhhbmRzaGFrZUF0dGVtcHRzID0gMDtcclxuICAgIHByaXZhdGUgcmVhZHlIYW5kc2hha2VBY2tub3dsZWRnZWQgPSBmYWxzZTtcclxuICAgIHByaXZhdGUgcmVhZHlIYW5kc2hha2VSZXRyeVRpbWVyOiBudW1iZXIgfCBudWxsID0gbnVsbDtcclxuICAgIFxyXG4gICAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAgICAgdGhpcy5hc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKTtcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJPcGVuRXh0ZXJuYWwsIF8gPT4ge1xyXG4gICAgICAgICAgICB0aGlzLm1hcmtSZWFkeUhhbmRzaGFrZUFja25vd2xlZGdlZCgpO1xyXG4gICAgICAgICAgICB0aGlzLnJlZ2lzdGVyT3BlbkV4dGVybmFsKCk7XHJcbiAgICAgICAgfSlcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJGdWxsc2NyZWVuQ2hhbmdlLCBfID0+IHtcclxuICAgICAgICAgICAgdGhpcy5tYXJrUmVhZHlIYW5kc2hha2VBY2tub3dsZWRnZWQoKTtcclxuICAgICAgICAgICAgdGhpcy5yZWdpc3RlckZ1bGxzY3JlZW5DaGFuZ2UoKTtcclxuICAgICAgICB9KVxyXG5cclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3RlclRpdGxlQ2hhbmdlLCBfID0+IHtcclxuICAgICAgICAgICAgdGhpcy5tYXJrUmVhZHlIYW5kc2hha2VBY2tub3dsZWRnZWQoKTtcclxuICAgICAgICAgICAgdGhpcy5yZWdpc3RlclRpdGxlQ2hhbmdlKCk7XHJcbiAgICAgICAgfSlcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJXaW5kb3dDbG9zZSwgXyA9PiB7XHJcbiAgICAgICAgICAgIHRoaXMubWFya1JlYWR5SGFuZHNoYWtlQWNrbm93bGVkZ2VkKCk7XHJcbiAgICAgICAgICAgIHRoaXMucmVnaXN0ZXJXaW5kb3dDbG9zZSgpO1xyXG4gICAgICAgIH0pXHJcblxyXG4gICAgICAgIHRoaXMuc2VuZFJlYWR5SGFuZHNoYWtlV2l0aFJldHJ5KCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkIHwgc3RyaW5nLCBkYXRhPzogdW5rbm93bikge1xyXG4gICAgICAgIGNvbnN0IGVudmVsb3BlID0gY3JlYXRlRW52ZWxvcGUoaWQsIGRhdGEpO1xyXG5cclxuICAgICAgICBpZiAod2luZG93Ll9faW5maW5pZnJhbWU/Lmhvc3Q/LnBvc3REYXRhKSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5fX2luZmluaWZyYW1lLmhvc3QucG9zdERhdGEoZW52ZWxvcGUpO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUud2FybihcIk1lc3NhZ2UgdG8gaG9zdCBmYWlsZWQuIEhvc3QgYnJpZGdlIEFQSSBpcyBub3QgaW5pdGlhbGl6ZWQuXCIpO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG4gICAgXHJcbiAgICBwdWJsaWMgYXN5bmMgZ2V0TWVzc2FnZUZyb21Ib3N0QXN5bmMobWVzc2FnZTogSW50ZXJvcEVudmVsb3BlVjEgfCBzdHJpbmcpOiBQcm9taXNlPHN0cmluZz4ge1xyXG4gICAgICAgIGNvbnN0IGhvc3QgPSB3aW5kb3cuX19pbmZpbmlmcmFtZT8uaG9zdDtcclxuICAgICAgICBpZiAoIWhvc3Q/LmdldERhdGEpIHRocm93IG5ldyBFcnJvcihcIk1lc3NhZ2UgdG8gaG9zdCBmYWlsZWQuIEhvc3QgZ2V0RGF0YSBBUEkgaXMgbm90IGluaXRpYWxpemVkLlwiKTtcclxuXHJcbiAgICAgICAgY29uc3QgZW52ZWxvcGUgPSB0eXBlb2YgbWVzc2FnZSA9PT0gXCJzdHJpbmdcIlxyXG4gICAgICAgICAgICA/IGNyZWF0ZUVudmVsb3BlKG1lc3NhZ2UpXHJcbiAgICAgICAgICAgIDogbWVzc2FnZTtcclxuXHJcbiAgICAgICAgcmV0dXJuIGF3YWl0IGhvc3QuZ2V0RGF0YShlbnZlbG9wZSk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIobWVzc2FnZUlkOiBzdHJpbmcsIGNhbGxiYWNrOiBNZXNzYWdlQ2FsbGJhY2spIHtcclxuICAgICAgICB0aGlzLm1lc3NhZ2VIYW5kbGVycy5zZXQobWVzc2FnZUlkLCBjYWxsYmFjayk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHVucmVnaXN0ZXJNZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nKSB7XHJcbiAgICAgICAgdGhpcy5tZXNzYWdlSGFuZGxlcnMuZGVsZXRlKG1lc3NhZ2VJZCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBhc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKSB7XHJcbiAgICAgICAgaWYgKHdpbmRvdy5fX2luZmluaWZyYW1lPy5ob3N0Py5yZWNlaXZlQ2FsbGJhY2spIHtcclxuICAgICAgICAgICAgd2luZG93Ll9faW5maW5pZnJhbWUuaG9zdC5yZWNlaXZlQ2FsbGJhY2soKG1lc3NhZ2U6IHN0cmluZykgPT4ge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5oYW5kbGVJbnRlcm9wTWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2Uge1xyXG4gICAgICAgICAgICBjb25zb2xlLndhcm4oXCJXZWIgbWVzc2FnZSByZWNlaXZlciBmYWlsZWQuIEhvc3QgYnJpZGdlIEFQSSBpcyBub3QgaW5pdGlhbGl6ZWQuXCIpO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgaGFuZGxlSW50ZXJvcE1lc3NhZ2UobWVzc2FnZTogYW55KTogYm9vbGVhbiB7XHJcbiAgICAgICAgaWYgKHR5cGVvZiBtZXNzYWdlICE9PSAnc3RyaW5nJykgcmV0dXJuIGZhbHNlO1xyXG4gICAgICAgIGlmICghbWVzc2FnZSkgcmV0dXJuIGZhbHNlO1xyXG4gICAgICAgIC8vIFJvdXRlIG9ubHkgbWVzc2FnZXMgdGhhdCBtYXRjaCB0aGUgZXhwbGljaXQgaW50ZXJvcCBlbnZlbG9wZSBjb250cmFjdC5cclxuICAgICAgICBjb25zdCBwYXJzZWRNZXNzYWdlID0gcGFyc2VJbmNvbWluZ01lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICAgICAgaWYgKFwiZXJyb3JcIiBpbiBwYXJzZWRNZXNzYWdlKSByZXR1cm4gZmFsc2U7XHJcblxyXG4gICAgICAgIC8vIEJsYXpvciBXZWJWaWV3IGludGVybmFsIHRyYW5zcG9ydCBtZXNzYWdlcyBhcmUgcm91dGVkIGJ5IGJsYXpvci53ZWJ2aWV3LmpzLlxyXG4gICAgICAgIC8vIFRoZXkgYXJlIG5vdCBJbmZpbmlGcmFtZSBob3N0LW1lc3NhZ2UgY29udHJhY3RzIGFuZCBzaG91bGQgbm90IGVtaXQgd2FybmluZ3MuXHJcbiAgICAgICAgaWYgKHBhcnNlZE1lc3NhZ2UubWVzc2FnZUlkLnN0YXJ0c1dpdGgoSW5maW5pRnJhbWVIb3N0TWVzc2FnaW5nLkJsYXpvcldlYlZpZXdNZXNzYWdlUHJlZml4KSkge1xyXG4gICAgICAgICAgICByZXR1cm4gdHJ1ZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIEV4ZWN1dGUgcmVnaXN0ZXJlZCBoYW5kbGVyXHJcbiAgICAgICAgY29uc3QgaGFuZGxlciA9IHRoaXMubWVzc2FnZUhhbmRsZXJzLmdldChwYXJzZWRNZXNzYWdlLm1lc3NhZ2VJZCk7XHJcbiAgICAgICAgaWYgKCFoYW5kbGVyKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUud2FybignTm8gaGFuZGxlciByZWdpc3RlcmVkIGZvciBtZXNzYWdlOicsIHBhcnNlZE1lc3NhZ2UpO1xyXG4gICAgICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBoYW5kbGVyKHBhcnNlZE1lc3NhZ2UucGF5bG9hZCk7XHJcbiAgICAgICAgcmV0dXJuIHRydWU7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWdpc3Rlck9wZW5FeHRlcm5hbCgpIHtcclxuICAgICAgICBpZiAodGhpcy5vcGVuRXh0ZXJuYWxSZWdpc3RlcmVkKSByZXR1cm47XHJcbiAgICAgICAgdGhpcy5vcGVuRXh0ZXJuYWxSZWdpc3RlcmVkID0gdHJ1ZTtcclxuICAgICAgICBkb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwiY2xpY2tcIiwgYmxhbmtUYXJnZXRIYW5kbGVyLCB7Y2FwdHVyZTogdHJ1ZX0pO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgcmVnaXN0ZXJGdWxsc2NyZWVuQ2hhbmdlKCkge1xyXG4gICAgICAgIGlmICh0aGlzLmZ1bGxzY3JlZW5SZWdpc3RlcmVkKSByZXR1cm47XHJcbiAgICAgICAgdGhpcy5mdWxsc2NyZWVuUmVnaXN0ZXJlZCA9IHRydWU7XHJcbiAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImZ1bGxzY3JlZW5jaGFuZ2VcIiwgKF86IEV2ZW50KSA9PiB7XHJcbiAgICAgICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkgdGhpcy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy5mdWxsc2NyZWVuRW50ZXIpO1xyXG4gICAgICAgICAgICBlbHNlIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMuZnVsbHNjcmVlbkV4aXQpO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBkb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwia2V5ZG93blwiLCBhc3luYyAoZTogS2V5Ym9hcmRFdmVudCkgPT4ge1xyXG4gICAgICAgICAgICBpZiAoZS5rZXkgIT09IFwiRjExXCIpIHJldHVybjtcclxuICAgICAgICAgICAgaWYgKGRvY3VtZW50LmZ1bGxzY3JlZW5FbGVtZW50KSBhd2FpdCBkb2N1bWVudC5leGl0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgICAgICBlbHNlIGF3YWl0IGRvY3VtZW50LmJvZHkucmVxdWVzdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHJlZ2lzdGVyVGl0bGVDaGFuZ2UoKSB7XHJcbiAgICAgICAgaWYgKHRoaXMudGl0bGVSZWdpc3RlcmVkKSByZXR1cm47XHJcbiAgICAgICAgdGhpcy50aXRsZVJlZ2lzdGVyZWQgPSB0cnVlO1xyXG4gICAgICAgIGNvbnN0IHRpdGxlVGFyZ2V0ID0gZ2V0VGl0bGVPYnNlcnZlclRhcmdldCgpO1xyXG4gICAgICAgIGlmICh0aXRsZVRhcmdldCkge1xyXG4gICAgICAgICAgICBnZXRUaXRsZU9ic2VydmVyKCkub2JzZXJ2ZSh0aXRsZVRhcmdldCwge2NoaWxkTGlzdDogdHJ1ZX0pO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCBoZWFkVGFyZ2V0ID0gZG9jdW1lbnQuaGVhZCB8fCBkb2N1bWVudC5kb2N1bWVudEVsZW1lbnQ7XHJcbiAgICAgICAgaWYgKCFoZWFkVGFyZ2V0KSByZXR1cm47XHJcblxyXG4gICAgICAgIGNvbnN0IGhlYWRPYnNlcnZlciA9IG5ldyBNdXRhdGlvbk9ic2VydmVyKCgpID0+IHtcclxuICAgICAgICAgICAgY29uc3QgdGFyZ2V0ID0gZ2V0VGl0bGVPYnNlcnZlclRhcmdldCgpO1xyXG4gICAgICAgICAgICBpZiAoIXRhcmdldCkgcmV0dXJuO1xyXG4gICAgICAgICAgICBoZWFkT2JzZXJ2ZXIuZGlzY29ubmVjdCgpO1xyXG4gICAgICAgICAgICBnZXRUaXRsZU9ic2VydmVyKCkub2JzZXJ2ZSh0YXJnZXQsIHtjaGlsZExpc3Q6IHRydWV9KTtcclxuICAgICAgICB9KTtcclxuICAgICAgICBoZWFkT2JzZXJ2ZXIub2JzZXJ2ZShoZWFkVGFyZ2V0LCB7Y2hpbGRMaXN0OiB0cnVlLCBzdWJ0cmVlOiB0cnVlfSk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWdpc3RlcldpbmRvd0Nsb3NlKCkge1xyXG4gICAgICAgIGlmICh0aGlzLndpbmRvd0Nsb3NlUmVnaXN0ZXJlZCkgcmV0dXJuO1xyXG4gICAgICAgIHRoaXMud2luZG93Q2xvc2VSZWdpc3RlcmVkID0gdHJ1ZTtcclxuICAgICAgICB3aW5kb3cuY2xvc2UgPSAoKSA9PiB7XHJcbiAgICAgICAgICAgIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMud2luZG93Q2xvc2UpO1xyXG4gICAgICAgIH07XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBzZW5kUmVhZHlIYW5kc2hha2VXaXRoUmV0cnkoKSB7XHJcbiAgICAgICAgdGhpcy5zZW5kUmVhZHlIYW5kc2hha2UoKTtcclxuXHJcbiAgICAgICAgdGhpcy5yZWFkeUhhbmRzaGFrZVJldHJ5VGltZXIgPSB3aW5kb3cuc2V0SW50ZXJ2YWwoKCkgPT4ge1xyXG4gICAgICAgICAgICBpZiAodGhpcy5yZWFkeUhhbmRzaGFrZUFja25vd2xlZGdlZCB8fCB0aGlzLnJlYWR5SGFuZHNoYWtlQXR0ZW1wdHMgPj0gSW5maW5pRnJhbWVIb3N0TWVzc2FnaW5nLk1heFJlYWR5SGFuZHNoYWtlQXR0ZW1wdHMpIHtcclxuICAgICAgICAgICAgICAgIHRoaXMuc3RvcFJlYWR5SGFuZHNoYWtlUmV0cnkoKTtcclxuICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgdGhpcy5zZW5kUmVhZHlIYW5kc2hha2UoKTtcclxuICAgICAgICB9LCBJbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmcuUmVhZHlIYW5kc2hha2VSZXRyeUludGVydmFsTXMpO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgc2VuZFJlYWR5SGFuZHNoYWtlKCkge1xyXG4gICAgICAgIHRoaXMucmVhZHlIYW5kc2hha2VBdHRlbXB0cysrO1xyXG4gICAgICAgIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMucmVhZHkpO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgbWFya1JlYWR5SGFuZHNoYWtlQWNrbm93bGVkZ2VkKCkge1xyXG4gICAgICAgIGlmICh0aGlzLnJlYWR5SGFuZHNoYWtlQWNrbm93bGVkZ2VkKSByZXR1cm47XHJcbiAgICAgICAgdGhpcy5yZWFkeUhhbmRzaGFrZUFja25vd2xlZGdlZCA9IHRydWU7XHJcbiAgICAgICAgdGhpcy5zdG9wUmVhZHlIYW5kc2hha2VSZXRyeSgpO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgc3RvcFJlYWR5SGFuZHNoYWtlUmV0cnkoKSB7XHJcbiAgICAgICAgaWYgKHRoaXMucmVhZHlIYW5kc2hha2VSZXRyeVRpbWVyID09PSBudWxsKSByZXR1cm47XHJcbiAgICAgICAgd2luZG93LmNsZWFySW50ZXJ2YWwodGhpcy5yZWFkeUhhbmRzaGFrZVJldHJ5VGltZXIpO1xyXG4gICAgICAgIHRoaXMucmVhZHlIYW5kc2hha2VSZXRyeVRpbWVyID0gbnVsbDtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGRlZmF1bHQgSW5maW5pRnJhbWVIb3N0TWVzc2FnaW5nXHJcbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge0lJbmZpbmlGcmFtZVV0aWxzfSBmcm9tIFwiLi9Db250cmFjdHNcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY2xhc3MgSW5maW5pRnJhbWVVdGlscyBpbXBsZW1lbnRzIElJbmZpbmlGcmFtZVV0aWxzIHtcclxuICAgIHNldFBvaW50ZXJDYXB0dXJlKGVsZW1lbnQ6IEVsZW1lbnQsIHBvaW50ZXJJZDogbnVtYmVyKTogdm9pZCB7XHJcbiAgICAgICAgaWYgKGVsZW1lbnQgPT09IG51bGwpIHJldHVybjtcclxuICAgICAgICBpZiAocG9pbnRlcklkID09PSBudWxsKSByZXR1cm47XHJcbiAgICAgICAgXHJcbiAgICAgICAgaWYgKGVsZW1lbnQuaGFzUG9pbnRlckNhcHR1cmUocG9pbnRlcklkKSkgcmV0dXJuO1xyXG4gICAgICAgIGVsZW1lbnQuc2V0UG9pbnRlckNhcHR1cmUocG9pbnRlcklkKTtcclxuICAgIH1cclxuICAgIFxyXG4gICAgcmVsZWFzZVBvaW50ZXJDYXB0dXJlKGVsZW1lbnQ6IEVsZW1lbnQsIHBvaW50ZXJJZDogbnVtYmVyKTogdm9pZCB7XHJcbiAgICAgICAgaWYgKGVsZW1lbnQgPT09IG51bGwpIHJldHVybjtcclxuICAgICAgICBpZiAocG9pbnRlcklkID09PSBudWxsKSByZXR1cm47XHJcbiAgICAgICAgXHJcbiAgICAgICAgaWYgKCFlbGVtZW50Lmhhc1BvaW50ZXJDYXB0dXJlKHBvaW50ZXJJZCkpIHJldHVybjtcclxuICAgICAgICBlbGVtZW50LnJlbGVhc2VQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG4gICAgfVxyXG59IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7SUluZmluaUZyYW1lV2luZG93LCBTZW5kVG9Ib3N0TWVzc2FnZUlkc30gZnJvbSBcIi4vQ29udHJhY3RzXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGNsYXNzIEluZmluaUZyYW1lV2luZG93IGltcGxlbWVudHMgSUluZmluaUZyYW1lV2luZG93IHtcclxuICAgIFxyXG4gICAgc2V0VGl0bGUodGl0bGU6c3RyaW5nKSB7XHJcbiAgICAgICAgd2luZG93LmluZmluaWZyYW1lLm1lc3NhZ2luZy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy50aXRsZUNoYW5nZSwgdGl0bGUpO1xyXG4gICAgfVxyXG59IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7SW50ZXJvcEVudmVsb3BlVjEsIFBhcnNlZEludGVyb3BNZXNzYWdlLCBJbnRlcm9wUGFyc2VFcnJvcn0gZnJvbSBcIi4uLy4uL0NvbnRyYWN0c1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBjb25zdCBJbnRlcm9wRW52ZWxvcGVWZXJzaW9uID0gMTtcclxuZXhwb3J0IGNvbnN0IEludGVyb3BNZXNzYWdlTWF4U2l6ZUJ5dGVzID0gMTAyNCAqIDEwMjQ7XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gY3JlYXRlRW52ZWxvcGUoaWQ6IHN0cmluZywgZGF0YT86IHVua25vd24sIGNoYW5uZWw/OiBzdHJpbmcpOiBJbnRlcm9wRW52ZWxvcGVWMSB7XHJcbiAgICBpZiAoIWlkIHx8IGlkLnRyaW0oKS5sZW5ndGggPT09IDApIHtcclxuICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJFbnZlbG9wZSAnaWQnIGlzIHJlcXVpcmVkLlwiKTtcclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4ge1xyXG4gICAgICAgIGlkLFxyXG4gICAgICAgIGRhdGEsXHJcbiAgICAgICAgdmVyc2lvbjogSW50ZXJvcEVudmVsb3BlVmVyc2lvbixcclxuICAgICAgICBjaGFubmVsXHJcbiAgICB9O1xyXG59XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gY3JlYXRlRW52ZWxvcGVNZXNzYWdlKGlkOiBzdHJpbmcsIGRhdGE/OiB1bmtub3duLCBjaGFubmVsPzogc3RyaW5nKTogc3RyaW5nIHtcclxuICAgIGNvbnN0IGVudmVsb3BlID0gY3JlYXRlRW52ZWxvcGUoaWQsIGRhdGEsIGNoYW5uZWwpO1xyXG5cclxuICAgIHJldHVybiBKU09OLnN0cmluZ2lmeShlbnZlbG9wZSk7XHJcbn1cclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBwYXJzZUluY29taW5nTWVzc2FnZShtZXNzYWdlOiBzdHJpbmcpOiBQYXJzZWRJbnRlcm9wTWVzc2FnZSB8IEludGVyb3BQYXJzZUVycm9yIHtcclxuICAgIGlmICghbWVzc2FnZSB8fCBtZXNzYWdlLnRyaW0oKS5sZW5ndGggPT09IDApIHtcclxuICAgICAgICByZXR1cm4ge2Vycm9yOiBcIk1lc3NhZ2UgaXMgZW1wdHkuXCJ9O1xyXG4gICAgfVxyXG5cclxuICAgIGlmIChnZXRVdGY4Qnl0ZUNvdW50KG1lc3NhZ2UpID4gSW50ZXJvcE1lc3NhZ2VNYXhTaXplQnl0ZXMpIHtcclxuICAgICAgICByZXR1cm4ge2Vycm9yOiBgTWVzc2FnZSBleGNlZWRzIG1heCBzaXplIG9mICR7SW50ZXJvcE1lc3NhZ2VNYXhTaXplQnl0ZXN9IGJ5dGVzLmB9O1xyXG4gICAgfVxyXG5cclxuICAgIHRyeSB7XHJcbiAgICAgICAgY29uc3QgcGFyc2VkID0gSlNPTi5wYXJzZShtZXNzYWdlKSBhcyB1bmtub3duO1xyXG4gICAgICAgIGlmICghaXNPYmplY3QocGFyc2VkKSkge1xyXG4gICAgICAgICAgICByZXR1cm4ge2Vycm9yOiBcIkVudmVsb3BlIHJvb3QgbXVzdCBiZSBhIEpTT04gb2JqZWN0LlwifTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICh0eXBlb2YgcGFyc2VkLmlkICE9PSBcInN0cmluZ1wiIHx8IHBhcnNlZC5pZC50cmltKCkubGVuZ3RoID09PSAwKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB7ZXJyb3I6IFwiRW52ZWxvcGUgJ2lkJyBpcyByZXF1aXJlZCBhbmQgbXVzdCBiZSBhIHN0cmluZy5cIn07XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAodHlwZW9mIHBhcnNlZC52ZXJzaW9uICE9PSBcIm51bWJlclwiIHx8ICFOdW1iZXIuaXNJbnRlZ2VyKHBhcnNlZC52ZXJzaW9uKSkge1xyXG4gICAgICAgICAgICByZXR1cm4ge2Vycm9yOiBcIkVudmVsb3BlICd2ZXJzaW9uJyBpcyByZXF1aXJlZCBhbmQgbXVzdCBiZSBhbiBpbnRlZ2VyLlwifTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmIChwYXJzZWQudmVyc2lvbiAhPT0gSW50ZXJvcEVudmVsb3BlVmVyc2lvbikge1xyXG4gICAgICAgICAgICByZXR1cm4ge2Vycm9yOiBgVW5zdXBwb3J0ZWQgZW52ZWxvcGUgdmVyc2lvbiAnJHtwYXJzZWQudmVyc2lvbn0nLmB9O1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgcGF5bG9hZCA9IGNvbnZlcnREYXRhVG9QYXlsb2FkKHBhcnNlZC5kYXRhKTtcclxuICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICBtZXNzYWdlSWQ6IHBhcnNlZC5pZCxcclxuICAgICAgICAgICAgcGF5bG9hZFxyXG4gICAgICAgIH07XHJcbiAgICB9IGNhdGNoIHtcclxuICAgICAgICByZXR1cm4ge2Vycm9yOiBcIkVudmVsb3BlIEpTT04gaXMgbWFsZm9ybWVkLlwifTtcclxuICAgIH1cclxufVxyXG5cclxuZnVuY3Rpb24gY29udmVydERhdGFUb1BheWxvYWQoZGF0YTogdW5rbm93bik6IHN0cmluZyB8IHVuZGVmaW5lZCB7XHJcbiAgICBpZiAoZGF0YSA9PT0gbnVsbCB8fCBkYXRhID09PSB1bmRlZmluZWQpIHtcclxuICAgICAgICByZXR1cm4gdW5kZWZpbmVkO1xyXG4gICAgfVxyXG5cclxuICAgIGlmICh0eXBlb2YgZGF0YSA9PT0gXCJzdHJpbmdcIikge1xyXG4gICAgICAgIHJldHVybiBkYXRhO1xyXG4gICAgfVxyXG5cclxuICAgIHJldHVybiBKU09OLnN0cmluZ2lmeShkYXRhKTtcclxufVxyXG5cclxuZnVuY3Rpb24gZ2V0VXRmOEJ5dGVDb3VudChtZXNzYWdlOiBzdHJpbmcpOiBudW1iZXIge1xyXG4gICAgcmV0dXJuIG5ldyBUZXh0RW5jb2RlcigpLmVuY29kZShtZXNzYWdlKS5sZW5ndGg7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGlzT2JqZWN0KHZhbHVlOiB1bmtub3duKTogdmFsdWUgaXMgUmVjb3JkPHN0cmluZywgdW5rbm93bj4ge1xyXG4gICAgcmV0dXJuIHR5cGVvZiB2YWx1ZSA9PT0gXCJvYmplY3RcIiAmJiB2YWx1ZSAhPT0gbnVsbDtcclxufVxyXG4iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cbi8vIEltcG9ydHNcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxuaW1wb3J0IHtJbnRlcm9wRW52ZWxvcGVWMX0gZnJvbSBcIi4uLy4uL0NvbnRyYWN0c1wiO1xuaW1wb3J0IHtJbnRlcm9wRW52ZWxvcGVWZXJzaW9uLCBwYXJzZUluY29taW5nTWVzc2FnZX0gZnJvbSBcIi4uL0VudmVsb3BlUHJvdG9jb2wvSW50ZXJvcEVudmVsb3BlUHJvdG9jb2xcIjtcblxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXG4vLyBDb2RlXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cbmNvbnN0IEdldE1lc3NhZ2VSZXF1ZXN0SWQgPSBcIl9faW5maW5pZnJhbWU6Z2V0OnJlcXVlc3RcIjtcbmNvbnN0IEdldE1lc3NhZ2VSZXNwb25zZUlkID0gXCJfX2luZmluaWZyYW1lOmdldDpyZXNwb25zZVwiO1xuY29uc3QgR2V0TWVzc2FnZVRpbWVvdXRNcyA9IDEwXzAwMDtcblxudHlwZSBSZWNlaXZlQ2FsbGJhY2sgPSAobWVzc2FnZTogc3RyaW5nKSA9PiB2b2lkO1xuY29uc3QgcmVjZWl2ZUNhbGxiYWNrcyA9IG5ldyBTZXQ8UmVjZWl2ZUNhbGxiYWNrPigpO1xubGV0IHJlY2VpdmVCcmlkZ2VBdHRhY2hlZCA9IGZhbHNlO1xuXG5leHBvcnQgZnVuY3Rpb24gaW5zdGFsbEhvc3RCcmlkZ2UoKTogdm9pZCB7XG4gICAgY29uc3Qgcm9vdDogTm9uTnVsbGFibGU8V2luZG93W1wiX19pbmZpbmlmcmFtZVwiXT4gPSB3aW5kb3cuX19pbmZpbmlmcmFtZSA/PyB7fTtcbiAgICBjb25zdCBob3N0ID0gKHJvb3QuaG9zdCA/PyB7fSkgYXMgTm9uTnVsbGFibGU8Tm9uTnVsbGFibGU8V2luZG93W1wiX19pbmZpbmlmcmFtZVwiXT5bXCJob3N0XCJdPjtcbiAgICBjb25zdCBleGlzdGluZ1Bvc3REYXRhID0gaG9zdC5wb3N0RGF0YTtcbiAgICBjb25zdCBleGlzdGluZ1JlY2VpdmVDYWxsYmFjayA9IGhvc3QucmVjZWl2ZUNhbGxiYWNrO1xuICAgIGNvbnN0IGV4aXN0aW5nR2V0RGF0YSA9IGhvc3QuZ2V0RGF0YTtcblxuICAgIGhvc3QucG9zdERhdGEgPSAoZW52ZWxvcGU6IEludGVyb3BFbnZlbG9wZVYxIHwgc3RyaW5nKSA9PiB7XG4gICAgICAgIGRpc3BhdGNoRW52ZWxvcGVUb0hvc3QoZW52ZWxvcGUsIGV4aXN0aW5nUG9zdERhdGEpO1xuICAgIH07XG4gICAgaG9zdC5yZWNlaXZlQ2FsbGJhY2sgPSAoY2FsbGJhY2s6IChtZXNzYWdlOiBzdHJpbmcpID0+IHZvaWQpID0+IHtcbiAgICAgICAgcmVnaXN0ZXJXZWJNZXNzYWdlUmVjZWl2ZXIoY2FsbGJhY2ssIGV4aXN0aW5nUmVjZWl2ZUNhbGxiYWNrKTtcbiAgICB9O1xuICAgIGhvc3QuZ2V0RGF0YSA9IChtZXNzYWdlOiBJbnRlcm9wRW52ZWxvcGVWMSB8IHN0cmluZykgPT4ge1xuICAgICAgICByZXR1cm4gcmVxdWVzdE1lc3NhZ2VGcm9tSG9zdChtZXNzYWdlLCBob3N0LCBleGlzdGluZ0dldERhdGEsIGV4aXN0aW5nUmVjZWl2ZUNhbGxiYWNrKTtcbiAgICB9O1xuXG4gICAgcm9vdC5ob3N0ID0gaG9zdDtcbiAgICB3aW5kb3cuX19pbmZpbmlmcmFtZSA9IHJvb3Q7XG59XG5cbmZ1bmN0aW9uIGRpc3BhdGNoRW52ZWxvcGVUb0hvc3QoXG4gICAgZW52ZWxvcGU6IEludGVyb3BFbnZlbG9wZVYxIHwgc3RyaW5nLFxuICAgIGV4aXN0aW5nUG9zdERhdGE/OiAoKGVudmVsb3BlOiBJbnRlcm9wRW52ZWxvcGVWMSB8IHN0cmluZykgPT4gdm9pZClcbik6IHZvaWQge1xuICAgIGlmICh0eXBlb2YgZW52ZWxvcGUgPT09IFwic3RyaW5nXCIpIHtcbiAgICAgICAgY29uc3QgcmF3TWVzc2FnZSA9IGVudmVsb3BlLnRyaW0oKTtcbiAgICAgICAgaWYgKHJhd01lc3NhZ2UubGVuZ3RoID09PSAwKSB7XG4gICAgICAgICAgICBjb25zb2xlLndhcm4oXCJJZ25vcmluZyBlbXB0eSBob3N0IGJyaWRnZSBwYXlsb2FkLlwiKTtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfVxuXG4gICAgICAgIGlmIChleGlzdGluZ1Bvc3REYXRhKSB7XG4gICAgICAgICAgICB0cnkge1xuICAgICAgICAgICAgICAgIGV4aXN0aW5nUG9zdERhdGEocmF3TWVzc2FnZSk7XG4gICAgICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgICAgICBjb25zb2xlLndhcm4oXCJFeGlzdGluZyBJbmZpbmlGcmFtZSBob3N0IGJyaWRnZSBmYWlsZWQuIEZhbGxpbmcgYmFjayB0byBwbGF0Zm9ybSBhZGFwdGVycy5cIiwgZXJyb3IpO1xuICAgICAgICAgICAgfVxuICAgICAgICB9XG5cbiAgICAgICAgc2VuZFZpYVBsYXRmb3JtVHJhbnNwb3J0KHJhd01lc3NhZ2UpO1xuICAgICAgICByZXR1cm47XG4gICAgfVxuXG4gICAgY29uc3Qgbm9ybWFsaXplZCA9IG5vcm1hbGl6ZUVudmVsb3BlKGVudmVsb3BlKTtcbiAgICBpZiAoIW5vcm1hbGl6ZWQpIHtcbiAgICAgICAgcmV0dXJuO1xuICAgIH1cblxuICAgIGNvbnN0IHNlcmlhbGl6ZWRFbnZlbG9wZSA9IEpTT04uc3RyaW5naWZ5KG5vcm1hbGl6ZWQpO1xuXG4gICAgaWYgKGV4aXN0aW5nUG9zdERhdGEpIHtcbiAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgIC8vIFByZWZlciB0aGUgc3RyaW5nIGNvbnRyYWN0IGZvciBob3N0IGFkYXB0ZXJzIHRoYXQgb25seSBhY2NlcHQgcmF3IG1lc3NhZ2VzLlxuICAgICAgICAgICAgZXhpc3RpbmdQb3N0RGF0YShzZXJpYWxpemVkRW52ZWxvcGUpO1xuICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgICAgICAvLyBCYWNrd2FyZCBjb21wYXRpYmlsaXR5IGZvciBhZGFwdGVycyB0aGF0IHN0aWxsIGV4cGVjdCBhbiBlbnZlbG9wZSBvYmplY3QuXG4gICAgICAgICAgICAgICAgZXhpc3RpbmdQb3N0RGF0YShub3JtYWxpemVkKTtcbiAgICAgICAgICAgICAgICByZXR1cm47XG4gICAgICAgICAgICB9IGNhdGNoIHtcbiAgICAgICAgICAgICAgICBjb25zb2xlLndhcm4oXCJFeGlzdGluZyBJbmZpbmlGcmFtZSBob3N0IGJyaWRnZSBmYWlsZWQuIEZhbGxpbmcgYmFjayB0byBwbGF0Zm9ybSBhZGFwdGVycy5cIiwgZXJyb3IpO1xuICAgICAgICAgICAgfVxuICAgICAgICB9XG4gICAgfVxuXG4gICAgc2VuZFZpYVBsYXRmb3JtVHJhbnNwb3J0KHNlcmlhbGl6ZWRFbnZlbG9wZSk7XG59XG5cbmZ1bmN0aW9uIHJlcXVlc3RNZXNzYWdlRnJvbUhvc3QoXG4gICAgbWVzc2FnZTogSW50ZXJvcEVudmVsb3BlVjEgfCBzdHJpbmcsXG4gICAgaG9zdDogTm9uTnVsbGFibGU8Tm9uTnVsbGFibGU8V2luZG93W1wiX19pbmZpbmlmcmFtZVwiXT5bXCJob3N0XCJdPixcbiAgICBleGlzdGluZ0dldERhdGE/OiAoKG1lc3NhZ2U6IEludGVyb3BFbnZlbG9wZVYxIHwgc3RyaW5nKSA9PiBQcm9taXNlPHN0cmluZz4gfCBzdHJpbmcpLFxuICAgIGV4aXN0aW5nUmVjZWl2ZUNhbGxiYWNrPzogKGNhbGxiYWNrOiAobWVzc2FnZTogc3RyaW5nKSA9PiB2b2lkKSA9PiB2b2lkXG4pOiBQcm9taXNlPHN0cmluZz4ge1xuICAgIGNvbnN0IG5vcm1hbGl6ZWRNZXNzYWdlID0gbm9ybWFsaXplR2V0TWVzc2FnZUlucHV0KG1lc3NhZ2UpO1xuICAgIGlmICghbm9ybWFsaXplZE1lc3NhZ2UpIHtcbiAgICAgICAgcmV0dXJuIFByb21pc2UucmVqZWN0KG5ldyBFcnJvcihcIkhvc3QgZ2V0RGF0YSBwYXlsb2FkIGlzIGludmFsaWQuXCIpKTtcbiAgICB9XG5cbiAgICBpZiAoZXhpc3RpbmdHZXREYXRhKSB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICBjb25zdCBleGlzdGluZ1Jlc3VsdCA9IGV4aXN0aW5nR2V0RGF0YShub3JtYWxpemVkTWVzc2FnZSk7XG4gICAgICAgICAgICBpZiAoZXhpc3RpbmdSZXN1bHQgJiYgdHlwZW9mIChleGlzdGluZ1Jlc3VsdCBhcyBQcm9taXNlPHN0cmluZz4pLnRoZW4gPT09IFwiZnVuY3Rpb25cIikge1xuICAgICAgICAgICAgICAgIHJldHVybiBleGlzdGluZ1Jlc3VsdCBhcyBQcm9taXNlPHN0cmluZz47XG4gICAgICAgICAgICB9XG5cbiAgICAgICAgICAgIHJldHVybiBQcm9taXNlLnJlc29sdmUoU3RyaW5nKGV4aXN0aW5nUmVzdWx0ID8/IFwiXCIpKTtcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIGNvbnNvbGUud2FybihcIkV4aXN0aW5nIEluZmluaUZyYW1lIGdldERhdGEgYnJpZGdlIGZhaWxlZC4gRmFsbGluZyBiYWNrIHRvIHJlcXVlc3QvcmVzcG9uc2UgdHJhbnNwb3J0LlwiLCBlcnJvcik7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBjb25zdCByZXF1ZXN0SWQgPSBjcmVhdGVSZXF1ZXN0SWQoKTtcblxuICAgIHJldHVybiBuZXcgUHJvbWlzZTxzdHJpbmc+KChyZXNvbHZlLCByZWplY3QpID0+IHtcbiAgICAgICAgY29uc3QgdGltZW91dCA9IHdpbmRvdy5zZXRUaW1lb3V0KCgpID0+IHtcbiAgICAgICAgICAgIHVucmVnaXN0ZXJXZWJNZXNzYWdlUmVjZWl2ZXIocmVzcG9uc2VDYWxsYmFjayk7XG4gICAgICAgICAgICByZWplY3QobmV3IEVycm9yKFwiVGltZWQgb3V0IHdhaXRpbmcgZm9yIGdldERhdGEgcmVzcG9uc2UgZnJvbSBob3N0LlwiKSk7XG4gICAgICAgIH0sIEdldE1lc3NhZ2VUaW1lb3V0TXMpO1xuXG4gICAgICAgIGNvbnN0IHJlc3BvbnNlQ2FsbGJhY2sgPSAocmF3TWVzc2FnZTogc3RyaW5nKSA9PiB7XG4gICAgICAgICAgICBjb25zdCBwYXJzZWQgPSBwYXJzZUluY29taW5nTWVzc2FnZShyYXdNZXNzYWdlKTtcbiAgICAgICAgICAgIGlmIChcImVycm9yXCIgaW4gcGFyc2VkIHx8IHBhcnNlZC5tZXNzYWdlSWQgIT09IEdldE1lc3NhZ2VSZXNwb25zZUlkIHx8ICFwYXJzZWQucGF5bG9hZCkge1xuICAgICAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgbGV0IHBheWxvYWQ6IHVua25vd247XG4gICAgICAgICAgICB0cnkge1xuICAgICAgICAgICAgICAgIHBheWxvYWQgPSBKU09OLnBhcnNlKHBhcnNlZC5wYXlsb2FkKTtcbiAgICAgICAgICAgIH0gY2F0Y2gge1xuICAgICAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgaWYgKCFpc0dldE1lc3NhZ2VSZXNwb25zZVBheWxvYWQocGF5bG9hZCkgfHwgcGF5bG9hZC5yZXF1ZXN0SWQgIT09IHJlcXVlc3RJZCkge1xuICAgICAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgd2luZG93LmNsZWFyVGltZW91dCh0aW1lb3V0KTtcbiAgICAgICAgICAgIHVucmVnaXN0ZXJXZWJNZXNzYWdlUmVjZWl2ZXIocmVzcG9uc2VDYWxsYmFjayk7XG5cbiAgICAgICAgICAgIGlmIChwYXlsb2FkLnN1Y2Nlc3MpIHtcbiAgICAgICAgICAgICAgICByZXNvbHZlKHBheWxvYWQuZGF0YSA/PyBcIlwiKTtcbiAgICAgICAgICAgICAgICByZXR1cm47XG4gICAgICAgICAgICB9XG5cbiAgICAgICAgICAgIHJlamVjdChuZXcgRXJyb3IocGF5bG9hZC5lcnJvciA/PyBcIkhvc3QgZ2V0RGF0YSBmYWlsZWQuXCIpKTtcbiAgICAgICAgfTtcblxuICAgICAgICByZWdpc3RlcldlYk1lc3NhZ2VSZWNlaXZlcihyZXNwb25zZUNhbGxiYWNrLCBleGlzdGluZ1JlY2VpdmVDYWxsYmFjayk7XG4gICAgICAgIGhvc3QucG9zdERhdGE/Lih7XG4gICAgICAgICAgICBpZDogR2V0TWVzc2FnZVJlcXVlc3RJZCxcbiAgICAgICAgICAgIGRhdGE6IHtcbiAgICAgICAgICAgICAgICByZXF1ZXN0SWQsXG4gICAgICAgICAgICAgICAgbWVzc2FnZTogbm9ybWFsaXplZE1lc3NhZ2VcbiAgICAgICAgICAgIH0sXG4gICAgICAgICAgICB2ZXJzaW9uOiBJbnRlcm9wRW52ZWxvcGVWZXJzaW9uXG4gICAgICAgIH0pO1xuICAgIH0pO1xufVxuXG5mdW5jdGlvbiBub3JtYWxpemVHZXRNZXNzYWdlSW5wdXQobWVzc2FnZTogSW50ZXJvcEVudmVsb3BlVjEgfCBzdHJpbmcpOiBJbnRlcm9wRW52ZWxvcGVWMSB8IHN0cmluZyB8IG51bGwge1xuICAgIGlmICh0eXBlb2YgbWVzc2FnZSA9PT0gXCJzdHJpbmdcIikge1xuICAgICAgICBjb25zdCB0cmltbWVkID0gbWVzc2FnZS50cmltKCk7XG4gICAgICAgIGlmICh0cmltbWVkLmxlbmd0aCA9PT0gMCkge1xuICAgICAgICAgICAgcmV0dXJuIG51bGw7XG4gICAgICAgIH1cblxuICAgICAgICByZXR1cm4gdHJpbW1lZDtcbiAgICB9XG5cbiAgICBjb25zdCBub3JtYWxpemVkRW52ZWxvcGUgPSBub3JtYWxpemVFbnZlbG9wZShtZXNzYWdlKTtcbiAgICBpZiAoIW5vcm1hbGl6ZWRFbnZlbG9wZSkge1xuICAgICAgICByZXR1cm4gbnVsbDtcbiAgICB9XG5cbiAgICByZXR1cm4gbm9ybWFsaXplZEVudmVsb3BlO1xufVxuXG5mdW5jdGlvbiBjcmVhdGVSZXF1ZXN0SWQoKTogc3RyaW5nIHtcbiAgICByZXR1cm4gYGlmX3JlcV8ke0RhdGUubm93KCkudG9TdHJpbmcoMzYpfV8ke01hdGgucmFuZG9tKCkudG9TdHJpbmcoMzYpLnNsaWNlKDIsIDEwKX1gO1xufVxuXG5mdW5jdGlvbiBub3JtYWxpemVFbnZlbG9wZShlbnZlbG9wZTogSW50ZXJvcEVudmVsb3BlVjEpOiBJbnRlcm9wRW52ZWxvcGVWMSB8IG51bGwge1xuICAgIGlmICghZW52ZWxvcGUgfHwgdHlwZW9mIGVudmVsb3BlICE9PSBcIm9iamVjdFwiKSB7XG4gICAgICAgIGNvbnNvbGUud2FybihcIkhvc3QgYnJpZGdlIHBheWxvYWQgbXVzdCBiZSBhbiBlbnZlbG9wZSBvYmplY3QuXCIpO1xuICAgICAgICByZXR1cm4gbnVsbDtcbiAgICB9XG5cbiAgICAvLyBub2luc3BlY3Rpb24gU3VzcGljaW91c1R5cGVPZkd1YXJkXG4gICAgaWYgKHR5cGVvZiBlbnZlbG9wZS5pZCAhPT0gXCJzdHJpbmdcIiB8fCBlbnZlbG9wZS5pZC50cmltKCkubGVuZ3RoID09PSAwKSB7XG4gICAgICAgIGNvbnNvbGUud2FybihcIkhvc3QgYnJpZGdlIGVudmVsb3BlIHJlcXVpcmVzIGEgbm9uLWVtcHR5ICdpZCcuXCIpO1xuICAgICAgICByZXR1cm4gbnVsbDtcbiAgICB9XG5cbiAgICBjb25zdCB2ZXJzaW9uID0gTnVtYmVyLmlzSW50ZWdlcihlbnZlbG9wZS52ZXJzaW9uKVxuICAgICAgICA/IGVudmVsb3BlLnZlcnNpb25cbiAgICAgICAgOiBJbnRlcm9wRW52ZWxvcGVWZXJzaW9uO1xuXG4gICAgY29uc3Qgbm9ybWFsaXplZDogSW50ZXJvcEVudmVsb3BlVjEgPSB7XG4gICAgICAgIGlkOiBlbnZlbG9wZS5pZCxcbiAgICAgICAgZGF0YTogZW52ZWxvcGUuZGF0YSxcbiAgICAgICAgdmVyc2lvblxuICAgIH07XG5cbiAgICAvLyBub2luc3BlY3Rpb24gU3VzcGljaW91c1R5cGVPZkd1YXJkXG4gICAgaWYgKGVudmVsb3BlLmNoYW5uZWwgIT09IHVuZGVmaW5lZCAmJiB0eXBlb2YgZW52ZWxvcGUuY2hhbm5lbCA9PT0gXCJzdHJpbmdcIiAmJiBlbnZlbG9wZS5jaGFubmVsLnRyaW0oKS5sZW5ndGggPiAwKSB7XG4gICAgICAgIG5vcm1hbGl6ZWQuY2hhbm5lbCA9IGVudmVsb3BlLmNoYW5uZWw7XG4gICAgfVxuXG4gICAgcmV0dXJuIG5vcm1hbGl6ZWQ7XG59XG5cbmZ1bmN0aW9uIHNlbmRWaWFQbGF0Zm9ybVRyYW5zcG9ydChtZXNzYWdlOiBzdHJpbmcpOiB2b2lkIHtcbiAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xuICAgICAgICB3aW5kb3cuY2hyb21lLndlYnZpZXcucG9zdE1lc3NhZ2UobWVzc2FnZSk7XG4gICAgICAgIHJldHVybjtcbiAgICB9XG5cbiAgICBjb25zb2xlLndhcm4oXCJNZXNzYWdlIHRvIGhvc3QgZmFpbGVkLiBObyBzdXBwb3J0ZWQgaG9zdCB0cmFuc3BvcnQgd2FzIGZvdW5kLlwiKTtcbn1cblxuZnVuY3Rpb24gcmVnaXN0ZXJXZWJNZXNzYWdlUmVjZWl2ZXIoXG4gICAgY2FsbGJhY2s6IChtZXNzYWdlOiBzdHJpbmcpID0+IHZvaWQsXG4gICAgZXhpc3RpbmdSZWNlaXZlQ2FsbGJhY2s/OiAoY2FsbGJhY2s6IChtZXNzYWdlOiBzdHJpbmcpID0+IHZvaWQpID0+IHZvaWRcbik6IHZvaWQge1xuICAgIHJlY2VpdmVDYWxsYmFja3MuYWRkKGNhbGxiYWNrKTtcbiAgICBhdHRhY2hSZWNlaXZlQnJpZGdlT25jZShleGlzdGluZ1JlY2VpdmVDYWxsYmFjayk7XG59XG5cbmZ1bmN0aW9uIHVucmVnaXN0ZXJXZWJNZXNzYWdlUmVjZWl2ZXIoY2FsbGJhY2s6IFJlY2VpdmVDYWxsYmFjayk6IHZvaWQge1xuICAgIHJlY2VpdmVDYWxsYmFja3MuZGVsZXRlKGNhbGxiYWNrKTtcbn1cblxuZnVuY3Rpb24gYXR0YWNoUmVjZWl2ZUJyaWRnZU9uY2UoZXhpc3RpbmdSZWNlaXZlQ2FsbGJhY2s/OiAoY2FsbGJhY2s6IChtZXNzYWdlOiBzdHJpbmcpID0+IHZvaWQpID0+IHZvaWQpOiB2b2lkIHtcbiAgICBpZiAocmVjZWl2ZUJyaWRnZUF0dGFjaGVkKSB7XG4gICAgICAgIHJldHVybjtcbiAgICB9XG5cbiAgICBjb25zdCBkaXNwYXRjaCA9IChtZXNzYWdlOiBzdHJpbmcpID0+IHtcbiAgICAgICAgZm9yIChjb25zdCBjYWxsYmFjayBvZiByZWNlaXZlQ2FsbGJhY2tzKSB7XG4gICAgICAgICAgICBjYWxsYmFjayhtZXNzYWdlKTtcbiAgICAgICAgfVxuICAgIH07XG5cbiAgICBpZiAoZXhpc3RpbmdSZWNlaXZlQ2FsbGJhY2spIHtcbiAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgIGV4aXN0aW5nUmVjZWl2ZUNhbGxiYWNrKGRpc3BhdGNoKTtcbiAgICAgICAgICAgIHJlY2VpdmVCcmlkZ2VBdHRhY2hlZCA9IHRydWU7XG4gICAgICAgICAgICByZXR1cm47XG4gICAgICAgIH0gY2F0Y2ggKGVycm9yKSB7XG4gICAgICAgICAgICBjb25zb2xlLndhcm4oXCJFeGlzdGluZyBJbmZpbmlGcmFtZSBob3N0IHJlY2VpdmUgYnJpZGdlIGZhaWxlZC4gRmFsbGluZyBiYWNrIHRvIHBsYXRmb3JtIGFkYXB0ZXJzLlwiLCBlcnJvcik7XG4gICAgICAgIH1cbiAgICB9XG5cbiAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xuICAgICAgICB3aW5kb3cuY2hyb21lLndlYnZpZXcuYWRkRXZlbnRMaXN0ZW5lcihcIm1lc3NhZ2VcIiwgKGV2ZW50KSA9PiB7XG4gICAgICAgICAgICBkaXNwYXRjaChldmVudC5kYXRhKTtcbiAgICAgICAgfSk7XG4gICAgICAgIHJlY2VpdmVCcmlkZ2VBdHRhY2hlZCA9IHRydWU7XG4gICAgICAgIHJldHVybjtcbiAgICB9XG5cbiAgICBjb25zb2xlLndhcm4oXCJSZWNlaXZlIG1lc3NhZ2UgcmVnaXN0cmF0aW9uIGZhaWxlZC4gTm8gc3VwcG9ydGVkIGhvc3QgcmVjZWl2ZSB0cmFuc3BvcnQgd2FzIGZvdW5kLlwiKTtcbn1cblxuZnVuY3Rpb24gaXNPYmplY3QodmFsdWU6IHVua25vd24pOiB2YWx1ZSBpcyBSZWNvcmQ8c3RyaW5nLCB1bmtub3duPiB7XG4gICAgcmV0dXJuIHR5cGVvZiB2YWx1ZSA9PT0gXCJvYmplY3RcIiAmJiB2YWx1ZSAhPT0gbnVsbDtcbn1cblxuZnVuY3Rpb24gaXNHZXRNZXNzYWdlUmVzcG9uc2VQYXlsb2FkKHZhbHVlOiB1bmtub3duKTogdmFsdWUgaXMge1xuICAgIHJlcXVlc3RJZDogc3RyaW5nO1xuICAgIHN1Y2Nlc3M6IGJvb2xlYW47XG4gICAgZGF0YT86IHN0cmluZztcbiAgICBlcnJvcj86IHN0cmluZztcbn0ge1xuICAgIHJldHVybiBpc09iamVjdCh2YWx1ZSlcbiAgICAgICAgJiYgdHlwZW9mIHZhbHVlLnJlcXVlc3RJZCA9PT0gXCJzdHJpbmdcIlxuICAgICAgICAmJiB0eXBlb2YgdmFsdWUuc3VjY2VzcyA9PT0gXCJib29sZWFuXCJcbiAgICAgICAgJiYgKHZhbHVlLmRhdGEgPT09IHVuZGVmaW5lZCB8fCB0eXBlb2YgdmFsdWUuZGF0YSA9PT0gXCJzdHJpbmdcIilcbiAgICAgICAgJiYgKHZhbHVlLmVycm9yID09PSB1bmRlZmluZWQgfHwgdHlwZW9mIHZhbHVlLmVycm9yID09PSBcInN0cmluZ1wiKTtcbn1cbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1NlbmRUb0hvc3RNZXNzYWdlSWRzfSBmcm9tIFwiLi4vQ29udHJhY3RzXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZnVuY3Rpb24gaXNFeHRlcm5hbExpbmsodXJsOiBzdHJpbmcpOiBib29sZWFuIHtcclxuICAgIHRyeSB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBVUkwodXJsLCBsb2NhdGlvbi5ocmVmKS5ob3N0bmFtZSAhPT0gbG9jYXRpb24uaG9zdG5hbWU7XHJcbiAgICB9IGNhdGNoIHtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBhc3luYyBmdW5jdGlvbiBibGFua1RhcmdldEhhbmRsZXIoZTogTW91c2VFdmVudCkge1xyXG4gICAgbGV0IGVsID0gZS50YXJnZXQgYXMgSFRNTEVsZW1lbnQgfCBudWxsO1xyXG5cclxuICAgIHdoaWxlIChlbCAmJiBlbCAhPT0gZG9jdW1lbnQuYm9keSkge1xyXG4gICAgICAgIGlmIChlbC50YWdOYW1lPy50b0xvd2VyQ2FzZSgpICE9PSBcImFcIikge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgYW5jaG9yID0gZWwgYXMgSFRNTEFuY2hvckVsZW1lbnQ7XHJcbiAgICAgICAgaWYgKCFhbmNob3IuaHJlZikge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgdGFyZ2V0ID0gYW5jaG9yLmdldEF0dHJpYnV0ZShcInRhcmdldFwiKTtcclxuICAgICAgICBjb25zdCBzaG91bGRIYW5kbGUgPSB0YXJnZXQgPT09IFwiX2JsYW5rXCIgfHwgYW5jaG9yLmhhc0F0dHJpYnV0ZShcImRhdGEtZXh0ZXJuYWxcIikgfHwgaXNFeHRlcm5hbExpbmsoYW5jaG9yLmhyZWYpO1xyXG5cclxuICAgICAgICBpZiAoIXNob3VsZEhhbmRsZSkge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG4gICAgICAgIHdpbmRvdy5pbmZpbmlmcmFtZS5tZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMub3BlbkV4dGVybmFsTGluaywgYW5jaG9yLmhyZWYpO1xyXG4gICAgICAgIHJldHVybjtcclxuICAgIH1cclxufSIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1NlbmRUb0hvc3RNZXNzYWdlSWRzfSBmcm9tIFwiLi4vQ29udHJhY3RzXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGZ1bmN0aW9uIGdldFRpdGxlT2JzZXJ2ZXJUYXJnZXQoKTogSFRNTFRpdGxlRWxlbWVudCB8IG51bGwge1xyXG4gICAgcmV0dXJuIGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IoJ3RpdGxlJyk7XHJcbn1cclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBnZXRUaXRsZU9ic2VydmVyKCk6IE11dGF0aW9uT2JzZXJ2ZXIge1xyXG4gICAgcmV0dXJuIG5ldyBNdXRhdGlvbk9ic2VydmVyKChtdXRhdGlvbnMsIF8pID0+IHtcclxuICAgICAgICBtdXRhdGlvbnMuZm9yRWFjaCgobXV0YXRpb24pID0+IHtcclxuICAgICAgICAgICAgaWYgKG11dGF0aW9uLnR5cGUgIT09IFwiY2hpbGRMaXN0XCIpIHJldHVybjtcclxuICAgICAgICAgICAgd2luZG93LmluZmluaWZyYW1lLm1lc3NhZ2luZy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy50aXRsZUNoYW5nZSwgZG9jdW1lbnQudGl0bGUpO1xyXG4gICAgICAgIH0pXHJcbiAgICB9KVxyXG59XHJcbiIsIi8vIFRoZSBtb2R1bGUgY2FjaGVcbnZhciBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX18gPSB7fTtcblxuLy8gVGhlIHJlcXVpcmUgZnVuY3Rpb25cbmZ1bmN0aW9uIF9fd2VicGFja19yZXF1aXJlX18obW9kdWxlSWQpIHtcblx0Ly8gQ2hlY2sgaWYgbW9kdWxlIGlzIGluIGNhY2hlXG5cdHZhciBjYWNoZWRNb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdO1xuXHRpZiAoY2FjaGVkTW9kdWxlICE9PSB1bmRlZmluZWQpIHtcblx0XHRyZXR1cm4gY2FjaGVkTW9kdWxlLmV4cG9ydHM7XG5cdH1cblx0Ly8gQ3JlYXRlIGEgbmV3IG1vZHVsZSAoYW5kIHB1dCBpdCBpbnRvIHRoZSBjYWNoZSlcblx0dmFyIG1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF0gPSB7XG5cdFx0Ly8gbm8gbW9kdWxlLmlkIG5lZWRlZFxuXHRcdC8vIG5vIG1vZHVsZS5sb2FkZWQgbmVlZGVkXG5cdFx0ZXhwb3J0czoge31cblx0fTtcblxuXHQvLyBFeGVjdXRlIHRoZSBtb2R1bGUgZnVuY3Rpb25cblx0aWYgKCEobW9kdWxlSWQgaW4gX193ZWJwYWNrX21vZHVsZXNfXykpIHtcblx0XHRkZWxldGUgX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXTtcblx0XHR2YXIgZSA9IG5ldyBFcnJvcihcIkNhbm5vdCBmaW5kIG1vZHVsZSAnXCIgKyBtb2R1bGVJZCArIFwiJ1wiKTtcblx0XHRlLmNvZGUgPSAnTU9EVUxFX05PVF9GT1VORCc7XG5cdFx0dGhyb3cgZTtcblx0fVxuXHRfX3dlYnBhY2tfbW9kdWxlc19fW21vZHVsZUlkXS5jYWxsKG1vZHVsZS5leHBvcnRzLCBtb2R1bGUsIG1vZHVsZS5leHBvcnRzLCBfX3dlYnBhY2tfcmVxdWlyZV9fKTtcblxuXHQvLyBSZXR1cm4gdGhlIGV4cG9ydHMgb2YgdGhlIG1vZHVsZVxuXHRyZXR1cm4gbW9kdWxlLmV4cG9ydHM7XG59XG5cbiIsIiIsIi8vIHN0YXJ0dXBcbi8vIExvYWQgZW50cnkgbW9kdWxlIGFuZCByZXR1cm4gZXhwb3J0c1xuLy8gVGhpcyBlbnRyeSBtb2R1bGUgaXMgcmVmZXJlbmNlZCBieSBvdGhlciBtb2R1bGVzIHNvIGl0IGNhbid0IGJlIGlubGluZWRcbnZhciBfX3dlYnBhY2tfZXhwb3J0c19fID0gX193ZWJwYWNrX3JlcXVpcmVfXyhcIi4vVHlwZVNjcmlwdC9JbmRleC50c1wiKTtcbiIsIiJdLCJuYW1lcyI6W10sInNvdXJjZVJvb3QiOiIifQ==