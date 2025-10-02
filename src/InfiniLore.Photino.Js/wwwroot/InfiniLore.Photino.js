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
const MessagingToHost_1 = __webpack_require__(/*! ./MessagingToHost */ "./src/InfiniLore.Photino.Js/TsSource/MessagingToHost.ts");
function isExternalLink(url) {
    try {
        const u = new URL(url, location.href);
        return u.hostname !== location.hostname;
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
            if (target === "_blank" || anchor.hasAttribute("data-external") || isExternalLink(anchor.href)) {
                e.preventDefault();
                (0, MessagingToHost_1.sendMessageToHost)(MessagingToHost_1.HostMessageIds.openExternalLink, anchor.href);
                return;
            }
        }
    });
}


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
const MessagingToHost_1 = __webpack_require__(/*! ./MessagingToHost */ "./src/InfiniLore.Photino.Js/TsSource/MessagingToHost.ts");
const BlankTargetHandler_1 = __webpack_require__(/*! ./BlankTargetHandler */ "./src/InfiniLore.Photino.Js/TsSource/BlankTargetHandler.ts");
class InfiniWindow {
    constructor() {
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
                (0, MessagingToHost_1.sendMessageToHost)(MessagingToHost_1.HostMessageIds.fullscreenEnter);
            else
                (0, MessagingToHost_1.sendMessageToHost)(MessagingToHost_1.HostMessageIds.fullscreenExit);
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

/***/ "./src/InfiniLore.Photino.Js/TsSource/MessagingToHost.ts":
/*!***************************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/MessagingToHost.ts ***!
  \***************************************************************/
/***/ ((__unused_webpack_module, exports) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.HostMessageIds = void 0;
exports.sendMessageToHost = sendMessageToHost;
exports.HostMessageIds = {
    titleChange: "title:change",
    fullscreenEnter: "fullscreen:enter",
    fullscreenExit: "fullscreen:exit",
    openExternalLink: "open:external",
};
function sendMessageToHost(id, data) {
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


/***/ }),

/***/ "./src/InfiniLore.Photino.Js/TsSource/Observers.ts":
/*!*********************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/Observers.ts ***!
  \*********************************************************/
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.TitleObserverTarget = void 0;
exports.getTitleObserver = getTitleObserver;
const MessagingToHost_1 = __webpack_require__(/*! ./MessagingToHost */ "./src/InfiniLore.Photino.Js/TsSource/MessagingToHost.ts");
exports.TitleObserverTarget = document.querySelector('title');
function getTitleObserver() {
    return new MutationObserver((mutations, _) => {
        mutations.forEach((mutation) => {
            if (mutation.type !== "childList")
                return;
            (0, MessagingToHost_1.sendMessageToHost)(MessagingToHost_1.HostMessageIds.titleChange, document.title);
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
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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QUM7Ozs7Ozs7Ozs7Ozs7O0FDakJELDRDQU9DO0FBZEQsa0lBQW9FO0FBS3ZELDJCQUFtQixHQUE2QixRQUFRLENBQUMsYUFBYSxDQUFDLE9BQU8sQ0FBQyxDQUFDO0FBRTdGLFNBQWdCLGdCQUFnQjtJQUM1QixPQUFPLElBQUksZ0JBQWdCLENBQUMsQ0FBQyxTQUFTLEVBQUUsQ0FBQyxFQUFFLEVBQUU7UUFDekMsU0FBUyxDQUFDLE9BQU8sQ0FBQyxDQUFDLFFBQVEsRUFBRSxFQUFFO1lBQzNCLElBQUksUUFBUSxDQUFDLElBQUksS0FBSyxXQUFXO2dCQUFFLE9BQU87WUFDMUMsdUNBQWlCLEVBQUMsZ0NBQWMsQ0FBQyxXQUFXLEVBQUUsUUFBUSxDQUFDLEtBQUssQ0FBQztRQUNqRSxDQUFDLENBQUM7SUFDTixDQUFDLENBQUM7QUFDTixDQUFDOzs7Ozs7O1VDakJEO1VBQ0E7O1VBRUE7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7O1VBRUE7VUFDQTs7VUFFQTtVQUNBO1VBQ0E7Ozs7VUV0QkE7VUFDQTtVQUNBO1VBQ0EiLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL0JsYW5rVGFyZ2V0SGFuZGxlci50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL0luZGV4LnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvSW5maW5pV2luZG93LnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvTWVzc2FnaW5nVG9Ib3N0LnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvT2JzZXJ2ZXJzLnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL2Jvb3RzdHJhcCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vd2VicGFjay9iZWZvcmUtc3RhcnR1cCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vd2VicGFjay9zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL2FmdGVyLXN0YXJ0dXAiXSwic291cmNlc0NvbnRlbnQiOlsiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7SG9zdE1lc3NhZ2VJZHMsIHNlbmRNZXNzYWdlVG9Ib3N0fSBmcm9tIFwiLi9NZXNzYWdpbmdUb0hvc3RcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5mdW5jdGlvbiBpc0V4dGVybmFsTGluayh1cmw6IHN0cmluZyk6IGJvb2xlYW4ge1xyXG4gICAgdHJ5IHtcclxuICAgICAgICBjb25zdCB1ID0gbmV3IFVSTCh1cmwsIGxvY2F0aW9uLmhyZWYpO1xyXG4gICAgICAgIHJldHVybiB1Lmhvc3RuYW1lICE9PSBsb2NhdGlvbi5ob3N0bmFtZTtcclxuICAgIH0gY2F0Y2gge1xyXG4gICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGFzeW5jIGZ1bmN0aW9uIGJsYW5rVGFyZ2V0SGFuZGxlcihlOiBNb3VzZUV2ZW50KSB7XHJcbiAgICBsZXQgZWwgPSBlLnRhcmdldCBhcyBIVE1MRWxlbWVudCB8IG51bGw7XHJcblxyXG4gICAgd2hpbGUgKGVsICYmIGVsICE9PSBkb2N1bWVudC5ib2R5KSB7XHJcbiAgICAgICAgaWYgKGVsLnRhZ05hbWU/LnRvTG93ZXJDYXNlKCkgIT09IFwiYVwiKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGNvbnN0IGFuY2hvciA9IGVsIGFzIEhUTUxBbmNob3JFbGVtZW50O1xyXG4gICAgICAgIGlmICghYW5jaG9yLmhyZWYpIHtcclxuICAgICAgICAgICAgZWwgPSBlbC5wYXJlbnRFbGVtZW50O1xyXG4gICAgICAgICAgICBjb250aW51ZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGNvbnN0IHRhcmdldCA9IGFuY2hvci5nZXRBdHRyaWJ1dGUoXCJ0YXJnZXRcIik7XHJcbiAgICAgICAgaWYgKHRhcmdldCA9PT0gXCJfYmxhbmtcIiB8fCBhbmNob3IuaGFzQXR0cmlidXRlKFwiZGF0YS1leHRlcm5hbFwiKSB8fCBpc0V4dGVybmFsTGluayhhbmNob3IuaHJlZikpIHtcclxuICAgICAgICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG4gICAgICAgICAgICBzZW5kTWVzc2FnZVRvSG9zdChIb3N0TWVzc2FnZUlkcy5vcGVuRXh0ZXJuYWxMaW5rLCBhbmNob3IuaHJlZik7XHJcbiAgICAgICAgICAgIHJldHVybjtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IEluZmluaVdpbmRvdyBmcm9tIFwiLi9JbmZpbmlXaW5kb3dcIjtcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCB7fTtcclxud2luZG93LmluZmluaVdpbmRvdyA9IG5ldyBJbmZpbmlXaW5kb3coKTtcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7SUluZmluaVdpbmRvd30gZnJvbSBcIi4vQ29udHJhY3RzL0lJbmZpbmlXaW5kb3dcIjtcclxuaW1wb3J0IHtnZXRUaXRsZU9ic2VydmVyLCBUaXRsZU9ic2VydmVyVGFyZ2V0fSBmcm9tIFwiLi9PYnNlcnZlcnNcIjtcclxuaW1wb3J0IHtIb3N0TWVzc2FnZUlkcywgc2VuZE1lc3NhZ2VUb0hvc3R9IGZyb20gXCIuL01lc3NhZ2luZ1RvSG9zdFwiO1xyXG5pbXBvcnQge2JsYW5rVGFyZ2V0SGFuZGxlcn0gZnJvbSBcIi4vQmxhbmtUYXJnZXRIYW5kbGVyXCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY2xhc3MgSW5maW5pV2luZG93IGltcGxlbWVudHMgSUluZmluaVdpbmRvdyB7XHJcbiAgICBcclxuICAgIGNvbnN0cnVjdG9yKCkge1xyXG4gICAgICAgIHRoaXMuYXNzaWduRXZlbnRMaXN0ZW5lcnMoKTtcclxuICAgICAgICB0aGlzLmFzc2lnbk9ic2VydmVycygpO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICBzZXRQb2ludGVyQ2FwdHVyZShlbGVtZW50OkVsZW1lbnQsIHBvaW50ZXJJZDogbnVtYmVyKSB7XHJcbiAgICAgICAgZWxlbWVudC5zZXRQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG4gICAgfVxyXG5cclxuICAgIHJlbGVhc2VQb2ludGVyQ2FwdHVyZShlbGVtZW50OkVsZW1lbnQsIHBvaW50ZXJJZDpudW1iZXIpIHtcclxuICAgICAgICBlbGVtZW50LnJlbGVhc2VQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICBwcml2YXRlIGFzc2lnbk9ic2VydmVycygpIHtcclxuICAgICAgICBpZiAoVGl0bGVPYnNlcnZlclRhcmdldCkgZ2V0VGl0bGVPYnNlcnZlcigpLm9ic2VydmUoVGl0bGVPYnNlcnZlclRhcmdldCwge2NoaWxkTGlzdDogdHJ1ZX0pO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICBwcml2YXRlIGFzc2lnbkV2ZW50TGlzdGVuZXJzKCkge1xyXG4gICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJmdWxsc2NyZWVuY2hhbmdlXCIsIChfOiBFdmVudCkgPT4ge1xyXG4gICAgICAgICAgICBpZiAoZG9jdW1lbnQuZnVsbHNjcmVlbkVsZW1lbnQpIHNlbmRNZXNzYWdlVG9Ib3N0KEhvc3RNZXNzYWdlSWRzLmZ1bGxzY3JlZW5FbnRlcik7XHJcbiAgICAgICAgICAgIGVsc2Ugc2VuZE1lc3NhZ2VUb0hvc3QoSG9zdE1lc3NhZ2VJZHMuZnVsbHNjcmVlbkV4aXQpO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBkb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwia2V5ZG93blwiLCBhc3luYyAoZTogS2V5Ym9hcmRFdmVudCkgPT4ge1xyXG4gICAgICAgICAgICBpZiAoZS5rZXkgIT09IFwiRjExXCIpIHJldHVybjtcclxuICAgICAgICAgICAgaWYgKGRvY3VtZW50LmZ1bGxzY3JlZW5FbGVtZW50KSBhd2FpdCBkb2N1bWVudC5leGl0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgICAgICBlbHNlIGF3YWl0IGRvY3VtZW50LmJvZHkucmVxdWVzdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lciggXCJjbGlja1wiLCBibGFua1RhcmdldEhhbmRsZXIsIHsgY2FwdHVyZTogdHJ1ZSB9KTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGRlZmF1bHQgSW5maW5pV2luZG93IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGNvbnN0IEhvc3RNZXNzYWdlSWRzID0ge1xyXG4gICAgdGl0bGVDaGFuZ2U6IFwidGl0bGU6Y2hhbmdlXCIsXHJcbiAgICBmdWxsc2NyZWVuRW50ZXI6IFwiZnVsbHNjcmVlbjplbnRlclwiLFxyXG4gICAgZnVsbHNjcmVlbkV4aXQ6IFwiZnVsbHNjcmVlbjpleGl0XCIsXHJcbiAgICBvcGVuRXh0ZXJuYWxMaW5rOiBcIm9wZW46ZXh0ZXJuYWxcIixcclxufVxyXG5cclxuZXhwb3J0IHR5cGUgSG9zdE1lc3NhZ2VJZCA9IHR5cGVvZiBIb3N0TWVzc2FnZUlkc1trZXlvZiB0eXBlb2YgSG9zdE1lc3NhZ2VJZHNdO1xyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBIb3N0TWVzc2FnZUlkLCBkYXRhPzogc3RyaW5nKSB7XHJcbiAgICBjb25zdCBtZXNzYWdlID0gZGF0YSA/IGAke2lkfTske2RhdGF9YCA6IGlkO1xyXG4gICAgXHJcbiAgICAvLyBUT0RPIC0gZGV0ZXJtaW5lIG1lc3NhZ2luZyBtZXRob2RzIGZvciBQaG90aW5vLk5FVCBmb3IgYWxsIHBsYXRmb3Jtc1xyXG4gICAgaWYgKHdpbmRvdy5jaHJvbWU/LndlYnZpZXcpIHtcclxuICAgICAgICB3aW5kb3cuY2hyb21lLndlYnZpZXcucG9zdE1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICB9IGVsc2UgaWYgKHdpbmRvdy5leHRlcm5hbD8uc2VuZE1lc3NhZ2UpIHtcclxuICAgICAgICB3aW5kb3cuZXh0ZXJuYWwuc2VuZE1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICAgIGNvbnNvbGUud2FybihcIk1lc3NhZ2UgdG8gaG9zdCBmYWlsZWQ6XCIsIG1lc3NhZ2UpO1xyXG4gICAgfVxyXG59IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7c2VuZE1lc3NhZ2VUb0hvc3QsIEhvc3RNZXNzYWdlSWRzfSBmcm9tIFwiLi9NZXNzYWdpbmdUb0hvc3RcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY29uc3QgVGl0bGVPYnNlcnZlclRhcmdldCA6IEhUTUxUaXRsZUVsZW1lbnQgfCBudWxsID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcigndGl0bGUnKTtcclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBnZXRUaXRsZU9ic2VydmVyKCkgOiBNdXRhdGlvbk9ic2VydmVyIHtcclxuICAgIHJldHVybiBuZXcgTXV0YXRpb25PYnNlcnZlcigobXV0YXRpb25zLCBfKSA9PiB7XHJcbiAgICAgICAgbXV0YXRpb25zLmZvckVhY2goKG11dGF0aW9uKSA9PiB7XHJcbiAgICAgICAgICAgIGlmIChtdXRhdGlvbi50eXBlICE9PSBcImNoaWxkTGlzdFwiKSByZXR1cm47XHJcbiAgICAgICAgICAgIHNlbmRNZXNzYWdlVG9Ib3N0KEhvc3RNZXNzYWdlSWRzLnRpdGxlQ2hhbmdlLCBkb2N1bWVudC50aXRsZSlcclxuICAgICAgICB9KVxyXG4gICAgfSlcclxufSIsIi8vIFRoZSBtb2R1bGUgY2FjaGVcbnZhciBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX18gPSB7fTtcblxuLy8gVGhlIHJlcXVpcmUgZnVuY3Rpb25cbmZ1bmN0aW9uIF9fd2VicGFja19yZXF1aXJlX18obW9kdWxlSWQpIHtcblx0Ly8gQ2hlY2sgaWYgbW9kdWxlIGlzIGluIGNhY2hlXG5cdHZhciBjYWNoZWRNb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdO1xuXHRpZiAoY2FjaGVkTW9kdWxlICE9PSB1bmRlZmluZWQpIHtcblx0XHRyZXR1cm4gY2FjaGVkTW9kdWxlLmV4cG9ydHM7XG5cdH1cblx0Ly8gQ3JlYXRlIGEgbmV3IG1vZHVsZSAoYW5kIHB1dCBpdCBpbnRvIHRoZSBjYWNoZSlcblx0dmFyIG1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF0gPSB7XG5cdFx0Ly8gbm8gbW9kdWxlLmlkIG5lZWRlZFxuXHRcdC8vIG5vIG1vZHVsZS5sb2FkZWQgbmVlZGVkXG5cdFx0ZXhwb3J0czoge31cblx0fTtcblxuXHQvLyBFeGVjdXRlIHRoZSBtb2R1bGUgZnVuY3Rpb25cblx0X193ZWJwYWNrX21vZHVsZXNfX1ttb2R1bGVJZF0uY2FsbChtb2R1bGUuZXhwb3J0cywgbW9kdWxlLCBtb2R1bGUuZXhwb3J0cywgX193ZWJwYWNrX3JlcXVpcmVfXyk7XG5cblx0Ly8gUmV0dXJuIHRoZSBleHBvcnRzIG9mIHRoZSBtb2R1bGVcblx0cmV0dXJuIG1vZHVsZS5leHBvcnRzO1xufVxuXG4iLCIiLCIvLyBzdGFydHVwXG4vLyBMb2FkIGVudHJ5IG1vZHVsZSBhbmQgcmV0dXJuIGV4cG9ydHNcbi8vIFRoaXMgZW50cnkgbW9kdWxlIGlzIHJlZmVyZW5jZWQgYnkgb3RoZXIgbW9kdWxlcyBzbyBpdCBjYW4ndCBiZSBpbmxpbmVkXG52YXIgX193ZWJwYWNrX2V4cG9ydHNfXyA9IF9fd2VicGFja19yZXF1aXJlX18oXCIuL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvSW5kZXgudHNcIik7XG4iLCIiXSwibmFtZXMiOltdLCJzb3VyY2VSb290IjoiIn0=