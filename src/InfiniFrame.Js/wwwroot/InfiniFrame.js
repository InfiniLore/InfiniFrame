/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/InfiniFrame.Js/TsSource/BlankTargetHandler.ts":
/*!***********************************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/BlankTargetHandler.ts ***!
  \***********************************************************/
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

/***/ "./src/InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts":
/*!*****************************************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts ***!
  \*****************************************************************/
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

/***/ "./src/InfiniFrame.Js/TsSource/HostMessaging.ts":
/*!******************************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/HostMessaging.ts ***!
  \******************************************************/
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
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts");
const BlankTargetHandler_1 = __webpack_require__(/*! ./BlankTargetHandler */ "./src/InfiniFrame.Js/TsSource/BlankTargetHandler.ts");
const Observers_1 = __webpack_require__(/*! ./Observers */ "./src/InfiniFrame.Js/TsSource/Observers.ts");
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

/***/ "./src/InfiniFrame.Js/TsSource/Index.ts":
/*!**********************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/Index.ts ***!
  \**********************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
const InfiniFrame_1 = __importDefault(__webpack_require__(/*! ./InfiniFrame */ "./src/InfiniFrame.Js/TsSource/InfiniFrame.ts"));
window.infiniFrame = new InfiniFrame_1.default();


/***/ }),

/***/ "./src/InfiniFrame.Js/TsSource/InfiniFrame.ts":
/*!****************************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/InfiniFrame.ts ***!
  \****************************************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


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


/***/ }),

/***/ "./src/InfiniFrame.Js/TsSource/Observers.ts":
/*!**************************************************!*\
  !*** ./src/InfiniFrame.Js/TsSource/Observers.ts ***!
  \**************************************************/
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.TitleObserverTarget = void 0;
exports.getTitleObserver = getTitleObserver;
const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts");
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
/******/ 	var __webpack_exports__ = __webpack_require__("./src/InfiniFrame.Js/TsSource/Index.ts");
/******/ 	
/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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LE9BQWdCLEVBQUUsU0FBaUI7UUFDakQsT0FBTyxDQUFDLGlCQUFpQixDQUFDLFNBQVMsQ0FBQyxDQUFDO0lBQ3pDLENBQUM7SUFFRCxxQkFBcUIsQ0FBQyxPQUFnQixFQUFFLFNBQWlCO1FBQ3JELE9BQU8sQ0FBQyxxQkFBcUIsQ0FBQyxTQUFTLENBQUMsQ0FBQztJQUM3QyxDQUFDO0NBQ0o7QUFmRCxrQ0FlQztBQUVELHFCQUFlLFdBQVc7Ozs7Ozs7Ozs7Ozs7O0FDaEIxQiw0Q0FPQztBQWRELDRJQUFnRTtBQUtuRCwyQkFBbUIsR0FBNEIsUUFBUSxDQUFDLGFBQWEsQ0FBQyxPQUFPLENBQUMsQ0FBQztBQUU1RixTQUFnQixnQkFBZ0I7SUFDNUIsT0FBTyxJQUFJLGdCQUFnQixDQUFDLENBQUMsU0FBUyxFQUFFLENBQUMsRUFBRSxFQUFFO1FBQ3pDLFNBQVMsQ0FBQyxPQUFPLENBQUMsQ0FBQyxRQUFRLEVBQUUsRUFBRTtZQUMzQixJQUFJLFFBQVEsQ0FBQyxJQUFJLEtBQUssV0FBVztnQkFBRSxPQUFPO1lBQzFDLE1BQU0sQ0FBQyxXQUFXLENBQUMsYUFBYSxDQUFDLGlCQUFpQixDQUFDLHFDQUFvQixDQUFDLFdBQVcsRUFBRSxRQUFRLENBQUMsS0FBSyxDQUFDLENBQUM7UUFDekcsQ0FBQyxDQUFDO0lBQ04sQ0FBQyxDQUFDO0FBQ04sQ0FBQzs7Ozs7OztVQ2pCRDtVQUNBOztVQUVBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBOztVQUVBO1VBQ0E7O1VBRUE7VUFDQTtVQUNBOzs7O1VFdEJBO1VBQ0E7VUFDQTtVQUNBIiwic291cmNlcyI6WyJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS8uL3NyYy9JbmZpbmlGcmFtZS5Kcy9Uc1NvdXJjZS9CbGFua1RhcmdldEhhbmRsZXIudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS8uL3NyYy9JbmZpbmlGcmFtZS5Kcy9Uc1NvdXJjZS9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS8uL3NyYy9JbmZpbmlGcmFtZS5Kcy9Uc1NvdXJjZS9Ib3N0TWVzc2FnaW5nLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvLi9zcmMvSW5maW5pRnJhbWUuSnMvVHNTb3VyY2UvSW5kZXgudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS8uL3NyYy9JbmZpbmlGcmFtZS5Kcy9Uc1NvdXJjZS9JbmZpbmlGcmFtZS50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUZyYW1lLkpzL1RzU291cmNlL09ic2VydmVycy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lL3dlYnBhY2svYm9vdHN0cmFwIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvd2VicGFjay9iZWZvcmUtc3RhcnR1cCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lL3dlYnBhY2svc3RhcnR1cCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lL3dlYnBhY2svYWZ0ZXItc3RhcnR1cCJdLCJzb3VyY2VzQ29udGVudCI6WyIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtTZW5kVG9Ib3N0TWVzc2FnZUlkc30gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZnVuY3Rpb24gaXNFeHRlcm5hbExpbmsodXJsOiBzdHJpbmcpOiBib29sZWFuIHtcclxuICAgIHRyeSB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBVUkwodXJsLCBsb2NhdGlvbi5ocmVmKS5ob3N0bmFtZSAhPT0gbG9jYXRpb24uaG9zdG5hbWU7XHJcbiAgICB9IGNhdGNoIHtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBhc3luYyBmdW5jdGlvbiBibGFua1RhcmdldEhhbmRsZXIoZTogTW91c2VFdmVudCkge1xyXG4gICAgbGV0IGVsID0gZS50YXJnZXQgYXMgSFRNTEVsZW1lbnQgfCBudWxsO1xyXG5cclxuICAgIHdoaWxlIChlbCAmJiBlbCAhPT0gZG9jdW1lbnQuYm9keSkge1xyXG4gICAgICAgIGlmIChlbC50YWdOYW1lPy50b0xvd2VyQ2FzZSgpICE9PSBcImFcIikge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgYW5jaG9yID0gZWwgYXMgSFRNTEFuY2hvckVsZW1lbnQ7XHJcbiAgICAgICAgaWYgKCFhbmNob3IuaHJlZikge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgdGFyZ2V0ID0gYW5jaG9yLmdldEF0dHJpYnV0ZShcInRhcmdldFwiKTtcclxuICAgICAgICBpZiAoISh0YXJnZXQgPT09IFwiX2JsYW5rXCIgfHwgYW5jaG9yLmhhc0F0dHJpYnV0ZShcImRhdGEtZXh0ZXJuYWxcIikgfHwgaXNFeHRlcm5hbExpbmsoYW5jaG9yLmhyZWYpKSkge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG4gICAgICAgIHdpbmRvdy5pbmZpbmlGcmFtZS5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLm9wZW5FeHRlcm5hbExpbmssIGFuY2hvci5ocmVmKTtcclxuICAgICAgICByZXR1cm47XHJcbiAgICB9XHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5jb25zdCBpbmZpbmlGcmFtZSA6IHN0cmluZyA9IFwiX19pbmZpbmlmcmFtZVwiO1xyXG5cclxuZXhwb3J0IGNvbnN0IFNlbmRUb0hvc3RNZXNzYWdlSWRzID0ge1xyXG4gICAgdGl0bGVDaGFuZ2U6IGAke2luZmluaUZyYW1lfTp0aXRsZTpjaGFuZ2VgLFxyXG4gICAgZnVsbHNjcmVlbkVudGVyOiBgJHtpbmZpbmlGcmFtZX06ZnVsbHNjcmVlbjplbnRlcmAsXHJcbiAgICBmdWxsc2NyZWVuRXhpdDogYCR7aW5maW5pRnJhbWV9OmZ1bGxzY3JlZW46ZXhpdGAsXHJcbiAgICBvcGVuRXh0ZXJuYWxMaW5rOiBgJHtpbmZpbmlGcmFtZX06b3BlbjpleHRlcm5hbGAsXHJcbiAgICB3aW5kb3dDbG9zZTogYCR7aW5maW5pRnJhbWV9OndpbmRvdzpjbG9zZWAsXHJcbn1cclxuXHJcbmV4cG9ydCBjb25zdCBSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzID0ge1xyXG4gICAgcmVnaXN0ZXJPcGVuRXh0ZXJuYWw6IGAke2luZmluaUZyYW1lfTpyZWdpc3RlcjpvcGVuOmV4dGVybmFsYCxcclxuICAgIHJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZTogYCR7aW5maW5pRnJhbWV9OnJlZ2lzdGVyOmZ1bGxzY3JlZW46Y2hhbmdlYCxcclxuICAgIHJlZ2lzdGVyVGl0bGVDaGFuZ2U6IGAke2luZmluaUZyYW1lfTpyZWdpc3Rlcjp0aXRsZTpjaGFuZ2VgLFxyXG4gICAgcmVnaXN0ZXJXaW5kb3dDbG9zZTogYCR7aW5maW5pRnJhbWV9OnJlZ2lzdGVyOndpbmRvdzpjbG9zZWAsXHJcbn1cclxuXHJcbmV4cG9ydCB0eXBlIFNlbmRUb0hvc3RNZXNzYWdlSWQgPSB0eXBlb2YgU2VuZFRvSG9zdE1lc3NhZ2VJZHNba2V5b2YgdHlwZW9mIFNlbmRUb0hvc3RNZXNzYWdlSWRzXTtcclxuZXhwb3J0IHR5cGUgTWVzc2FnZUNhbGxiYWNrID0gKGRhdGE/OiBzdHJpbmcpID0+IHZvaWQ7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIElIb3N0TWVzc2FnaW5nIHtcclxuICAgIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkIHwgc3RyaW5nLCBkYXRhPzogc3RyaW5nKTogdm9pZDtcclxuXHJcbiAgICBhc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nLCBjYWxsYmFjazogTWVzc2FnZUNhbGxiYWNrKTogdm9pZDtcclxuXHJcbiAgICB1bnJlZ2lzdGVyTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZyk6IHZvaWQ7XHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtcclxuICAgIElIb3N0TWVzc2FnaW5nLFxyXG4gICAgTWVzc2FnZUNhbGxiYWNrLFxyXG4gICAgUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcyxcclxuICAgIFNlbmRUb0hvc3RNZXNzYWdlSWQsIFNlbmRUb0hvc3RNZXNzYWdlSWRzXHJcbn0gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcbmltcG9ydCB7YmxhbmtUYXJnZXRIYW5kbGVyfSBmcm9tIFwiLi9CbGFua1RhcmdldEhhbmRsZXJcIjtcclxuaW1wb3J0IHtnZXRUaXRsZU9ic2VydmVyLCBUaXRsZU9ic2VydmVyVGFyZ2V0fSBmcm9tIFwiLi9PYnNlcnZlcnNcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5jbGFzcyBIb3N0TWVzc2FnaW5nIGltcGxlbWVudHMgSUhvc3RNZXNzYWdpbmcge1xyXG4gICAgcHJpdmF0ZSBtZXNzYWdlSGFuZGxlcnM6IE1hcDxzdHJpbmcsIE1lc3NhZ2VDYWxsYmFjaz4gPSBuZXcgTWFwKCk7XHJcblxyXG4gICAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAgICAgdGhpcy5hc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKTtcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJPcGVuRXh0ZXJuYWwsIF8gPT4ge1xyXG4gICAgICAgICAgICBkb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwiY2xpY2tcIiwgYmxhbmtUYXJnZXRIYW5kbGVyLCB7Y2FwdHVyZTogdHJ1ZX0pO1xyXG4gICAgICAgIH0pXHJcblxyXG4gICAgICAgIHRoaXMuYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLnJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZSwgXyA9PiB7XHJcbiAgICAgICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJmdWxsc2NyZWVuY2hhbmdlXCIsIChfOiBFdmVudCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKGRvY3VtZW50LmZ1bGxzY3JlZW5FbGVtZW50KSB0aGlzLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLmZ1bGxzY3JlZW5FbnRlcik7XHJcbiAgICAgICAgICAgICAgICBlbHNlIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMuZnVsbHNjcmVlbkV4aXQpO1xyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJrZXlkb3duXCIsIGFzeW5jIChlOiBLZXlib2FyZEV2ZW50KSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoZS5rZXkgIT09IFwiRjExXCIpIHJldHVybjtcclxuICAgICAgICAgICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkgYXdhaXQgZG9jdW1lbnQuZXhpdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICAgICAgICAgIGVsc2UgYXdhaXQgZG9jdW1lbnQuYm9keS5yZXF1ZXN0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9KVxyXG5cclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3RlclRpdGxlQ2hhbmdlLCBfID0+IHtcclxuICAgICAgICAgICAgaWYgKFRpdGxlT2JzZXJ2ZXJUYXJnZXQpIGdldFRpdGxlT2JzZXJ2ZXIoKS5vYnNlcnZlKFRpdGxlT2JzZXJ2ZXJUYXJnZXQsIHtjaGlsZExpc3Q6IHRydWV9KTtcclxuICAgICAgICB9KVxyXG5cclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3RlcldpbmRvd0Nsb3NlLCBfID0+IHtcclxuICAgICAgICAgICAgd2luZG93LmNsb3NlID0gKCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy53aW5kb3dDbG9zZSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzZW5kTWVzc2FnZVRvSG9zdChpZDogU2VuZFRvSG9zdE1lc3NhZ2VJZCB8IHN0cmluZywgZGF0YT86IHN0cmluZykge1xyXG4gICAgICAgIGNvbnN0IG1lc3NhZ2UgPSBkYXRhID8gYCR7aWR9OyR7ZGF0YX1gIDogaWQ7XHJcblxyXG4gICAgICAgIC8vIFRPRE8gLSBkZXRlcm1pbmUgbWVzc2FnaW5nIG1ldGhvZHMgZm9yIFBob3Rpbm8uTkVUIGZvciBhbGwgcGxhdGZvcm1zXHJcbiAgICAgICAgaWYgKHdpbmRvdy5jaHJvbWU/LndlYnZpZXcpIHtcclxuICAgICAgICAgICAgd2luZG93LmNocm9tZS53ZWJ2aWV3LnBvc3RNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgIH0gZWxzZSBpZiAod2luZG93LmV4dGVybmFsPy5zZW5kTWVzc2FnZSkge1xyXG4gICAgICAgICAgICB3aW5kb3cuZXh0ZXJuYWwuc2VuZE1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgY29uc29sZS53YXJuKFwiTWVzc2FnZSB0byBob3N0IGZhaWxlZDpcIiwgbWVzc2FnZSk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgYXNzaWduV2ViTWVzc2FnZVJlY2VpdmVyKCkge1xyXG4gICAgICAgIC8vIFN0b3JlIHRoZSBvcmlnaW5hbCByZWNlaXZlTWVzc2FnZSBpZiBpdCBleGlzdHMgKGZvciBCbGF6b3IgY29tcGF0aWJpbGl0eSlcclxuICAgICAgICBjb25zdCBvcmlnaW5hbFJlY2VpdmVNZXNzYWdlID0gd2luZG93LmV4dGVybmFsPy5yZWNlaXZlTWVzc2FnZTtcclxuXHJcbiAgICAgICAgLy8gSGFuZGxlIFdlYlZpZXcyIG1lc3NhZ2VzIChXaW5kb3dzKVxyXG4gICAgICAgIGlmICh3aW5kb3cuY2hyb21lPy53ZWJ2aWV3KSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5jaHJvbWUud2Vidmlldy5hZGRFdmVudExpc3RlbmVyKCdtZXNzYWdlJywgKGV2ZW50KSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoIXRoaXMuaXNCbGF6b3JNZXNzYWdlKGV2ZW50LmRhdGEpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5oYW5kbGVXZWJNZXNzYWdlKGV2ZW50LmRhdGEpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIEhhbmRsZSBnZW5lcmFsIFBob3Rpbm8gbWVzc2FnZXMgKGNyb3NzLXBsYXRmb3JtKVxyXG4gICAgICAgIGlmICh0eXBlb2Ygd2luZG93ICE9PSAndW5kZWZpbmVkJyAmJiB3aW5kb3cuZXh0ZXJuYWwpIHtcclxuICAgICAgICAgICAgd2luZG93LmV4dGVybmFsLnJlY2VpdmVNZXNzYWdlID0gKG1lc3NhZ2U6IGFueSkgPT4ge1xyXG4gICAgICAgICAgICAgICAgLy8gQ2hlY2sgaWYgaXQncyBhIEJsYXpvciBtZXNzYWdlIGFuZCBwYXNzIGl0IHRvIHRoZSBvcmlnaW5hbCBoYW5kbGVyXHJcbiAgICAgICAgICAgICAgICBpZiAodGhpcy5pc0JsYXpvck1lc3NhZ2UobWVzc2FnZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAob3JpZ2luYWxSZWNlaXZlTWVzc2FnZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBvcmlnaW5hbFJlY2VpdmVNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgLy8gSGFuZGxlIG91ciBjdXN0b20gbWVzc2FnZXNcclxuICAgICAgICAgICAgICAgIHRoaXMuaGFuZGxlV2ViTWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICAgICAgfTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBpc0JsYXpvck1lc3NhZ2UobWVzc2FnZTogYW55KTogYm9vbGVhbiB7XHJcbiAgICAgICAgaWYgKHR5cGVvZiBtZXNzYWdlICE9PSAnc3RyaW5nJykgcmV0dXJuIHRydWU7IC8vIEFzc3VtZSBub24tc3RyaW5nIG1lc3NhZ2VzIGFyZSBCbGF6b3JcclxuXHJcbiAgICAgICAgLy8gQ2hlY2sgZm9yIGNvbW1vbiBCbGF6b3IgbWVzc2FnZSBwYXR0ZXJuc1xyXG4gICAgICAgIHJldHVybiBtZXNzYWdlLnN0YXJ0c1dpdGgoJ19fYnd2OicpXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2Uuc3RhcnRzV2l0aCgnZT0+eycpXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2UuaW5jbHVkZXMoJ0JlZ2luSW52b2tlSlMnKVxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLmluY2x1ZGVzKCdBdHRhY2hUb0RvY3VtZW50JylcclxuICAgICAgICAgICAgfHwgbWVzc2FnZS5pbmNsdWRlcygnUmVuZGVyQmF0Y2gnKVxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLmluY2x1ZGVzKCdCbGF6b3IuJyk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBoYW5kbGVXZWJNZXNzYWdlKG1lc3NhZ2U6IGFueSkge1xyXG4gICAgICAgIC8vIEVuc3VyZSBtZXNzYWdlIGlzIGEgc3RyaW5nXHJcbiAgICAgICAgY29uc3QgbWVzc2FnZVN0ciA9IHR5cGVvZiBtZXNzYWdlID09PSAnc3RyaW5nJyA/IG1lc3NhZ2UgOiBTdHJpbmcobWVzc2FnZSB8fCAnJyk7XHJcblxyXG4gICAgICAgIGlmICghbWVzc2FnZVN0cikge1xyXG4gICAgICAgICAgICBjb25zb2xlLndhcm4oJ1JlY2VpdmVkIGVtcHR5IG9yIGludmFsaWQgbWVzc2FnZScpO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBsZXQgbWVzc2FnZUlkOiBzdHJpbmc7XHJcbiAgICAgICAgbGV0IGRhdGE6IHN0cmluZyB8IHVuZGVmaW5lZDtcclxuXHJcbiAgICAgICAgLy8gUGFyc2UgbWVzc2FnZSAtIGNoZWNrIGlmIGl0IGNvbnRhaW5zIGRhdGEgc2VwYXJhdGVkIGJ5IHNlbWljb2xvblxyXG4gICAgICAgIGlmIChtZXNzYWdlU3RyLmluY2x1ZGVzKCc7JykpIHtcclxuICAgICAgICAgICAgY29uc3QgcGFydHMgPSBtZXNzYWdlU3RyLnNwbGl0KCc7JywgMik7XHJcbiAgICAgICAgICAgIG1lc3NhZ2VJZCA9IHBhcnRzWzBdO1xyXG4gICAgICAgICAgICBkYXRhID0gcGFydHNbMV07XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgbWVzc2FnZUlkID0gbWVzc2FnZVN0cjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIEV4ZWN1dGUgcmVnaXN0ZXJlZCBoYW5kbGVyXHJcbiAgICAgICAgY29uc3QgaGFuZGxlciA9IHRoaXMubWVzc2FnZUhhbmRsZXJzLmdldChtZXNzYWdlSWQpO1xyXG4gICAgICAgIGlmIChoYW5kbGVyKSB7XHJcbiAgICAgICAgICAgIGhhbmRsZXIoZGF0YSk7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgY29uc29sZS53YXJuKCdObyBoYW5kbGVyIHJlZ2lzdGVyZWQgZm9yIG1lc3NhZ2UgSUQ6JywgbWVzc2FnZUlkKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIobWVzc2FnZUlkOiBzdHJpbmcsIGNhbGxiYWNrOiBNZXNzYWdlQ2FsbGJhY2spIHtcclxuICAgICAgICB0aGlzLm1lc3NhZ2VIYW5kbGVycy5zZXQobWVzc2FnZUlkLCBjYWxsYmFjayk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHVucmVnaXN0ZXJNZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nKSB7XHJcbiAgICAgICAgdGhpcy5tZXNzYWdlSGFuZGxlcnMuZGVsZXRlKG1lc3NhZ2VJZCk7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBkZWZhdWx0IEhvc3RNZXNzYWdpbmdcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCBJbmZpbmlGcmFtZSBmcm9tIFwiLi9JbmZpbmlGcmFtZVwiO1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IHt9O1xyXG53aW5kb3cuaW5maW5pRnJhbWUgPSBuZXcgSW5maW5pRnJhbWUoKTtcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7SUluZmluaUZyYW1lfSBmcm9tIFwiLi9Db250cmFjdHMvSUluZmluaUZyYW1lXCI7XHJcbmltcG9ydCB7SUhvc3RNZXNzYWdpbmcsIFNlbmRUb0hvc3RNZXNzYWdlSWR9IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5pbXBvcnQgSG9zdE1lc3NhZ2luZyBmcm9tIFwiLi9Ib3N0TWVzc2FnaW5nXCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY2xhc3MgSW5maW5pRnJhbWUgaW1wbGVtZW50cyBJSW5maW5pRnJhbWUge1xyXG4gICAgSG9zdE1lc3NhZ2luZzogSUhvc3RNZXNzYWdpbmcgPSBuZXcgSG9zdE1lc3NhZ2luZygpO1xyXG5cclxuICAgIC8vIE92ZXJsb2FkIHRvIG1ha2UgYSBkZXYncyBsaWZlIGVhc2llciBpbnN0ZWFkIG9mIGhhdmluZyB0byBnbyB0byB0aGUgSG9zdE1lc3NhZ2luZyBjbGFzc1xyXG4gICAgc2VuZE1lc3NhZ2VUb0hvc3QoaWQ6IFNlbmRUb0hvc3RNZXNzYWdlSWQsIGRhdGE/OiBzdHJpbmcpIHtcclxuICAgICAgICB0aGlzLkhvc3RNZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoaWQsIGRhdGEpO1xyXG4gICAgfVxyXG5cclxuICAgIHNldFBvaW50ZXJDYXB0dXJlKGVsZW1lbnQ6IEVsZW1lbnQsIHBvaW50ZXJJZDogbnVtYmVyKSB7XHJcbiAgICAgICAgZWxlbWVudC5zZXRQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG4gICAgfVxyXG5cclxuICAgIHJlbGVhc2VQb2ludGVyQ2FwdHVyZShlbGVtZW50OiBFbGVtZW50LCBwb2ludGVySWQ6IG51bWJlcikge1xyXG4gICAgICAgIGVsZW1lbnQucmVsZWFzZVBvaW50ZXJDYXB0dXJlKHBvaW50ZXJJZCk7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBkZWZhdWx0IEluZmluaUZyYW1lIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7U2VuZFRvSG9zdE1lc3NhZ2VJZHN9IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBjb25zdCBUaXRsZU9ic2VydmVyVGFyZ2V0OiBIVE1MVGl0bGVFbGVtZW50IHwgbnVsbCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IoJ3RpdGxlJyk7XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gZ2V0VGl0bGVPYnNlcnZlcigpOiBNdXRhdGlvbk9ic2VydmVyIHtcclxuICAgIHJldHVybiBuZXcgTXV0YXRpb25PYnNlcnZlcigobXV0YXRpb25zLCBfKSA9PiB7XHJcbiAgICAgICAgbXV0YXRpb25zLmZvckVhY2goKG11dGF0aW9uKSA9PiB7XHJcbiAgICAgICAgICAgIGlmIChtdXRhdGlvbi50eXBlICE9PSBcImNoaWxkTGlzdFwiKSByZXR1cm47XHJcbiAgICAgICAgICAgIHdpbmRvdy5pbmZpbmlGcmFtZS5Ib3N0TWVzc2FnaW5nLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLnRpdGxlQ2hhbmdlLCBkb2N1bWVudC50aXRsZSk7XHJcbiAgICAgICAgfSlcclxuICAgIH0pXHJcbn0iLCIvLyBUaGUgbW9kdWxlIGNhY2hlXG52YXIgX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fID0ge307XG5cbi8vIFRoZSByZXF1aXJlIGZ1bmN0aW9uXG5mdW5jdGlvbiBfX3dlYnBhY2tfcmVxdWlyZV9fKG1vZHVsZUlkKSB7XG5cdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuXHR2YXIgY2FjaGVkTW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXTtcblx0aWYgKGNhY2hlZE1vZHVsZSAhPT0gdW5kZWZpbmVkKSB7XG5cdFx0cmV0dXJuIGNhY2hlZE1vZHVsZS5leHBvcnRzO1xuXHR9XG5cdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG5cdHZhciBtb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdID0ge1xuXHRcdC8vIG5vIG1vZHVsZS5pZCBuZWVkZWRcblx0XHQvLyBubyBtb2R1bGUubG9hZGVkIG5lZWRlZFxuXHRcdGV4cG9ydHM6IHt9XG5cdH07XG5cblx0Ly8gRXhlY3V0ZSB0aGUgbW9kdWxlIGZ1bmN0aW9uXG5cdF9fd2VicGFja19tb2R1bGVzX19bbW9kdWxlSWRdLmNhbGwobW9kdWxlLmV4cG9ydHMsIG1vZHVsZSwgbW9kdWxlLmV4cG9ydHMsIF9fd2VicGFja19yZXF1aXJlX18pO1xuXG5cdC8vIFJldHVybiB0aGUgZXhwb3J0cyBvZiB0aGUgbW9kdWxlXG5cdHJldHVybiBtb2R1bGUuZXhwb3J0cztcbn1cblxuIiwiIiwiLy8gc3RhcnR1cFxuLy8gTG9hZCBlbnRyeSBtb2R1bGUgYW5kIHJldHVybiBleHBvcnRzXG4vLyBUaGlzIGVudHJ5IG1vZHVsZSBpcyByZWZlcmVuY2VkIGJ5IG90aGVyIG1vZHVsZXMgc28gaXQgY2FuJ3QgYmUgaW5saW5lZFxudmFyIF9fd2VicGFja19leHBvcnRzX18gPSBfX3dlYnBhY2tfcmVxdWlyZV9fKFwiLi9zcmMvSW5maW5pRnJhbWUuSnMvVHNTb3VyY2UvSW5kZXgudHNcIik7XG4iLCIiXSwibmFtZXMiOltdLCJzb3VyY2VSb290IjoiIn0=