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
    titleChange: "title:change",
    fullscreenEnter: "fullscreen:enter",
    fullscreenExit: "fullscreen:exit",
    openExternalLink: "open:external",
};
exports.ReceiveFromHostMessageIds = {
    registerOpenExternal: "register:open:external",
    registerFullscreenChange: "register:fullscreen:change",
    registerTitleChange: "register:title:change",
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
exports.HostMessaging = void 0;
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
        var _a;
        if ((_a = window.chrome) === null || _a === void 0 ? void 0 : _a.webview) {
            window.chrome.webview.addEventListener('message', (event) => {
                this.handleWebMessage(event.data);
            });
        }
        if (typeof window !== 'undefined' && window.external) {
            window.external.receiveMessage = (message) => {
                this.handleWebMessage(message);
            };
        }
    }
    handleWebMessage(message) {
        let messageId;
        let data;
        if (message.includes(';')) {
            const parts = message.split(';', 2);
            messageId = parts[0];
            data = parts[1];
        }
        else {
            messageId = message;
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
exports.HostMessaging = HostMessaging;


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
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.InfiniWindow = void 0;
const HostMessaging_1 = __webpack_require__(/*! ./HostMessaging */ "./src/InfiniLore.Photino.Js/TsSource/HostMessaging.ts");
class InfiniWindow {
    constructor() {
        this.HostMessaging = new HostMessaging_1.HostMessaging();
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
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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LFFBQVEsQ0FBQyxhQUFhLENBQUMsT0FBTyxDQUFDLENBQUM7QUFFN0YsU0FBZ0IsZ0JBQWdCO0lBQzVCLE9BQU8sSUFBSSxnQkFBZ0IsQ0FBQyxDQUFDLFNBQVMsRUFBRSxDQUFDLEVBQUUsRUFBRTtRQUN6QyxTQUFTLENBQUMsT0FBTyxDQUFDLENBQUMsUUFBUSxFQUFFLEVBQUU7WUFDM0IsSUFBSSxRQUFRLENBQUMsSUFBSSxLQUFLLFdBQVc7Z0JBQUUsT0FBTztZQUMxQyxNQUFNLENBQUMsWUFBWSxDQUFDLGFBQWEsQ0FBQyxpQkFBaUIsQ0FBQyxxQ0FBb0IsQ0FBQyxXQUFXLEVBQUUsUUFBUSxDQUFDLEtBQUssQ0FBQyxDQUFDO1FBQzFHLENBQUMsQ0FBQztJQUNOLENBQUMsQ0FBQztBQUNOLENBQUM7Ozs7Ozs7VUNqQkQ7VUFDQTs7VUFFQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTs7VUFFQTtVQUNBOztVQUVBO1VBQ0E7VUFDQTs7OztVRXRCQTtVQUNBO1VBQ0E7VUFDQSIsInNvdXJjZXMiOlsid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvQmxhbmtUYXJnZXRIYW5kbGVyLnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nLnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvSG9zdE1lc3NhZ2luZy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL0luZGV4LnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvSW5maW5pV2luZG93LnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvT2JzZXJ2ZXJzLnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL2Jvb3RzdHJhcCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vd2VicGFjay9iZWZvcmUtc3RhcnR1cCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vd2VicGFjay9zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL2FmdGVyLXN0YXJ0dXAiXSwic291cmNlc0NvbnRlbnQiOlsiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7U2VuZFRvSG9zdE1lc3NhZ2VJZHN9IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmZ1bmN0aW9uIGlzRXh0ZXJuYWxMaW5rKHVybDogc3RyaW5nKTogYm9vbGVhbiB7XHJcbiAgICB0cnkge1xyXG4gICAgICAgIHJldHVybiBuZXcgVVJMKHVybCwgbG9jYXRpb24uaHJlZikuaG9zdG5hbWUgIT09IGxvY2F0aW9uLmhvc3RuYW1lO1xyXG4gICAgfSBjYXRjaCB7XHJcbiAgICAgICAgcmV0dXJuIGZhbHNlO1xyXG4gICAgfVxyXG59XHJcblxyXG5leHBvcnQgYXN5bmMgZnVuY3Rpb24gYmxhbmtUYXJnZXRIYW5kbGVyKGU6IE1vdXNlRXZlbnQpIHtcclxuICAgIGxldCBlbCA9IGUudGFyZ2V0IGFzIEhUTUxFbGVtZW50IHwgbnVsbDtcclxuICAgIFxyXG4gICAgd2hpbGUgKGVsICYmIGVsICE9PSBkb2N1bWVudC5ib2R5KSB7XHJcbiAgICAgICAgaWYgKGVsLnRhZ05hbWU/LnRvTG93ZXJDYXNlKCkgIT09IFwiYVwiKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIFxyXG4gICAgICAgIGNvbnN0IGFuY2hvciA9IGVsIGFzIEhUTUxBbmNob3JFbGVtZW50O1xyXG4gICAgICAgIGlmICghYW5jaG9yLmhyZWYpIHtcclxuICAgICAgICAgICAgZWwgPSBlbC5wYXJlbnRFbGVtZW50O1xyXG4gICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGNvbnN0IHRhcmdldCA9IGFuY2hvci5nZXRBdHRyaWJ1dGUoXCJ0YXJnZXRcIik7XHJcbiAgICAgICAgaWYgKCEodGFyZ2V0ID09PSBcIl9ibGFua1wiIHx8IGFuY2hvci5oYXNBdHRyaWJ1dGUoXCJkYXRhLWV4dGVybmFsXCIpIHx8IGlzRXh0ZXJuYWxMaW5rKGFuY2hvci5ocmVmKSkpIHtcclxuICAgICAgICAgICAgZWwgPSBlbC5wYXJlbnRFbGVtZW50O1xyXG4gICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICB9XHJcbiAgICAgICAgXHJcbiAgICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG4gICAgICAgIHdpbmRvdy5pbmZpbmlXaW5kb3cuSG9zdE1lc3NhZ2luZy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy5vcGVuRXh0ZXJuYWxMaW5rLCBhbmNob3IuaHJlZik7XHJcbiAgICAgICAgcmV0dXJuO1xyXG4gICAgfVxyXG59IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGNvbnN0IFNlbmRUb0hvc3RNZXNzYWdlSWRzID0ge1xyXG4gICAgdGl0bGVDaGFuZ2U6IFwidGl0bGU6Y2hhbmdlXCIsXHJcbiAgICBmdWxsc2NyZWVuRW50ZXI6IFwiZnVsbHNjcmVlbjplbnRlclwiLFxyXG4gICAgZnVsbHNjcmVlbkV4aXQ6IFwiZnVsbHNjcmVlbjpleGl0XCIsXHJcbiAgICBvcGVuRXh0ZXJuYWxMaW5rOiBcIm9wZW46ZXh0ZXJuYWxcIixcclxufVxyXG5cclxuZXhwb3J0IGNvbnN0IFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMgPSB7XHJcbiAgICByZWdpc3Rlck9wZW5FeHRlcm5hbDogXCJyZWdpc3RlcjpvcGVuOmV4dGVybmFsXCIsXHJcbiAgICByZWdpc3RlckZ1bGxzY3JlZW5DaGFuZ2U6IFwicmVnaXN0ZXI6ZnVsbHNjcmVlbjpjaGFuZ2VcIixcclxuICAgIHJlZ2lzdGVyVGl0bGVDaGFuZ2U6IFwicmVnaXN0ZXI6dGl0bGU6Y2hhbmdlXCIsXHJcbn1cclxuXHJcbmV4cG9ydCB0eXBlIFNlbmRUb0hvc3RNZXNzYWdlSWQgPSB0eXBlb2YgU2VuZFRvSG9zdE1lc3NhZ2VJZHNba2V5b2YgdHlwZW9mIFNlbmRUb0hvc3RNZXNzYWdlSWRzXTtcclxuZXhwb3J0IHR5cGUgTWVzc2FnZUNhbGxiYWNrID0gKGRhdGE/OiBzdHJpbmcpID0+IHZvaWQ7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIElIb3N0TWVzc2FnaW5nIHtcclxuICAgIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkLCBkYXRhPzogc3RyaW5nKTogdm9pZDtcclxuICAgIGFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIobWVzc2FnZUlkOnN0cmluZywgY2FsbGJhY2s6TWVzc2FnZUNhbGxiYWNrKTogdm9pZDtcclxuICAgIHVucmVnaXN0ZXJNZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nKSA6IHZvaWQ7XHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtcclxuICAgIElIb3N0TWVzc2FnaW5nLFxyXG4gICAgTWVzc2FnZUNhbGxiYWNrLFxyXG4gICAgUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcyxcclxuICAgIFNlbmRUb0hvc3RNZXNzYWdlSWQsIFNlbmRUb0hvc3RNZXNzYWdlSWRzXHJcbn0gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcbmltcG9ydCB7YmxhbmtUYXJnZXRIYW5kbGVyfSBmcm9tIFwiLi9CbGFua1RhcmdldEhhbmRsZXJcIjtcclxuaW1wb3J0IHtnZXRUaXRsZU9ic2VydmVyLCBUaXRsZU9ic2VydmVyVGFyZ2V0fSBmcm9tIFwiLi9PYnNlcnZlcnNcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY2xhc3MgSG9zdE1lc3NhZ2luZyBpbXBsZW1lbnRzIElIb3N0TWVzc2FnaW5nIHtcclxuICAgIHByaXZhdGUgbWVzc2FnZUhhbmRsZXJzOiBNYXA8c3RyaW5nLCBNZXNzYWdlQ2FsbGJhY2s+ID0gbmV3IE1hcCgpO1xyXG4gICAgXHJcbiAgICBjb25zdHJ1Y3RvcigpIHtcclxuICAgICAgICB0aGlzLmFzc2lnbldlYk1lc3NhZ2VSZWNlaXZlcigpO1xyXG4gICAgICAgIFxyXG4gICAgICAgIHRoaXMuYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLnJlZ2lzdGVyT3BlbkV4dGVybmFsLCBfID0+IHtcclxuICAgICAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lciggXCJjbGlja1wiLCBibGFua1RhcmdldEhhbmRsZXIsIHsgY2FwdHVyZTogdHJ1ZSB9KTtcclxuICAgICAgICB9KVxyXG4gICAgICAgIFxyXG4gICAgICAgIHRoaXMuYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLnJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZSwgXyA9PiB7XHJcbiAgICAgICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJmdWxsc2NyZWVuY2hhbmdlXCIsIChfOiBFdmVudCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKGRvY3VtZW50LmZ1bGxzY3JlZW5FbGVtZW50KSB0aGlzLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLmZ1bGxzY3JlZW5FbnRlcik7XHJcbiAgICAgICAgICAgICAgICBlbHNlIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMuZnVsbHNjcmVlbkV4aXQpO1xyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJrZXlkb3duXCIsIGFzeW5jIChlOiBLZXlib2FyZEV2ZW50KSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoZS5rZXkgIT09IFwiRjExXCIpIHJldHVybjtcclxuICAgICAgICAgICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkgYXdhaXQgZG9jdW1lbnQuZXhpdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICAgICAgICAgIGVsc2UgYXdhaXQgZG9jdW1lbnQuYm9keS5yZXF1ZXN0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9KVxyXG4gICAgICAgIFxyXG4gICAgICAgIHRoaXMuYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLnJlZ2lzdGVyVGl0bGVDaGFuZ2UsIF8gPT4ge1xyXG4gICAgICAgICAgICBpZiAoVGl0bGVPYnNlcnZlclRhcmdldCkgZ2V0VGl0bGVPYnNlcnZlcigpLm9ic2VydmUoVGl0bGVPYnNlcnZlclRhcmdldCwge2NoaWxkTGlzdDogdHJ1ZX0pO1xyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcbiAgICAgICAgXHJcbiAgICBwdWJsaWMgc2VuZE1lc3NhZ2VUb0hvc3QoaWQ6IFNlbmRUb0hvc3RNZXNzYWdlSWQsIGRhdGE/OiBzdHJpbmcpIHtcclxuICAgICAgICBjb25zdCBtZXNzYWdlID0gZGF0YSA/IGAke2lkfTske2RhdGF9YCA6IGlkO1xyXG5cclxuICAgICAgICAvLyBUT0RPIC0gZGV0ZXJtaW5lIG1lc3NhZ2luZyBtZXRob2RzIGZvciBQaG90aW5vLk5FVCBmb3IgYWxsIHBsYXRmb3Jtc1xyXG4gICAgICAgIGlmICh3aW5kb3cuY2hyb21lPy53ZWJ2aWV3KSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5jaHJvbWUud2Vidmlldy5wb3N0TWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICB9IGVsc2UgaWYgKHdpbmRvdy5leHRlcm5hbD8uc2VuZE1lc3NhZ2UpIHtcclxuICAgICAgICAgICAgd2luZG93LmV4dGVybmFsLnNlbmRNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUud2FybihcIk1lc3NhZ2UgdG8gaG9zdCBmYWlsZWQ6XCIsIG1lc3NhZ2UpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIGFzc2lnbldlYk1lc3NhZ2VSZWNlaXZlcigpIHtcclxuICAgICAgICAvLyBIYW5kbGUgV2ViVmlldzIgbWVzc2FnZXMgKFdpbmRvd3MpXHJcbiAgICAgICAgaWYgKHdpbmRvdy5jaHJvbWU/LndlYnZpZXcpIHtcclxuICAgICAgICAgICAgd2luZG93LmNocm9tZS53ZWJ2aWV3LmFkZEV2ZW50TGlzdGVuZXIoJ21lc3NhZ2UnLCAoZXZlbnQpID0+IHtcclxuICAgICAgICAgICAgICAgIHRoaXMuaGFuZGxlV2ViTWVzc2FnZShldmVudC5kYXRhKTtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBIYW5kbGUgZ2VuZXJhbCBQaG90aW5vIG1lc3NhZ2VzIChjcm9zcy1wbGF0Zm9ybSlcclxuICAgICAgICBpZiAodHlwZW9mIHdpbmRvdyAhPT0gJ3VuZGVmaW5lZCcgJiYgd2luZG93LmV4dGVybmFsKSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5leHRlcm5hbC5yZWNlaXZlTWVzc2FnZSA9IChtZXNzYWdlOiBzdHJpbmcpID0+IHtcclxuICAgICAgICAgICAgICAgIHRoaXMuaGFuZGxlV2ViTWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICAgICAgfTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBoYW5kbGVXZWJNZXNzYWdlKG1lc3NhZ2U6IHN0cmluZykge1xyXG4gICAgICAgIGxldCBtZXNzYWdlSWQ6IHN0cmluZztcclxuICAgICAgICBsZXQgZGF0YTogc3RyaW5nIHwgdW5kZWZpbmVkO1xyXG5cclxuICAgICAgICAvLyBQYXJzZSBtZXNzYWdlIC0gY2hlY2sgaWYgaXQgY29udGFpbnMgZGF0YSBzZXBhcmF0ZWQgYnkgc2VtaWNvbG9uXHJcbiAgICAgICAgaWYgKG1lc3NhZ2UuaW5jbHVkZXMoJzsnKSkge1xyXG4gICAgICAgICAgICBjb25zdCBwYXJ0cyA9IG1lc3NhZ2Uuc3BsaXQoJzsnLCAyKTtcclxuICAgICAgICAgICAgbWVzc2FnZUlkID0gcGFydHNbMF07XHJcbiAgICAgICAgICAgIGRhdGEgPSBwYXJ0c1sxXTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICBtZXNzYWdlSWQgPSBtZXNzYWdlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gRXhlY3V0ZSByZWdpc3RlcmVkIGhhbmRsZXJcclxuICAgICAgICBjb25zdCBoYW5kbGVyID0gdGhpcy5tZXNzYWdlSGFuZGxlcnMuZ2V0KG1lc3NhZ2VJZCk7XHJcbiAgICAgICAgaWYgKGhhbmRsZXIpIHtcclxuICAgICAgICAgICAgaGFuZGxlcihkYXRhKTtcclxuICAgICAgICB9IGVsc2Uge1xyXG4gICAgICAgICAgICBjb25zb2xlLndhcm4oJ05vIGhhbmRsZXIgcmVnaXN0ZXJlZCBmb3IgbWVzc2FnZSBJRDonLCBtZXNzYWdlSWQpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuICAgIFxyXG4gICAgcHVibGljIGFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIobWVzc2FnZUlkOnN0cmluZywgY2FsbGJhY2s6TWVzc2FnZUNhbGxiYWNrKSB7XHJcbiAgICAgICAgdGhpcy5tZXNzYWdlSGFuZGxlcnMuc2V0KG1lc3NhZ2VJZCwgY2FsbGJhY2spO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICBwdWJsaWMgdW5yZWdpc3Rlck1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIobWVzc2FnZUlkOiBzdHJpbmcpIHtcclxuICAgICAgICB0aGlzLm1lc3NhZ2VIYW5kbGVycy5kZWxldGUobWVzc2FnZUlkKTtcclxuICAgIH1cclxufVxyXG4iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IEluZmluaVdpbmRvdyBmcm9tIFwiLi9JbmZpbmlXaW5kb3dcIjtcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCB7fTtcclxud2luZG93LmluZmluaVdpbmRvdyA9IG5ldyBJbmZpbmlXaW5kb3coKTtcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7SUluZmluaVdpbmRvd30gZnJvbSBcIi4vQ29udHJhY3RzL0lJbmZpbmlXaW5kb3dcIjtcclxuaW1wb3J0IHtJSG9zdE1lc3NhZ2luZywgU2VuZFRvSG9zdE1lc3NhZ2VJZH0gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcbmltcG9ydCB7SG9zdE1lc3NhZ2luZ30gZnJvbSBcIi4vSG9zdE1lc3NhZ2luZ1wiO1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGNsYXNzIEluZmluaVdpbmRvdyBpbXBsZW1lbnRzIElJbmZpbmlXaW5kb3cge1xyXG4gICAgSG9zdE1lc3NhZ2luZyA6IElIb3N0TWVzc2FnaW5nID0gbmV3IEhvc3RNZXNzYWdpbmcoKTtcclxuXHJcbiAgICAvLyBPdmVybG9hZCB0byBtYWtlIGEgZGV2J3MgbGlmZSBlYXNpZXIgaW5zdGVhZCBvZiBoYXZpbmcgdG8gZ28gdG8gdGhlIEhvc3RNZXNzYWdpbmcgY2xhc3NcclxuICAgIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkLCBkYXRhPzogc3RyaW5nKSB7XHJcbiAgICAgICAgdGhpcy5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KGlkLCBkYXRhKTtcclxuICAgIH1cclxuICAgIFxyXG4gICAgc2V0UG9pbnRlckNhcHR1cmUoZWxlbWVudDpFbGVtZW50LCBwb2ludGVySWQ6IG51bWJlcikge1xyXG4gICAgICAgIGVsZW1lbnQuc2V0UG9pbnRlckNhcHR1cmUocG9pbnRlcklkKTtcclxuICAgIH1cclxuXHJcbiAgICByZWxlYXNlUG9pbnRlckNhcHR1cmUoZWxlbWVudDpFbGVtZW50LCBwb2ludGVySWQ6bnVtYmVyKSB7XHJcbiAgICAgICAgZWxlbWVudC5yZWxlYXNlUG9pbnRlckNhcHR1cmUocG9pbnRlcklkKTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGRlZmF1bHQgSW5maW5pV2luZG93IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7U2VuZFRvSG9zdE1lc3NhZ2VJZHN9IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBjb25zdCBUaXRsZU9ic2VydmVyVGFyZ2V0IDogSFRNTFRpdGxlRWxlbWVudCB8IG51bGwgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKCd0aXRsZScpO1xyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIGdldFRpdGxlT2JzZXJ2ZXIoKSA6IE11dGF0aW9uT2JzZXJ2ZXIge1xyXG4gICAgcmV0dXJuIG5ldyBNdXRhdGlvbk9ic2VydmVyKChtdXRhdGlvbnMsIF8pID0+IHtcclxuICAgICAgICBtdXRhdGlvbnMuZm9yRWFjaCgobXV0YXRpb24pID0+IHtcclxuICAgICAgICAgICAgaWYgKG11dGF0aW9uLnR5cGUgIT09IFwiY2hpbGRMaXN0XCIpIHJldHVybjtcclxuICAgICAgICAgICAgd2luZG93LmluZmluaVdpbmRvdy5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLnRpdGxlQ2hhbmdlLCBkb2N1bWVudC50aXRsZSk7XHJcbiAgICAgICAgfSlcclxuICAgIH0pXHJcbn0iLCIvLyBUaGUgbW9kdWxlIGNhY2hlXG52YXIgX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fID0ge307XG5cbi8vIFRoZSByZXF1aXJlIGZ1bmN0aW9uXG5mdW5jdGlvbiBfX3dlYnBhY2tfcmVxdWlyZV9fKG1vZHVsZUlkKSB7XG5cdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuXHR2YXIgY2FjaGVkTW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXTtcblx0aWYgKGNhY2hlZE1vZHVsZSAhPT0gdW5kZWZpbmVkKSB7XG5cdFx0cmV0dXJuIGNhY2hlZE1vZHVsZS5leHBvcnRzO1xuXHR9XG5cdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG5cdHZhciBtb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdID0ge1xuXHRcdC8vIG5vIG1vZHVsZS5pZCBuZWVkZWRcblx0XHQvLyBubyBtb2R1bGUubG9hZGVkIG5lZWRlZFxuXHRcdGV4cG9ydHM6IHt9XG5cdH07XG5cblx0Ly8gRXhlY3V0ZSB0aGUgbW9kdWxlIGZ1bmN0aW9uXG5cdF9fd2VicGFja19tb2R1bGVzX19bbW9kdWxlSWRdLmNhbGwobW9kdWxlLmV4cG9ydHMsIG1vZHVsZSwgbW9kdWxlLmV4cG9ydHMsIF9fd2VicGFja19yZXF1aXJlX18pO1xuXG5cdC8vIFJldHVybiB0aGUgZXhwb3J0cyBvZiB0aGUgbW9kdWxlXG5cdHJldHVybiBtb2R1bGUuZXhwb3J0cztcbn1cblxuIiwiIiwiLy8gc3RhcnR1cFxuLy8gTG9hZCBlbnRyeSBtb2R1bGUgYW5kIHJldHVybiBleHBvcnRzXG4vLyBUaGlzIGVudHJ5IG1vZHVsZSBpcyByZWZlcmVuY2VkIGJ5IG90aGVyIG1vZHVsZXMgc28gaXQgY2FuJ3QgYmUgaW5saW5lZFxudmFyIF9fd2VicGFja19leHBvcnRzX18gPSBfX3dlYnBhY2tfcmVxdWlyZV9fKFwiLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL0luZGV4LnRzXCIpO1xuIiwiIl0sIm5hbWVzIjpbXSwic291cmNlUm9vdCI6IiJ9