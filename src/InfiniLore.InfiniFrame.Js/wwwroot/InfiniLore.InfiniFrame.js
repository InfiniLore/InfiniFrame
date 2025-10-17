/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/BlankTargetHandler.ts":
/*!**********************************************************************!*\
  !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/BlankTargetHandler.ts ***!
  \**********************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


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
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts");
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
            if (!(target === "_blank" || anchor.hasAttribute("data-external") || isExternalLink(anchor.href))) {
                el = el.parentElement;
                continue;
            }
            e.preventDefault();
            window.infiniFrame.HostMessaging.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.openExternalLink, anchor.href);
            return;
        }
    });
}


/***/ }),

/***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts":
/*!****************************************************************************!*\
  !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts ***!
  \****************************************************************************/
/***/ ((__unused_webpack_module, exports) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.ReceiveFromHostMessageIds = exports.SendToHostMessageIds = void 0;
const infiniFrame = "__infiniframe";
exports.SendToHostMessageIds = {
    titleChange: `${infiniFrame}:title:change`,
    fullscreenEnter: `${infiniFrame}:fullscreen:enter`,
    fullscreenExit: `${infiniFrame}:fullscreen:exit`,
    openExternalLink: `${infiniFrame}:open:external`,
    windowClose: `${infiniFrame}:window:close`,
};
exports.ReceiveFromHostMessageIds = {
    registerOpenExternal: `${infiniFrame}:register:open:external`,
    registerFullscreenChange: `${infiniFrame}:register:fullscreen:change`,
    registerTitleChange: `${infiniFrame}:register:title:change`,
    registerWindowClose: `${infiniFrame}:register:window:close`,
};


/***/ }),

/***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/HostMessaging.ts":
/*!*****************************************************************!*\
  !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/HostMessaging.ts ***!
  \*****************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


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
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts");
const BlankTargetHandler_1 = __webpack_require__(/*! ./BlankTargetHandler */ "./src/InfiniLore.InfiniFrame.Js/TsSource/BlankTargetHandler.ts");
const Observers_1 = __webpack_require__(/*! ./Observers */ "./src/InfiniLore.InfiniFrame.Js/TsSource/Observers.ts");
class HostMessaging {
    constructor() {
        this.messageHandlers = new Map();
        this.assignWebMessageReceiver();
        this.assignMessageReceivedHandler(IHostMessaging_1.ReceiveFromHostMessageIds.registerOpenExternal, _ => {
            document.addEventListener("click", BlankTargetHandler_1.blankTargetHandler, { capture: true });
        });
        this.assignMessageReceivedHandler(IHostMessaging_1.ReceiveFromHostMessageIds.registerFullscreenChange, _ => {
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
        });
        this.assignMessageReceivedHandler(IHostMessaging_1.ReceiveFromHostMessageIds.registerTitleChange, _ => {
            if (Observers_1.TitleObserverTarget)
                (0, Observers_1.getTitleObserver)().observe(Observers_1.TitleObserverTarget, { childList: true });
        });
        this.assignMessageReceivedHandler(IHostMessaging_1.ReceiveFromHostMessageIds.registerWindowClose, _ => {
            window.close = () => {
                this.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.windowClose);
            };
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
}
exports["default"] = HostMessaging;


/***/ }),

/***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/Index.ts":
/*!*********************************************************!*\
  !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/Index.ts ***!
  \*********************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
const InfiniFrame_1 = __importDefault(__webpack_require__(/*! ./InfiniFrame */ "./src/InfiniLore.InfiniFrame.Js/TsSource/InfiniFrame.ts"));
window.infiniFrame = new InfiniFrame_1.default();


/***/ }),

/***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/InfiniFrame.ts":
/*!***************************************************************!*\
  !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/InfiniFrame.ts ***!
  \***************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.InfiniFrame = void 0;
const HostMessaging_1 = __importDefault(__webpack_require__(/*! ./HostMessaging */ "./src/InfiniLore.InfiniFrame.Js/TsSource/HostMessaging.ts"));
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


/***/ }),

/***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/Observers.ts":
/*!*************************************************************!*\
  !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/Observers.ts ***!
  \*************************************************************/
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.TitleObserverTarget = void 0;
exports.getTitleObserver = getTitleObserver;
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts");
exports.TitleObserverTarget = document.querySelector('title');
function getTitleObserver() {
    return new MutationObserver((mutations, _) => {
        mutations.forEach((mutation) => {
            if (mutation.type !== "childList")
                return;
            window.infiniFrame.HostMessaging.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.titleChange, document.title);
        });
    });
}


/***/ })

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
/******/ 	var __webpack_exports__ = __webpack_require__("./src/InfiniLore.InfiniFrame.Js/TsSource/Index.ts");
/******/ 	
/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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aUJBQWlCLENBQUMsT0FBZ0IsRUFBRSxTQUFpQjtRQUNqRCxPQUFPLENBQUMsaUJBQWlCLENBQUMsU0FBUyxDQUFDLENBQUM7SUFDekMsQ0FBQztJQUVELHFCQUFxQixDQUFDLE9BQWdCLEVBQUUsU0FBaUI7UUFDckQsT0FBTyxDQUFDLHFCQUFxQixDQUFDLFNBQVMsQ0FBQyxDQUFDO0lBQzdDLENBQUM7Q0FDSjtBQWZELGtDQWVDO0FBRUQscUJBQWUsV0FBVzs7Ozs7Ozs7Ozs7Ozs7QUNoQjFCLDRDQU9DO0FBZEQsdUpBQWdFO0FBS25ELDJCQUFtQixHQUE0QixRQUFRLENBQUMsYUFBYSxDQUFDLE9BQU8sQ0FBQyxDQUFDO0FBRTVGLFNBQWdCLGdCQUFnQjtJQUM1QixPQUFPLElBQUksZ0JBQWdCLENBQUMsQ0FBQyxTQUFTLEVBQUUsQ0FBQyxFQUFFLEVBQUU7UUFDekMsU0FBUyxDQUFDLE9BQU8sQ0FBQyxDQUFDLFFBQVEsRUFBRSxFQUFFO1lBQzNCLElBQUksUUFBUSxDQUFDLElBQUksS0FBSyxXQUFXO2dCQUFFLE9BQU87WUFDMUMsTUFBTSxDQUFDLFdBQVcsQ0FBQyxhQUFhLENBQUMsaUJBQWlCLENBQUMscUNBQW9CLENBQUMsV0FBVyxFQUFFLFFBQVEsQ0FBQyxLQUFLLENBQUMsQ0FBQztRQUN6RyxDQUFDLENBQUM7SUFDTixDQUFDLENBQUM7QUFDTixDQUFDOzs7Ozs7O1VDakJEO1VBQ0E7O1VBRUE7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7O1VBRUE7VUFDQTs7VUFFQTtVQUNBO1VBQ0E7Ozs7VUV0QkE7VUFDQTtVQUNBO1VBQ0EiLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUxvcmUuSW5maW5pRnJhbWUuSnMvVHNTb3VyY2UvQmxhbmtUYXJnZXRIYW5kbGVyLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvLi9zcmMvSW5maW5pTG9yZS5JbmZpbmlGcmFtZS5Kcy9Uc1NvdXJjZS9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS8uL3NyYy9JbmZpbmlMb3JlLkluZmluaUZyYW1lLkpzL1RzU291cmNlL0hvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS8uL3NyYy9JbmZpbmlMb3JlLkluZmluaUZyYW1lLkpzL1RzU291cmNlL0luZGV4LnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvLi9zcmMvSW5maW5pTG9yZS5JbmZpbmlGcmFtZS5Kcy9Uc1NvdXJjZS9JbmZpbmlGcmFtZS50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUxvcmUuSW5maW5pRnJhbWUuSnMvVHNTb3VyY2UvT2JzZXJ2ZXJzLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvd2VicGFjay9ib290c3RyYXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS93ZWJwYWNrL2JlZm9yZS1zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvd2VicGFjay9zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvd2VicGFjay9hZnRlci1zdGFydHVwIl0sInNvdXJjZXNDb250ZW50IjpbIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1NlbmRUb0hvc3RNZXNzYWdlSWRzfSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5mdW5jdGlvbiBpc0V4dGVybmFsTGluayh1cmw6IHN0cmluZyk6IGJvb2xlYW4ge1xyXG4gICAgdHJ5IHtcclxuICAgICAgICByZXR1cm4gbmV3IFVSTCh1cmwsIGxvY2F0aW9uLmhyZWYpLmhvc3RuYW1lICE9PSBsb2NhdGlvbi5ob3N0bmFtZTtcclxuICAgIH0gY2F0Y2gge1xyXG4gICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGFzeW5jIGZ1bmN0aW9uIGJsYW5rVGFyZ2V0SGFuZGxlcihlOiBNb3VzZUV2ZW50KSB7XHJcbiAgICBsZXQgZWwgPSBlLnRhcmdldCBhcyBIVE1MRWxlbWVudCB8IG51bGw7XHJcblxyXG4gICAgd2hpbGUgKGVsICYmIGVsICE9PSBkb2N1bWVudC5ib2R5KSB7XHJcbiAgICAgICAgaWYgKGVsLnRhZ05hbWU/LnRvTG93ZXJDYXNlKCkgIT09IFwiYVwiKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCBhbmNob3IgPSBlbCBhcyBIVE1MQW5jaG9yRWxlbWVudDtcclxuICAgICAgICBpZiAoIWFuY2hvci5ocmVmKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCB0YXJnZXQgPSBhbmNob3IuZ2V0QXR0cmlidXRlKFwidGFyZ2V0XCIpO1xyXG4gICAgICAgIGlmICghKHRhcmdldCA9PT0gXCJfYmxhbmtcIiB8fCBhbmNob3IuaGFzQXR0cmlidXRlKFwiZGF0YS1leHRlcm5hbFwiKSB8fCBpc0V4dGVybmFsTGluayhhbmNob3IuaHJlZikpKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcbiAgICAgICAgd2luZG93LmluZmluaUZyYW1lLkhvc3RNZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMub3BlbkV4dGVybmFsTGluaywgYW5jaG9yLmhyZWYpO1xyXG4gICAgICAgIHJldHVybjtcclxuICAgIH1cclxufSIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmNvbnN0IGluZmluaUZyYW1lIDogc3RyaW5nID0gXCJfX2luZmluaWZyYW1lXCI7XHJcblxyXG5leHBvcnQgY29uc3QgU2VuZFRvSG9zdE1lc3NhZ2VJZHMgPSB7XHJcbiAgICB0aXRsZUNoYW5nZTogYCR7aW5maW5pRnJhbWV9OnRpdGxlOmNoYW5nZWAsXHJcbiAgICBmdWxsc2NyZWVuRW50ZXI6IGAke2luZmluaUZyYW1lfTpmdWxsc2NyZWVuOmVudGVyYCxcclxuICAgIGZ1bGxzY3JlZW5FeGl0OiBgJHtpbmZpbmlGcmFtZX06ZnVsbHNjcmVlbjpleGl0YCxcclxuICAgIG9wZW5FeHRlcm5hbExpbms6IGAke2luZmluaUZyYW1lfTpvcGVuOmV4dGVybmFsYCxcclxuICAgIHdpbmRvd0Nsb3NlOiBgJHtpbmZpbmlGcmFtZX06d2luZG93OmNsb3NlYCxcclxufVxyXG5cclxuZXhwb3J0IGNvbnN0IFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMgPSB7XHJcbiAgICByZWdpc3Rlck9wZW5FeHRlcm5hbDogYCR7aW5maW5pRnJhbWV9OnJlZ2lzdGVyOm9wZW46ZXh0ZXJuYWxgLFxyXG4gICAgcmVnaXN0ZXJGdWxsc2NyZWVuQ2hhbmdlOiBgJHtpbmZpbmlGcmFtZX06cmVnaXN0ZXI6ZnVsbHNjcmVlbjpjaGFuZ2VgLFxyXG4gICAgcmVnaXN0ZXJUaXRsZUNoYW5nZTogYCR7aW5maW5pRnJhbWV9OnJlZ2lzdGVyOnRpdGxlOmNoYW5nZWAsXHJcbiAgICByZWdpc3RlcldpbmRvd0Nsb3NlOiBgJHtpbmZpbmlGcmFtZX06cmVnaXN0ZXI6d2luZG93OmNsb3NlYCxcclxufVxyXG5cclxuZXhwb3J0IHR5cGUgU2VuZFRvSG9zdE1lc3NhZ2VJZCA9IHR5cGVvZiBTZW5kVG9Ib3N0TWVzc2FnZUlkc1trZXlvZiB0eXBlb2YgU2VuZFRvSG9zdE1lc3NhZ2VJZHNdO1xyXG5leHBvcnQgdHlwZSBNZXNzYWdlQ2FsbGJhY2sgPSAoZGF0YT86IHN0cmluZykgPT4gdm9pZDtcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgSUhvc3RNZXNzYWdpbmcge1xyXG4gICAgc2VuZE1lc3NhZ2VUb0hvc3QoaWQ6IFNlbmRUb0hvc3RNZXNzYWdlSWQgfCBzdHJpbmcsIGRhdGE/OiBzdHJpbmcpOiB2b2lkO1xyXG5cclxuICAgIGFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIobWVzc2FnZUlkOiBzdHJpbmcsIGNhbGxiYWNrOiBNZXNzYWdlQ2FsbGJhY2spOiB2b2lkO1xyXG5cclxuICAgIHVucmVnaXN0ZXJNZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nKTogdm9pZDtcclxufSIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1xyXG4gICAgSUhvc3RNZXNzYWdpbmcsXHJcbiAgICBNZXNzYWdlQ2FsbGJhY2ssXHJcbiAgICBSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLFxyXG4gICAgU2VuZFRvSG9zdE1lc3NhZ2VJZCwgU2VuZFRvSG9zdE1lc3NhZ2VJZHNcclxufSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuaW1wb3J0IHtibGFua1RhcmdldEhhbmRsZXJ9IGZyb20gXCIuL0JsYW5rVGFyZ2V0SGFuZGxlclwiO1xyXG5pbXBvcnQge2dldFRpdGxlT2JzZXJ2ZXIsIFRpdGxlT2JzZXJ2ZXJUYXJnZXR9IGZyb20gXCIuL09ic2VydmVyc1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmNsYXNzIEhvc3RNZXNzYWdpbmcgaW1wbGVtZW50cyBJSG9zdE1lc3NhZ2luZyB7XHJcbiAgICBwcml2YXRlIG1lc3NhZ2VIYW5kbGVyczogTWFwPHN0cmluZywgTWVzc2FnZUNhbGxiYWNrPiA9IG5ldyBNYXAoKTtcclxuXHJcbiAgICBjb25zdHJ1Y3RvcigpIHtcclxuICAgICAgICB0aGlzLmFzc2lnbldlYk1lc3NhZ2VSZWNlaXZlcigpO1xyXG5cclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3Rlck9wZW5FeHRlcm5hbCwgXyA9PiB7XHJcbiAgICAgICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJjbGlja1wiLCBibGFua1RhcmdldEhhbmRsZXIsIHtjYXB0dXJlOiB0cnVlfSk7XHJcbiAgICAgICAgfSlcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJGdWxsc2NyZWVuQ2hhbmdlLCBfID0+IHtcclxuICAgICAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImZ1bGxzY3JlZW5jaGFuZ2VcIiwgKF86IEV2ZW50KSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoZG9jdW1lbnQuZnVsbHNjcmVlbkVsZW1lbnQpIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMuZnVsbHNjcmVlbkVudGVyKTtcclxuICAgICAgICAgICAgICAgIGVsc2UgdGhpcy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy5mdWxsc2NyZWVuRXhpdCk7XHJcbiAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImtleWRvd25cIiwgYXN5bmMgKGU6IEtleWJvYXJkRXZlbnQpID0+IHtcclxuICAgICAgICAgICAgICAgIGlmIChlLmtleSAhPT0gXCJGMTFcIikgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgaWYgKGRvY3VtZW50LmZ1bGxzY3JlZW5FbGVtZW50KSBhd2FpdCBkb2N1bWVudC5leGl0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgICAgICAgICAgZWxzZSBhd2FpdCBkb2N1bWVudC5ib2R5LnJlcXVlc3RGdWxsc2NyZWVuKCk7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH0pXHJcblxyXG4gICAgICAgIHRoaXMuYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLnJlZ2lzdGVyVGl0bGVDaGFuZ2UsIF8gPT4ge1xyXG4gICAgICAgICAgICBpZiAoVGl0bGVPYnNlcnZlclRhcmdldCkgZ2V0VGl0bGVPYnNlcnZlcigpLm9ic2VydmUoVGl0bGVPYnNlcnZlclRhcmdldCwge2NoaWxkTGlzdDogdHJ1ZX0pO1xyXG4gICAgICAgIH0pXHJcblxyXG4gICAgICAgIHRoaXMuYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLnJlZ2lzdGVyV2luZG93Q2xvc2UsIF8gPT4ge1xyXG4gICAgICAgICAgICB3aW5kb3cuY2xvc2UgPSAoKSA9PiB7XHJcbiAgICAgICAgICAgICAgICB0aGlzLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLndpbmRvd0Nsb3NlKTtcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkIHwgc3RyaW5nLCBkYXRhPzogc3RyaW5nKSB7XHJcbiAgICAgICAgY29uc3QgbWVzc2FnZSA9IGRhdGEgPyBgJHtpZH07JHtkYXRhfWAgOiBpZDtcclxuXHJcbiAgICAgICAgLy8gVE9ETyAtIGRldGVybWluZSBtZXNzYWdpbmcgbWV0aG9kcyBmb3IgUGhvdGluby5ORVQgZm9yIGFsbCBwbGF0Zm9ybXNcclxuICAgICAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xyXG4gICAgICAgICAgICB3aW5kb3cuY2hyb21lLndlYnZpZXcucG9zdE1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICAgICAgfSBlbHNlIGlmICh3aW5kb3cuZXh0ZXJuYWw/LnNlbmRNZXNzYWdlKSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5leHRlcm5hbC5zZW5kTWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICBjb25zb2xlLndhcm4oXCJNZXNzYWdlIHRvIGhvc3QgZmFpbGVkOlwiLCBtZXNzYWdlKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBhc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKSB7XHJcbiAgICAgICAgLy8gU3RvcmUgdGhlIG9yaWdpbmFsIHJlY2VpdmVNZXNzYWdlIGlmIGl0IGV4aXN0cyAoZm9yIEJsYXpvciBjb21wYXRpYmlsaXR5KVxyXG4gICAgICAgIGNvbnN0IG9yaWdpbmFsUmVjZWl2ZU1lc3NhZ2UgPSB3aW5kb3cuZXh0ZXJuYWw/LnJlY2VpdmVNZXNzYWdlO1xyXG5cclxuICAgICAgICAvLyBIYW5kbGUgV2ViVmlldzIgbWVzc2FnZXMgKFdpbmRvd3MpXHJcbiAgICAgICAgaWYgKHdpbmRvdy5jaHJvbWU/LndlYnZpZXcpIHtcclxuICAgICAgICAgICAgd2luZG93LmNocm9tZS53ZWJ2aWV3LmFkZEV2ZW50TGlzdGVuZXIoJ21lc3NhZ2UnLCAoZXZlbnQpID0+IHtcclxuICAgICAgICAgICAgICAgIGlmICghdGhpcy5pc0JsYXpvck1lc3NhZ2UoZXZlbnQuZGF0YSkpIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLmhhbmRsZVdlYk1lc3NhZ2UoZXZlbnQuZGF0YSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gSGFuZGxlIGdlbmVyYWwgUGhvdGlubyBtZXNzYWdlcyAoY3Jvc3MtcGxhdGZvcm0pXHJcbiAgICAgICAgaWYgKHR5cGVvZiB3aW5kb3cgIT09ICd1bmRlZmluZWQnICYmIHdpbmRvdy5leHRlcm5hbCkge1xyXG4gICAgICAgICAgICB3aW5kb3cuZXh0ZXJuYWwucmVjZWl2ZU1lc3NhZ2UgPSAobWVzc2FnZTogYW55KSA9PiB7XHJcbiAgICAgICAgICAgICAgICAvLyBDaGVjayBpZiBpdCdzIGEgQmxhem9yIG1lc3NhZ2UgYW5kIHBhc3MgaXQgdG8gdGhlIG9yaWdpbmFsIGhhbmRsZXJcclxuICAgICAgICAgICAgICAgIGlmICh0aGlzLmlzQmxhem9yTWVzc2FnZShtZXNzYWdlKSkge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChvcmlnaW5hbFJlY2VpdmVNZXNzYWdlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG9yaWdpbmFsUmVjZWl2ZU1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAvLyBIYW5kbGUgb3VyIGN1c3RvbSBtZXNzYWdlc1xyXG4gICAgICAgICAgICAgICAgdGhpcy5oYW5kbGVXZWJNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgICAgICB9O1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIGlzQmxhem9yTWVzc2FnZShtZXNzYWdlOiBhbnkpOiBib29sZWFuIHtcclxuICAgICAgICBpZiAodHlwZW9mIG1lc3NhZ2UgIT09ICdzdHJpbmcnKSByZXR1cm4gdHJ1ZTsgLy8gQXNzdW1lIG5vbi1zdHJpbmcgbWVzc2FnZXMgYXJlIEJsYXpvclxyXG5cclxuICAgICAgICAvLyBDaGVjayBmb3IgY29tbW9uIEJsYXpvciBtZXNzYWdlIHBhdHRlcm5zXHJcbiAgICAgICAgcmV0dXJuIG1lc3NhZ2Uuc3RhcnRzV2l0aCgnX19id3Y6JylcclxuICAgICAgICAgICAgfHwgbWVzc2FnZS5zdGFydHNXaXRoKCdlPT57JylcclxuICAgICAgICAgICAgfHwgbWVzc2FnZS5pbmNsdWRlcygnQmVnaW5JbnZva2VKUycpXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2UuaW5jbHVkZXMoJ0F0dGFjaFRvRG9jdW1lbnQnKVxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLmluY2x1ZGVzKCdSZW5kZXJCYXRjaCcpXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2UuaW5jbHVkZXMoJ0JsYXpvci4nKTtcclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIGhhbmRsZVdlYk1lc3NhZ2UobWVzc2FnZTogYW55KSB7XHJcbiAgICAgICAgLy8gRW5zdXJlIG1lc3NhZ2UgaXMgYSBzdHJpbmdcclxuICAgICAgICBjb25zdCBtZXNzYWdlU3RyID0gdHlwZW9mIG1lc3NhZ2UgPT09ICdzdHJpbmcnID8gbWVzc2FnZSA6IFN0cmluZyhtZXNzYWdlIHx8ICcnKTtcclxuXHJcbiAgICAgICAgaWYgKCFtZXNzYWdlU3RyKSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUud2FybignUmVjZWl2ZWQgZW1wdHkgb3IgaW52YWxpZCBtZXNzYWdlJyk7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGxldCBtZXNzYWdlSWQ6IHN0cmluZztcclxuICAgICAgICBsZXQgZGF0YTogc3RyaW5nIHwgdW5kZWZpbmVkO1xyXG5cclxuICAgICAgICAvLyBQYXJzZSBtZXNzYWdlIC0gY2hlY2sgaWYgaXQgY29udGFpbnMgZGF0YSBzZXBhcmF0ZWQgYnkgc2VtaWNvbG9uXHJcbiAgICAgICAgaWYgKG1lc3NhZ2VTdHIuaW5jbHVkZXMoJzsnKSkge1xyXG4gICAgICAgICAgICBjb25zdCBwYXJ0cyA9IG1lc3NhZ2VTdHIuc3BsaXQoJzsnLCAyKTtcclxuICAgICAgICAgICAgbWVzc2FnZUlkID0gcGFydHNbMF07XHJcbiAgICAgICAgICAgIGRhdGEgPSBwYXJ0c1sxXTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICBtZXNzYWdlSWQgPSBtZXNzYWdlU3RyO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gRXhlY3V0ZSByZWdpc3RlcmVkIGhhbmRsZXJcclxuICAgICAgICBjb25zdCBoYW5kbGVyID0gdGhpcy5tZXNzYWdlSGFuZGxlcnMuZ2V0KG1lc3NhZ2VJZCk7XHJcbiAgICAgICAgaWYgKGhhbmRsZXIpIHtcclxuICAgICAgICAgICAgaGFuZGxlcihkYXRhKTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICBjb25zb2xlLndhcm4oJ05vIGhhbmRsZXIgcmVnaXN0ZXJlZCBmb3IgbWVzc2FnZSBJRDonLCBtZXNzYWdlSWQpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZywgY2FsbGJhY2s6IE1lc3NhZ2VDYWxsYmFjaykge1xyXG4gICAgICAgIHRoaXMubWVzc2FnZUhhbmRsZXJzLnNldChtZXNzYWdlSWQsIGNhbGxiYWNrKTtcclxuICAgIH1cclxuXHJcbiAgICBwdWJsaWMgdW5yZWdpc3Rlck1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIobWVzc2FnZUlkOiBzdHJpbmcpIHtcclxuICAgICAgICB0aGlzLm1lc3NhZ2VIYW5kbGVycy5kZWxldGUobWVzc2FnZUlkKTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGRlZmF1bHQgSG9zdE1lc3NhZ2luZ1xyXG4iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IEluZmluaUZyYW1lIGZyb20gXCIuL0luZmluaUZyYW1lXCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQge307XHJcbndpbmRvdy5pbmZpbmlGcmFtZSA9IG5ldyBJbmZpbmlGcmFtZSgpO1xyXG4iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtJSW5maW5pRnJhbWV9IGZyb20gXCIuL0NvbnRyYWN0cy9JSW5maW5pRnJhbWVcIjtcclxuaW1wb3J0IHtJSG9zdE1lc3NhZ2luZywgU2VuZFRvSG9zdE1lc3NhZ2VJZH0gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcbmltcG9ydCBIb3N0TWVzc2FnaW5nIGZyb20gXCIuL0hvc3RNZXNzYWdpbmdcIjtcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBjbGFzcyBJbmZpbmlGcmFtZSBpbXBsZW1lbnRzIElJbmZpbmlGcmFtZSB7XHJcbiAgICBIb3N0TWVzc2FnaW5nOiBJSG9zdE1lc3NhZ2luZyA9IG5ldyBIb3N0TWVzc2FnaW5nKCk7XHJcblxyXG4gICAgLy8gT3ZlcmxvYWQgdG8gbWFrZSBhIGRldidzIGxpZmUgZWFzaWVyIGluc3RlYWQgb2YgaGF2aW5nIHRvIGdvIHRvIHRoZSBIb3N0TWVzc2FnaW5nIGNsYXNzXHJcbiAgICBzZW5kTWVzc2FnZVRvSG9zdChpZDogU2VuZFRvSG9zdE1lc3NhZ2VJZCwgZGF0YT86IHN0cmluZykge1xyXG4gICAgICAgIHRoaXMuSG9zdE1lc3NhZ2luZy5zZW5kTWVzc2FnZVRvSG9zdChpZCwgZGF0YSk7XHJcbiAgICB9XHJcblxyXG4gICAgc2V0UG9pbnRlckNhcHR1cmUoZWxlbWVudDogRWxlbWVudCwgcG9pbnRlcklkOiBudW1iZXIpIHtcclxuICAgICAgICBlbGVtZW50LnNldFBvaW50ZXJDYXB0dXJlKHBvaW50ZXJJZCk7XHJcbiAgICB9XHJcblxyXG4gICAgcmVsZWFzZVBvaW50ZXJDYXB0dXJlKGVsZW1lbnQ6IEVsZW1lbnQsIHBvaW50ZXJJZDogbnVtYmVyKSB7XHJcbiAgICAgICAgZWxlbWVudC5yZWxlYXNlUG9pbnRlckNhcHR1cmUocG9pbnRlcklkKTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGRlZmF1bHQgSW5maW5pRnJhbWUiLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtTZW5kVG9Ib3N0TWVzc2FnZUlkc30gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGNvbnN0IFRpdGxlT2JzZXJ2ZXJUYXJnZXQ6IEhUTUxUaXRsZUVsZW1lbnQgfCBudWxsID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcigndGl0bGUnKTtcclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBnZXRUaXRsZU9ic2VydmVyKCk6IE11dGF0aW9uT2JzZXJ2ZXIge1xyXG4gICAgcmV0dXJuIG5ldyBNdXRhdGlvbk9ic2VydmVyKChtdXRhdGlvbnMsIF8pID0+IHtcclxuICAgICAgICBtdXRhdGlvbnMuZm9yRWFjaCgobXV0YXRpb24pID0+IHtcclxuICAgICAgICAgICAgaWYgKG11dGF0aW9uLnR5cGUgIT09IFwiY2hpbGRMaXN0XCIpIHJldHVybjtcclxuICAgICAgICAgICAgd2luZG93LmluZmluaUZyYW1lLkhvc3RNZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMudGl0bGVDaGFuZ2UsIGRvY3VtZW50LnRpdGxlKTtcclxuICAgICAgICB9KVxyXG4gICAgfSlcclxufSIsIi8vIFRoZSBtb2R1bGUgY2FjaGVcbnZhciBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX18gPSB7fTtcblxuLy8gVGhlIHJlcXVpcmUgZnVuY3Rpb25cbmZ1bmN0aW9uIF9fd2VicGFja19yZXF1aXJlX18obW9kdWxlSWQpIHtcblx0Ly8gQ2hlY2sgaWYgbW9kdWxlIGlzIGluIGNhY2hlXG5cdHZhciBjYWNoZWRNb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdO1xuXHRpZiAoY2FjaGVkTW9kdWxlICE9PSB1bmRlZmluZWQpIHtcblx0XHRyZXR1cm4gY2FjaGVkTW9kdWxlLmV4cG9ydHM7XG5cdH1cblx0Ly8gQ3JlYXRlIGEgbmV3IG1vZHVsZSAoYW5kIHB1dCBpdCBpbnRvIHRoZSBjYWNoZSlcblx0dmFyIG1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF0gPSB7XG5cdFx0Ly8gbm8gbW9kdWxlLmlkIG5lZWRlZFxuXHRcdC8vIG5vIG1vZHVsZS5sb2FkZWQgbmVlZGVkXG5cdFx0ZXhwb3J0czoge31cblx0fTtcblxuXHQvLyBFeGVjdXRlIHRoZSBtb2R1bGUgZnVuY3Rpb25cblx0X193ZWJwYWNrX21vZHVsZXNfX1ttb2R1bGVJZF0uY2FsbChtb2R1bGUuZXhwb3J0cywgbW9kdWxlLCBtb2R1bGUuZXhwb3J0cywgX193ZWJwYWNrX3JlcXVpcmVfXyk7XG5cblx0Ly8gUmV0dXJuIHRoZSBleHBvcnRzIG9mIHRoZSBtb2R1bGVcblx0cmV0dXJuIG1vZHVsZS5leHBvcnRzO1xufVxuXG4iLCIiLCIvLyBzdGFydHVwXG4vLyBMb2FkIGVudHJ5IG1vZHVsZSBhbmQgcmV0dXJuIGV4cG9ydHNcbi8vIFRoaXMgZW50cnkgbW9kdWxlIGlzIHJlZmVyZW5jZWQgYnkgb3RoZXIgbW9kdWxlcyBzbyBpdCBjYW4ndCBiZSBpbmxpbmVkXG52YXIgX193ZWJwYWNrX2V4cG9ydHNfXyA9IF9fd2VicGFja19yZXF1aXJlX18oXCIuL3NyYy9JbmZpbmlMb3JlLkluZmluaUZyYW1lLkpzL1RzU291cmNlL0luZGV4LnRzXCIpO1xuIiwiIl0sIm5hbWVzIjpbXSwic291cmNlUm9vdCI6IiJ9