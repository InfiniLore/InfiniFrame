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
/******/ 	var __webpack_exports__ = __webpack_require__("./src/InfiniFrame.Js/TsSource/Index.ts");
/******/ 	
/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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QyxLQUFLLENBQUMsQ0FBQztRQUN6RyxDQUFDLENBQUM7SUFDTixDQUFDLENBQUM7QUFDTixDQUFDOzs7Ozs7O1VDbkJEO1VBQ0E7O1VBRUE7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7O1VBRUE7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTs7VUFFQTtVQUNBO1VBQ0E7Ozs7VUU1QkE7VUFDQTtVQUNBO1VBQ0EiLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUZyYW1lLkpzL1RzU291cmNlL0JsYW5rVGFyZ2V0SGFuZGxlci50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUZyYW1lLkpzL1RzU291cmNlL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUZyYW1lLkpzL1RzU291cmNlL0hvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS8uL3NyYy9JbmZpbmlGcmFtZS5Kcy9Uc1NvdXJjZS9JbmRleC50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUZyYW1lLkpzL1RzU291cmNlL0luZmluaUZyYW1lLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvLi9zcmMvSW5maW5pRnJhbWUuSnMvVHNTb3VyY2UvT2JzZXJ2ZXJzLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvd2VicGFjay9ib290c3RyYXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS93ZWJwYWNrL2JlZm9yZS1zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvd2VicGFjay9zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvd2VicGFjay9hZnRlci1zdGFydHVwIl0sInNvdXJjZXNDb250ZW50IjpbIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1NlbmRUb0hvc3RNZXNzYWdlSWRzfSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5mdW5jdGlvbiBpc0V4dGVybmFsTGluayh1cmw6IHN0cmluZyk6IGJvb2xlYW4ge1xyXG4gICAgdHJ5IHtcclxuICAgICAgICByZXR1cm4gbmV3IFVSTCh1cmwsIGxvY2F0aW9uLmhyZWYpLmhvc3RuYW1lICE9PSBsb2NhdGlvbi5ob3N0bmFtZTtcclxuICAgIH0gY2F0Y2gge1xyXG4gICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGFzeW5jIGZ1bmN0aW9uIGJsYW5rVGFyZ2V0SGFuZGxlcihlOiBNb3VzZUV2ZW50KSB7XHJcbiAgICBsZXQgZWwgPSBlLnRhcmdldCBhcyBIVE1MRWxlbWVudCB8IG51bGw7XHJcblxyXG4gICAgd2hpbGUgKGVsICYmIGVsICE9PSBkb2N1bWVudC5ib2R5KSB7XHJcbiAgICAgICAgaWYgKGVsLnRhZ05hbWU/LnRvTG93ZXJDYXNlKCkgIT09IFwiYVwiKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCBhbmNob3IgPSBlbCBhcyBIVE1MQW5jaG9yRWxlbWVudDtcclxuICAgICAgICBpZiAoIWFuY2hvci5ocmVmKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCB0YXJnZXQgPSBhbmNob3IuZ2V0QXR0cmlidXRlKFwidGFyZ2V0XCIpO1xyXG4gICAgICAgIGNvbnN0IHNob3VsZEhhbmRsZSA9IHRhcmdldCA9PT0gXCJfYmxhbmtcIiB8fCBhbmNob3IuaGFzQXR0cmlidXRlKFwiZGF0YS1leHRlcm5hbFwiKSB8fCBpc0V4dGVybmFsTGluayhhbmNob3IuaHJlZik7XHJcblxyXG4gICAgICAgIGlmICghc2hvdWxkSGFuZGxlKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcbiAgICAgICAgd2luZG93LmluZmluaUZyYW1lLkhvc3RNZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMub3BlbkV4dGVybmFsTGluaywgYW5jaG9yLmhyZWYpO1xyXG4gICAgICAgIHJldHVybjtcclxuICAgIH1cclxufSIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmNvbnN0IGluZmluaUZyYW1lOiBzdHJpbmcgPSBcIl9faW5maW5pZnJhbWVcIjtcclxuXHJcbmV4cG9ydCBjb25zdCBTZW5kVG9Ib3N0TWVzc2FnZUlkcyA9IHtcclxuICAgIHRpdGxlQ2hhbmdlOiBgJHtpbmZpbmlGcmFtZX06dGl0bGU6Y2hhbmdlYCxcclxuICAgIGZ1bGxzY3JlZW5FbnRlcjogYCR7aW5maW5pRnJhbWV9OmZ1bGxzY3JlZW46ZW50ZXJgLFxyXG4gICAgZnVsbHNjcmVlbkV4aXQ6IGAke2luZmluaUZyYW1lfTpmdWxsc2NyZWVuOmV4aXRgLFxyXG4gICAgb3BlbkV4dGVybmFsTGluazogYCR7aW5maW5pRnJhbWV9Om9wZW46ZXh0ZXJuYWxgLFxyXG4gICAgd2luZG93Q2xvc2U6IGAke2luZmluaUZyYW1lfTp3aW5kb3c6Y2xvc2VgLFxyXG4gICAgcmVhZHk6IGAke2luZmluaUZyYW1lfTpyZWFkeWAsXHJcbn1cclxuXHJcbmV4cG9ydCBjb25zdCBSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzID0ge1xyXG4gICAgcmVnaXN0ZXJPcGVuRXh0ZXJuYWw6IGAke2luZmluaUZyYW1lfTpyZWdpc3RlcjpvcGVuOmV4dGVybmFsYCxcclxuICAgIHJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZTogYCR7aW5maW5pRnJhbWV9OnJlZ2lzdGVyOmZ1bGxzY3JlZW46Y2hhbmdlYCxcclxuICAgIHJlZ2lzdGVyVGl0bGVDaGFuZ2U6IGAke2luZmluaUZyYW1lfTpyZWdpc3Rlcjp0aXRsZTpjaGFuZ2VgLFxyXG4gICAgcmVnaXN0ZXJXaW5kb3dDbG9zZTogYCR7aW5maW5pRnJhbWV9OnJlZ2lzdGVyOndpbmRvdzpjbG9zZWAsXHJcbn1cclxuXHJcbmV4cG9ydCB0eXBlIFNlbmRUb0hvc3RNZXNzYWdlSWQgPSB0eXBlb2YgU2VuZFRvSG9zdE1lc3NhZ2VJZHNba2V5b2YgdHlwZW9mIFNlbmRUb0hvc3RNZXNzYWdlSWRzXTtcclxuZXhwb3J0IHR5cGUgTWVzc2FnZUNhbGxiYWNrID0gKGRhdGE/OiBzdHJpbmcpID0+IHZvaWQ7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIElIb3N0TWVzc2FnaW5nIHtcclxuICAgIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkIHwgc3RyaW5nLCBkYXRhPzogc3RyaW5nKTogdm9pZDtcclxuXHJcbiAgICBhc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nLCBjYWxsYmFjazogTWVzc2FnZUNhbGxiYWNrKTogdm9pZDtcclxuXHJcbiAgICB1bnJlZ2lzdGVyTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZyk6IHZvaWQ7XHJcbn1cclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7XHJcbiAgICBJSG9zdE1lc3NhZ2luZyxcclxuICAgIE1lc3NhZ2VDYWxsYmFjayxcclxuICAgIFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMsXHJcbiAgICBTZW5kVG9Ib3N0TWVzc2FnZUlkLCBTZW5kVG9Ib3N0TWVzc2FnZUlkc1xyXG59IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5pbXBvcnQge2JsYW5rVGFyZ2V0SGFuZGxlcn0gZnJvbSBcIi4vQmxhbmtUYXJnZXRIYW5kbGVyXCI7XHJcbmltcG9ydCB7Z2V0VGl0bGVPYnNlcnZlciwgZ2V0VGl0bGVPYnNlcnZlclRhcmdldH0gZnJvbSBcIi4vT2JzZXJ2ZXJzXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuY2xhc3MgSG9zdE1lc3NhZ2luZyBpbXBsZW1lbnRzIElIb3N0TWVzc2FnaW5nIHtcclxuICAgIHByaXZhdGUgbWVzc2FnZUhhbmRsZXJzOiBNYXA8c3RyaW5nLCBNZXNzYWdlQ2FsbGJhY2s+ID0gbmV3IE1hcCgpO1xyXG4gICAgcHJpdmF0ZSBvcGVuRXh0ZXJuYWxSZWdpc3RlcmVkID0gZmFsc2U7XHJcbiAgICBwcml2YXRlIGZ1bGxzY3JlZW5SZWdpc3RlcmVkID0gZmFsc2U7XHJcbiAgICBwcml2YXRlIHRpdGxlUmVnaXN0ZXJlZCA9IGZhbHNlO1xyXG4gICAgcHJpdmF0ZSB3aW5kb3dDbG9zZVJlZ2lzdGVyZWQgPSBmYWxzZTtcclxuXHJcbiAgICBjb25zdHJ1Y3RvcigpIHtcclxuICAgICAgICB0aGlzLmFzc2lnbldlYk1lc3NhZ2VSZWNlaXZlcigpO1xyXG4gICAgICAgIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMucmVhZHkpO1xyXG5cclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3Rlck9wZW5FeHRlcm5hbCwgXyA9PiB7XHJcbiAgICAgICAgICAgIHRoaXMucmVnaXN0ZXJPcGVuRXh0ZXJuYWwoKTtcclxuICAgICAgICB9KVxyXG5cclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3RlckZ1bGxzY3JlZW5DaGFuZ2UsIF8gPT4ge1xyXG4gICAgICAgICAgICB0aGlzLnJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZSgpO1xyXG4gICAgICAgIH0pXHJcblxyXG4gICAgICAgIHRoaXMuYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLnJlZ2lzdGVyVGl0bGVDaGFuZ2UsIF8gPT4ge1xyXG4gICAgICAgICAgICB0aGlzLnJlZ2lzdGVyVGl0bGVDaGFuZ2UoKTtcclxuICAgICAgICB9KVxyXG5cclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3RlcldpbmRvd0Nsb3NlLCBfID0+IHtcclxuICAgICAgICAgICAgdGhpcy5yZWdpc3RlcldpbmRvd0Nsb3NlKCk7XHJcbiAgICAgICAgfSlcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgc2VuZE1lc3NhZ2VUb0hvc3QoaWQ6IFNlbmRUb0hvc3RNZXNzYWdlSWQgfCBzdHJpbmcsIGRhdGE/OiBzdHJpbmcpIHtcclxuICAgICAgICBjb25zdCBtZXNzYWdlID0gZGF0YSA/IGAke2lkfTske2RhdGF9YCA6IGlkO1xyXG5cclxuICAgICAgICAvLyBUT0RPIC0gZGV0ZXJtaW5lIG1lc3NhZ2luZyBtZXRob2RzIGZvciBJbmZpbmlGcmFtZS5ORVQgZm9yIGFsbCBwbGF0Zm9ybXNcclxuICAgICAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xyXG4gICAgICAgICAgICB3aW5kb3cuY2hyb21lLndlYnZpZXcucG9zdE1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICAgICAgfSBlbHNlIGlmICh3aW5kb3cuZXh0ZXJuYWw/LnNlbmRNZXNzYWdlKSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5leHRlcm5hbC5zZW5kTWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICBjb25zb2xlLndhcm4oXCJNZXNzYWdlIHRvIGhvc3QgZmFpbGVkOlwiLCBtZXNzYWdlKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBhc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKSB7XHJcbiAgICAgICAgLy8gU3RvcmUgdGhlIG9yaWdpbmFsIHJlY2VpdmVNZXNzYWdlIGlmIGl0IGV4aXN0cyAoZm9yIEJsYXpvciBjb21wYXRpYmlsaXR5KVxyXG4gICAgICAgIGNvbnN0IG9yaWdpbmFsUmVjZWl2ZU1lc3NhZ2UgPSB3aW5kb3cuZXh0ZXJuYWw/LnJlY2VpdmVNZXNzYWdlO1xyXG5cclxuICAgICAgICAvLyBIYW5kbGUgV2ViVmlldzIgbWVzc2FnZXMgKFdpbmRvd3MpXHJcbiAgICAgICAgaWYgKHdpbmRvdy5jaHJvbWU/LndlYnZpZXcpIHtcclxuICAgICAgICAgICAgd2luZG93LmNocm9tZS53ZWJ2aWV3LmFkZEV2ZW50TGlzdGVuZXIoJ21lc3NhZ2UnLCAoZXZlbnQpID0+IHtcclxuICAgICAgICAgICAgICAgIGlmICghdGhpcy5pc0JsYXpvck1lc3NhZ2UoZXZlbnQuZGF0YSkpIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLmhhbmRsZVdlYk1lc3NhZ2UoZXZlbnQuZGF0YSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gSGFuZGxlIGdlbmVyYWwgSW5maW5pRnJhbWUgbWVzc2FnZXMgKGNyb3NzLXBsYXRmb3JtKVxyXG4gICAgICAgIGlmICh0eXBlb2Ygd2luZG93ICE9PSAndW5kZWZpbmVkJyAmJiB3aW5kb3cuZXh0ZXJuYWwpIHtcclxuICAgICAgICAgICAgd2luZG93LmV4dGVybmFsLnJlY2VpdmVNZXNzYWdlID0gKG1lc3NhZ2U6IGFueSkgPT4ge1xyXG4gICAgICAgICAgICAgICAgLy8gQ2hlY2sgaWYgaXQncyBhIEJsYXpvciBtZXNzYWdlIGFuZCBwYXNzIGl0IHRvIHRoZSBvcmlnaW5hbCBoYW5kbGVyXHJcbiAgICAgICAgICAgICAgICBpZiAodGhpcy5pc0JsYXpvck1lc3NhZ2UobWVzc2FnZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAob3JpZ2luYWxSZWNlaXZlTWVzc2FnZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBvcmlnaW5hbFJlY2VpdmVNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgLy8gSGFuZGxlIG91ciBjdXN0b20gbWVzc2FnZXNcclxuICAgICAgICAgICAgICAgIHRoaXMuaGFuZGxlV2ViTWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICAgICAgfTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBpc0JsYXpvck1lc3NhZ2UobWVzc2FnZTogYW55KTogYm9vbGVhbiB7XHJcbiAgICAgICAgaWYgKHR5cGVvZiBtZXNzYWdlICE9PSAnc3RyaW5nJykgcmV0dXJuIHRydWU7IC8vIEFzc3VtZSBub24tc3RyaW5nIG1lc3NhZ2VzIGFyZSBCbGF6b3JcclxuXHJcbiAgICAgICAgLy8gQ2hlY2sgZm9yIGNvbW1vbiBCbGF6b3IgbWVzc2FnZSBwYXR0ZXJuc1xyXG4gICAgICAgIHJldHVybiBtZXNzYWdlLnN0YXJ0c1dpdGgoJ19fYnd2OicpXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2Uuc3RhcnRzV2l0aCgnZT0+eycpXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2UuaW5jbHVkZXMoJ0JlZ2luSW52b2tlSlMnKVxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLmluY2x1ZGVzKCdBdHRhY2hUb0RvY3VtZW50JylcclxuICAgICAgICAgICAgfHwgbWVzc2FnZS5pbmNsdWRlcygnUmVuZGVyQmF0Y2gnKVxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLmluY2x1ZGVzKCdCbGF6b3IuJyk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBoYW5kbGVXZWJNZXNzYWdlKG1lc3NhZ2U6IGFueSkge1xyXG4gICAgICAgIC8vIEVuc3VyZSBtZXNzYWdlIGlzIGEgc3RyaW5nXHJcbiAgICAgICAgY29uc3QgbWVzc2FnZVN0ciA9IHR5cGVvZiBtZXNzYWdlID09PSAnc3RyaW5nJyA/IG1lc3NhZ2UgOiBTdHJpbmcobWVzc2FnZSB8fCAnJyk7XHJcblxyXG4gICAgICAgIGlmICghbWVzc2FnZVN0cikge1xyXG4gICAgICAgICAgICBjb25zb2xlLndhcm4oJ1JlY2VpdmVkIGVtcHR5IG9yIGludmFsaWQgbWVzc2FnZScpO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBsZXQgbWVzc2FnZUlkOiBzdHJpbmc7XHJcbiAgICAgICAgbGV0IGRhdGE6IHN0cmluZyB8IHVuZGVmaW5lZDtcclxuXHJcbiAgICAgICAgLy8gUGFyc2UgbWVzc2FnZSAtIGNoZWNrIGlmIGl0IGNvbnRhaW5zIGRhdGEgc2VwYXJhdGVkIGJ5IHNlbWljb2xvblxyXG4gICAgICAgIGlmIChtZXNzYWdlU3RyLmluY2x1ZGVzKCc7JykpIHtcclxuICAgICAgICAgICAgY29uc3QgcGFydHMgPSBtZXNzYWdlU3RyLnNwbGl0KCc7JywgMik7XHJcbiAgICAgICAgICAgIG1lc3NhZ2VJZCA9IHBhcnRzWzBdO1xyXG4gICAgICAgICAgICBkYXRhID0gcGFydHNbMV07XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgbWVzc2FnZUlkID0gbWVzc2FnZVN0cjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIEV4ZWN1dGUgcmVnaXN0ZXJlZCBoYW5kbGVyXHJcbiAgICAgICAgY29uc3QgaGFuZGxlciA9IHRoaXMubWVzc2FnZUhhbmRsZXJzLmdldChtZXNzYWdlSWQpO1xyXG4gICAgICAgIGlmIChoYW5kbGVyKSB7XHJcbiAgICAgICAgICAgIGhhbmRsZXIoZGF0YSk7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgY29uc29sZS53YXJuKCdObyBoYW5kbGVyIHJlZ2lzdGVyZWQgZm9yIG1lc3NhZ2UgSUQ6JywgbWVzc2FnZUlkKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIobWVzc2FnZUlkOiBzdHJpbmcsIGNhbGxiYWNrOiBNZXNzYWdlQ2FsbGJhY2spIHtcclxuICAgICAgICB0aGlzLm1lc3NhZ2VIYW5kbGVycy5zZXQobWVzc2FnZUlkLCBjYWxsYmFjayk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHVucmVnaXN0ZXJNZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nKSB7XHJcbiAgICAgICAgdGhpcy5tZXNzYWdlSGFuZGxlcnMuZGVsZXRlKG1lc3NhZ2VJZCk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWdpc3Rlck9wZW5FeHRlcm5hbCgpIHtcclxuICAgICAgICBpZiAodGhpcy5vcGVuRXh0ZXJuYWxSZWdpc3RlcmVkKSByZXR1cm47XHJcbiAgICAgICAgdGhpcy5vcGVuRXh0ZXJuYWxSZWdpc3RlcmVkID0gdHJ1ZTtcclxuICAgICAgICBkb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwiY2xpY2tcIiwgYmxhbmtUYXJnZXRIYW5kbGVyLCB7Y2FwdHVyZTogdHJ1ZX0pO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgcmVnaXN0ZXJGdWxsc2NyZWVuQ2hhbmdlKCkge1xyXG4gICAgICAgIGlmICh0aGlzLmZ1bGxzY3JlZW5SZWdpc3RlcmVkKSByZXR1cm47XHJcbiAgICAgICAgdGhpcy5mdWxsc2NyZWVuUmVnaXN0ZXJlZCA9IHRydWU7XHJcbiAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImZ1bGxzY3JlZW5jaGFuZ2VcIiwgKF86IEV2ZW50KSA9PiB7XHJcbiAgICAgICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkgdGhpcy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy5mdWxsc2NyZWVuRW50ZXIpO1xyXG4gICAgICAgICAgICBlbHNlIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMuZnVsbHNjcmVlbkV4aXQpO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBkb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwia2V5ZG93blwiLCBhc3luYyAoZTogS2V5Ym9hcmRFdmVudCkgPT4ge1xyXG4gICAgICAgICAgICBpZiAoZS5rZXkgIT09IFwiRjExXCIpIHJldHVybjtcclxuICAgICAgICAgICAgaWYgKGRvY3VtZW50LmZ1bGxzY3JlZW5FbGVtZW50KSBhd2FpdCBkb2N1bWVudC5leGl0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgICAgICBlbHNlIGF3YWl0IGRvY3VtZW50LmJvZHkucmVxdWVzdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIHJlZ2lzdGVyVGl0bGVDaGFuZ2UoKSB7XHJcbiAgICAgICAgaWYgKHRoaXMudGl0bGVSZWdpc3RlcmVkKSByZXR1cm47XHJcbiAgICAgICAgdGhpcy50aXRsZVJlZ2lzdGVyZWQgPSB0cnVlO1xyXG4gICAgICAgIGNvbnN0IHRpdGxlVGFyZ2V0ID0gZ2V0VGl0bGVPYnNlcnZlclRhcmdldCgpO1xyXG4gICAgICAgIGlmICh0aXRsZVRhcmdldCkge1xyXG4gICAgICAgICAgICBnZXRUaXRsZU9ic2VydmVyKCkub2JzZXJ2ZSh0aXRsZVRhcmdldCwge2NoaWxkTGlzdDogdHJ1ZX0pO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCBoZWFkVGFyZ2V0ID0gZG9jdW1lbnQuaGVhZCB8fCBkb2N1bWVudC5kb2N1bWVudEVsZW1lbnQ7XHJcbiAgICAgICAgaWYgKCFoZWFkVGFyZ2V0KSByZXR1cm47XHJcblxyXG4gICAgICAgIGNvbnN0IGhlYWRPYnNlcnZlciA9IG5ldyBNdXRhdGlvbk9ic2VydmVyKCgpID0+IHtcclxuICAgICAgICAgICAgY29uc3QgdGFyZ2V0ID0gZ2V0VGl0bGVPYnNlcnZlclRhcmdldCgpO1xyXG4gICAgICAgICAgICBpZiAoIXRhcmdldCkgcmV0dXJuO1xyXG4gICAgICAgICAgICBoZWFkT2JzZXJ2ZXIuZGlzY29ubmVjdCgpO1xyXG4gICAgICAgICAgICBnZXRUaXRsZU9ic2VydmVyKCkub2JzZXJ2ZSh0YXJnZXQsIHtjaGlsZExpc3Q6IHRydWV9KTtcclxuICAgICAgICB9KTtcclxuICAgICAgICBoZWFkT2JzZXJ2ZXIub2JzZXJ2ZShoZWFkVGFyZ2V0LCB7Y2hpbGRMaXN0OiB0cnVlLCBzdWJ0cmVlOiB0cnVlfSk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSByZWdpc3RlcldpbmRvd0Nsb3NlKCkge1xyXG4gICAgICAgIGlmICh0aGlzLndpbmRvd0Nsb3NlUmVnaXN0ZXJlZCkgcmV0dXJuO1xyXG4gICAgICAgIHRoaXMud2luZG93Q2xvc2VSZWdpc3RlcmVkID0gdHJ1ZTtcclxuICAgICAgICB3aW5kb3cuY2xvc2UgPSAoKSA9PiB7XHJcbiAgICAgICAgICAgIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMud2luZG93Q2xvc2UpO1xyXG4gICAgICAgIH07XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBkZWZhdWx0IEhvc3RNZXNzYWdpbmdcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCBJbmZpbmlGcmFtZSBmcm9tIFwiLi9JbmZpbmlGcmFtZVwiO1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IHt9O1xyXG53aW5kb3cuaW5maW5pRnJhbWUgPSBuZXcgSW5maW5pRnJhbWUoKTtcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7SUluZmluaUZyYW1lfSBmcm9tIFwiLi9Db250cmFjdHMvSUluZmluaUZyYW1lXCI7XHJcbmltcG9ydCB7SUhvc3RNZXNzYWdpbmcsIFNlbmRUb0hvc3RNZXNzYWdlSWR9IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5pbXBvcnQgSG9zdE1lc3NhZ2luZyBmcm9tIFwiLi9Ib3N0TWVzc2FnaW5nXCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY2xhc3MgSW5maW5pRnJhbWUgaW1wbGVtZW50cyBJSW5maW5pRnJhbWUge1xyXG4gICAgSG9zdE1lc3NhZ2luZzogSUhvc3RNZXNzYWdpbmcgPSBuZXcgSG9zdE1lc3NhZ2luZygpO1xyXG5cclxuICAgIC8vIE92ZXJsb2FkIHRvIG1ha2UgYSBkZXYncyBsaWZlIGVhc2llciBpbnN0ZWFkIG9mIGhhdmluZyB0byBnbyB0byB0aGUgSG9zdE1lc3NhZ2luZyBjbGFzc1xyXG4gICAgc2VuZE1lc3NhZ2VUb0hvc3QoaWQ6IFNlbmRUb0hvc3RNZXNzYWdlSWQsIGRhdGE/OiBzdHJpbmcpIHtcclxuICAgICAgICB0aGlzLkhvc3RNZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoaWQsIGRhdGEpO1xyXG4gICAgfVxyXG5cclxuICAgIHNldFBvaW50ZXJDYXB0dXJlKGVsZW1lbnQ6IEVsZW1lbnQsIHBvaW50ZXJJZDogbnVtYmVyKSB7XHJcbiAgICAgICAgZWxlbWVudC5zZXRQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG4gICAgfVxyXG5cclxuICAgIHJlbGVhc2VQb2ludGVyQ2FwdHVyZShlbGVtZW50OiBFbGVtZW50LCBwb2ludGVySWQ6IG51bWJlcikge1xyXG4gICAgICAgIGVsZW1lbnQucmVsZWFzZVBvaW50ZXJDYXB0dXJlKHBvaW50ZXJJZCk7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBkZWZhdWx0IEluZmluaUZyYW1lIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7U2VuZFRvSG9zdE1lc3NhZ2VJZHN9IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBmdW5jdGlvbiBnZXRUaXRsZU9ic2VydmVyVGFyZ2V0KCk6IEhUTUxUaXRsZUVsZW1lbnQgfCBudWxsIHtcclxuICAgIHJldHVybiBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKCd0aXRsZScpO1xyXG59XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gZ2V0VGl0bGVPYnNlcnZlcigpOiBNdXRhdGlvbk9ic2VydmVyIHtcclxuICAgIHJldHVybiBuZXcgTXV0YXRpb25PYnNlcnZlcigobXV0YXRpb25zLCBfKSA9PiB7XHJcbiAgICAgICAgbXV0YXRpb25zLmZvckVhY2goKG11dGF0aW9uKSA9PiB7XHJcbiAgICAgICAgICAgIGlmIChtdXRhdGlvbi50eXBlICE9PSBcImNoaWxkTGlzdFwiKSByZXR1cm47XHJcbiAgICAgICAgICAgIHdpbmRvdy5pbmZpbmlGcmFtZS5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLnRpdGxlQ2hhbmdlLCBkb2N1bWVudC50aXRsZSk7XHJcbiAgICAgICAgfSlcclxuICAgIH0pXHJcbn1cclxuIiwiLy8gVGhlIG1vZHVsZSBjYWNoZVxudmFyIF9fd2VicGFja19tb2R1bGVfY2FjaGVfXyA9IHt9O1xuXG4vLyBUaGUgcmVxdWlyZSBmdW5jdGlvblxuZnVuY3Rpb24gX193ZWJwYWNrX3JlcXVpcmVfXyhtb2R1bGVJZCkge1xuXHQvLyBDaGVjayBpZiBtb2R1bGUgaXMgaW4gY2FjaGVcblx0dmFyIGNhY2hlZE1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF07XG5cdGlmIChjYWNoZWRNb2R1bGUgIT09IHVuZGVmaW5lZCkge1xuXHRcdHJldHVybiBjYWNoZWRNb2R1bGUuZXhwb3J0cztcblx0fVxuXHQvLyBDcmVhdGUgYSBuZXcgbW9kdWxlIChhbmQgcHV0IGl0IGludG8gdGhlIGNhY2hlKVxuXHR2YXIgbW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXSA9IHtcblx0XHQvLyBubyBtb2R1bGUuaWQgbmVlZGVkXG5cdFx0Ly8gbm8gbW9kdWxlLmxvYWRlZCBuZWVkZWRcblx0XHRleHBvcnRzOiB7fVxuXHR9O1xuXG5cdC8vIEV4ZWN1dGUgdGhlIG1vZHVsZSBmdW5jdGlvblxuXHRpZiAoIShtb2R1bGVJZCBpbiBfX3dlYnBhY2tfbW9kdWxlc19fKSkge1xuXHRcdGRlbGV0ZSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdO1xuXHRcdHZhciBlID0gbmV3IEVycm9yKFwiQ2Fubm90IGZpbmQgbW9kdWxlICdcIiArIG1vZHVsZUlkICsgXCInXCIpO1xuXHRcdGUuY29kZSA9ICdNT0RVTEVfTk9UX0ZPVU5EJztcblx0XHR0aHJvdyBlO1xuXHR9XG5cdF9fd2VicGFja19tb2R1bGVzX19bbW9kdWxlSWRdLmNhbGwobW9kdWxlLmV4cG9ydHMsIG1vZHVsZSwgbW9kdWxlLmV4cG9ydHMsIF9fd2VicGFja19yZXF1aXJlX18pO1xuXG5cdC8vIFJldHVybiB0aGUgZXhwb3J0cyBvZiB0aGUgbW9kdWxlXG5cdHJldHVybiBtb2R1bGUuZXhwb3J0cztcbn1cblxuIiwiIiwiLy8gc3RhcnR1cFxuLy8gTG9hZCBlbnRyeSBtb2R1bGUgYW5kIHJldHVybiBleHBvcnRzXG4vLyBUaGlzIGVudHJ5IG1vZHVsZSBpcyByZWZlcmVuY2VkIGJ5IG90aGVyIG1vZHVsZXMgc28gaXQgY2FuJ3QgYmUgaW5saW5lZFxudmFyIF9fd2VicGFja19leHBvcnRzX18gPSBfX3dlYnBhY2tfcmVxdWlyZV9fKFwiLi9zcmMvSW5maW5pRnJhbWUuSnMvVHNTb3VyY2UvSW5kZXgudHNcIik7XG4iLCIiXSwibmFtZXMiOltdLCJzb3VyY2VSb290IjoiIn0=