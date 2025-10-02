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
exports.SendToHostMessageIds = void 0;
exports.SendToHostMessageIds = {
    titleChange: "title:change",
    fullscreenEnter: "fullscreen:enter",
    fullscreenExit: "fullscreen:exit",
    openExternalLink: "open:external",
};


/***/ }),

/***/ "./src/InfiniLore.Photino.Js/TsSource/HostMessaging.ts":
/*!*************************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/HostMessaging.ts ***!
  \*************************************************************/
/***/ ((__unused_webpack_module, exports) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.HostMessaging = void 0;
class HostMessaging {
    constructor() {
        this.messageHandlers = new Map();
        this.assignWebMessageReceiver();
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
exports.InfiniWindow = void 0;
const Observers_1 = __webpack_require__(/*! ./Observers */ "./src/InfiniLore.Photino.Js/TsSource/Observers.ts");
const BlankTargetHandler_1 = __webpack_require__(/*! ./BlankTargetHandler */ "./src/InfiniLore.Photino.Js/TsSource/BlankTargetHandler.ts");
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.Photino.Js/TsSource/Contracts/IHostMessaging.ts");
const HostMessaging_1 = __webpack_require__(/*! ./HostMessaging */ "./src/InfiniLore.Photino.Js/TsSource/HostMessaging.ts");
class InfiniWindow {
    constructor() {
        this.HostMessaging = new HostMessaging_1.HostMessaging();
        this.assignEventListeners();
        this.assignObservers();
    }
    setPointerCapture(element, pointerId) {
        element.setPointerCapture(pointerId);
    }
    releasePointerCapture(element, pointerId) {
        element.releasePointerCapture(pointerId);
    }
    assignObservers() {
        if (Observers_1.TitleObserverTarget)
            (0, Observers_1.getTitleObserver)().observe(Observers_1.TitleObserverTarget, { childList: true });
    }
    assignEventListeners() {
        document.addEventListener("fullscreenchange", (_) => {
            if (document.fullscreenElement)
                this.HostMessaging.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.fullscreenEnter);
            else
                this.HostMessaging.sendMessageToHost(IHostMessaging_1.SendToHostMessageIds.fullscreenExit);
        });
        document.addEventListener("keydown", (e) => __awaiter(this, void 0, void 0, function* () {
            if (e.key !== "F11")
                return;
            if (document.fullscreenElement)
                yield document.exitFullscreen();
            else
                yield document.body.requestFullscreen();
        }));
        document.addEventListener("click", BlankTargetHandler_1.blankTargetHandler, { capture: true });
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
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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QyxDQUFDO0FBQ04sQ0FBQzs7Ozs7OztVQ2pCRDtVQUNBOztVQUVBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBOztVQUVBO1VBQ0E7O1VBRUE7VUFDQTtVQUNBOzs7O1VFdEJBO1VBQ0E7VUFDQTtVQUNBIiwic291cmNlcyI6WyJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9CbGFua1RhcmdldEhhbmRsZXIudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9Ib3N0TWVzc2FnaW5nLnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvSW5kZXgudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9JbmZpbmlXaW5kb3cudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9PYnNlcnZlcnMudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svYm9vdHN0cmFwIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL2JlZm9yZS1zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL3N0YXJ0dXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svYWZ0ZXItc3RhcnR1cCJdLCJzb3VyY2VzQ29udGVudCI6WyIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtTZW5kVG9Ib3N0TWVzc2FnZUlkc30gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZnVuY3Rpb24gaXNFeHRlcm5hbExpbmsodXJsOiBzdHJpbmcpOiBib29sZWFuIHtcclxuICAgIHRyeSB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBVUkwodXJsLCBsb2NhdGlvbi5ocmVmKS5ob3N0bmFtZSAhPT0gbG9jYXRpb24uaG9zdG5hbWU7XHJcbiAgICB9IGNhdGNoIHtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBhc3luYyBmdW5jdGlvbiBibGFua1RhcmdldEhhbmRsZXIoZTogTW91c2VFdmVudCkge1xyXG4gICAgbGV0IGVsID0gZS50YXJnZXQgYXMgSFRNTEVsZW1lbnQgfCBudWxsO1xyXG4gICAgXHJcbiAgICB3aGlsZSAoZWwgJiYgZWwgIT09IGRvY3VtZW50LmJvZHkpIHtcclxuICAgICAgICBpZiAoZWwudGFnTmFtZT8udG9Mb3dlckNhc2UoKSAhPT0gXCJhXCIpIHtcclxuICAgICAgICAgICAgZWwgPSBlbC5wYXJlbnRFbGVtZW50O1xyXG4gICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICB9XHJcbiAgICAgICAgXHJcbiAgICAgICAgY29uc3QgYW5jaG9yID0gZWwgYXMgSFRNTEFuY2hvckVsZW1lbnQ7XHJcbiAgICAgICAgaWYgKCFhbmNob3IuaHJlZikge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgdGFyZ2V0ID0gYW5jaG9yLmdldEF0dHJpYnV0ZShcInRhcmdldFwiKTtcclxuICAgICAgICBpZiAoISh0YXJnZXQgPT09IFwiX2JsYW5rXCIgfHwgYW5jaG9yLmhhc0F0dHJpYnV0ZShcImRhdGEtZXh0ZXJuYWxcIikgfHwgaXNFeHRlcm5hbExpbmsoYW5jaG9yLmhyZWYpKSkge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuICAgICAgICBcclxuICAgICAgICBlLnByZXZlbnREZWZhdWx0KCk7XHJcbiAgICAgICAgd2luZG93LmluZmluaVdpbmRvdy5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLm9wZW5FeHRlcm5hbExpbmssIGFuY2hvci5ocmVmKTtcclxuICAgICAgICByZXR1cm47XHJcbiAgICB9XHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY29uc3QgU2VuZFRvSG9zdE1lc3NhZ2VJZHMgPSB7XHJcbiAgICB0aXRsZUNoYW5nZTogXCJ0aXRsZTpjaGFuZ2VcIixcclxuICAgIGZ1bGxzY3JlZW5FbnRlcjogXCJmdWxsc2NyZWVuOmVudGVyXCIsXHJcbiAgICBmdWxsc2NyZWVuRXhpdDogXCJmdWxsc2NyZWVuOmV4aXRcIixcclxuICAgIG9wZW5FeHRlcm5hbExpbms6IFwib3BlbjpleHRlcm5hbFwiLFxyXG59XHJcblxyXG5leHBvcnQgdHlwZSBTZW5kVG9Ib3N0TWVzc2FnZUlkID0gdHlwZW9mIFNlbmRUb0hvc3RNZXNzYWdlSWRzW2tleW9mIHR5cGVvZiBTZW5kVG9Ib3N0TWVzc2FnZUlkc107XHJcbmV4cG9ydCB0eXBlIE1lc3NhZ2VDYWxsYmFjayA9IChkYXRhPzogc3RyaW5nKSA9PiB2b2lkO1xyXG5cclxuZXhwb3J0IGludGVyZmFjZSBJSG9zdE1lc3NhZ2luZyB7XHJcbiAgICBzZW5kTWVzc2FnZVRvSG9zdChpZDogU2VuZFRvSG9zdE1lc3NhZ2VJZCwgZGF0YT86IHN0cmluZyk6IHZvaWQ7XHJcbiAgICBhc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDpzdHJpbmcsIGNhbGxiYWNrOk1lc3NhZ2VDYWxsYmFjayk6IHZvaWQ7XHJcbiAgICB1bnJlZ2lzdGVyTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZykgOiB2b2lkO1xyXG59IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7SUhvc3RNZXNzYWdpbmcsIE1lc3NhZ2VDYWxsYmFjaywgU2VuZFRvSG9zdE1lc3NhZ2VJZH0gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGNsYXNzIEhvc3RNZXNzYWdpbmcgaW1wbGVtZW50cyBJSG9zdE1lc3NhZ2luZyB7XHJcbiAgICBwcml2YXRlIG1lc3NhZ2VIYW5kbGVyczogTWFwPHN0cmluZywgTWVzc2FnZUNhbGxiYWNrPiA9IG5ldyBNYXAoKTtcclxuICAgIFxyXG4gICAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAgICAgdGhpcy5hc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKTtcclxuICAgIH1cclxuICAgICAgICBcclxuICAgIHB1YmxpYyBzZW5kTWVzc2FnZVRvSG9zdChpZDogU2VuZFRvSG9zdE1lc3NhZ2VJZCwgZGF0YT86IHN0cmluZykge1xyXG4gICAgICAgIGNvbnN0IG1lc3NhZ2UgPSBkYXRhID8gYCR7aWR9OyR7ZGF0YX1gIDogaWQ7XHJcblxyXG4gICAgICAgIC8vIFRPRE8gLSBkZXRlcm1pbmUgbWVzc2FnaW5nIG1ldGhvZHMgZm9yIFBob3Rpbm8uTkVUIGZvciBhbGwgcGxhdGZvcm1zXHJcbiAgICAgICAgaWYgKHdpbmRvdy5jaHJvbWU/LndlYnZpZXcpIHtcclxuICAgICAgICAgICAgd2luZG93LmNocm9tZS53ZWJ2aWV3LnBvc3RNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgIH0gZWxzZSBpZiAod2luZG93LmV4dGVybmFsPy5zZW5kTWVzc2FnZSkge1xyXG4gICAgICAgICAgICB3aW5kb3cuZXh0ZXJuYWwuc2VuZE1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgY29uc29sZS53YXJuKFwiTWVzc2FnZSB0byBob3N0IGZhaWxlZDpcIiwgbWVzc2FnZSk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgYXNzaWduV2ViTWVzc2FnZVJlY2VpdmVyKCkge1xyXG4gICAgICAgIC8vIEhhbmRsZSBXZWJWaWV3MiBtZXNzYWdlcyAoV2luZG93cylcclxuICAgICAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xyXG4gICAgICAgICAgICB3aW5kb3cuY2hyb21lLndlYnZpZXcuYWRkRXZlbnRMaXN0ZW5lcignbWVzc2FnZScsIChldmVudCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5oYW5kbGVXZWJNZXNzYWdlKGV2ZW50LmRhdGEpO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIEhhbmRsZSBnZW5lcmFsIFBob3Rpbm8gbWVzc2FnZXMgKGNyb3NzLXBsYXRmb3JtKVxyXG4gICAgICAgIGlmICh0eXBlb2Ygd2luZG93ICE9PSAndW5kZWZpbmVkJyAmJiB3aW5kb3cuZXh0ZXJuYWwpIHtcclxuICAgICAgICAgICAgd2luZG93LmV4dGVybmFsLnJlY2VpdmVNZXNzYWdlID0gKG1lc3NhZ2U6IHN0cmluZykgPT4ge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5oYW5kbGVXZWJNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgICAgICB9O1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIGhhbmRsZVdlYk1lc3NhZ2UobWVzc2FnZTogc3RyaW5nKSB7XHJcbiAgICAgICAgbGV0IG1lc3NhZ2VJZDogc3RyaW5nO1xyXG4gICAgICAgIGxldCBkYXRhOiBzdHJpbmcgfCB1bmRlZmluZWQ7XHJcblxyXG4gICAgICAgIC8vIFBhcnNlIG1lc3NhZ2UgLSBjaGVjayBpZiBpdCBjb250YWlucyBkYXRhIHNlcGFyYXRlZCBieSBzZW1pY29sb25cclxuICAgICAgICBpZiAobWVzc2FnZS5pbmNsdWRlcygnOycpKSB7XHJcbiAgICAgICAgICAgIGNvbnN0IHBhcnRzID0gbWVzc2FnZS5zcGxpdCgnOycsIDIpO1xyXG4gICAgICAgICAgICBtZXNzYWdlSWQgPSBwYXJ0c1swXTtcclxuICAgICAgICAgICAgZGF0YSA9IHBhcnRzWzFdO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIG1lc3NhZ2VJZCA9IG1lc3NhZ2U7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBFeGVjdXRlIHJlZ2lzdGVyZWQgaGFuZGxlclxyXG4gICAgICAgIGNvbnN0IGhhbmRsZXIgPSB0aGlzLm1lc3NhZ2VIYW5kbGVycy5nZXQobWVzc2FnZUlkKTtcclxuICAgICAgICBpZiAoaGFuZGxlcikge1xyXG4gICAgICAgICAgICBoYW5kbGVyKGRhdGEpO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUud2FybignTm8gaGFuZGxlciByZWdpc3RlcmVkIGZvciBtZXNzYWdlIElEOicsIG1lc3NhZ2VJZCk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG4gICAgXHJcbiAgICBwdWJsaWMgYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6c3RyaW5nLCBjYWxsYmFjazpNZXNzYWdlQ2FsbGJhY2spIHtcclxuICAgICAgICB0aGlzLm1lc3NhZ2VIYW5kbGVycy5zZXQobWVzc2FnZUlkLCBjYWxsYmFjayk7XHJcbiAgICB9XHJcbiAgICBcclxuICAgIHB1YmxpYyB1bnJlZ2lzdGVyTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZykge1xyXG4gICAgICAgIHRoaXMubWVzc2FnZUhhbmRsZXJzLmRlbGV0ZShtZXNzYWdlSWQpO1xyXG4gICAgfVxyXG59XHJcbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQgSW5maW5pV2luZG93IGZyb20gXCIuL0luZmluaVdpbmRvd1wiO1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IHt9O1xyXG53aW5kb3cuaW5maW5pV2luZG93ID0gbmV3IEluZmluaVdpbmRvdygpO1xyXG4iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtJSW5maW5pV2luZG93fSBmcm9tIFwiLi9Db250cmFjdHMvSUluZmluaVdpbmRvd1wiO1xyXG5pbXBvcnQge2dldFRpdGxlT2JzZXJ2ZXIsIFRpdGxlT2JzZXJ2ZXJUYXJnZXR9IGZyb20gXCIuL09ic2VydmVyc1wiO1xyXG5pbXBvcnQge2JsYW5rVGFyZ2V0SGFuZGxlcn0gZnJvbSBcIi4vQmxhbmtUYXJnZXRIYW5kbGVyXCI7XHJcbmltcG9ydCB7U2VuZFRvSG9zdE1lc3NhZ2VJZHMsIElIb3N0TWVzc2FnaW5nfSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuaW1wb3J0IHtIb3N0TWVzc2FnaW5nfSBmcm9tIFwiLi9Ib3N0TWVzc2FnaW5nXCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY2xhc3MgSW5maW5pV2luZG93IGltcGxlbWVudHMgSUluZmluaVdpbmRvdyB7XHJcbiAgICBIb3N0TWVzc2FnaW5nIDogSUhvc3RNZXNzYWdpbmcgPSBuZXcgSG9zdE1lc3NhZ2luZygpOyBcclxuICAgIFxyXG4gICAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAgICAgdGhpcy5hc3NpZ25FdmVudExpc3RlbmVycygpO1xyXG4gICAgICAgIHRoaXMuYXNzaWduT2JzZXJ2ZXJzKCk7XHJcbiAgICB9XHJcbiAgICBcclxuICAgIHNldFBvaW50ZXJDYXB0dXJlKGVsZW1lbnQ6RWxlbWVudCwgcG9pbnRlcklkOiBudW1iZXIpIHtcclxuICAgICAgICBlbGVtZW50LnNldFBvaW50ZXJDYXB0dXJlKHBvaW50ZXJJZCk7XHJcbiAgICB9XHJcblxyXG4gICAgcmVsZWFzZVBvaW50ZXJDYXB0dXJlKGVsZW1lbnQ6RWxlbWVudCwgcG9pbnRlcklkOm51bWJlcikge1xyXG4gICAgICAgIGVsZW1lbnQucmVsZWFzZVBvaW50ZXJDYXB0dXJlKHBvaW50ZXJJZCk7XHJcbiAgICB9XHJcbiAgICBcclxuICAgIHByaXZhdGUgYXNzaWduT2JzZXJ2ZXJzKCkge1xyXG4gICAgICAgIGlmIChUaXRsZU9ic2VydmVyVGFyZ2V0KSBnZXRUaXRsZU9ic2VydmVyKCkub2JzZXJ2ZShUaXRsZU9ic2VydmVyVGFyZ2V0LCB7Y2hpbGRMaXN0OiB0cnVlfSk7XHJcbiAgICB9XHJcbiAgICBcclxuICAgIHByaXZhdGUgYXNzaWduRXZlbnRMaXN0ZW5lcnMoKSB7XHJcbiAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImZ1bGxzY3JlZW5jaGFuZ2VcIiwgKF86IEV2ZW50KSA9PiB7XHJcbiAgICAgICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkgdGhpcy5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLmZ1bGxzY3JlZW5FbnRlcik7XHJcbiAgICAgICAgICAgIGVsc2UgdGhpcy5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLmZ1bGxzY3JlZW5FeGl0KTtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImtleWRvd25cIiwgYXN5bmMgKGU6IEtleWJvYXJkRXZlbnQpID0+IHtcclxuICAgICAgICAgICAgaWYgKGUua2V5ICE9PSBcIkYxMVwiKSByZXR1cm47XHJcbiAgICAgICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkgYXdhaXQgZG9jdW1lbnQuZXhpdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICAgICAgZWxzZSBhd2FpdCBkb2N1bWVudC5ib2R5LnJlcXVlc3RGdWxsc2NyZWVuKCk7XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoIFwiY2xpY2tcIiwgYmxhbmtUYXJnZXRIYW5kbGVyLCB7IGNhcHR1cmU6IHRydWUgfSk7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBkZWZhdWx0IEluZmluaVdpbmRvdyIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1NlbmRUb0hvc3RNZXNzYWdlSWRzfSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY29uc3QgVGl0bGVPYnNlcnZlclRhcmdldCA6IEhUTUxUaXRsZUVsZW1lbnQgfCBudWxsID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcigndGl0bGUnKTtcclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBnZXRUaXRsZU9ic2VydmVyKCkgOiBNdXRhdGlvbk9ic2VydmVyIHtcclxuICAgIHJldHVybiBuZXcgTXV0YXRpb25PYnNlcnZlcigobXV0YXRpb25zLCBfKSA9PiB7XHJcbiAgICAgICAgbXV0YXRpb25zLmZvckVhY2goKG11dGF0aW9uKSA9PiB7XHJcbiAgICAgICAgICAgIGlmIChtdXRhdGlvbi50eXBlICE9PSBcImNoaWxkTGlzdFwiKSByZXR1cm47XHJcbiAgICAgICAgICAgIHdpbmRvdy5pbmZpbmlXaW5kb3cuSG9zdE1lc3NhZ2luZy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy50aXRsZUNoYW5nZSwgZG9jdW1lbnQudGl0bGUpO1xyXG4gICAgICAgIH0pXHJcbiAgICB9KVxyXG59IiwiLy8gVGhlIG1vZHVsZSBjYWNoZVxudmFyIF9fd2VicGFja19tb2R1bGVfY2FjaGVfXyA9IHt9O1xuXG4vLyBUaGUgcmVxdWlyZSBmdW5jdGlvblxuZnVuY3Rpb24gX193ZWJwYWNrX3JlcXVpcmVfXyhtb2R1bGVJZCkge1xuXHQvLyBDaGVjayBpZiBtb2R1bGUgaXMgaW4gY2FjaGVcblx0dmFyIGNhY2hlZE1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF07XG5cdGlmIChjYWNoZWRNb2R1bGUgIT09IHVuZGVmaW5lZCkge1xuXHRcdHJldHVybiBjYWNoZWRNb2R1bGUuZXhwb3J0cztcblx0fVxuXHQvLyBDcmVhdGUgYSBuZXcgbW9kdWxlIChhbmQgcHV0IGl0IGludG8gdGhlIGNhY2hlKVxuXHR2YXIgbW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXSA9IHtcblx0XHQvLyBubyBtb2R1bGUuaWQgbmVlZGVkXG5cdFx0Ly8gbm8gbW9kdWxlLmxvYWRlZCBuZWVkZWRcblx0XHRleHBvcnRzOiB7fVxuXHR9O1xuXG5cdC8vIEV4ZWN1dGUgdGhlIG1vZHVsZSBmdW5jdGlvblxuXHRfX3dlYnBhY2tfbW9kdWxlc19fW21vZHVsZUlkXS5jYWxsKG1vZHVsZS5leHBvcnRzLCBtb2R1bGUsIG1vZHVsZS5leHBvcnRzLCBfX3dlYnBhY2tfcmVxdWlyZV9fKTtcblxuXHQvLyBSZXR1cm4gdGhlIGV4cG9ydHMgb2YgdGhlIG1vZHVsZVxuXHRyZXR1cm4gbW9kdWxlLmV4cG9ydHM7XG59XG5cbiIsIiIsIi8vIHN0YXJ0dXBcbi8vIExvYWQgZW50cnkgbW9kdWxlIGFuZCByZXR1cm4gZXhwb3J0c1xuLy8gVGhpcyBlbnRyeSBtb2R1bGUgaXMgcmVmZXJlbmNlZCBieSBvdGhlciBtb2R1bGVzIHNvIGl0IGNhbid0IGJlIGlubGluZWRcbnZhciBfX3dlYnBhY2tfZXhwb3J0c19fID0gX193ZWJwYWNrX3JlcXVpcmVfXyhcIi4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9JbmRleC50c1wiKTtcbiIsIiJdLCJuYW1lcyI6W10sInNvdXJjZVJvb3QiOiIifQ==