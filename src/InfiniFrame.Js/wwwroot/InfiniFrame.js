/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/InfiniFrame.Js/TsSource/BlankTargetHandler.ts"
/*!***********************************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/BlankTargetHandler.ts ***!
  \***********************************************************/
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
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts");
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
            window.infiniFrame.HostMessaging.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.openExternalLink, anchor.href);
            return;
        }
    });
}


/***/ },

/***/ "./src/InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts"
/*!*****************************************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts ***!
  \*****************************************************************/
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
};
exports.ReceiveFromHostMessageIds = {
    registerOpenExternal: `${infiniFrame}:register:open:external`,
    registerFullscreenChange: `${infiniFrame}:register:fullscreen:change`,
    registerTitleChange: `${infiniFrame}:register:title:change`,
    registerWindowClose: `${infiniFrame}:register:window:close`,
};


/***/ },

/***/ "./src/InfiniFrame.Js/TsSource/HostMessaging.ts"
/*!******************************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/HostMessaging.ts ***!
  \******************************************************/
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
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts");
const BlankTargetHandler_1 = __webpack_require__(/*! ./BlankTargetHandler */ "./src/InfiniFrame.Js/TsSource/BlankTargetHandler.ts");
const Observers_1 = __webpack_require__(/*! ./Observers */ "./src/InfiniFrame.Js/TsSource/Observers.ts");
class HostMessaging {
    constructor() {
        this.messageHandlers = new Map();
        this.openExternalRegistered = false;
        this.fullscreenRegistered = false;
        this.titleRegistered = false;
        this.windowCloseRegistered = false;
        this.assignWebMessageReceiver();
        this.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.ready);
        this.assignMessageReceivedHandler(IHostMessaging_1.ReceiveFromHostMessageIds.registerOpenExternal, _ => {
            this.registerOpenExternal();
        });
        this.assignMessageReceivedHandler(IHostMessaging_1.ReceiveFromHostMessageIds.registerFullscreenChange, _ => {
            this.registerFullscreenChange();
        });
        this.assignMessageReceivedHandler(IHostMessaging_1.ReceiveFromHostMessageIds.registerTitleChange, _ => {
            this.registerTitleChange();
        });
        this.assignMessageReceivedHandler(IHostMessaging_1.ReceiveFromHostMessageIds.registerWindowClose, _ => {
            this.registerWindowClose();
        });
    }
    sendMessageToHost(id, data) {
        var _a, _b;
        const message = data ? `${id};${data}` : id;
        if ((_a = window.chrome) === null || _a === void 0 ? void 0 : _a.webview) {
            window.chrome.webview.postMessage(message);
        }
        else if ((_b = window.external) === null || _b === void 0 ? void 0 : _b.sendMessage) {
            window.external.sendMessage(message);
        }
        else {
            console.warn("Message to host failed:", message);
        }
    }
    assignWebMessageReceiver() {
        var _a, _b;
        const originalReceiveMessage = (_a = window.external) === null || _a === void 0 ? void 0 : _a.receiveMessage;
        if ((_b = window.chrome) === null || _b === void 0 ? void 0 : _b.webview) {
            window.chrome.webview.addEventListener('message', (event) => {
                if (!this.isBlazorMessage(event.data)) {
                    this.handleWebMessage(event.data);
                }
            });
        }
        if (typeof window !== 'undefined' && window.external) {
            window.external.receiveMessage = (message) => {
                if (this.isBlazorMessage(message)) {
                    if (originalReceiveMessage) {
                        originalReceiveMessage(message);
                    }
                    return;
                }
                this.handleWebMessage(message);
            };
        }
    }
    isBlazorMessage(message) {
        if (typeof message !== 'string')
            return true;
        return message.startsWith('__bwv:')
            || message.startsWith('e=>{')
            || message.includes('BeginInvokeJS')
            || message.includes('AttachToDocument')
            || message.includes('RenderBatch')
            || message.includes('Blazor.');
    }
    handleWebMessage(message) {
        const messageStr = typeof message === 'string' ? message : String(message || '');
        if (!messageStr) {
            console.warn('Received empty or invalid message');
            return;
        }
        let messageId;
        let data;
        if (messageStr.includes(';')) {
            const parts = messageStr.split(';', 2);
            messageId = parts[0];
            data = parts[1];
        }
        else {
            messageId = messageStr;
        }
        const handler = this.messageHandlers.get(messageId);
        if (handler) {
            handler(data);
        }
        else {
            console.warn('No handler registered for message ID:', messageId);
        }
    }
    assignMessageReceivedHandler(messageId, callback) {
        this.messageHandlers.set(messageId, callback);
    }
    unregisterMessageReceivedHandler(messageId) {
        this.messageHandlers.delete(messageId);
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
                this.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.fullscreenEnter);
            else
                this.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.fullscreenExit);
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
            this.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.windowClose);
        };
    }
}
exports["default"] = HostMessaging;


/***/ },

/***/ "./src/InfiniFrame.Js/TsSource/Index.ts"
/*!**********************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/Index.ts ***!
  \**********************************************/
(__unused_webpack_module, exports, __webpack_require__) {


var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
const InfiniFrame_1 = __importDefault(__webpack_require__(/*! ./InfiniFrame */ "./src/InfiniFrame.Js/TsSource/InfiniFrame.ts"));
window.infiniFrame = new InfiniFrame_1.default();


/***/ },

/***/ "./src/InfiniFrame.Js/TsSource/InfiniFrame.ts"
/*!****************************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/InfiniFrame.ts ***!
  \****************************************************/
(__unused_webpack_module, exports, __webpack_require__) {


var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.InfiniFrame = void 0;
const HostMessaging_1 = __importDefault(__webpack_require__(/*! ./HostMessaging */ "./src/InfiniFrame.Js/TsSource/HostMessaging.ts"));
class InfiniFrame {
    constructor() {
        this.HostMessaging = new HostMessaging_1.default();
    }
    sendMessageToHost(id, data) {
        this.HostMessaging.sendMessageToHost(id, data);
    }
    setPointerCapture(element, pointerId) {
        element.setPointerCapture(pointerId);
    }
    releasePointerCapture(element, pointerId) {
        element.releasePointerCapture(pointerId);
    }
}
exports.InfiniFrame = InfiniFrame;
exports["default"] = InfiniFrame;


/***/ },

/***/ "./src/InfiniFrame.Js/TsSource/Observers.ts"
/*!**************************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/Observers.ts ***!
  \**************************************************/
(__unused_webpack_module, exports, __webpack_require__) {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.getTitleObserverTarget = getTitleObserverTarget;
exports.getTitleObserver = getTitleObserver;
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts");
function getTitleObserverTarget() {
    return document.querySelector('title');
}
function getTitleObserver() {
    return new MutationObserver((mutations, _) => {
        mutations.forEach((mutation) => {
            if (mutation.type !== "childList")
                return;
            window.infiniFrame.HostMessaging.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.titleChange, document.title);
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
/******/ 		// Check if module exists (development only)
/******/ 		if (__webpack_modules__[moduleId] === undefined) {
/******/ 			var e = new Error("Cannot find module '" + moduleId + "'");
/******/ 			e.code = 'MODULE_NOT_FOUND';
/******/ 			throw e;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
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
/******/ 	var __webpack_exports__ = __webpack_require__("./src/InfiniFrame.Js/TsSource/Index.ts");
/******/ 	
/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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QyxLQUFLLENBQUMsQ0FBQztRQUN6RyxDQUFDLENBQUM7SUFDTixDQUFDLENBQUM7QUFDTixDQUFDOzs7Ozs7O1VDbkJEO1VBQ0E7O1VBRUE7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7O1VBRUE7VUFDQTs7VUFFQTtVQUNBO1VBQ0E7Ozs7VUU1QkE7VUFDQTtVQUNBO1VBQ0EiLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUZyYW1lLkpzL1RzU291cmNlL0JsYW5rVGFyZ2V0SGFuZGxlci50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUZyYW1lLkpzL1RzU291cmNlL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUZyYW1lLkpzL1RzU291cmNlL0hvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS8uL3NyYy9JbmZpbmlGcmFtZS5Kcy9Uc1NvdXJjZS9JbmRleC50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUZyYW1lLkpzL1RzU291cmNlL0luZmluaUZyYW1lLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvLi9zcmMvSW5maW5pRnJhbWUuSnMvVHNTb3VyY2UvT2JzZXJ2ZXJzLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvd2VicGFjay9ib290c3RyYXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS93ZWJwYWNrL2JlZm9yZS1zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvd2VicGFjay9zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvd2VicGFjay9hZnRlci1zdGFydHVwIl0sInNvdXJjZXNDb250ZW50IjpbIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1NlbmRUb0hvc3RNZXNzYWdlSWRzfSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5mdW5jdGlvbiBpc0V4dGVybmFsTGluayh1cmw6IHN0cmluZyk6IGJvb2xlYW4ge1xyXG4gICAgdHJ5IHtcclxuICAgICAgICByZXR1cm4gbmV3IFVSTCh1cmwsIGxvY2F0aW9uLmhyZWYpLmhvc3RuYW1lICE9PSBsb2NhdGlvbi5ob3N0bmFtZTtcclxuICAgIH0gY2F0Y2gge1xyXG4gICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGFzeW5jIGZ1bmN0aW9uIGJsYW5rVGFyZ2V0SGFuZGxlcihlOiBNb3VzZUV2ZW50KSB7XHJcbiAgICBsZXQgZWwgPSBlLnRhcmdldCBhcyBIVE1MRWxlbWVudCB8IG51bGw7XHJcblxyXG4gICAgd2hpbGUgKGVsICYmIGVsICE9PSBkb2N1bWVudC5ib2R5KSB7XHJcbiAgICAgICAgaWYgKGVsLnRhZ05hbWU/LnRvTG93ZXJDYXNlKCkgIT09IFwiYVwiKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCBhbmNob3IgPSBlbCBhcyBIVE1MQW5jaG9yRWxlbWVudDtcclxuICAgICAgICBpZiAoIWFuY2hvci5ocmVmKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCB0YXJnZXQgPSBhbmNob3IuZ2V0QXR0cmlidXRlKFwidGFyZ2V0XCIpO1xyXG4gICAgICAgIGNvbnN0IHNob3VsZEhhbmRsZSA9IHRhcmdldCA9PT0gXCJfYmxhbmtcIiB8fCBhbmNob3IuaGFzQXR0cmlidXRlKFwiZGF0YS1leHRlcm5hbFwiKSB8fCBpc0V4dGVybmFsTGluayhhbmNob3IuaHJlZik7XHJcblxyXG4gICAgICAgIGlmICghc2hvdWxkSGFuZGxlKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcbiAgICAgICAgd2luZG93LmluZmluaUZyYW1lLkhvc3RNZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMub3BlbkV4dGVybmFsTGluaywgYW5jaG9yLmhyZWYpO1xyXG4gICAgICAgIHJldHVybjtcclxuICAgIH1cclxufSIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmNvbnN0IGluZmluaUZyYW1lIDogc3RyaW5nID0gXCJfX2luZmluaWZyYW1lXCI7XHJcblxyXG5leHBvcnQgY29uc3QgU2VuZFRvSG9zdE1lc3NhZ2VJZHMgPSB7XG4gICAgdGl0bGVDaGFuZ2U6IGAke2luZmluaUZyYW1lfTp0aXRsZTpjaGFuZ2VgLFxuICAgIGZ1bGxzY3JlZW5FbnRlcjogYCR7aW5maW5pRnJhbWV9OmZ1bGxzY3JlZW46ZW50ZXJgLFxuICAgIGZ1bGxzY3JlZW5FeGl0OiBgJHtpbmZpbmlGcmFtZX06ZnVsbHNjcmVlbjpleGl0YCxcbiAgICBvcGVuRXh0ZXJuYWxMaW5rOiBgJHtpbmZpbmlGcmFtZX06b3BlbjpleHRlcm5hbGAsXG4gICAgd2luZG93Q2xvc2U6IGAke2luZmluaUZyYW1lfTp3aW5kb3c6Y2xvc2VgLFxuICAgIHJlYWR5OiBgJHtpbmZpbmlGcmFtZX06cmVhZHlgLFxufVxuXHJcbmV4cG9ydCBjb25zdCBSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzID0ge1xyXG4gICAgcmVnaXN0ZXJPcGVuRXh0ZXJuYWw6IGAke2luZmluaUZyYW1lfTpyZWdpc3RlcjpvcGVuOmV4dGVybmFsYCxcclxuICAgIHJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZTogYCR7aW5maW5pRnJhbWV9OnJlZ2lzdGVyOmZ1bGxzY3JlZW46Y2hhbmdlYCxcclxuICAgIHJlZ2lzdGVyVGl0bGVDaGFuZ2U6IGAke2luZmluaUZyYW1lfTpyZWdpc3Rlcjp0aXRsZTpjaGFuZ2VgLFxyXG4gICAgcmVnaXN0ZXJXaW5kb3dDbG9zZTogYCR7aW5maW5pRnJhbWV9OnJlZ2lzdGVyOndpbmRvdzpjbG9zZWAsXHJcbn1cclxuXHJcbmV4cG9ydCB0eXBlIFNlbmRUb0hvc3RNZXNzYWdlSWQgPSB0eXBlb2YgU2VuZFRvSG9zdE1lc3NhZ2VJZHNba2V5b2YgdHlwZW9mIFNlbmRUb0hvc3RNZXNzYWdlSWRzXTtcclxuZXhwb3J0IHR5cGUgTWVzc2FnZUNhbGxiYWNrID0gKGRhdGE/OiBzdHJpbmcpID0+IHZvaWQ7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIElIb3N0TWVzc2FnaW5nIHtcclxuICAgIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkIHwgc3RyaW5nLCBkYXRhPzogc3RyaW5nKTogdm9pZDtcclxuXHJcbiAgICBhc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nLCBjYWxsYmFjazogTWVzc2FnZUNhbGxiYWNrKTogdm9pZDtcclxuXHJcbiAgICB1bnJlZ2lzdGVyTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZyk6IHZvaWQ7XHJcbn1cbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1xyXG4gICAgSUhvc3RNZXNzYWdpbmcsXHJcbiAgICBNZXNzYWdlQ2FsbGJhY2ssXHJcbiAgICBSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLFxyXG4gICAgU2VuZFRvSG9zdE1lc3NhZ2VJZCwgU2VuZFRvSG9zdE1lc3NhZ2VJZHNcclxufSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuaW1wb3J0IHtibGFua1RhcmdldEhhbmRsZXJ9IGZyb20gXCIuL0JsYW5rVGFyZ2V0SGFuZGxlclwiO1xyXG5pbXBvcnQge2dldFRpdGxlT2JzZXJ2ZXIsIGdldFRpdGxlT2JzZXJ2ZXJUYXJnZXR9IGZyb20gXCIuL09ic2VydmVyc1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmNsYXNzIEhvc3RNZXNzYWdpbmcgaW1wbGVtZW50cyBJSG9zdE1lc3NhZ2luZyB7XHJcbiAgICBwcml2YXRlIG1lc3NhZ2VIYW5kbGVyczogTWFwPHN0cmluZywgTWVzc2FnZUNhbGxiYWNrPiA9IG5ldyBNYXAoKTtcclxuICAgIHByaXZhdGUgb3BlbkV4dGVybmFsUmVnaXN0ZXJlZCA9IGZhbHNlO1xyXG4gICAgcHJpdmF0ZSBmdWxsc2NyZWVuUmVnaXN0ZXJlZCA9IGZhbHNlO1xyXG4gICAgcHJpdmF0ZSB0aXRsZVJlZ2lzdGVyZWQgPSBmYWxzZTtcclxuICAgIHByaXZhdGUgd2luZG93Q2xvc2VSZWdpc3RlcmVkID0gZmFsc2U7XHJcblxyXG4gICAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAgICAgdGhpcy5hc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKTtcclxuICAgICAgICB0aGlzLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLnJlYWR5KTtcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJPcGVuRXh0ZXJuYWwsIF8gPT4ge1xyXG4gICAgICAgICAgICB0aGlzLnJlZ2lzdGVyT3BlbkV4dGVybmFsKCk7XHJcbiAgICAgICAgfSlcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJGdWxsc2NyZWVuQ2hhbmdlLCBfID0+IHtcclxuICAgICAgICAgICAgdGhpcy5yZWdpc3RlckZ1bGxzY3JlZW5DaGFuZ2UoKTtcclxuICAgICAgICB9KVxyXG5cclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3RlclRpdGxlQ2hhbmdlLCBfID0+IHtcclxuICAgICAgICAgICAgdGhpcy5yZWdpc3RlclRpdGxlQ2hhbmdlKCk7XHJcbiAgICAgICAgfSlcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJXaW5kb3dDbG9zZSwgXyA9PiB7XHJcbiAgICAgICAgICAgIHRoaXMucmVnaXN0ZXJXaW5kb3dDbG9zZSgpO1xyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkIHwgc3RyaW5nLCBkYXRhPzogc3RyaW5nKSB7XHJcbiAgICAgICAgY29uc3QgbWVzc2FnZSA9IGRhdGEgPyBgJHtpZH07JHtkYXRhfWAgOiBpZDtcclxuICAgICAgICBcclxuICAgICAgICAvLyBUT0RPIC0gZGV0ZXJtaW5lIG1lc3NhZ2luZyBtZXRob2RzIGZvciBQaG90aW5vLk5FVCBmb3IgYWxsIHBsYXRmb3Jtc1xyXG4gICAgICAgIGlmICh3aW5kb3cuY2hyb21lPy53ZWJ2aWV3KSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5jaHJvbWUud2Vidmlldy5wb3N0TWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICB9IGVsc2UgaWYgKHdpbmRvdy5leHRlcm5hbD8uc2VuZE1lc3NhZ2UpIHtcclxuICAgICAgICAgICAgd2luZG93LmV4dGVybmFsLnNlbmRNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUud2FybihcIk1lc3NhZ2UgdG8gaG9zdCBmYWlsZWQ6XCIsIG1lc3NhZ2UpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIGFzc2lnbldlYk1lc3NhZ2VSZWNlaXZlcigpIHtcclxuICAgICAgICAvLyBTdG9yZSB0aGUgb3JpZ2luYWwgcmVjZWl2ZU1lc3NhZ2UgaWYgaXQgZXhpc3RzIChmb3IgQmxhem9yIGNvbXBhdGliaWxpdHkpXHJcbiAgICAgICAgY29uc3Qgb3JpZ2luYWxSZWNlaXZlTWVzc2FnZSA9IHdpbmRvdy5leHRlcm5hbD8ucmVjZWl2ZU1lc3NhZ2U7XHJcblxyXG4gICAgICAgIC8vIEhhbmRsZSBXZWJWaWV3MiBtZXNzYWdlcyAoV2luZG93cylcclxuICAgICAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xyXG4gICAgICAgICAgICB3aW5kb3cuY2hyb21lLndlYnZpZXcuYWRkRXZlbnRMaXN0ZW5lcignbWVzc2FnZScsIChldmVudCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKCF0aGlzLmlzQmxhem9yTWVzc2FnZShldmVudC5kYXRhKSkge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuaGFuZGxlV2ViTWVzc2FnZShldmVudC5kYXRhKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBIYW5kbGUgZ2VuZXJhbCBQaG90aW5vIG1lc3NhZ2VzIChjcm9zcy1wbGF0Zm9ybSlcclxuICAgICAgICBpZiAodHlwZW9mIHdpbmRvdyAhPT0gJ3VuZGVmaW5lZCcgJiYgd2luZG93LmV4dGVybmFsKSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5leHRlcm5hbC5yZWNlaXZlTWVzc2FnZSA9IChtZXNzYWdlOiBhbnkpID0+IHtcclxuICAgICAgICAgICAgICAgIC8vIENoZWNrIGlmIGl0J3MgYSBCbGF6b3IgbWVzc2FnZSBhbmQgcGFzcyBpdCB0byB0aGUgb3JpZ2luYWwgaGFuZGxlclxyXG4gICAgICAgICAgICAgICAgaWYgKHRoaXMuaXNCbGF6b3JNZXNzYWdlKG1lc3NhZ2UpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKG9yaWdpbmFsUmVjZWl2ZU1lc3NhZ2UpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgb3JpZ2luYWxSZWNlaXZlTWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIC8vIEhhbmRsZSBvdXIgY3VzdG9tIG1lc3NhZ2VzXHJcbiAgICAgICAgICAgICAgICB0aGlzLmhhbmRsZVdlYk1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICAgICAgICAgIH07XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgaXNCbGF6b3JNZXNzYWdlKG1lc3NhZ2U6IGFueSk6IGJvb2xlYW4ge1xyXG4gICAgICAgIGlmICh0eXBlb2YgbWVzc2FnZSAhPT0gJ3N0cmluZycpIHJldHVybiB0cnVlOyAvLyBBc3N1bWUgbm9uLXN0cmluZyBtZXNzYWdlcyBhcmUgQmxhem9yXHJcblxyXG4gICAgICAgIC8vIENoZWNrIGZvciBjb21tb24gQmxhem9yIG1lc3NhZ2UgcGF0dGVybnNcclxuICAgICAgICByZXR1cm4gbWVzc2FnZS5zdGFydHNXaXRoKCdfX2J3djonKVxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLnN0YXJ0c1dpdGgoJ2U9PnsnKVxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLmluY2x1ZGVzKCdCZWdpbkludm9rZUpTJylcclxuICAgICAgICAgICAgfHwgbWVzc2FnZS5pbmNsdWRlcygnQXR0YWNoVG9Eb2N1bWVudCcpXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2UuaW5jbHVkZXMoJ1JlbmRlckJhdGNoJylcclxuICAgICAgICAgICAgfHwgbWVzc2FnZS5pbmNsdWRlcygnQmxhem9yLicpO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgaGFuZGxlV2ViTWVzc2FnZShtZXNzYWdlOiBhbnkpIHtcclxuICAgICAgICAvLyBFbnN1cmUgbWVzc2FnZSBpcyBhIHN0cmluZ1xyXG4gICAgICAgIGNvbnN0IG1lc3NhZ2VTdHIgPSB0eXBlb2YgbWVzc2FnZSA9PT0gJ3N0cmluZycgPyBtZXNzYWdlIDogU3RyaW5nKG1lc3NhZ2UgfHwgJycpO1xyXG5cclxuICAgICAgICBpZiAoIW1lc3NhZ2VTdHIpIHtcclxuICAgICAgICAgICAgY29uc29sZS53YXJuKCdSZWNlaXZlZCBlbXB0eSBvciBpbnZhbGlkIG1lc3NhZ2UnKTtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgbGV0IG1lc3NhZ2VJZDogc3RyaW5nO1xyXG4gICAgICAgIGxldCBkYXRhOiBzdHJpbmcgfCB1bmRlZmluZWQ7XHJcblxyXG4gICAgICAgIC8vIFBhcnNlIG1lc3NhZ2UgLSBjaGVjayBpZiBpdCBjb250YWlucyBkYXRhIHNlcGFyYXRlZCBieSBzZW1pY29sb25cclxuICAgICAgICBpZiAobWVzc2FnZVN0ci5pbmNsdWRlcygnOycpKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IHBhcnRzID0gbWVzc2FnZVN0ci5zcGxpdCgnOycsIDIpO1xyXG4gICAgICAgICAgICBtZXNzYWdlSWQgPSBwYXJ0c1swXTtcclxuICAgICAgICAgICAgZGF0YSA9IHBhcnRzWzFdO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIG1lc3NhZ2VJZCA9IG1lc3NhZ2VTdHI7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBFeGVjdXRlIHJlZ2lzdGVyZWQgaGFuZGxlclxyXG4gICAgICAgIGNvbnN0IGhhbmRsZXIgPSB0aGlzLm1lc3NhZ2VIYW5kbGVycy5nZXQobWVzc2FnZUlkKTtcclxuICAgICAgICBpZiAoaGFuZGxlcikge1xyXG4gICAgICAgICAgICBoYW5kbGVyKGRhdGEpO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUud2FybignTm8gaGFuZGxlciByZWdpc3RlcmVkIGZvciBtZXNzYWdlIElEOicsIG1lc3NhZ2VJZCk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBhc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nLCBjYWxsYmFjazogTWVzc2FnZUNhbGxiYWNrKSB7XHJcbiAgICAgICAgdGhpcy5tZXNzYWdlSGFuZGxlcnMuc2V0KG1lc3NhZ2VJZCwgY2FsbGJhY2spO1xyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyB1bnJlZ2lzdGVyTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZykge1xyXG4gICAgICAgIHRoaXMubWVzc2FnZUhhbmRsZXJzLmRlbGV0ZShtZXNzYWdlSWQpO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgcmVnaXN0ZXJPcGVuRXh0ZXJuYWwoKSB7XHJcbiAgICAgICAgaWYgKHRoaXMub3BlbkV4dGVybmFsUmVnaXN0ZXJlZCkgcmV0dXJuO1xyXG4gICAgICAgIHRoaXMub3BlbkV4dGVybmFsUmVnaXN0ZXJlZCA9IHRydWU7XHJcbiAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImNsaWNrXCIsIGJsYW5rVGFyZ2V0SGFuZGxlciwge2NhcHR1cmU6IHRydWV9KTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZSgpIHtcclxuICAgICAgICBpZiAodGhpcy5mdWxsc2NyZWVuUmVnaXN0ZXJlZCkgcmV0dXJuO1xyXG4gICAgICAgIHRoaXMuZnVsbHNjcmVlblJlZ2lzdGVyZWQgPSB0cnVlO1xyXG4gICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJmdWxsc2NyZWVuY2hhbmdlXCIsIChfOiBFdmVudCkgPT4ge1xyXG4gICAgICAgICAgICBpZiAoZG9jdW1lbnQuZnVsbHNjcmVlbkVsZW1lbnQpIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMuZnVsbHNjcmVlbkVudGVyKTtcclxuICAgICAgICAgICAgZWxzZSB0aGlzLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLmZ1bGxzY3JlZW5FeGl0KTtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImtleWRvd25cIiwgYXN5bmMgKGU6IEtleWJvYXJkRXZlbnQpID0+IHtcclxuICAgICAgICAgICAgaWYgKGUua2V5ICE9PSBcIkYxMVwiKSByZXR1cm47XHJcbiAgICAgICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkgYXdhaXQgZG9jdW1lbnQuZXhpdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICAgICAgZWxzZSBhd2FpdCBkb2N1bWVudC5ib2R5LnJlcXVlc3RGdWxsc2NyZWVuKCk7XHJcbiAgICAgICAgfSk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWdpc3RlclRpdGxlQ2hhbmdlKCkge1xyXG4gICAgICAgIGlmICh0aGlzLnRpdGxlUmVnaXN0ZXJlZCkgcmV0dXJuO1xyXG4gICAgICAgIHRoaXMudGl0bGVSZWdpc3RlcmVkID0gdHJ1ZTtcclxuICAgICAgICBjb25zdCB0aXRsZVRhcmdldCA9IGdldFRpdGxlT2JzZXJ2ZXJUYXJnZXQoKTtcclxuICAgICAgICBpZiAodGl0bGVUYXJnZXQpIHtcclxuICAgICAgICAgICAgZ2V0VGl0bGVPYnNlcnZlcigpLm9ic2VydmUodGl0bGVUYXJnZXQsIHtjaGlsZExpc3Q6IHRydWV9KTtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgaGVhZFRhcmdldCA9IGRvY3VtZW50LmhlYWQgfHwgZG9jdW1lbnQuZG9jdW1lbnRFbGVtZW50O1xyXG4gICAgICAgIGlmICghaGVhZFRhcmdldCkgcmV0dXJuO1xyXG5cclxuICAgICAgICBjb25zdCBoZWFkT2JzZXJ2ZXIgPSBuZXcgTXV0YXRpb25PYnNlcnZlcigoKSA9PiB7XHJcbiAgICAgICAgICAgIGNvbnN0IHRhcmdldCA9IGdldFRpdGxlT2JzZXJ2ZXJUYXJnZXQoKTtcclxuICAgICAgICAgICAgaWYgKCF0YXJnZXQpIHJldHVybjtcclxuICAgICAgICAgICAgaGVhZE9ic2VydmVyLmRpc2Nvbm5lY3QoKTtcclxuICAgICAgICAgICAgZ2V0VGl0bGVPYnNlcnZlcigpLm9ic2VydmUodGFyZ2V0LCB7Y2hpbGRMaXN0OiB0cnVlfSk7XHJcbiAgICAgICAgfSk7XHJcbiAgICAgICAgaGVhZE9ic2VydmVyLm9ic2VydmUoaGVhZFRhcmdldCwge2NoaWxkTGlzdDogdHJ1ZSwgc3VidHJlZTogdHJ1ZX0pO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgcmVnaXN0ZXJXaW5kb3dDbG9zZSgpIHtcclxuICAgICAgICBpZiAodGhpcy53aW5kb3dDbG9zZVJlZ2lzdGVyZWQpIHJldHVybjtcclxuICAgICAgICB0aGlzLndpbmRvd0Nsb3NlUmVnaXN0ZXJlZCA9IHRydWU7XHJcbiAgICAgICAgd2luZG93LmNsb3NlID0gKCkgPT4ge1xyXG4gICAgICAgICAgICB0aGlzLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLndpbmRvd0Nsb3NlKTtcclxuICAgICAgICB9O1xyXG4gICAgfVxyXG59XHJcblxyXG5leHBvcnQgZGVmYXVsdCBIb3N0TWVzc2FnaW5nXHJcbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQgSW5maW5pRnJhbWUgZnJvbSBcIi4vSW5maW5pRnJhbWVcIjtcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCB7fTtcclxud2luZG93LmluZmluaUZyYW1lID0gbmV3IEluZmluaUZyYW1lKCk7XHJcbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge0lJbmZpbmlGcmFtZX0gZnJvbSBcIi4vQ29udHJhY3RzL0lJbmZpbmlGcmFtZVwiO1xyXG5pbXBvcnQge0lIb3N0TWVzc2FnaW5nLCBTZW5kVG9Ib3N0TWVzc2FnZUlkfSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuaW1wb3J0IEhvc3RNZXNzYWdpbmcgZnJvbSBcIi4vSG9zdE1lc3NhZ2luZ1wiO1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGNsYXNzIEluZmluaUZyYW1lIGltcGxlbWVudHMgSUluZmluaUZyYW1lIHtcclxuICAgIEhvc3RNZXNzYWdpbmc6IElIb3N0TWVzc2FnaW5nID0gbmV3IEhvc3RNZXNzYWdpbmcoKTtcclxuXHJcbiAgICAvLyBPdmVybG9hZCB0byBtYWtlIGEgZGV2J3MgbGlmZSBlYXNpZXIgaW5zdGVhZCBvZiBoYXZpbmcgdG8gZ28gdG8gdGhlIEhvc3RNZXNzYWdpbmcgY2xhc3NcclxuICAgIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkLCBkYXRhPzogc3RyaW5nKSB7XHJcbiAgICAgICAgdGhpcy5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KGlkLCBkYXRhKTtcclxuICAgIH1cclxuXHJcbiAgICBzZXRQb2ludGVyQ2FwdHVyZShlbGVtZW50OiBFbGVtZW50LCBwb2ludGVySWQ6IG51bWJlcikge1xyXG4gICAgICAgIGVsZW1lbnQuc2V0UG9pbnRlckNhcHR1cmUocG9pbnRlcklkKTtcclxuICAgIH1cclxuXHJcbiAgICByZWxlYXNlUG9pbnRlckNhcHR1cmUoZWxlbWVudDogRWxlbWVudCwgcG9pbnRlcklkOiBudW1iZXIpIHtcclxuICAgICAgICBlbGVtZW50LnJlbGVhc2VQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG4gICAgfVxyXG59XHJcblxyXG5leHBvcnQgZGVmYXVsdCBJbmZpbmlGcmFtZSIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1NlbmRUb0hvc3RNZXNzYWdlSWRzfSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgZnVuY3Rpb24gZ2V0VGl0bGVPYnNlcnZlclRhcmdldCgpOiBIVE1MVGl0bGVFbGVtZW50IHwgbnVsbCB7XG4gICAgcmV0dXJuIGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IoJ3RpdGxlJyk7XG59XG5cclxuZXhwb3J0IGZ1bmN0aW9uIGdldFRpdGxlT2JzZXJ2ZXIoKTogTXV0YXRpb25PYnNlcnZlciB7XHJcbiAgICByZXR1cm4gbmV3IE11dGF0aW9uT2JzZXJ2ZXIoKG11dGF0aW9ucywgXykgPT4ge1xyXG4gICAgICAgIG11dGF0aW9ucy5mb3JFYWNoKChtdXRhdGlvbikgPT4ge1xyXG4gICAgICAgICAgICBpZiAobXV0YXRpb24udHlwZSAhPT0gXCJjaGlsZExpc3RcIikgcmV0dXJuO1xyXG4gICAgICAgICAgICB3aW5kb3cuaW5maW5pRnJhbWUuSG9zdE1lc3NhZ2luZy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy50aXRsZUNoYW5nZSwgZG9jdW1lbnQudGl0bGUpO1xyXG4gICAgICAgIH0pXHJcbiAgICB9KVxyXG59XG4iLCIvLyBUaGUgbW9kdWxlIGNhY2hlXG52YXIgX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fID0ge307XG5cbi8vIFRoZSByZXF1aXJlIGZ1bmN0aW9uXG5mdW5jdGlvbiBfX3dlYnBhY2tfcmVxdWlyZV9fKG1vZHVsZUlkKSB7XG5cdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuXHR2YXIgY2FjaGVkTW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXTtcblx0aWYgKGNhY2hlZE1vZHVsZSAhPT0gdW5kZWZpbmVkKSB7XG5cdFx0cmV0dXJuIGNhY2hlZE1vZHVsZS5leHBvcnRzO1xuXHR9XG5cdC8vIENoZWNrIGlmIG1vZHVsZSBleGlzdHMgKGRldmVsb3BtZW50IG9ubHkpXG5cdGlmIChfX3dlYnBhY2tfbW9kdWxlc19fW21vZHVsZUlkXSA9PT0gdW5kZWZpbmVkKSB7XG5cdFx0dmFyIGUgPSBuZXcgRXJyb3IoXCJDYW5ub3QgZmluZCBtb2R1bGUgJ1wiICsgbW9kdWxlSWQgKyBcIidcIik7XG5cdFx0ZS5jb2RlID0gJ01PRFVMRV9OT1RfRk9VTkQnO1xuXHRcdHRocm93IGU7XG5cdH1cblx0Ly8gQ3JlYXRlIGEgbmV3IG1vZHVsZSAoYW5kIHB1dCBpdCBpbnRvIHRoZSBjYWNoZSlcblx0dmFyIG1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF0gPSB7XG5cdFx0Ly8gbm8gbW9kdWxlLmlkIG5lZWRlZFxuXHRcdC8vIG5vIG1vZHVsZS5sb2FkZWQgbmVlZGVkXG5cdFx0ZXhwb3J0czoge31cblx0fTtcblxuXHQvLyBFeGVjdXRlIHRoZSBtb2R1bGUgZnVuY3Rpb25cblx0X193ZWJwYWNrX21vZHVsZXNfX1ttb2R1bGVJZF0uY2FsbChtb2R1bGUuZXhwb3J0cywgbW9kdWxlLCBtb2R1bGUuZXhwb3J0cywgX193ZWJwYWNrX3JlcXVpcmVfXyk7XG5cblx0Ly8gUmV0dXJuIHRoZSBleHBvcnRzIG9mIHRoZSBtb2R1bGVcblx0cmV0dXJuIG1vZHVsZS5leHBvcnRzO1xufVxuXG4iLCIiLCIvLyBzdGFydHVwXG4vLyBMb2FkIGVudHJ5IG1vZHVsZSBhbmQgcmV0dXJuIGV4cG9ydHNcbi8vIFRoaXMgZW50cnkgbW9kdWxlIGlzIHJlZmVyZW5jZWQgYnkgb3RoZXIgbW9kdWxlcyBzbyBpdCBjYW4ndCBiZSBpbmxpbmVkXG52YXIgX193ZWJwYWNrX2V4cG9ydHNfXyA9IF9fd2VicGFja19yZXF1aXJlX18oXCIuL3NyYy9JbmZpbmlGcmFtZS5Kcy9Uc1NvdXJjZS9JbmRleC50c1wiKTtcbiIsIiJdLCJuYW1lcyI6W10sInNvdXJjZVJvb3QiOiIifQ==