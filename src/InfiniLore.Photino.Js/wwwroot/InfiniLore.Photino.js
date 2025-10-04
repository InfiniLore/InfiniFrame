/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/InfiniLore.Photino.Js/TsSource/BlankTargetHandler.ts":
/*!******************************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/BlankTargetHandler.ts ***!
  \******************************************************************/
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
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.Photino.Js/TsSource/Contracts/IHostMessaging.ts");
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
            window.infiniWindow.HostMessaging.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.openExternalLink, anchor.href);
            return;
        }
    });
}


/***/ }),

/***/ "./src/InfiniLore.Photino.Js/TsSource/Contracts/IHostMessaging.ts":
/*!************************************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/Contracts/IHostMessaging.ts ***!
  \************************************************************************/
/***/ ((__unused_webpack_module, exports) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.ReceiveFromHostMessageIds = exports.SendToHostMessageIds = void 0;
exports.SendToHostMessageIds = {
    titleChange: "__infiniWindow:title:change",
    fullscreenEnter: "__infiniWindow:fullscreen:enter",
    fullscreenExit: "__infiniWindow:fullscreen:exit",
    openExternalLink: "__infiniWindow:open:external",
};
exports.ReceiveFromHostMessageIds = {
    registerOpenExternal: "__infiniWindow:register:open:external",
    registerFullscreenChange: "__infiniWindow:register:fullscreen:change",
    registerTitleChange: "__infiniWindow:register:title:change",
};


/***/ }),

/***/ "./src/InfiniLore.Photino.Js/TsSource/HostMessaging.ts":
/*!*************************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/HostMessaging.ts ***!
  \*************************************************************/
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
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.Photino.Js/TsSource/Contracts/IHostMessaging.ts");
const BlankTargetHandler_1 = __webpack_require__(/*! ./BlankTargetHandler */ "./src/InfiniLore.Photino.Js/TsSource/BlankTargetHandler.ts");
const Observers_1 = __webpack_require__(/*! ./Observers */ "./src/InfiniLore.Photino.Js/TsSource/Observers.ts");
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

/***/ "./src/InfiniLore.Photino.Js/TsSource/Index.ts":
/*!*****************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/Index.ts ***!
  \*****************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
const InfiniWindow_1 = __importDefault(__webpack_require__(/*! ./InfiniWindow */ "./src/InfiniLore.Photino.Js/TsSource/InfiniWindow.ts"));
window.infiniWindow = new InfiniWindow_1.default();


/***/ }),

/***/ "./src/InfiniLore.Photino.Js/TsSource/InfiniWindow.ts":
/*!************************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/InfiniWindow.ts ***!
  \************************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.InfiniWindow = void 0;
const HostMessaging_1 = __importDefault(__webpack_require__(/*! ./HostMessaging */ "./src/InfiniLore.Photino.Js/TsSource/HostMessaging.ts"));
class InfiniWindow {
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
exports.InfiniWindow = InfiniWindow;
exports["default"] = InfiniWindow;


/***/ }),

/***/ "./src/InfiniLore.Photino.Js/TsSource/Observers.ts":
/*!*********************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/Observers.ts ***!
  \*********************************************************/
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.TitleObserverTarget = void 0;
exports.getTitleObserver = getTitleObserver;
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.Photino.Js/TsSource/Contracts/IHostMessaging.ts");
exports.TitleObserverTarget = document.querySelector('title');
function getTitleObserver() {
    return new MutationObserver((mutations, _) => {
        mutations.forEach((mutation) => {
            if (mutation.type !== "childList")
                return;
            window.infiniWindow.HostMessaging.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.titleChange, document.title);
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
/******/ 	var __webpack_exports__ = __webpack_require__("./src/InfiniLore.Photino.Js/TsSource/Index.ts");
/******/ 	
/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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LENBQUMsRUFBRSxFQUFFO1FBQ3pDLFNBQVMsQ0FBQyxPQUFPLENBQUMsQ0FBQyxRQUFRLEVBQUUsRUFBRTtZQUMzQixJQUFJLFFBQVEsQ0FBQyxJQUFJLEtBQUssV0FBVztnQkFBRSxPQUFPO1lBQzFDLE1BQU0sQ0FBQyxZQUFZLENBQUMsYUFBYSxDQUFDLGlCQUFpQixDQUFDLHFDQUFvQixDQUFDLFdBQVcsRUFBRSxRQUFRLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDMUcsQ0FBQyxDQUFDO0lBQ04sQ0FBQyxDQUFDO0FBQ04sQ0FBQzs7Ozs7OztVQ2pCRDtVQUNBOztVQUVBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBOztVQUVBO1VBQ0E7O1VBRUE7VUFDQTtVQUNBOzs7O1VFdEJBO1VBQ0E7VUFDQTtVQUNBIiwic291cmNlcyI6WyJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9CbGFua1RhcmdldEhhbmRsZXIudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9Ib3N0TWVzc2FnaW5nLnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvSW5kZXgudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9JbmZpbmlXaW5kb3cudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9PYnNlcnZlcnMudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svYm9vdHN0cmFwIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL2JlZm9yZS1zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL3N0YXJ0dXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svYWZ0ZXItc3RhcnR1cCJdLCJzb3VyY2VzQ29udGVudCI6WyIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtTZW5kVG9Ib3N0TWVzc2FnZUlkc30gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZnVuY3Rpb24gaXNFeHRlcm5hbExpbmsodXJsOiBzdHJpbmcpOiBib29sZWFuIHtcclxuICAgIHRyeSB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBVUkwodXJsLCBsb2NhdGlvbi5ocmVmKS5ob3N0bmFtZSAhPT0gbG9jYXRpb24uaG9zdG5hbWU7XHJcbiAgICB9IGNhdGNoIHtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBhc3luYyBmdW5jdGlvbiBibGFua1RhcmdldEhhbmRsZXIoZTogTW91c2VFdmVudCkge1xyXG4gICAgbGV0IGVsID0gZS50YXJnZXQgYXMgSFRNTEVsZW1lbnQgfCBudWxsO1xyXG4gICAgXHJcbiAgICB3aGlsZSAoZWwgJiYgZWwgIT09IGRvY3VtZW50LmJvZHkpIHtcclxuICAgICAgICBpZiAoZWwudGFnTmFtZT8udG9Mb3dlckNhc2UoKSAhPT0gXCJhXCIpIHtcclxuICAgICAgICAgICAgZWwgPSBlbC5wYXJlbnRFbGVtZW50O1xyXG4gICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICB9XHJcbiAgICAgICAgXHJcbiAgICAgICAgY29uc3QgYW5jaG9yID0gZWwgYXMgSFRNTEFuY2hvckVsZW1lbnQ7XHJcbiAgICAgICAgaWYgKCFhbmNob3IuaHJlZikge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgdGFyZ2V0ID0gYW5jaG9yLmdldEF0dHJpYnV0ZShcInRhcmdldFwiKTtcclxuICAgICAgICBpZiAoISh0YXJnZXQgPT09IFwiX2JsYW5rXCIgfHwgYW5jaG9yLmhhc0F0dHJpYnV0ZShcImRhdGEtZXh0ZXJuYWxcIikgfHwgaXNFeHRlcm5hbExpbmsoYW5jaG9yLmhyZWYpKSkge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuICAgICAgICBcclxuICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcbiAgICAgICAgd2luZG93LmluZmluaVdpbmRvdy5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLm9wZW5FeHRlcm5hbExpbmssIGFuY2hvci5ocmVmKTtcclxuICAgICAgICByZXR1cm47XHJcbiAgICB9XHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY29uc3QgU2VuZFRvSG9zdE1lc3NhZ2VJZHMgPSB7XHJcbiAgICB0aXRsZUNoYW5nZTogXCJfX2luZmluaVdpbmRvdzp0aXRsZTpjaGFuZ2VcIixcclxuICAgIGZ1bGxzY3JlZW5FbnRlcjogXCJfX2luZmluaVdpbmRvdzpmdWxsc2NyZWVuOmVudGVyXCIsXHJcbiAgICBmdWxsc2NyZWVuRXhpdDogXCJfX2luZmluaVdpbmRvdzpmdWxsc2NyZWVuOmV4aXRcIixcclxuICAgIG9wZW5FeHRlcm5hbExpbms6IFwiX19pbmZpbmlXaW5kb3c6b3BlbjpleHRlcm5hbFwiLFxyXG59XHJcblxyXG5leHBvcnQgY29uc3QgUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcyA9IHtcclxuICAgIHJlZ2lzdGVyT3BlbkV4dGVybmFsOiBcIl9faW5maW5pV2luZG93OnJlZ2lzdGVyOm9wZW46ZXh0ZXJuYWxcIixcclxuICAgIHJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZTogXCJfX2luZmluaVdpbmRvdzpyZWdpc3RlcjpmdWxsc2NyZWVuOmNoYW5nZVwiLFxyXG4gICAgcmVnaXN0ZXJUaXRsZUNoYW5nZTogXCJfX2luZmluaVdpbmRvdzpyZWdpc3Rlcjp0aXRsZTpjaGFuZ2VcIixcclxufVxyXG5cclxuZXhwb3J0IHR5cGUgU2VuZFRvSG9zdE1lc3NhZ2VJZCA9IHR5cGVvZiBTZW5kVG9Ib3N0TWVzc2FnZUlkc1trZXlvZiB0eXBlb2YgU2VuZFRvSG9zdE1lc3NhZ2VJZHNdO1xyXG5leHBvcnQgdHlwZSBNZXNzYWdlQ2FsbGJhY2sgPSAoZGF0YT86IHN0cmluZykgPT4gdm9pZDtcclxuXHJcbmV4cG9ydCBpbnRlcmZhY2UgSUhvc3RNZXNzYWdpbmcge1xyXG4gICAgc2VuZE1lc3NhZ2VUb0hvc3QoaWQ6IFNlbmRUb0hvc3RNZXNzYWdlSWQgfCBzdHJpbmcsIGRhdGE/OiBzdHJpbmcpOiB2b2lkO1xyXG4gICAgYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6c3RyaW5nLCBjYWxsYmFjazpNZXNzYWdlQ2FsbGJhY2spOiB2b2lkO1xyXG4gICAgdW5yZWdpc3Rlck1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIobWVzc2FnZUlkOiBzdHJpbmcpIDogdm9pZDtcclxufSIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1xyXG4gICAgSUhvc3RNZXNzYWdpbmcsXHJcbiAgICBNZXNzYWdlQ2FsbGJhY2ssXHJcbiAgICBSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLFxyXG4gICAgU2VuZFRvSG9zdE1lc3NhZ2VJZCwgU2VuZFRvSG9zdE1lc3NhZ2VJZHNcclxufSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuaW1wb3J0IHtibGFua1RhcmdldEhhbmRsZXJ9IGZyb20gXCIuL0JsYW5rVGFyZ2V0SGFuZGxlclwiO1xyXG5pbXBvcnQge2dldFRpdGxlT2JzZXJ2ZXIsIFRpdGxlT2JzZXJ2ZXJUYXJnZXR9IGZyb20gXCIuL09ic2VydmVyc1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmNsYXNzIEhvc3RNZXNzYWdpbmcgaW1wbGVtZW50cyBJSG9zdE1lc3NhZ2luZyB7XHJcbiAgICBwcml2YXRlIG1lc3NhZ2VIYW5kbGVyczogTWFwPHN0cmluZywgTWVzc2FnZUNhbGxiYWNrPiA9IG5ldyBNYXAoKTtcclxuICAgIFxyXG4gICAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAgICAgdGhpcy5hc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKTtcclxuICAgICAgICBcclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3Rlck9wZW5FeHRlcm5hbCwgXyA9PiB7XHJcbiAgICAgICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoIFwiY2xpY2tcIiwgYmxhbmtUYXJnZXRIYW5kbGVyLCB7IGNhcHR1cmU6IHRydWUgfSk7XHJcbiAgICAgICAgfSlcclxuICAgICAgICBcclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3RlckZ1bGxzY3JlZW5DaGFuZ2UsIF8gPT4ge1xyXG4gICAgICAgICAgICBkb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwiZnVsbHNjcmVlbmNoYW5nZVwiLCAoXzogRXZlbnQpID0+IHtcclxuICAgICAgICAgICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkgdGhpcy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy5mdWxsc2NyZWVuRW50ZXIpO1xyXG4gICAgICAgICAgICAgICAgZWxzZSB0aGlzLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLmZ1bGxzY3JlZW5FeGl0KTtcclxuICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICBkb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwia2V5ZG93blwiLCBhc3luYyAoZTogS2V5Ym9hcmRFdmVudCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKGUua2V5ICE9PSBcIkYxMVwiKSByZXR1cm47XHJcbiAgICAgICAgICAgICAgICBpZiAoZG9jdW1lbnQuZnVsbHNjcmVlbkVsZW1lbnQpIGF3YWl0IGRvY3VtZW50LmV4aXRGdWxsc2NyZWVuKCk7XHJcbiAgICAgICAgICAgICAgICBlbHNlIGF3YWl0IGRvY3VtZW50LmJvZHkucmVxdWVzdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfSlcclxuICAgICAgICBcclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3RlclRpdGxlQ2hhbmdlLCBfID0+IHtcclxuICAgICAgICAgICAgaWYgKFRpdGxlT2JzZXJ2ZXJUYXJnZXQpIGdldFRpdGxlT2JzZXJ2ZXIoKS5vYnNlcnZlKFRpdGxlT2JzZXJ2ZXJUYXJnZXQsIHtjaGlsZExpc3Q6IHRydWV9KTtcclxuICAgICAgICB9KVxyXG4gICAgfVxyXG4gICAgICAgIFxyXG4gICAgcHVibGljIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkIHwgc3RyaW5nLCBkYXRhPzogc3RyaW5nKSB7XHJcbiAgICAgICAgY29uc3QgbWVzc2FnZSA9IGRhdGEgPyBgJHtpZH07JHtkYXRhfWAgOiBpZDtcclxuXHJcbiAgICAgICAgLy8gVE9ETyAtIGRldGVybWluZSBtZXNzYWdpbmcgbWV0aG9kcyBmb3IgUGhvdGluby5ORVQgZm9yIGFsbCBwbGF0Zm9ybXNcclxuICAgICAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xyXG4gICAgICAgICAgICB3aW5kb3cuY2hyb21lLndlYnZpZXcucG9zdE1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICAgICAgfSBlbHNlIGlmICh3aW5kb3cuZXh0ZXJuYWw/LnNlbmRNZXNzYWdlKSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5leHRlcm5hbC5zZW5kTWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICBjb25zb2xlLndhcm4oXCJNZXNzYWdlIHRvIGhvc3QgZmFpbGVkOlwiLCBtZXNzYWdlKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBhc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKSB7XHJcbiAgICAgICAgLy8gU3RvcmUgdGhlIG9yaWdpbmFsIHJlY2VpdmVNZXNzYWdlIGlmIGl0IGV4aXN0cyAoZm9yIEJsYXpvciBjb21wYXRpYmlsaXR5KVxyXG4gICAgICAgIGNvbnN0IG9yaWdpbmFsUmVjZWl2ZU1lc3NhZ2UgPSB3aW5kb3cuZXh0ZXJuYWw/LnJlY2VpdmVNZXNzYWdlO1xyXG5cclxuICAgICAgICAvLyBIYW5kbGUgV2ViVmlldzIgbWVzc2FnZXMgKFdpbmRvd3MpXHJcbiAgICAgICAgaWYgKHdpbmRvdy5jaHJvbWU/LndlYnZpZXcpIHtcclxuICAgICAgICAgICAgd2luZG93LmNocm9tZS53ZWJ2aWV3LmFkZEV2ZW50TGlzdGVuZXIoJ21lc3NhZ2UnLCAoZXZlbnQpID0+IHtcclxuICAgICAgICAgICAgICAgIGlmICghdGhpcy5pc0JsYXpvck1lc3NhZ2UoZXZlbnQuZGF0YSkpIHtcclxuICAgICAgICAgICAgICAgICAgICB0aGlzLmhhbmRsZVdlYk1lc3NhZ2UoZXZlbnQuZGF0YSk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gSGFuZGxlIGdlbmVyYWwgUGhvdGlubyBtZXNzYWdlcyAoY3Jvc3MtcGxhdGZvcm0pXHJcbiAgICAgICAgaWYgKHR5cGVvZiB3aW5kb3cgIT09ICd1bmRlZmluZWQnICYmIHdpbmRvdy5leHRlcm5hbCkge1xyXG4gICAgICAgICAgICB3aW5kb3cuZXh0ZXJuYWwucmVjZWl2ZU1lc3NhZ2UgPSAobWVzc2FnZTogYW55KSA9PiB7XHJcbiAgICAgICAgICAgICAgICAvLyBDaGVjayBpZiBpdCdzIGEgQmxhem9yIG1lc3NhZ2UgYW5kIHBhc3MgaXQgdG8gdGhlIG9yaWdpbmFsIGhhbmRsZXJcclxuICAgICAgICAgICAgICAgIGlmICh0aGlzLmlzQmxhem9yTWVzc2FnZShtZXNzYWdlKSkge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChvcmlnaW5hbFJlY2VpdmVNZXNzYWdlKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgICAgIG9yaWdpbmFsUmVjZWl2ZU1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICAgICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgICAgICAvLyBIYW5kbGUgb3VyIGN1c3RvbSBtZXNzYWdlc1xyXG4gICAgICAgICAgICAgICAgdGhpcy5oYW5kbGVXZWJNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgICAgICB9O1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIGlzQmxhem9yTWVzc2FnZShtZXNzYWdlOiBhbnkpOiBib29sZWFuIHtcclxuICAgICAgICBpZiAodHlwZW9mIG1lc3NhZ2UgIT09ICdzdHJpbmcnKSByZXR1cm4gdHJ1ZTsgLy8gQXNzdW1lIG5vbi1zdHJpbmcgbWVzc2FnZXMgYXJlIEJsYXpvclxyXG5cclxuICAgICAgICAvLyBDaGVjayBmb3IgY29tbW9uIEJsYXpvciBtZXNzYWdlIHBhdHRlcm5zXHJcbiAgICAgICAgcmV0dXJuIG1lc3NhZ2Uuc3RhcnRzV2l0aCgnX19id3Y6JykgXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2Uuc3RhcnRzV2l0aCgnZT0+eycpIFxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLmluY2x1ZGVzKCdCZWdpbkludm9rZUpTJykgXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2UuaW5jbHVkZXMoJ0F0dGFjaFRvRG9jdW1lbnQnKSBcclxuICAgICAgICAgICAgfHwgbWVzc2FnZS5pbmNsdWRlcygnUmVuZGVyQmF0Y2gnKSBcclxuICAgICAgICAgICAgfHwgbWVzc2FnZS5pbmNsdWRlcygnQmxhem9yLicpO1xyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgaGFuZGxlV2ViTWVzc2FnZShtZXNzYWdlOiBhbnkpIHtcclxuICAgICAgICAvLyBFbnN1cmUgbWVzc2FnZSBpcyBhIHN0cmluZ1xyXG4gICAgICAgIGNvbnN0IG1lc3NhZ2VTdHIgPSB0eXBlb2YgbWVzc2FnZSA9PT0gJ3N0cmluZycgPyBtZXNzYWdlIDogU3RyaW5nKG1lc3NhZ2UgfHwgJycpO1xyXG5cclxuICAgICAgICBpZiAoIW1lc3NhZ2VTdHIpIHtcclxuICAgICAgICAgICAgY29uc29sZS53YXJuKCdSZWNlaXZlZCBlbXB0eSBvciBpbnZhbGlkIG1lc3NhZ2UnKTtcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgbGV0IG1lc3NhZ2VJZDogc3RyaW5nO1xyXG4gICAgICAgIGxldCBkYXRhOiBzdHJpbmcgfCB1bmRlZmluZWQ7XHJcblxyXG4gICAgICAgIC8vIFBhcnNlIG1lc3NhZ2UgLSBjaGVjayBpZiBpdCBjb250YWlucyBkYXRhIHNlcGFyYXRlZCBieSBzZW1pY29sb25cclxuICAgICAgICBpZiAobWVzc2FnZVN0ci5pbmNsdWRlcygnOycpKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IHBhcnRzID0gbWVzc2FnZVN0ci5zcGxpdCgnOycsIDIpO1xyXG4gICAgICAgICAgICBtZXNzYWdlSWQgPSBwYXJ0c1swXTtcclxuICAgICAgICAgICAgZGF0YSA9IHBhcnRzWzFdO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIG1lc3NhZ2VJZCA9IG1lc3NhZ2VTdHI7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBFeGVjdXRlIHJlZ2lzdGVyZWQgaGFuZGxlclxyXG4gICAgICAgIGNvbnN0IGhhbmRsZXIgPSB0aGlzLm1lc3NhZ2VIYW5kbGVycy5nZXQobWVzc2FnZUlkKTtcclxuICAgICAgICBpZiAoaGFuZGxlcikge1xyXG4gICAgICAgICAgICBoYW5kbGVyKGRhdGEpO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUud2FybignTm8gaGFuZGxlciByZWdpc3RlcmVkIGZvciBtZXNzYWdlIElEOicsIG1lc3NhZ2VJZCk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG4gICAgXHJcbiAgICBwdWJsaWMgYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6c3RyaW5nLCBjYWxsYmFjazpNZXNzYWdlQ2FsbGJhY2spIHtcclxuICAgICAgICB0aGlzLm1lc3NhZ2VIYW5kbGVycy5zZXQobWVzc2FnZUlkLCBjYWxsYmFjayk7XHJcbiAgICB9XHJcbiAgICBcclxuICAgIHB1YmxpYyB1bnJlZ2lzdGVyTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZykge1xyXG4gICAgICAgIHRoaXMubWVzc2FnZUhhbmRsZXJzLmRlbGV0ZShtZXNzYWdlSWQpO1xyXG4gICAgfVxyXG59XHJcblxyXG5leHBvcnQgZGVmYXVsdCBIb3N0TWVzc2FnaW5nXHJcbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQgSW5maW5pV2luZG93IGZyb20gXCIuL0luZmluaVdpbmRvd1wiO1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IHt9O1xyXG53aW5kb3cuaW5maW5pV2luZG93ID0gbmV3IEluZmluaVdpbmRvdygpO1xyXG4iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtJSW5maW5pV2luZG93fSBmcm9tIFwiLi9Db250cmFjdHMvSUluZmluaVdpbmRvd1wiO1xyXG5pbXBvcnQge0lIb3N0TWVzc2FnaW5nLCBTZW5kVG9Ib3N0TWVzc2FnZUlkfSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuaW1wb3J0IEhvc3RNZXNzYWdpbmcgZnJvbSBcIi4vSG9zdE1lc3NhZ2luZ1wiO1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGNsYXNzIEluZmluaVdpbmRvdyBpbXBsZW1lbnRzIElJbmZpbmlXaW5kb3cge1xyXG4gICAgSG9zdE1lc3NhZ2luZyA6IElIb3N0TWVzc2FnaW5nID0gbmV3IEhvc3RNZXNzYWdpbmcoKTtcclxuXHJcbiAgICAvLyBPdmVybG9hZCB0byBtYWtlIGEgZGV2J3MgbGlmZSBlYXNpZXIgaW5zdGVhZCBvZiBoYXZpbmcgdG8gZ28gdG8gdGhlIEhvc3RNZXNzYWdpbmcgY2xhc3NcclxuICAgIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkLCBkYXRhPzogc3RyaW5nKSB7XHJcbiAgICAgICAgdGhpcy5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KGlkLCBkYXRhKTtcclxuICAgIH1cclxuICAgIFxyXG4gICAgc2V0UG9pbnRlckNhcHR1cmUoZWxlbWVudDpFbGVtZW50LCBwb2ludGVySWQ6IG51bWJlcikge1xyXG4gICAgICAgIGVsZW1lbnQuc2V0UG9pbnRlckNhcHR1cmUocG9pbnRlcklkKTtcclxuICAgIH1cclxuXHJcbiAgICByZWxlYXNlUG9pbnRlckNhcHR1cmUoZWxlbWVudDpFbGVtZW50LCBwb2ludGVySWQ6bnVtYmVyKSB7XHJcbiAgICAgICAgZWxlbWVudC5yZWxlYXNlUG9pbnRlckNhcHR1cmUocG9pbnRlcklkKTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGRlZmF1bHQgSW5maW5pV2luZG93IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7U2VuZFRvSG9zdE1lc3NhZ2VJZHN9IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBjb25zdCBUaXRsZU9ic2VydmVyVGFyZ2V0IDogSFRNTFRpdGxlRWxlbWVudCB8IG51bGwgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKCd0aXRsZScpO1xyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIGdldFRpdGxlT2JzZXJ2ZXIoKSA6IE11dGF0aW9uT2JzZXJ2ZXIge1xyXG4gICAgcmV0dXJuIG5ldyBNdXRhdGlvbk9ic2VydmVyKChtdXRhdGlvbnMsIF8pID0+IHtcclxuICAgICAgICBtdXRhdGlvbnMuZm9yRWFjaCgobXV0YXRpb24pID0+IHtcclxuICAgICAgICAgICAgaWYgKG11dGF0aW9uLnR5cGUgIT09IFwiY2hpbGRMaXN0XCIpIHJldHVybjtcclxuICAgICAgICAgICAgd2luZG93LmluZmluaVdpbmRvdy5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLnRpdGxlQ2hhbmdlLCBkb2N1bWVudC50aXRsZSk7XHJcbiAgICAgICAgfSlcclxuICAgIH0pXHJcbn0iLCIvLyBUaGUgbW9kdWxlIGNhY2hlXG52YXIgX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fID0ge307XG5cbi8vIFRoZSByZXF1aXJlIGZ1bmN0aW9uXG5mdW5jdGlvbiBfX3dlYnBhY2tfcmVxdWlyZV9fKG1vZHVsZUlkKSB7XG5cdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuXHR2YXIgY2FjaGVkTW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXTtcblx0aWYgKGNhY2hlZE1vZHVsZSAhPT0gdW5kZWZpbmVkKSB7XG5cdFx0cmV0dXJuIGNhY2hlZE1vZHVsZS5leHBvcnRzO1xuXHR9XG5cdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG5cdHZhciBtb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdID0ge1xuXHRcdC8vIG5vIG1vZHVsZS5pZCBuZWVkZWRcblx0XHQvLyBubyBtb2R1bGUubG9hZGVkIG5lZWRlZFxuXHRcdGV4cG9ydHM6IHt9XG5cdH07XG5cblx0Ly8gRXhlY3V0ZSB0aGUgbW9kdWxlIGZ1bmN0aW9uXG5cdF9fd2VicGFja19tb2R1bGVzX19bbW9kdWxlSWRdLmNhbGwobW9kdWxlLmV4cG9ydHMsIG1vZHVsZSwgbW9kdWxlLmV4cG9ydHMsIF9fd2VicGFja19yZXF1aXJlX18pO1xuXG5cdC8vIFJldHVybiB0aGUgZXhwb3J0cyBvZiB0aGUgbW9kdWxlXG5cdHJldHVybiBtb2R1bGUuZXhwb3J0cztcbn1cblxuIiwiIiwiLy8gc3RhcnR1cFxuLy8gTG9hZCBlbnRyeSBtb2R1bGUgYW5kIHJldHVybiBleHBvcnRzXG4vLyBUaGlzIGVudHJ5IG1vZHVsZSBpcyByZWZlcmVuY2VkIGJ5IG90aGVyIG1vZHVsZXMgc28gaXQgY2FuJ3QgYmUgaW5saW5lZFxudmFyIF9fd2VicGFja19leHBvcnRzX18gPSBfX3dlYnBhY2tfcmVxdWlyZV9fKFwiLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL0luZGV4LnRzXCIpO1xuIiwiIl0sIm5hbWVzIjpbXSwic291cmNlUm9vdCI6IiJ9