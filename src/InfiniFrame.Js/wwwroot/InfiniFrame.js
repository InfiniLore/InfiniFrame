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
const infiniFrame = "__infiniframe";
exports.SendToHostMessageIds = {
    titleChange: `${infiniFrame}:title:change`,
    fullscreenEnter: `${infiniFrame}:fullscreen:enter`,
    fullscreenExit: `${infiniFrame}:fullscreen:exit`,
    openExternalLink: `${infiniFrame}:open:external`,
    windowClose: `${infiniFrame}:window:close`,
    ready: `${infiniFrame}:ready`,
    getMessageRequest: `${infiniFrame}:get:request`,
};
exports.ReceiveFromHostMessageIds = {
    registerOpenExternal: `${infiniFrame}:register:open:external`,
    registerFullscreenChange: `${infiniFrame}:register:fullscreen:change`,
    registerTitleChange: `${infiniFrame}:register:title:change`,
    registerWindowClose: `${infiniFrame}:register:window:close`,
    getMessageResponse: `${infiniFrame}:get:response`,
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
window.infiniFrame = new InfiniFrame_1.default();


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
        this.hostMessaging = new InfiniFrameHostMessaging_1.default();
        this.hostWindow = new InfiniFrameWindow_1.InfiniFrameWindow();
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
        if ((_b = (_a = window.infiniframe) === null || _a === void 0 ? void 0 : _a.host) === null || _b === void 0 ? void 0 : _b.postData) {
            window.infiniframe.host.postData(envelope);
        }
        else {
            console.warn("Message to host failed. Host bridge API is not initialized.");
            return;
        }
    }
    getMessageFromHostAsync(message) {
        return __awaiter(this, void 0, void 0, function* () {
            var _a;
            const host = (_a = window.infiniframe) === null || _a === void 0 ? void 0 : _a.host;
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
        if ((_b = (_a = window.infiniframe) === null || _a === void 0 ? void 0 : _a.host) === null || _b === void 0 ? void 0 : _b.receiveCallback) {
            window.infiniframe.host.receiveCallback((message) => {
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
        window.infiniFrame.hostMessaging.sendMessageToHost(Contracts_1.SendToHostMessageIds.titleChange, title);
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
    const root = (_a = window.infiniframe) !== null && _a !== void 0 ? _a : {};
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
    window.infiniframe = root;
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
            window.infiniFrame.hostMessaging.sendMessageToHost(Contracts_1.SendToHostMessageIds.openExternalLink, anchor.href);
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
            window.infiniFrame.hostMessaging.sendMessageToHost(Contracts_1.SendToHostMessageIds.titleChange, document.title);
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
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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QUNBO1VBQ0E7VUFDQTs7VUFFQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBOztVQUVBO1VBQ0E7VUFDQTs7OztVRTVCQTtVQUNBO1VBQ0E7VUFDQSIsInNvdXJjZXMiOlsid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvLi9UeXBlU2NyaXB0L0NvbnRyYWN0cy9JSW5maW5pRnJhbWVIb3N0TWVzc2FnaW5nLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvLi9UeXBlU2NyaXB0L0NvbnRyYWN0cy9pbmRleC50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLmpzLWJ1aWxkLy4vVHlwZVNjcmlwdC9JbmRleC50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLmpzLWJ1aWxkLy4vVHlwZVNjcmlwdC9JbmZpbmlGcmFtZS50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLmpzLWJ1aWxkLy4vVHlwZVNjcmlwdC9JbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS5qcy1idWlsZC8uL1R5cGVTY3JpcHQvSW5maW5pRnJhbWVVdGlscy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLmpzLWJ1aWxkLy4vVHlwZVNjcmlwdC9JbmZpbmlGcmFtZVdpbmRvdy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLmpzLWJ1aWxkLy4vVHlwZVNjcmlwdC9JbnRlcm9wL0VudmVsb3BlUHJvdG9jb2wvSW50ZXJvcEVudmVsb3BlUHJvdG9jb2wudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS5qcy1idWlsZC8uL1R5cGVTY3JpcHQvSW50ZXJvcC9OYXRpdmVIb3N0L0hvc3RCcmlkZ2UudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS5qcy1idWlsZC8uL1R5cGVTY3JpcHQvVXRpbHMvQmxhbmtUYXJnZXRIYW5kbGVyLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvLi9UeXBlU2NyaXB0L1V0aWxzL09ic2VydmVycy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLmpzLWJ1aWxkL3dlYnBhY2svYm9vdHN0cmFwIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUuanMtYnVpbGQvd2VicGFjay9iZWZvcmUtc3RhcnR1cCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLmpzLWJ1aWxkL3dlYnBhY2svc3RhcnR1cCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLmpzLWJ1aWxkL3dlYnBhY2svYWZ0ZXItc3RhcnR1cCJdLCJzb3VyY2VzQ29udGVudCI6WyIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cbi8vIEltcG9ydHNcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxuaW1wb3J0IHtJbnRlcm9wRW52ZWxvcGVWMX0gZnJvbSBcIi4vRW52ZWxvcGVQcm90b2NvbFwiO1xuXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cbi8vIENvZGVcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxuY29uc3QgaW5maW5pRnJhbWU6IHN0cmluZyA9IFwiX19pbmZpbmlmcmFtZVwiO1xuXG5leHBvcnQgY29uc3QgU2VuZFRvSG9zdE1lc3NhZ2VJZHMgPSB7XG4gICAgdGl0bGVDaGFuZ2U6IGAke2luZmluaUZyYW1lfTp0aXRsZTpjaGFuZ2VgLFxuICAgIGZ1bGxzY3JlZW5FbnRlcjogYCR7aW5maW5pRnJhbWV9OmZ1bGxzY3JlZW46ZW50ZXJgLFxuICAgIGZ1bGxzY3JlZW5FeGl0OiBgJHtpbmZpbmlGcmFtZX06ZnVsbHNjcmVlbjpleGl0YCxcbiAgICBvcGVuRXh0ZXJuYWxMaW5rOiBgJHtpbmZpbmlGcmFtZX06b3BlbjpleHRlcm5hbGAsXG4gICAgd2luZG93Q2xvc2U6IGAke2luZmluaUZyYW1lfTp3aW5kb3c6Y2xvc2VgLFxuICAgIHJlYWR5OiBgJHtpbmZpbmlGcmFtZX06cmVhZHlgLFxuICAgIGdldE1lc3NhZ2VSZXF1ZXN0OiBgJHtpbmZpbmlGcmFtZX06Z2V0OnJlcXVlc3RgLFxufVxuXG5leHBvcnQgY29uc3QgUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcyA9IHtcbiAgICByZWdpc3Rlck9wZW5FeHRlcm5hbDogYCR7aW5maW5pRnJhbWV9OnJlZ2lzdGVyOm9wZW46ZXh0ZXJuYWxgLFxuICAgIHJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZTogYCR7aW5maW5pRnJhbWV9OnJlZ2lzdGVyOmZ1bGxzY3JlZW46Y2hhbmdlYCxcbiAgICByZWdpc3RlclRpdGxlQ2hhbmdlOiBgJHtpbmZpbmlGcmFtZX06cmVnaXN0ZXI6dGl0bGU6Y2hhbmdlYCxcbiAgICByZWdpc3RlcldpbmRvd0Nsb3NlOiBgJHtpbmZpbmlGcmFtZX06cmVnaXN0ZXI6d2luZG93OmNsb3NlYCxcbiAgICBnZXRNZXNzYWdlUmVzcG9uc2U6IGAke2luZmluaUZyYW1lfTpnZXQ6cmVzcG9uc2VgLFxufVxuXG5leHBvcnQgdHlwZSBTZW5kVG9Ib3N0TWVzc2FnZUlkID0gdHlwZW9mIFNlbmRUb0hvc3RNZXNzYWdlSWRzW2tleW9mIHR5cGVvZiBTZW5kVG9Ib3N0TWVzc2FnZUlkc107XG5leHBvcnQgdHlwZSBNZXNzYWdlQ2FsbGJhY2sgPSAoZGF0YT86IHN0cmluZykgPT4gdm9pZDtcblxuZXhwb3J0IGludGVyZmFjZSBJSW5maW5pRnJhbWVIb3N0TWVzc2FnaW5nIHtcbiAgICBzZW5kTWVzc2FnZVRvSG9zdChpZDogU2VuZFRvSG9zdE1lc3NhZ2VJZCB8IHN0cmluZywgZGF0YT86IHVua25vd24pOiB2b2lkO1xuICAgIGdldE1lc3NhZ2VGcm9tSG9zdEFzeW5jKG1lc3NhZ2U6IEludGVyb3BFbnZlbG9wZVYxIHwgc3RyaW5nKTogUHJvbWlzZTxzdHJpbmc+O1xuXG4gICAgYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZywgY2FsbGJhY2s6IE1lc3NhZ2VDYWxsYmFjayk6IHZvaWQ7XG5cbiAgICB1bnJlZ2lzdGVyTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZyk6IHZvaWQ7XG59XG4iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gRXhwb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0ICogZnJvbSBcIi4vRW52ZWxvcGVQcm90b2NvbFwiO1xyXG5leHBvcnQgKiBmcm9tIFwiLi9nbG9iYWxcIjtcclxuZXhwb3J0ICogZnJvbSBcIi4vSUluZmluaUZyYW1lXCI7XHJcbmV4cG9ydCAqIGZyb20gXCIuL0lJbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmdcIjtcclxuZXhwb3J0ICogZnJvbSBcIi4vSUluZmluaUZyYW1lVXRpbHNcIjtcclxuZXhwb3J0ICogZnJvbSBcIi4vSUluZmluaUZyYW1lV2luZG93XCI7IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCBJbmZpbmlGcmFtZSBmcm9tIFwiLi9JbmZpbmlGcmFtZVwiO1xyXG5pbXBvcnQge2luc3RhbGxIb3N0QnJpZGdlfSBmcm9tIFwiLi9JbnRlcm9wL05hdGl2ZUhvc3QvSG9zdEJyaWRnZVwiO1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IHt9O1xyXG5pbnN0YWxsSG9zdEJyaWRnZSgpO1xyXG5cclxud2luZG93LmluZmluaUZyYW1lID0gbmV3IEluZmluaUZyYW1lKCk7XHJcbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge0lJbmZpbmlGcmFtZSwgSUluZmluaUZyYW1lSG9zdE1lc3NhZ2luZywgSUluZmluaUZyYW1lVXRpbHMsIElJbmZpbmlGcmFtZVdpbmRvd30gZnJvbSBcIi4vQ29udHJhY3RzXCI7XHJcbmltcG9ydCBJbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmcgZnJvbSBcIi4vSW5maW5pRnJhbWVIb3N0TWVzc2FnaW5nXCI7XHJcbmltcG9ydCB7SW5maW5pRnJhbWVVdGlsc30gZnJvbSBcIi4vSW5maW5pRnJhbWVVdGlsc1wiO1xyXG5pbXBvcnQge0luZmluaUZyYW1lV2luZG93fSBmcm9tIFwiLi9JbmZpbmlGcmFtZVdpbmRvd1wiO1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGNsYXNzIEluZmluaUZyYW1lIGltcGxlbWVudHMgSUluZmluaUZyYW1lIHtcclxuICAgIGhvc3RNZXNzYWdpbmc6IElJbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmcgPSBuZXcgSW5maW5pRnJhbWVIb3N0TWVzc2FnaW5nKCk7XHJcbiAgICBob3N0V2luZG93OiBJSW5maW5pRnJhbWVXaW5kb3cgPSBuZXcgSW5maW5pRnJhbWVXaW5kb3coKTtcclxuICAgIFxyXG4gICAgdXRpbHM6IElJbmZpbmlGcmFtZVV0aWxzID0gbmV3IEluZmluaUZyYW1lVXRpbHMoKSAgICBcclxufVxyXG5cclxuZXhwb3J0IGRlZmF1bHQgSW5maW5pRnJhbWVcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7XHJcbiAgICBJSW5maW5pRnJhbWVIb3N0TWVzc2FnaW5nLFxyXG4gICAgSW50ZXJvcEVudmVsb3BlVjEsXHJcbiAgICBNZXNzYWdlQ2FsbGJhY2ssXHJcbiAgICBSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLFxyXG4gICAgU2VuZFRvSG9zdE1lc3NhZ2VJZCxcclxuICAgIFNlbmRUb0hvc3RNZXNzYWdlSWRzXHJcbn0gZnJvbSBcIi4vQ29udHJhY3RzXCI7XHJcbmltcG9ydCB7Y3JlYXRlRW52ZWxvcGUsIHBhcnNlSW5jb21pbmdNZXNzYWdlfSBmcm9tIFwiLi9JbnRlcm9wL0VudmVsb3BlUHJvdG9jb2wvSW50ZXJvcEVudmVsb3BlUHJvdG9jb2xcIjtcclxuaW1wb3J0IHtibGFua1RhcmdldEhhbmRsZXJ9IGZyb20gXCIuL1V0aWxzL0JsYW5rVGFyZ2V0SGFuZGxlclwiO1xyXG5pbXBvcnQge2dldFRpdGxlT2JzZXJ2ZXIsIGdldFRpdGxlT2JzZXJ2ZXJUYXJnZXR9IGZyb20gXCIuL1V0aWxzL09ic2VydmVyc1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmNsYXNzIEluZmluaUZyYW1lSG9zdE1lc3NhZ2luZyBpbXBsZW1lbnRzIElJbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmcge1xyXG4gICAgcHJpdmF0ZSBzdGF0aWMgcmVhZG9ubHkgQmxhem9yV2ViVmlld01lc3NhZ2VQcmVmaXggPSBcIl9fYnd2OlwiO1xyXG4gICAgcHJpdmF0ZSBzdGF0aWMgcmVhZG9ubHkgUmVhZHlIYW5kc2hha2VSZXRyeUludGVydmFsTXMgPSAxMDAwO1xyXG4gICAgcHJpdmF0ZSBzdGF0aWMgcmVhZG9ubHkgTWF4UmVhZHlIYW5kc2hha2VBdHRlbXB0cyA9IDIwO1xyXG4gICAgcHJpdmF0ZSBtZXNzYWdlSGFuZGxlcnM6IE1hcDxzdHJpbmcsIE1lc3NhZ2VDYWxsYmFjaz4gPSBuZXcgTWFwKCk7XHJcbiAgICBwcml2YXRlIG9wZW5FeHRlcm5hbFJlZ2lzdGVyZWQgPSBmYWxzZTtcclxuICAgIHByaXZhdGUgZnVsbHNjcmVlblJlZ2lzdGVyZWQgPSBmYWxzZTtcclxuICAgIHByaXZhdGUgdGl0bGVSZWdpc3RlcmVkID0gZmFsc2U7XHJcbiAgICBwcml2YXRlIHdpbmRvd0Nsb3NlUmVnaXN0ZXJlZCA9IGZhbHNlO1xyXG4gICAgcHJpdmF0ZSByZWFkeUhhbmRzaGFrZUF0dGVtcHRzID0gMDtcclxuICAgIHByaXZhdGUgcmVhZHlIYW5kc2hha2VBY2tub3dsZWRnZWQgPSBmYWxzZTtcclxuICAgIHByaXZhdGUgcmVhZHlIYW5kc2hha2VSZXRyeVRpbWVyOiBudW1iZXIgfCBudWxsID0gbnVsbDtcclxuICAgIFxyXG4gICAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAgICAgdGhpcy5hc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKTtcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJPcGVuRXh0ZXJuYWwsIF8gPT4ge1xyXG4gICAgICAgICAgICB0aGlzLm1hcmtSZWFkeUhhbmRzaGFrZUFja25vd2xlZGdlZCgpO1xyXG4gICAgICAgICAgICB0aGlzLnJlZ2lzdGVyT3BlbkV4dGVybmFsKCk7XHJcbiAgICAgICAgfSlcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJGdWxsc2NyZWVuQ2hhbmdlLCBfID0+IHtcclxuICAgICAgICAgICAgdGhpcy5tYXJrUmVhZHlIYW5kc2hha2VBY2tub3dsZWRnZWQoKTtcclxuICAgICAgICAgICAgdGhpcy5yZWdpc3RlckZ1bGxzY3JlZW5DaGFuZ2UoKTtcclxuICAgICAgICB9KVxyXG5cclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3RlclRpdGxlQ2hhbmdlLCBfID0+IHtcclxuICAgICAgICAgICAgdGhpcy5tYXJrUmVhZHlIYW5kc2hha2VBY2tub3dsZWRnZWQoKTtcclxuICAgICAgICAgICAgdGhpcy5yZWdpc3RlclRpdGxlQ2hhbmdlKCk7XHJcbiAgICAgICAgfSlcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJXaW5kb3dDbG9zZSwgXyA9PiB7XHJcbiAgICAgICAgICAgIHRoaXMubWFya1JlYWR5SGFuZHNoYWtlQWNrbm93bGVkZ2VkKCk7XHJcbiAgICAgICAgICAgIHRoaXMucmVnaXN0ZXJXaW5kb3dDbG9zZSgpO1xyXG4gICAgICAgIH0pXHJcblxyXG4gICAgICAgIHRoaXMuc2VuZFJlYWR5SGFuZHNoYWtlV2l0aFJldHJ5KCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkIHwgc3RyaW5nLCBkYXRhPzogdW5rbm93bikge1xyXG4gICAgICAgIGNvbnN0IGVudmVsb3BlID0gY3JlYXRlRW52ZWxvcGUoaWQsIGRhdGEpO1xyXG5cclxuICAgICAgICBpZiAod2luZG93LmluZmluaWZyYW1lPy5ob3N0Py5wb3N0RGF0YSkge1xyXG4gICAgICAgICAgICB3aW5kb3cuaW5maW5pZnJhbWUuaG9zdC5wb3N0RGF0YShlbnZlbG9wZSk7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgY29uc29sZS53YXJuKFwiTWVzc2FnZSB0byBob3N0IGZhaWxlZC4gSG9zdCBicmlkZ2UgQVBJIGlzIG5vdCBpbml0aWFsaXplZC5cIik7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbiAgICBcclxuICAgIHB1YmxpYyBhc3luYyBnZXRNZXNzYWdlRnJvbUhvc3RBc3luYyhtZXNzYWdlOiBJbnRlcm9wRW52ZWxvcGVWMSB8IHN0cmluZyk6IFByb21pc2U8c3RyaW5nPiB7XHJcbiAgICAgICAgY29uc3QgaG9zdCA9IHdpbmRvdy5pbmZpbmlmcmFtZT8uaG9zdDtcclxuICAgICAgICBpZiAoIWhvc3Q/LmdldERhdGEpIHRocm93IG5ldyBFcnJvcihcIk1lc3NhZ2UgdG8gaG9zdCBmYWlsZWQuIEhvc3QgZ2V0RGF0YSBBUEkgaXMgbm90IGluaXRpYWxpemVkLlwiKTtcclxuXHJcbiAgICAgICAgY29uc3QgZW52ZWxvcGUgPSB0eXBlb2YgbWVzc2FnZSA9PT0gXCJzdHJpbmdcIlxyXG4gICAgICAgICAgICA/IGNyZWF0ZUVudmVsb3BlKG1lc3NhZ2UpXHJcbiAgICAgICAgICAgIDogbWVzc2FnZTtcclxuXHJcbiAgICAgICAgcmV0dXJuIGF3YWl0IGhvc3QuZ2V0RGF0YShlbnZlbG9wZSk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIobWVzc2FnZUlkOiBzdHJpbmcsIGNhbGxiYWNrOiBNZXNzYWdlQ2FsbGJhY2spIHtcclxuICAgICAgICB0aGlzLm1lc3NhZ2VIYW5kbGVycy5zZXQobWVzc2FnZUlkLCBjYWxsYmFjayk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHVucmVnaXN0ZXJNZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nKSB7XHJcbiAgICAgICAgdGhpcy5tZXNzYWdlSGFuZGxlcnMuZGVsZXRlKG1lc3NhZ2VJZCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBhc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKSB7XHJcbiAgICAgICAgaWYgKHdpbmRvdy5pbmZpbmlmcmFtZT8uaG9zdD8ucmVjZWl2ZUNhbGxiYWNrKSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5pbmZpbmlmcmFtZS5ob3N0LnJlY2VpdmVDYWxsYmFjaygobWVzc2FnZTogc3RyaW5nKSA9PiB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLmhhbmRsZUludGVyb3BNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUud2FybihcIldlYiBtZXNzYWdlIHJlY2VpdmVyIGZhaWxlZC4gSG9zdCBicmlkZ2UgQVBJIGlzIG5vdCBpbml0aWFsaXplZC5cIik7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBoYW5kbGVJbnRlcm9wTWVzc2FnZShtZXNzYWdlOiBhbnkpOiBib29sZWFuIHtcclxuICAgICAgICBpZiAodHlwZW9mIG1lc3NhZ2UgIT09ICdzdHJpbmcnKSByZXR1cm4gZmFsc2U7XHJcbiAgICAgICAgaWYgKCFtZXNzYWdlKSByZXR1cm4gZmFsc2U7XHJcbiAgICAgICAgLy8gUm91dGUgb25seSBtZXNzYWdlcyB0aGF0IG1hdGNoIHRoZSBleHBsaWNpdCBpbnRlcm9wIGVudmVsb3BlIGNvbnRyYWN0LlxyXG4gICAgICAgIGNvbnN0IHBhcnNlZE1lc3NhZ2UgPSBwYXJzZUluY29taW5nTWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICBpZiAoXCJlcnJvclwiIGluIHBhcnNlZE1lc3NhZ2UpIHJldHVybiBmYWxzZTtcclxuXHJcbiAgICAgICAgLy8gQmxhem9yIFdlYlZpZXcgaW50ZXJuYWwgdHJhbnNwb3J0IG1lc3NhZ2VzIGFyZSByb3V0ZWQgYnkgYmxhem9yLndlYnZpZXcuanMuXHJcbiAgICAgICAgLy8gVGhleSBhcmUgbm90IEluZmluaUZyYW1lIGhvc3QtbWVzc2FnZSBjb250cmFjdHMgYW5kIHNob3VsZCBub3QgZW1pdCB3YXJuaW5ncy5cclxuICAgICAgICBpZiAocGFyc2VkTWVzc2FnZS5tZXNzYWdlSWQuc3RhcnRzV2l0aChJbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmcuQmxhem9yV2ViVmlld01lc3NhZ2VQcmVmaXgpKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB0cnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gRXhlY3V0ZSByZWdpc3RlcmVkIGhhbmRsZXJcclxuICAgICAgICBjb25zdCBoYW5kbGVyID0gdGhpcy5tZXNzYWdlSGFuZGxlcnMuZ2V0KHBhcnNlZE1lc3NhZ2UubWVzc2FnZUlkKTtcclxuICAgICAgICBpZiAoIWhhbmRsZXIpIHtcclxuICAgICAgICAgICAgY29uc29sZS53YXJuKCdObyBoYW5kbGVyIHJlZ2lzdGVyZWQgZm9yIG1lc3NhZ2U6JywgcGFyc2VkTWVzc2FnZSk7XHJcbiAgICAgICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGhhbmRsZXIocGFyc2VkTWVzc2FnZS5wYXlsb2FkKTtcclxuICAgICAgICByZXR1cm4gdHJ1ZTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHJlZ2lzdGVyT3BlbkV4dGVybmFsKCkge1xyXG4gICAgICAgIGlmICh0aGlzLm9wZW5FeHRlcm5hbFJlZ2lzdGVyZWQpIHJldHVybjtcclxuICAgICAgICB0aGlzLm9wZW5FeHRlcm5hbFJlZ2lzdGVyZWQgPSB0cnVlO1xyXG4gICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJjbGlja1wiLCBibGFua1RhcmdldEhhbmRsZXIsIHtjYXB0dXJlOiB0cnVlfSk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWdpc3RlckZ1bGxzY3JlZW5DaGFuZ2UoKSB7XHJcbiAgICAgICAgaWYgKHRoaXMuZnVsbHNjcmVlblJlZ2lzdGVyZWQpIHJldHVybjtcclxuICAgICAgICB0aGlzLmZ1bGxzY3JlZW5SZWdpc3RlcmVkID0gdHJ1ZTtcclxuICAgICAgICBkb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwiZnVsbHNjcmVlbmNoYW5nZVwiLCAoXzogRXZlbnQpID0+IHtcclxuICAgICAgICAgICAgaWYgKGRvY3VtZW50LmZ1bGxzY3JlZW5FbGVtZW50KSB0aGlzLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLmZ1bGxzY3JlZW5FbnRlcik7XHJcbiAgICAgICAgICAgIGVsc2UgdGhpcy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy5mdWxsc2NyZWVuRXhpdCk7XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJrZXlkb3duXCIsIGFzeW5jIChlOiBLZXlib2FyZEV2ZW50KSA9PiB7XHJcbiAgICAgICAgICAgIGlmIChlLmtleSAhPT0gXCJGMTFcIikgcmV0dXJuO1xyXG4gICAgICAgICAgICBpZiAoZG9jdW1lbnQuZnVsbHNjcmVlbkVsZW1lbnQpIGF3YWl0IGRvY3VtZW50LmV4aXRGdWxsc2NyZWVuKCk7XHJcbiAgICAgICAgICAgIGVsc2UgYXdhaXQgZG9jdW1lbnQuYm9keS5yZXF1ZXN0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgcmVnaXN0ZXJUaXRsZUNoYW5nZSgpIHtcclxuICAgICAgICBpZiAodGhpcy50aXRsZVJlZ2lzdGVyZWQpIHJldHVybjtcclxuICAgICAgICB0aGlzLnRpdGxlUmVnaXN0ZXJlZCA9IHRydWU7XHJcbiAgICAgICAgY29uc3QgdGl0bGVUYXJnZXQgPSBnZXRUaXRsZU9ic2VydmVyVGFyZ2V0KCk7XHJcbiAgICAgICAgaWYgKHRpdGxlVGFyZ2V0KSB7XHJcbiAgICAgICAgICAgIGdldFRpdGxlT2JzZXJ2ZXIoKS5vYnNlcnZlKHRpdGxlVGFyZ2V0LCB7Y2hpbGRMaXN0OiB0cnVlfSk7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGNvbnN0IGhlYWRUYXJnZXQgPSBkb2N1bWVudC5oZWFkIHx8IGRvY3VtZW50LmRvY3VtZW50RWxlbWVudDtcclxuICAgICAgICBpZiAoIWhlYWRUYXJnZXQpIHJldHVybjtcclxuXHJcbiAgICAgICAgY29uc3QgaGVhZE9ic2VydmVyID0gbmV3IE11dGF0aW9uT2JzZXJ2ZXIoKCkgPT4ge1xyXG4gICAgICAgICAgICBjb25zdCB0YXJnZXQgPSBnZXRUaXRsZU9ic2VydmVyVGFyZ2V0KCk7XHJcbiAgICAgICAgICAgIGlmICghdGFyZ2V0KSByZXR1cm47XHJcbiAgICAgICAgICAgIGhlYWRPYnNlcnZlci5kaXNjb25uZWN0KCk7XHJcbiAgICAgICAgICAgIGdldFRpdGxlT2JzZXJ2ZXIoKS5vYnNlcnZlKHRhcmdldCwge2NoaWxkTGlzdDogdHJ1ZX0pO1xyXG4gICAgICAgIH0pO1xyXG4gICAgICAgIGhlYWRPYnNlcnZlci5vYnNlcnZlKGhlYWRUYXJnZXQsIHtjaGlsZExpc3Q6IHRydWUsIHN1YnRyZWU6IHRydWV9KTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHJlZ2lzdGVyV2luZG93Q2xvc2UoKSB7XHJcbiAgICAgICAgaWYgKHRoaXMud2luZG93Q2xvc2VSZWdpc3RlcmVkKSByZXR1cm47XHJcbiAgICAgICAgdGhpcy53aW5kb3dDbG9zZVJlZ2lzdGVyZWQgPSB0cnVlO1xyXG4gICAgICAgIHdpbmRvdy5jbG9zZSA9ICgpID0+IHtcclxuICAgICAgICAgICAgdGhpcy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy53aW5kb3dDbG9zZSk7XHJcbiAgICAgICAgfTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHNlbmRSZWFkeUhhbmRzaGFrZVdpdGhSZXRyeSgpIHtcclxuICAgICAgICB0aGlzLnNlbmRSZWFkeUhhbmRzaGFrZSgpO1xyXG5cclxuICAgICAgICB0aGlzLnJlYWR5SGFuZHNoYWtlUmV0cnlUaW1lciA9IHdpbmRvdy5zZXRJbnRlcnZhbCgoKSA9PiB7XHJcbiAgICAgICAgICAgIGlmICh0aGlzLnJlYWR5SGFuZHNoYWtlQWNrbm93bGVkZ2VkIHx8IHRoaXMucmVhZHlIYW5kc2hha2VBdHRlbXB0cyA+PSBJbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmcuTWF4UmVhZHlIYW5kc2hha2VBdHRlbXB0cykge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5zdG9wUmVhZHlIYW5kc2hha2VSZXRyeSgpO1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICB0aGlzLnNlbmRSZWFkeUhhbmRzaGFrZSgpO1xyXG4gICAgICAgIH0sIEluZmluaUZyYW1lSG9zdE1lc3NhZ2luZy5SZWFkeUhhbmRzaGFrZVJldHJ5SW50ZXJ2YWxNcyk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBzZW5kUmVhZHlIYW5kc2hha2UoKSB7XHJcbiAgICAgICAgdGhpcy5yZWFkeUhhbmRzaGFrZUF0dGVtcHRzKys7XHJcbiAgICAgICAgdGhpcy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy5yZWFkeSk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBtYXJrUmVhZHlIYW5kc2hha2VBY2tub3dsZWRnZWQoKSB7XHJcbiAgICAgICAgaWYgKHRoaXMucmVhZHlIYW5kc2hha2VBY2tub3dsZWRnZWQpIHJldHVybjtcclxuICAgICAgICB0aGlzLnJlYWR5SGFuZHNoYWtlQWNrbm93bGVkZ2VkID0gdHJ1ZTtcclxuICAgICAgICB0aGlzLnN0b3BSZWFkeUhhbmRzaGFrZVJldHJ5KCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBzdG9wUmVhZHlIYW5kc2hha2VSZXRyeSgpIHtcclxuICAgICAgICBpZiAodGhpcy5yZWFkeUhhbmRzaGFrZVJldHJ5VGltZXIgPT09IG51bGwpIHJldHVybjtcclxuICAgICAgICB3aW5kb3cuY2xlYXJJbnRlcnZhbCh0aGlzLnJlYWR5SGFuZHNoYWtlUmV0cnlUaW1lcik7XHJcbiAgICAgICAgdGhpcy5yZWFkeUhhbmRzaGFrZVJldHJ5VGltZXIgPSBudWxsO1xyXG4gICAgfVxyXG59XHJcblxyXG5leHBvcnQgZGVmYXVsdCBJbmZpbmlGcmFtZUhvc3RNZXNzYWdpbmdcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7SUluZmluaUZyYW1lVXRpbHN9IGZyb20gXCIuL0NvbnRyYWN0c1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBjbGFzcyBJbmZpbmlGcmFtZVV0aWxzIGltcGxlbWVudHMgSUluZmluaUZyYW1lVXRpbHMge1xyXG4gICAgc2V0UG9pbnRlckNhcHR1cmUoZWxlbWVudDogRWxlbWVudCwgcG9pbnRlcklkOiBudW1iZXIpOiB2b2lkIHtcclxuICAgICAgICBpZiAoZWxlbWVudCA9PT0gbnVsbCkgcmV0dXJuO1xyXG4gICAgICAgIGlmIChwb2ludGVySWQgPT09IG51bGwpIHJldHVybjtcclxuICAgICAgICBcclxuICAgICAgICBpZiAoZWxlbWVudC5oYXNQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpKSByZXR1cm47XHJcbiAgICAgICAgZWxlbWVudC5zZXRQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICByZWxlYXNlUG9pbnRlckNhcHR1cmUoZWxlbWVudDogRWxlbWVudCwgcG9pbnRlcklkOiBudW1iZXIpOiB2b2lkIHtcclxuICAgICAgICBpZiAoZWxlbWVudCA9PT0gbnVsbCkgcmV0dXJuO1xyXG4gICAgICAgIGlmIChwb2ludGVySWQgPT09IG51bGwpIHJldHVybjtcclxuICAgICAgICBcclxuICAgICAgICBpZiAoIWVsZW1lbnQuaGFzUG9pbnRlckNhcHR1cmUocG9pbnRlcklkKSkgcmV0dXJuO1xyXG4gICAgICAgIGVsZW1lbnQucmVsZWFzZVBvaW50ZXJDYXB0dXJlKHBvaW50ZXJJZCk7XHJcbiAgICB9XHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtJSW5maW5pRnJhbWVXaW5kb3csIFNlbmRUb0hvc3RNZXNzYWdlSWRzfSBmcm9tIFwiLi9Db250cmFjdHNcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY2xhc3MgSW5maW5pRnJhbWVXaW5kb3cgaW1wbGVtZW50cyBJSW5maW5pRnJhbWVXaW5kb3cge1xyXG4gICAgXHJcbiAgICBzZXRUaXRsZSh0aXRsZTpzdHJpbmcpIHtcclxuICAgICAgICB3aW5kb3cuaW5maW5pRnJhbWUuaG9zdE1lc3NhZ2luZy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy50aXRsZUNoYW5nZSwgdGl0bGUpO1xyXG4gICAgfVxyXG59IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7SW50ZXJvcEVudmVsb3BlVjEsIFBhcnNlZEludGVyb3BNZXNzYWdlLCBJbnRlcm9wUGFyc2VFcnJvcn0gZnJvbSBcIi4uLy4uL0NvbnRyYWN0c1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBjb25zdCBJbnRlcm9wRW52ZWxvcGVWZXJzaW9uID0gMTtcclxuZXhwb3J0IGNvbnN0IEludGVyb3BNZXNzYWdlTWF4U2l6ZUJ5dGVzID0gMTAyNCAqIDEwMjQ7XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gY3JlYXRlRW52ZWxvcGUoaWQ6IHN0cmluZywgZGF0YT86IHVua25vd24sIGNoYW5uZWw/OiBzdHJpbmcpOiBJbnRlcm9wRW52ZWxvcGVWMSB7XHJcbiAgICBpZiAoIWlkIHx8IGlkLnRyaW0oKS5sZW5ndGggPT09IDApIHtcclxuICAgICAgICB0aHJvdyBuZXcgRXJyb3IoXCJFbnZlbG9wZSAnaWQnIGlzIHJlcXVpcmVkLlwiKTtcclxuICAgIH1cclxuXHJcbiAgICByZXR1cm4ge1xyXG4gICAgICAgIGlkLFxyXG4gICAgICAgIGRhdGEsXHJcbiAgICAgICAgdmVyc2lvbjogSW50ZXJvcEVudmVsb3BlVmVyc2lvbixcclxuICAgICAgICBjaGFubmVsXHJcbiAgICB9O1xyXG59XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gY3JlYXRlRW52ZWxvcGVNZXNzYWdlKGlkOiBzdHJpbmcsIGRhdGE/OiB1bmtub3duLCBjaGFubmVsPzogc3RyaW5nKTogc3RyaW5nIHtcclxuICAgIGNvbnN0IGVudmVsb3BlID0gY3JlYXRlRW52ZWxvcGUoaWQsIGRhdGEsIGNoYW5uZWwpO1xyXG5cclxuICAgIHJldHVybiBKU09OLnN0cmluZ2lmeShlbnZlbG9wZSk7XHJcbn1cclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBwYXJzZUluY29taW5nTWVzc2FnZShtZXNzYWdlOiBzdHJpbmcpOiBQYXJzZWRJbnRlcm9wTWVzc2FnZSB8IEludGVyb3BQYXJzZUVycm9yIHtcclxuICAgIGlmICghbWVzc2FnZSB8fCBtZXNzYWdlLnRyaW0oKS5sZW5ndGggPT09IDApIHtcclxuICAgICAgICByZXR1cm4ge2Vycm9yOiBcIk1lc3NhZ2UgaXMgZW1wdHkuXCJ9O1xyXG4gICAgfVxyXG5cclxuICAgIGlmIChnZXRVdGY4Qnl0ZUNvdW50KG1lc3NhZ2UpID4gSW50ZXJvcE1lc3NhZ2VNYXhTaXplQnl0ZXMpIHtcclxuICAgICAgICByZXR1cm4ge2Vycm9yOiBgTWVzc2FnZSBleGNlZWRzIG1heCBzaXplIG9mICR7SW50ZXJvcE1lc3NhZ2VNYXhTaXplQnl0ZXN9IGJ5dGVzLmB9O1xyXG4gICAgfVxyXG5cclxuICAgIHRyeSB7XHJcbiAgICAgICAgY29uc3QgcGFyc2VkID0gSlNPTi5wYXJzZShtZXNzYWdlKSBhcyB1bmtub3duO1xyXG4gICAgICAgIGlmICghaXNPYmplY3QocGFyc2VkKSkge1xyXG4gICAgICAgICAgICByZXR1cm4ge2Vycm9yOiBcIkVudmVsb3BlIHJvb3QgbXVzdCBiZSBhIEpTT04gb2JqZWN0LlwifTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmICh0eXBlb2YgcGFyc2VkLmlkICE9PSBcInN0cmluZ1wiIHx8IHBhcnNlZC5pZC50cmltKCkubGVuZ3RoID09PSAwKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB7ZXJyb3I6IFwiRW52ZWxvcGUgJ2lkJyBpcyByZXF1aXJlZCBhbmQgbXVzdCBiZSBhIHN0cmluZy5cIn07XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAodHlwZW9mIHBhcnNlZC52ZXJzaW9uICE9PSBcIm51bWJlclwiIHx8ICFOdW1iZXIuaXNJbnRlZ2VyKHBhcnNlZC52ZXJzaW9uKSkge1xyXG4gICAgICAgICAgICByZXR1cm4ge2Vycm9yOiBcIkVudmVsb3BlICd2ZXJzaW9uJyBpcyByZXF1aXJlZCBhbmQgbXVzdCBiZSBhbiBpbnRlZ2VyLlwifTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGlmIChwYXJzZWQudmVyc2lvbiAhPT0gSW50ZXJvcEVudmVsb3BlVmVyc2lvbikge1xyXG4gICAgICAgICAgICByZXR1cm4ge2Vycm9yOiBgVW5zdXBwb3J0ZWQgZW52ZWxvcGUgdmVyc2lvbiAnJHtwYXJzZWQudmVyc2lvbn0nLmB9O1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgcGF5bG9hZCA9IGNvbnZlcnREYXRhVG9QYXlsb2FkKHBhcnNlZC5kYXRhKTtcclxuICAgICAgICByZXR1cm4ge1xyXG4gICAgICAgICAgICBtZXNzYWdlSWQ6IHBhcnNlZC5pZCxcclxuICAgICAgICAgICAgcGF5bG9hZFxyXG4gICAgICAgIH07XHJcbiAgICB9IGNhdGNoIHtcclxuICAgICAgICByZXR1cm4ge2Vycm9yOiBcIkVudmVsb3BlIEpTT04gaXMgbWFsZm9ybWVkLlwifTtcclxuICAgIH1cclxufVxyXG5cclxuZnVuY3Rpb24gY29udmVydERhdGFUb1BheWxvYWQoZGF0YTogdW5rbm93bik6IHN0cmluZyB8IHVuZGVmaW5lZCB7XHJcbiAgICBpZiAoZGF0YSA9PT0gbnVsbCB8fCBkYXRhID09PSB1bmRlZmluZWQpIHtcclxuICAgICAgICByZXR1cm4gdW5kZWZpbmVkO1xyXG4gICAgfVxyXG5cclxuICAgIGlmICh0eXBlb2YgZGF0YSA9PT0gXCJzdHJpbmdcIikge1xyXG4gICAgICAgIHJldHVybiBkYXRhO1xyXG4gICAgfVxyXG5cclxuICAgIHJldHVybiBKU09OLnN0cmluZ2lmeShkYXRhKTtcclxufVxyXG5cclxuZnVuY3Rpb24gZ2V0VXRmOEJ5dGVDb3VudChtZXNzYWdlOiBzdHJpbmcpOiBudW1iZXIge1xyXG4gICAgcmV0dXJuIG5ldyBUZXh0RW5jb2RlcigpLmVuY29kZShtZXNzYWdlKS5sZW5ndGg7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIGlzT2JqZWN0KHZhbHVlOiB1bmtub3duKTogdmFsdWUgaXMgUmVjb3JkPHN0cmluZywgdW5rbm93bj4ge1xyXG4gICAgcmV0dXJuIHR5cGVvZiB2YWx1ZSA9PT0gXCJvYmplY3RcIiAmJiB2YWx1ZSAhPT0gbnVsbDtcclxufVxyXG4iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cbi8vIEltcG9ydHNcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxuaW1wb3J0IHtJbnRlcm9wRW52ZWxvcGVWMX0gZnJvbSBcIi4uLy4uL0NvbnRyYWN0c1wiO1xuaW1wb3J0IHtJbnRlcm9wRW52ZWxvcGVWZXJzaW9uLCBwYXJzZUluY29taW5nTWVzc2FnZX0gZnJvbSBcIi4uL0VudmVsb3BlUHJvdG9jb2wvSW50ZXJvcEVudmVsb3BlUHJvdG9jb2xcIjtcblxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXG4vLyBDb2RlXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cbmNvbnN0IEdldE1lc3NhZ2VSZXF1ZXN0SWQgPSBcIl9faW5maW5pZnJhbWU6Z2V0OnJlcXVlc3RcIjtcbmNvbnN0IEdldE1lc3NhZ2VSZXNwb25zZUlkID0gXCJfX2luZmluaWZyYW1lOmdldDpyZXNwb25zZVwiO1xuY29uc3QgR2V0TWVzc2FnZVRpbWVvdXRNcyA9IDEwXzAwMDtcblxudHlwZSBSZWNlaXZlQ2FsbGJhY2sgPSAobWVzc2FnZTogc3RyaW5nKSA9PiB2b2lkO1xuY29uc3QgcmVjZWl2ZUNhbGxiYWNrcyA9IG5ldyBTZXQ8UmVjZWl2ZUNhbGxiYWNrPigpO1xubGV0IHJlY2VpdmVCcmlkZ2VBdHRhY2hlZCA9IGZhbHNlO1xuXG5leHBvcnQgZnVuY3Rpb24gaW5zdGFsbEhvc3RCcmlkZ2UoKTogdm9pZCB7XG4gICAgY29uc3Qgcm9vdDogTm9uTnVsbGFibGU8V2luZG93W1wiaW5maW5pZnJhbWVcIl0+ID0gd2luZG93LmluZmluaWZyYW1lID8/IHt9O1xuICAgIGNvbnN0IGhvc3QgPSAocm9vdC5ob3N0ID8/IHt9KSBhcyBOb25OdWxsYWJsZTxOb25OdWxsYWJsZTxXaW5kb3dbXCJpbmZpbmlmcmFtZVwiXT5bXCJob3N0XCJdPjtcbiAgICBjb25zdCBleGlzdGluZ1Bvc3REYXRhID0gaG9zdC5wb3N0RGF0YTtcbiAgICBjb25zdCBleGlzdGluZ1JlY2VpdmVDYWxsYmFjayA9IGhvc3QucmVjZWl2ZUNhbGxiYWNrO1xuICAgIGNvbnN0IGV4aXN0aW5nR2V0RGF0YSA9IGhvc3QuZ2V0RGF0YTtcblxuICAgIGhvc3QucG9zdERhdGEgPSAoZW52ZWxvcGU6IEludGVyb3BFbnZlbG9wZVYxIHwgc3RyaW5nKSA9PiB7XG4gICAgICAgIGRpc3BhdGNoRW52ZWxvcGVUb0hvc3QoZW52ZWxvcGUsIGV4aXN0aW5nUG9zdERhdGEpO1xuICAgIH07XG4gICAgaG9zdC5yZWNlaXZlQ2FsbGJhY2sgPSAoY2FsbGJhY2s6IChtZXNzYWdlOiBzdHJpbmcpID0+IHZvaWQpID0+IHtcbiAgICAgICAgcmVnaXN0ZXJXZWJNZXNzYWdlUmVjZWl2ZXIoY2FsbGJhY2ssIGV4aXN0aW5nUmVjZWl2ZUNhbGxiYWNrKTtcbiAgICB9O1xuICAgIGhvc3QuZ2V0RGF0YSA9IChtZXNzYWdlOiBJbnRlcm9wRW52ZWxvcGVWMSB8IHN0cmluZykgPT4ge1xuICAgICAgICByZXR1cm4gcmVxdWVzdE1lc3NhZ2VGcm9tSG9zdChtZXNzYWdlLCBob3N0LCBleGlzdGluZ0dldERhdGEsIGV4aXN0aW5nUmVjZWl2ZUNhbGxiYWNrKTtcbiAgICB9O1xuXG4gICAgcm9vdC5ob3N0ID0gaG9zdDtcbiAgICB3aW5kb3cuaW5maW5pZnJhbWUgPSByb290O1xufVxuXHJcbmZ1bmN0aW9uIGRpc3BhdGNoRW52ZWxvcGVUb0hvc3QoXG4gICAgZW52ZWxvcGU6IEludGVyb3BFbnZlbG9wZVYxIHwgc3RyaW5nLFxuICAgIGV4aXN0aW5nUG9zdERhdGE/OiAoKGVudmVsb3BlOiBJbnRlcm9wRW52ZWxvcGVWMSB8IHN0cmluZykgPT4gdm9pZClcbik6IHZvaWQge1xuICAgIGlmICh0eXBlb2YgZW52ZWxvcGUgPT09IFwic3RyaW5nXCIpIHtcclxuICAgICAgICBjb25zdCByYXdNZXNzYWdlID0gZW52ZWxvcGUudHJpbSgpO1xyXG4gICAgICAgIGlmIChyYXdNZXNzYWdlLmxlbmd0aCA9PT0gMCkge1xyXG4gICAgICAgICAgICBjb25zb2xlLndhcm4oXCJJZ25vcmluZyBlbXB0eSBob3N0IGJyaWRnZSBwYXlsb2FkLlwiKTtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKGV4aXN0aW5nUG9zdERhdGEpIHtcbiAgICAgICAgICAgIHRyeSB7XG4gICAgICAgICAgICAgICAgZXhpc3RpbmdQb3N0RGF0YShyYXdNZXNzYWdlKTtcbiAgICAgICAgICAgICAgICByZXR1cm47XG4gICAgICAgICAgICB9IGNhdGNoIChlcnJvcikge1xyXG4gICAgICAgICAgICAgICAgY29uc29sZS53YXJuKFwiRXhpc3RpbmcgSW5maW5pRnJhbWUgaG9zdCBicmlkZ2UgZmFpbGVkLiBGYWxsaW5nIGJhY2sgdG8gcGxhdGZvcm0gYWRhcHRlcnMuXCIsIGVycm9yKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgc2VuZFZpYVBsYXRmb3JtVHJhbnNwb3J0KHJhd01lc3NhZ2UpO1xyXG4gICAgICAgIHJldHVybjtcclxuICAgIH1cclxuXHJcbiAgICBjb25zdCBub3JtYWxpemVkID0gbm9ybWFsaXplRW52ZWxvcGUoZW52ZWxvcGUpO1xyXG4gICAgaWYgKCFub3JtYWxpemVkKSB7XHJcbiAgICAgICAgcmV0dXJuO1xyXG4gICAgfVxyXG5cclxuICAgIGNvbnN0IHNlcmlhbGl6ZWRFbnZlbG9wZSA9IEpTT04uc3RyaW5naWZ5KG5vcm1hbGl6ZWQpO1xyXG5cclxuICAgIGlmIChleGlzdGluZ1Bvc3REYXRhKSB7XG4gICAgICAgIHRyeSB7XG4gICAgICAgICAgICAvLyBQcmVmZXIgdGhlIHN0cmluZyBjb250cmFjdCBmb3IgaG9zdCBhZGFwdGVycyB0aGF0IG9ubHkgYWNjZXB0IHJhdyBtZXNzYWdlcy5cbiAgICAgICAgICAgIGV4aXN0aW5nUG9zdERhdGEoc2VyaWFsaXplZEVudmVsb3BlKTtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIHRyeSB7XG4gICAgICAgICAgICAgICAgLy8gQmFja3dhcmQgY29tcGF0aWJpbGl0eSBmb3IgYWRhcHRlcnMgdGhhdCBzdGlsbCBleHBlY3QgYW4gZW52ZWxvcGUgb2JqZWN0LlxuICAgICAgICAgICAgICAgIGV4aXN0aW5nUG9zdERhdGEobm9ybWFsaXplZCk7XG4gICAgICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICAgICAgfSBjYXRjaCB7XHJcbiAgICAgICAgICAgICAgICBjb25zb2xlLndhcm4oXCJFeGlzdGluZyBJbmZpbmlGcmFtZSBob3N0IGJyaWRnZSBmYWlsZWQuIEZhbGxpbmcgYmFjayB0byBwbGF0Zm9ybSBhZGFwdGVycy5cIiwgZXJyb3IpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHNlbmRWaWFQbGF0Zm9ybVRyYW5zcG9ydChzZXJpYWxpemVkRW52ZWxvcGUpO1xufVxuXG5mdW5jdGlvbiByZXF1ZXN0TWVzc2FnZUZyb21Ib3N0KFxuICAgIG1lc3NhZ2U6IEludGVyb3BFbnZlbG9wZVYxIHwgc3RyaW5nLFxuICAgIGhvc3Q6IE5vbk51bGxhYmxlPE5vbk51bGxhYmxlPFdpbmRvd1tcImluZmluaWZyYW1lXCJdPltcImhvc3RcIl0+LFxuICAgIGV4aXN0aW5nR2V0RGF0YT86ICgobWVzc2FnZTogSW50ZXJvcEVudmVsb3BlVjEgfCBzdHJpbmcpID0+IFByb21pc2U8c3RyaW5nPiB8IHN0cmluZyksXG4gICAgZXhpc3RpbmdSZWNlaXZlQ2FsbGJhY2s/OiAoY2FsbGJhY2s6IChtZXNzYWdlOiBzdHJpbmcpID0+IHZvaWQpID0+IHZvaWRcbik6IFByb21pc2U8c3RyaW5nPiB7XG4gICAgY29uc3Qgbm9ybWFsaXplZE1lc3NhZ2UgPSBub3JtYWxpemVHZXRNZXNzYWdlSW5wdXQobWVzc2FnZSk7XG4gICAgaWYgKCFub3JtYWxpemVkTWVzc2FnZSkge1xuICAgICAgICByZXR1cm4gUHJvbWlzZS5yZWplY3QobmV3IEVycm9yKFwiSG9zdCBnZXREYXRhIHBheWxvYWQgaXMgaW52YWxpZC5cIikpO1xuICAgIH1cblxuICAgIGlmIChleGlzdGluZ0dldERhdGEpIHtcbiAgICAgICAgdHJ5IHtcbiAgICAgICAgICAgIGNvbnN0IGV4aXN0aW5nUmVzdWx0ID0gZXhpc3RpbmdHZXREYXRhKG5vcm1hbGl6ZWRNZXNzYWdlKTtcbiAgICAgICAgICAgIGlmIChleGlzdGluZ1Jlc3VsdCAmJiB0eXBlb2YgKGV4aXN0aW5nUmVzdWx0IGFzIFByb21pc2U8c3RyaW5nPikudGhlbiA9PT0gXCJmdW5jdGlvblwiKSB7XG4gICAgICAgICAgICAgICAgcmV0dXJuIGV4aXN0aW5nUmVzdWx0IGFzIFByb21pc2U8c3RyaW5nPjtcbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgcmV0dXJuIFByb21pc2UucmVzb2x2ZShTdHJpbmcoZXhpc3RpbmdSZXN1bHQgPz8gXCJcIikpO1xuICAgICAgICB9IGNhdGNoIChlcnJvcikge1xuICAgICAgICAgICAgY29uc29sZS53YXJuKFwiRXhpc3RpbmcgSW5maW5pRnJhbWUgZ2V0RGF0YSBicmlkZ2UgZmFpbGVkLiBGYWxsaW5nIGJhY2sgdG8gcmVxdWVzdC9yZXNwb25zZSB0cmFuc3BvcnQuXCIsIGVycm9yKTtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIGNvbnN0IHJlcXVlc3RJZCA9IGNyZWF0ZVJlcXVlc3RJZCgpO1xuXG4gICAgcmV0dXJuIG5ldyBQcm9taXNlPHN0cmluZz4oKHJlc29sdmUsIHJlamVjdCkgPT4ge1xuICAgICAgICBjb25zdCB0aW1lb3V0ID0gd2luZG93LnNldFRpbWVvdXQoKCkgPT4ge1xuICAgICAgICAgICAgdW5yZWdpc3RlcldlYk1lc3NhZ2VSZWNlaXZlcihyZXNwb25zZUNhbGxiYWNrKTtcbiAgICAgICAgICAgIHJlamVjdChuZXcgRXJyb3IoXCJUaW1lZCBvdXQgd2FpdGluZyBmb3IgZ2V0RGF0YSByZXNwb25zZSBmcm9tIGhvc3QuXCIpKTtcbiAgICAgICAgfSwgR2V0TWVzc2FnZVRpbWVvdXRNcyk7XG5cbiAgICAgICAgY29uc3QgcmVzcG9uc2VDYWxsYmFjayA9IChyYXdNZXNzYWdlOiBzdHJpbmcpID0+IHtcbiAgICAgICAgICAgIGNvbnN0IHBhcnNlZCA9IHBhcnNlSW5jb21pbmdNZXNzYWdlKHJhd01lc3NhZ2UpO1xuICAgICAgICAgICAgaWYgKFwiZXJyb3JcIiBpbiBwYXJzZWQgfHwgcGFyc2VkLm1lc3NhZ2VJZCAhPT0gR2V0TWVzc2FnZVJlc3BvbnNlSWQgfHwgIXBhcnNlZC5wYXlsb2FkKSB7XG4gICAgICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICBsZXQgcGF5bG9hZDogdW5rbm93bjtcbiAgICAgICAgICAgIHRyeSB7XG4gICAgICAgICAgICAgICAgcGF5bG9hZCA9IEpTT04ucGFyc2UocGFyc2VkLnBheWxvYWQpO1xuICAgICAgICAgICAgfSBjYXRjaCB7XG4gICAgICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICBpZiAoIWlzR2V0TWVzc2FnZVJlc3BvbnNlUGF5bG9hZChwYXlsb2FkKSB8fCBwYXlsb2FkLnJlcXVlc3RJZCAhPT0gcmVxdWVzdElkKSB7XG4gICAgICAgICAgICAgICAgcmV0dXJuO1xuICAgICAgICAgICAgfVxuXG4gICAgICAgICAgICB3aW5kb3cuY2xlYXJUaW1lb3V0KHRpbWVvdXQpO1xuICAgICAgICAgICAgdW5yZWdpc3RlcldlYk1lc3NhZ2VSZWNlaXZlcihyZXNwb25zZUNhbGxiYWNrKTtcblxuICAgICAgICAgICAgaWYgKHBheWxvYWQuc3VjY2Vzcykge1xuICAgICAgICAgICAgICAgIHJlc29sdmUocGF5bG9hZC5kYXRhID8/IFwiXCIpO1xuICAgICAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgICAgIH1cblxuICAgICAgICAgICAgcmVqZWN0KG5ldyBFcnJvcihwYXlsb2FkLmVycm9yID8/IFwiSG9zdCBnZXREYXRhIGZhaWxlZC5cIikpO1xuICAgICAgICB9O1xuXG4gICAgICAgIHJlZ2lzdGVyV2ViTWVzc2FnZVJlY2VpdmVyKHJlc3BvbnNlQ2FsbGJhY2ssIGV4aXN0aW5nUmVjZWl2ZUNhbGxiYWNrKTtcbiAgICAgICAgaG9zdC5wb3N0RGF0YT8uKHtcbiAgICAgICAgICAgIGlkOiBHZXRNZXNzYWdlUmVxdWVzdElkLFxuICAgICAgICAgICAgZGF0YToge1xuICAgICAgICAgICAgICAgIHJlcXVlc3RJZCxcbiAgICAgICAgICAgICAgICBtZXNzYWdlOiBub3JtYWxpemVkTWVzc2FnZVxuICAgICAgICAgICAgfSxcbiAgICAgICAgICAgIHZlcnNpb246IEludGVyb3BFbnZlbG9wZVZlcnNpb25cbiAgICAgICAgfSk7XG4gICAgfSk7XG59XG5cbmZ1bmN0aW9uIG5vcm1hbGl6ZUdldE1lc3NhZ2VJbnB1dChtZXNzYWdlOiBJbnRlcm9wRW52ZWxvcGVWMSB8IHN0cmluZyk6IEludGVyb3BFbnZlbG9wZVYxIHwgc3RyaW5nIHwgbnVsbCB7XG4gICAgaWYgKHR5cGVvZiBtZXNzYWdlID09PSBcInN0cmluZ1wiKSB7XG4gICAgICAgIGNvbnN0IHRyaW1tZWQgPSBtZXNzYWdlLnRyaW0oKTtcbiAgICAgICAgaWYgKHRyaW1tZWQubGVuZ3RoID09PSAwKSB7XG4gICAgICAgICAgICByZXR1cm4gbnVsbDtcbiAgICAgICAgfVxuXG4gICAgICAgIHJldHVybiB0cmltbWVkO1xuICAgIH1cblxuICAgIGNvbnN0IG5vcm1hbGl6ZWRFbnZlbG9wZSA9IG5vcm1hbGl6ZUVudmVsb3BlKG1lc3NhZ2UpO1xuICAgIGlmICghbm9ybWFsaXplZEVudmVsb3BlKSB7XG4gICAgICAgIHJldHVybiBudWxsO1xuICAgIH1cblxuICAgIHJldHVybiBub3JtYWxpemVkRW52ZWxvcGU7XG59XG5cbmZ1bmN0aW9uIGNyZWF0ZVJlcXVlc3RJZCgpOiBzdHJpbmcge1xuICAgIHJldHVybiBgaWZfcmVxXyR7RGF0ZS5ub3coKS50b1N0cmluZygzNil9XyR7TWF0aC5yYW5kb20oKS50b1N0cmluZygzNikuc2xpY2UoMiwgMTApfWA7XG59XG5cbmZ1bmN0aW9uIG5vcm1hbGl6ZUVudmVsb3BlKGVudmVsb3BlOiBJbnRlcm9wRW52ZWxvcGVWMSk6IEludGVyb3BFbnZlbG9wZVYxIHwgbnVsbCB7XG4gICAgaWYgKCFlbnZlbG9wZSB8fCB0eXBlb2YgZW52ZWxvcGUgIT09IFwib2JqZWN0XCIpIHtcbiAgICAgICAgY29uc29sZS53YXJuKFwiSG9zdCBicmlkZ2UgcGF5bG9hZCBtdXN0IGJlIGFuIGVudmVsb3BlIG9iamVjdC5cIik7XG4gICAgICAgIHJldHVybiBudWxsO1xuICAgIH1cclxuXHJcbiAgICAvLyBub2luc3BlY3Rpb24gU3VzcGljaW91c1R5cGVPZkd1YXJkXHJcbiAgICBpZiAodHlwZW9mIGVudmVsb3BlLmlkICE9PSBcInN0cmluZ1wiIHx8IGVudmVsb3BlLmlkLnRyaW0oKS5sZW5ndGggPT09IDApIHtcclxuICAgICAgICBjb25zb2xlLndhcm4oXCJIb3N0IGJyaWRnZSBlbnZlbG9wZSByZXF1aXJlcyBhIG5vbi1lbXB0eSAnaWQnLlwiKTtcclxuICAgICAgICByZXR1cm4gbnVsbDtcclxuICAgIH1cclxuXHJcbiAgICBjb25zdCB2ZXJzaW9uID0gTnVtYmVyLmlzSW50ZWdlcihlbnZlbG9wZS52ZXJzaW9uKVxyXG4gICAgICAgID8gZW52ZWxvcGUudmVyc2lvblxyXG4gICAgICAgIDogSW50ZXJvcEVudmVsb3BlVmVyc2lvbjtcclxuXHJcbiAgICBjb25zdCBub3JtYWxpemVkOiBJbnRlcm9wRW52ZWxvcGVWMSA9IHtcclxuICAgICAgICBpZDogZW52ZWxvcGUuaWQsXHJcbiAgICAgICAgZGF0YTogZW52ZWxvcGUuZGF0YSxcclxuICAgICAgICB2ZXJzaW9uXHJcbiAgICB9O1xyXG5cclxuICAgIC8vIG5vaW5zcGVjdGlvbiBTdXNwaWNpb3VzVHlwZU9mR3VhcmRcclxuICAgIGlmIChlbnZlbG9wZS5jaGFubmVsICE9PSB1bmRlZmluZWQgJiYgdHlwZW9mIGVudmVsb3BlLmNoYW5uZWwgPT09IFwic3RyaW5nXCIgJiYgZW52ZWxvcGUuY2hhbm5lbC50cmltKCkubGVuZ3RoID4gMCkge1xyXG4gICAgICAgIG5vcm1hbGl6ZWQuY2hhbm5lbCA9IGVudmVsb3BlLmNoYW5uZWw7XHJcbiAgICB9XHJcblxyXG4gICAgcmV0dXJuIG5vcm1hbGl6ZWQ7XHJcbn1cclxuXHJcbmZ1bmN0aW9uIHNlbmRWaWFQbGF0Zm9ybVRyYW5zcG9ydChtZXNzYWdlOiBzdHJpbmcpOiB2b2lkIHtcbiAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xuICAgICAgICB3aW5kb3cuY2hyb21lLndlYnZpZXcucG9zdE1lc3NhZ2UobWVzc2FnZSk7XG4gICAgICAgIHJldHVybjtcbiAgICB9XG5cclxuICAgIGNvbnNvbGUud2FybihcIk1lc3NhZ2UgdG8gaG9zdCBmYWlsZWQuIE5vIHN1cHBvcnRlZCBob3N0IHRyYW5zcG9ydCB3YXMgZm91bmQuXCIpO1xyXG59XHJcblxyXG5mdW5jdGlvbiByZWdpc3RlcldlYk1lc3NhZ2VSZWNlaXZlcihcbiAgICBjYWxsYmFjazogKG1lc3NhZ2U6IHN0cmluZykgPT4gdm9pZCxcbiAgICBleGlzdGluZ1JlY2VpdmVDYWxsYmFjaz86IChjYWxsYmFjazogKG1lc3NhZ2U6IHN0cmluZykgPT4gdm9pZCkgPT4gdm9pZFxuKTogdm9pZCB7XG4gICAgcmVjZWl2ZUNhbGxiYWNrcy5hZGQoY2FsbGJhY2spO1xuICAgIGF0dGFjaFJlY2VpdmVCcmlkZ2VPbmNlKGV4aXN0aW5nUmVjZWl2ZUNhbGxiYWNrKTtcbn1cblxuZnVuY3Rpb24gdW5yZWdpc3RlcldlYk1lc3NhZ2VSZWNlaXZlcihjYWxsYmFjazogUmVjZWl2ZUNhbGxiYWNrKTogdm9pZCB7XG4gICAgcmVjZWl2ZUNhbGxiYWNrcy5kZWxldGUoY2FsbGJhY2spO1xufVxuXG5mdW5jdGlvbiBhdHRhY2hSZWNlaXZlQnJpZGdlT25jZShleGlzdGluZ1JlY2VpdmVDYWxsYmFjaz86IChjYWxsYmFjazogKG1lc3NhZ2U6IHN0cmluZykgPT4gdm9pZCkgPT4gdm9pZCk6IHZvaWQge1xuICAgIGlmIChyZWNlaXZlQnJpZGdlQXR0YWNoZWQpIHtcbiAgICAgICAgcmV0dXJuO1xuICAgIH1cblxuICAgIGNvbnN0IGRpc3BhdGNoID0gKG1lc3NhZ2U6IHN0cmluZykgPT4ge1xuICAgICAgICBmb3IgKGNvbnN0IGNhbGxiYWNrIG9mIHJlY2VpdmVDYWxsYmFja3MpIHtcbiAgICAgICAgICAgIGNhbGxiYWNrKG1lc3NhZ2UpO1xuICAgICAgICB9XG4gICAgfTtcblxuICAgIGlmIChleGlzdGluZ1JlY2VpdmVDYWxsYmFjaykge1xuICAgICAgICB0cnkge1xuICAgICAgICAgICAgZXhpc3RpbmdSZWNlaXZlQ2FsbGJhY2soZGlzcGF0Y2gpO1xuICAgICAgICAgICAgcmVjZWl2ZUJyaWRnZUF0dGFjaGVkID0gdHJ1ZTtcbiAgICAgICAgICAgIHJldHVybjtcbiAgICAgICAgfSBjYXRjaCAoZXJyb3IpIHtcbiAgICAgICAgICAgIGNvbnNvbGUud2FybihcIkV4aXN0aW5nIEluZmluaUZyYW1lIGhvc3QgcmVjZWl2ZSBicmlkZ2UgZmFpbGVkLiBGYWxsaW5nIGJhY2sgdG8gcGxhdGZvcm0gYWRhcHRlcnMuXCIsIGVycm9yKTtcbiAgICAgICAgfVxuICAgIH1cblxuICAgIGlmICh3aW5kb3cuY2hyb21lPy53ZWJ2aWV3KSB7XG4gICAgICAgIHdpbmRvdy5jaHJvbWUud2Vidmlldy5hZGRFdmVudExpc3RlbmVyKFwibWVzc2FnZVwiLCAoZXZlbnQpID0+IHtcbiAgICAgICAgICAgIGRpc3BhdGNoKGV2ZW50LmRhdGEpO1xuICAgICAgICB9KTtcbiAgICAgICAgcmVjZWl2ZUJyaWRnZUF0dGFjaGVkID0gdHJ1ZTtcbiAgICAgICAgcmV0dXJuO1xuICAgIH1cblxuICAgIGNvbnNvbGUud2FybihcIlJlY2VpdmUgbWVzc2FnZSByZWdpc3RyYXRpb24gZmFpbGVkLiBObyBzdXBwb3J0ZWQgaG9zdCByZWNlaXZlIHRyYW5zcG9ydCB3YXMgZm91bmQuXCIpO1xufVxuXG5mdW5jdGlvbiBpc09iamVjdCh2YWx1ZTogdW5rbm93bik6IHZhbHVlIGlzIFJlY29yZDxzdHJpbmcsIHVua25vd24+IHtcbiAgICByZXR1cm4gdHlwZW9mIHZhbHVlID09PSBcIm9iamVjdFwiICYmIHZhbHVlICE9PSBudWxsO1xufVxuXG5mdW5jdGlvbiBpc0dldE1lc3NhZ2VSZXNwb25zZVBheWxvYWQodmFsdWU6IHVua25vd24pOiB2YWx1ZSBpcyB7XG4gICAgcmVxdWVzdElkOiBzdHJpbmc7XG4gICAgc3VjY2VzczogYm9vbGVhbjtcbiAgICBkYXRhPzogc3RyaW5nO1xuICAgIGVycm9yPzogc3RyaW5nO1xufSB7XG4gICAgcmV0dXJuIGlzT2JqZWN0KHZhbHVlKVxuICAgICAgICAmJiB0eXBlb2YgdmFsdWUucmVxdWVzdElkID09PSBcInN0cmluZ1wiXG4gICAgICAgICYmIHR5cGVvZiB2YWx1ZS5zdWNjZXNzID09PSBcImJvb2xlYW5cIlxuICAgICAgICAmJiAodmFsdWUuZGF0YSA9PT0gdW5kZWZpbmVkIHx8IHR5cGVvZiB2YWx1ZS5kYXRhID09PSBcInN0cmluZ1wiKVxuICAgICAgICAmJiAodmFsdWUuZXJyb3IgPT09IHVuZGVmaW5lZCB8fCB0eXBlb2YgdmFsdWUuZXJyb3IgPT09IFwic3RyaW5nXCIpO1xufVxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7U2VuZFRvSG9zdE1lc3NhZ2VJZHN9IGZyb20gXCIuLi9Db250cmFjdHNcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5mdW5jdGlvbiBpc0V4dGVybmFsTGluayh1cmw6IHN0cmluZyk6IGJvb2xlYW4ge1xyXG4gICAgdHJ5IHtcclxuICAgICAgICByZXR1cm4gbmV3IFVSTCh1cmwsIGxvY2F0aW9uLmhyZWYpLmhvc3RuYW1lICE9PSBsb2NhdGlvbi5ob3N0bmFtZTtcclxuICAgIH0gY2F0Y2gge1xyXG4gICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGFzeW5jIGZ1bmN0aW9uIGJsYW5rVGFyZ2V0SGFuZGxlcihlOiBNb3VzZUV2ZW50KSB7XHJcbiAgICBsZXQgZWwgPSBlLnRhcmdldCBhcyBIVE1MRWxlbWVudCB8IG51bGw7XHJcblxyXG4gICAgd2hpbGUgKGVsICYmIGVsICE9PSBkb2N1bWVudC5ib2R5KSB7XHJcbiAgICAgICAgaWYgKGVsLnRhZ05hbWU/LnRvTG93ZXJDYXNlKCkgIT09IFwiYVwiKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCBhbmNob3IgPSBlbCBhcyBIVE1MQW5jaG9yRWxlbWVudDtcclxuICAgICAgICBpZiAoIWFuY2hvci5ocmVmKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCB0YXJnZXQgPSBhbmNob3IuZ2V0QXR0cmlidXRlKFwidGFyZ2V0XCIpO1xyXG4gICAgICAgIGNvbnN0IHNob3VsZEhhbmRsZSA9IHRhcmdldCA9PT0gXCJfYmxhbmtcIiB8fCBhbmNob3IuaGFzQXR0cmlidXRlKFwiZGF0YS1leHRlcm5hbFwiKSB8fCBpc0V4dGVybmFsTGluayhhbmNob3IuaHJlZik7XHJcblxyXG4gICAgICAgIGlmICghc2hvdWxkSGFuZGxlKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcbiAgICAgICAgd2luZG93LmluZmluaUZyYW1lLmhvc3RNZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMub3BlbkV4dGVybmFsTGluaywgYW5jaG9yLmhyZWYpO1xyXG4gICAgICAgIHJldHVybjtcclxuICAgIH1cclxufSIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1NlbmRUb0hvc3RNZXNzYWdlSWRzfSBmcm9tIFwiLi4vQ29udHJhY3RzXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGZ1bmN0aW9uIGdldFRpdGxlT2JzZXJ2ZXJUYXJnZXQoKTogSFRNTFRpdGxlRWxlbWVudCB8IG51bGwge1xyXG4gICAgcmV0dXJuIGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IoJ3RpdGxlJyk7XHJcbn1cclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBnZXRUaXRsZU9ic2VydmVyKCk6IE11dGF0aW9uT2JzZXJ2ZXIge1xyXG4gICAgcmV0dXJuIG5ldyBNdXRhdGlvbk9ic2VydmVyKChtdXRhdGlvbnMsIF8pID0+IHtcclxuICAgICAgICBtdXRhdGlvbnMuZm9yRWFjaCgobXV0YXRpb24pID0+IHtcclxuICAgICAgICAgICAgaWYgKG11dGF0aW9uLnR5cGUgIT09IFwiY2hpbGRMaXN0XCIpIHJldHVybjtcclxuICAgICAgICAgICAgd2luZG93LmluZmluaUZyYW1lLmhvc3RNZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMudGl0bGVDaGFuZ2UsIGRvY3VtZW50LnRpdGxlKTtcclxuICAgICAgICB9KVxyXG4gICAgfSlcclxufVxyXG4iLCIvLyBUaGUgbW9kdWxlIGNhY2hlXG52YXIgX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fID0ge307XG5cbi8vIFRoZSByZXF1aXJlIGZ1bmN0aW9uXG5mdW5jdGlvbiBfX3dlYnBhY2tfcmVxdWlyZV9fKG1vZHVsZUlkKSB7XG5cdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuXHR2YXIgY2FjaGVkTW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXTtcblx0aWYgKGNhY2hlZE1vZHVsZSAhPT0gdW5kZWZpbmVkKSB7XG5cdFx0cmV0dXJuIGNhY2hlZE1vZHVsZS5leHBvcnRzO1xuXHR9XG5cdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG5cdHZhciBtb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdID0ge1xuXHRcdC8vIG5vIG1vZHVsZS5pZCBuZWVkZWRcblx0XHQvLyBubyBtb2R1bGUubG9hZGVkIG5lZWRlZFxuXHRcdGV4cG9ydHM6IHt9XG5cdH07XG5cblx0Ly8gRXhlY3V0ZSB0aGUgbW9kdWxlIGZ1bmN0aW9uXG5cdGlmICghKG1vZHVsZUlkIGluIF9fd2VicGFja19tb2R1bGVzX18pKSB7XG5cdFx0ZGVsZXRlIF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF07XG5cdFx0dmFyIGUgPSBuZXcgRXJyb3IoXCJDYW5ub3QgZmluZCBtb2R1bGUgJ1wiICsgbW9kdWxlSWQgKyBcIidcIik7XG5cdFx0ZS5jb2RlID0gJ01PRFVMRV9OT1RfRk9VTkQnO1xuXHRcdHRocm93IGU7XG5cdH1cblx0X193ZWJwYWNrX21vZHVsZXNfX1ttb2R1bGVJZF0uY2FsbChtb2R1bGUuZXhwb3J0cywgbW9kdWxlLCBtb2R1bGUuZXhwb3J0cywgX193ZWJwYWNrX3JlcXVpcmVfXyk7XG5cblx0Ly8gUmV0dXJuIHRoZSBleHBvcnRzIG9mIHRoZSBtb2R1bGVcblx0cmV0dXJuIG1vZHVsZS5leHBvcnRzO1xufVxuXG4iLCIiLCIvLyBzdGFydHVwXG4vLyBMb2FkIGVudHJ5IG1vZHVsZSBhbmQgcmV0dXJuIGV4cG9ydHNcbi8vIFRoaXMgZW50cnkgbW9kdWxlIGlzIHJlZmVyZW5jZWQgYnkgb3RoZXIgbW9kdWxlcyBzbyBpdCBjYW4ndCBiZSBpbmxpbmVkXG52YXIgX193ZWJwYWNrX2V4cG9ydHNfXyA9IF9fd2VicGFja19yZXF1aXJlX18oXCIuL1R5cGVTY3JpcHQvSW5kZXgudHNcIik7XG4iLCIiXSwibmFtZXMiOltdLCJzb3VyY2VSb290IjoiIn0=