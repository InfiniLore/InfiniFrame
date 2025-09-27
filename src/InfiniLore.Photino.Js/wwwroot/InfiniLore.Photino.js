/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

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
            if (document.fullscreenElement) {
                (0, MessagingToHost_1.sendMessageToHost)(MessagingToHost_1.HostMessageIds.fullscreenEnter);
            }
            else {
                (0, MessagingToHost_1.sendMessageToHost)(MessagingToHost_1.HostMessageIds.fullscreenExit);
            }
        });
        document.addEventListener("keydown", (e) => __awaiter(this, void 0, void 0, function* () {
            if (e.key === "F11") {
                if (document.fullscreenElement) {
                    yield document.exitFullscreen();
                }
                else {
                    yield document.body.requestFullscreen();
                }
            }
        }));
    }
}
exports.InfiniWindow = InfiniWindow;


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
            if (mutation.type === "childList") {
                (0, MessagingToHost_1.sendMessageToHost)(MessagingToHost_1.HostMessageIds.titleChange, document.title);
            }
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
var __webpack_exports__ = {};
// This entry needs to be wrapped in an IIFE because it needs to be isolated against other modules in the chunk.
(() => {
var exports = __webpack_exports__;
/*!*****************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/Index.ts ***!
  \*****************************************************/

Object.defineProperty(exports, "__esModule", ({ value: true }));
const InfiniWindow_1 = __webpack_require__(/*! ./InfiniWindow */ "./src/InfiniLore.Photino.Js/TsSource/InfiniWindow.ts");
window.infiniWindow = new InfiniWindow_1.InfiniWindow();

})();

/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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QUFDLE9BQU8sQ0FBQyxDQUFDLFFBQVEsRUFBRSxFQUFFO1lBQzNCLElBQUksUUFBUSxDQUFDLElBQUksS0FBSyxXQUFXLEVBQUUsQ0FBQztnQkFDaEMsdUNBQWlCLEVBQUMsZ0NBQWMsQ0FBQyxXQUFXLEVBQUUsUUFBUSxDQUFDLEtBQUssQ0FBQztZQUNqRSxDQUFDO1FBQ0wsQ0FBQyxDQUFDO0lBQ04sQ0FBQyxDQUFDO0FBQ04sQ0FBQzs7Ozs7OztVQ2xCRDtVQUNBOztVQUVBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBOztVQUVBO1VBQ0E7O1VBRUE7VUFDQTtVQUNBOzs7Ozs7Ozs7Ozs7QUNuQkEseUhBQTRDO0FBSzVDLE1BQU0sQ0FBQyxZQUFZLEdBQUcsSUFBSSwyQkFBWSxFQUFFLENBQUMiLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL0luZmluaVdpbmRvdy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL01lc3NhZ2luZ1RvSG9zdC50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL09ic2VydmVycy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vd2VicGFjay9ib290c3RyYXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9JbmRleC50cyJdLCJzb3VyY2VzQ29udGVudCI6WyIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtJSW5maW5pV2luZG93fSBmcm9tIFwiLi9Db250cmFjdHMvSUluZmluaVdpbmRvd1wiO1xyXG5pbXBvcnQge2dldFRpdGxlT2JzZXJ2ZXIsIFRpdGxlT2JzZXJ2ZXJUYXJnZXR9IGZyb20gXCIuL09ic2VydmVyc1wiO1xyXG5pbXBvcnQge0hvc3RNZXNzYWdlSWRzLCBzZW5kTWVzc2FnZVRvSG9zdH0gZnJvbSBcIi4vTWVzc2FnaW5nVG9Ib3N0XCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY2xhc3MgSW5maW5pV2luZG93IGltcGxlbWVudHMgSUluZmluaVdpbmRvdyB7XHJcbiAgICBcclxuICAgIGNvbnN0cnVjdG9yKCkge1xyXG4gICAgICAgIHRoaXMuYXNzaWduRXZlbnRMaXN0ZW5lcnMoKTtcclxuICAgICAgICB0aGlzLmFzc2lnbk9ic2VydmVycygpO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICBzZXRQb2ludGVyQ2FwdHVyZShlbGVtZW50OkVsZW1lbnQsIHBvaW50ZXJJZDogbnVtYmVyKSB7XHJcbiAgICAgICAgZWxlbWVudC5zZXRQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG4gICAgfVxyXG5cclxuICAgIHJlbGVhc2VQb2ludGVyQ2FwdHVyZShlbGVtZW50OkVsZW1lbnQsIHBvaW50ZXJJZDpudW1iZXIpIHtcclxuICAgICAgICBlbGVtZW50LnJlbGVhc2VQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICBwcml2YXRlIGFzc2lnbk9ic2VydmVycygpIHtcclxuICAgICAgICBpZiAoVGl0bGVPYnNlcnZlclRhcmdldCkgZ2V0VGl0bGVPYnNlcnZlcigpLm9ic2VydmUoVGl0bGVPYnNlcnZlclRhcmdldCwge2NoaWxkTGlzdDogdHJ1ZX0pO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICBwcml2YXRlIGFzc2lnbkV2ZW50TGlzdGVuZXJzKCkge1xyXG4gICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJmdWxsc2NyZWVuY2hhbmdlXCIsIChfOiBFdmVudCkgPT4ge1xyXG4gICAgICAgICAgICBpZiAoZG9jdW1lbnQuZnVsbHNjcmVlbkVsZW1lbnQpIHtcclxuICAgICAgICAgICAgICAgIHNlbmRNZXNzYWdlVG9Ib3N0KEhvc3RNZXNzYWdlSWRzLmZ1bGxzY3JlZW5FbnRlcik7XHJcbiAgICAgICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgICAgICBzZW5kTWVzc2FnZVRvSG9zdChIb3N0TWVzc2FnZUlkcy5mdWxsc2NyZWVuRXhpdCk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImtleWRvd25cIiwgYXN5bmMgKGU6IEtleWJvYXJkRXZlbnQpID0+IHtcclxuICAgICAgICAgICAgaWYgKGUua2V5ID09PSBcIkYxMVwiKSB7XHJcbiAgICAgICAgICAgICAgICBpZiAoZG9jdW1lbnQuZnVsbHNjcmVlbkVsZW1lbnQpIHtcclxuICAgICAgICAgICAgICAgICAgICBhd2FpdCBkb2N1bWVudC5leGl0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgICAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgICAgICAgICBhd2FpdCBkb2N1bWVudC5ib2R5LnJlcXVlc3RGdWxsc2NyZWVuKCk7XHJcbiAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgIH1cclxufSIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBjb25zdCBIb3N0TWVzc2FnZUlkcyA9IHtcclxuICAgIHRpdGxlQ2hhbmdlOiBcInRpdGxlOmNoYW5nZVwiLFxyXG4gICAgZnVsbHNjcmVlbkVudGVyOiBcImZ1bGxzY3JlZW46ZW50ZXJcIixcclxuICAgIGZ1bGxzY3JlZW5FeGl0OiBcImZ1bGxzY3JlZW46ZXhpdFwiLFxyXG59XHJcblxyXG5leHBvcnQgdHlwZSBIb3N0TWVzc2FnZUlkID0gdHlwZW9mIEhvc3RNZXNzYWdlSWRzW2tleW9mIHR5cGVvZiBIb3N0TWVzc2FnZUlkc107XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gc2VuZE1lc3NhZ2VUb0hvc3QoaWQ6IEhvc3RNZXNzYWdlSWQsIGRhdGE/OiBzdHJpbmcpIHtcclxuICAgIGNvbnN0IG1lc3NhZ2UgPSBkYXRhID8gYCR7aWR9OyR7ZGF0YX1gIDogaWQ7XHJcbiAgICBcclxuICAgIC8vIFRPRE8gLSBkZXRlcm1pbmUgbWVzc2FnaW5nIG1ldGhvZHMgZm9yIFBob3Rpbm8uTkVUIGZvciBhbGwgcGxhdGZvcm1zXHJcbiAgICBcclxuICAgIC8vIFRyeSBkaWZmZXJlbnQgbWVzc2FnaW5nIG1ldGhvZHMgZm9yIFBob3Rpbm8uTkVUXHJcbiAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xyXG4gICAgICAgIHdpbmRvdy5jaHJvbWUud2Vidmlldy5wb3N0TWVzc2FnZShtZXNzYWdlKTtcclxuICAgIH0gZWxzZSBpZiAod2luZG93LmV4dGVybmFsPy5zZW5kTWVzc2FnZSkge1xyXG4gICAgICAgIHdpbmRvdy5leHRlcm5hbC5zZW5kTWVzc2FnZShtZXNzYWdlKTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgICAgLy8gRmFsbGJhY2sgZm9yIGRldmVsb3BtZW50XHJcbiAgICAgICAgY29uc29sZS53YXJuKFwiTWVzc2FnZSB0byBob3N0IGZhaWxlZDpcIiwgbWVzc2FnZSk7XHJcbiAgICB9XHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtzZW5kTWVzc2FnZVRvSG9zdCwgSG9zdE1lc3NhZ2VJZHN9IGZyb20gXCIuL01lc3NhZ2luZ1RvSG9zdFwiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBjb25zdCBUaXRsZU9ic2VydmVyVGFyZ2V0IDogSFRNTFRpdGxlRWxlbWVudCB8IG51bGwgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKCd0aXRsZScpO1xyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIGdldFRpdGxlT2JzZXJ2ZXIoKSA6IE11dGF0aW9uT2JzZXJ2ZXIge1xyXG4gICAgcmV0dXJuIG5ldyBNdXRhdGlvbk9ic2VydmVyKChtdXRhdGlvbnMsIF8pID0+IHtcclxuICAgICAgICBtdXRhdGlvbnMuZm9yRWFjaCgobXV0YXRpb24pID0+IHtcclxuICAgICAgICAgICAgaWYgKG11dGF0aW9uLnR5cGUgPT09IFwiY2hpbGRMaXN0XCIpIHtcclxuICAgICAgICAgICAgICAgIHNlbmRNZXNzYWdlVG9Ib3N0KEhvc3RNZXNzYWdlSWRzLnRpdGxlQ2hhbmdlLCBkb2N1bWVudC50aXRsZSlcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0pXHJcbiAgICB9KVxyXG59IiwiLy8gVGhlIG1vZHVsZSBjYWNoZVxudmFyIF9fd2VicGFja19tb2R1bGVfY2FjaGVfXyA9IHt9O1xuXG4vLyBUaGUgcmVxdWlyZSBmdW5jdGlvblxuZnVuY3Rpb24gX193ZWJwYWNrX3JlcXVpcmVfXyhtb2R1bGVJZCkge1xuXHQvLyBDaGVjayBpZiBtb2R1bGUgaXMgaW4gY2FjaGVcblx0dmFyIGNhY2hlZE1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF07XG5cdGlmIChjYWNoZWRNb2R1bGUgIT09IHVuZGVmaW5lZCkge1xuXHRcdHJldHVybiBjYWNoZWRNb2R1bGUuZXhwb3J0cztcblx0fVxuXHQvLyBDcmVhdGUgYSBuZXcgbW9kdWxlIChhbmQgcHV0IGl0IGludG8gdGhlIGNhY2hlKVxuXHR2YXIgbW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXSA9IHtcblx0XHQvLyBubyBtb2R1bGUuaWQgbmVlZGVkXG5cdFx0Ly8gbm8gbW9kdWxlLmxvYWRlZCBuZWVkZWRcblx0XHRleHBvcnRzOiB7fVxuXHR9O1xuXG5cdC8vIEV4ZWN1dGUgdGhlIG1vZHVsZSBmdW5jdGlvblxuXHRfX3dlYnBhY2tfbW9kdWxlc19fW21vZHVsZUlkXS5jYWxsKG1vZHVsZS5leHBvcnRzLCBtb2R1bGUsIG1vZHVsZS5leHBvcnRzLCBfX3dlYnBhY2tfcmVxdWlyZV9fKTtcblxuXHQvLyBSZXR1cm4gdGhlIGV4cG9ydHMgb2YgdGhlIG1vZHVsZVxuXHRyZXR1cm4gbW9kdWxlLmV4cG9ydHM7XG59XG5cbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge0luZmluaVdpbmRvd30gZnJvbSBcIi4vSW5maW5pV2luZG93XCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQge307XHJcbndpbmRvdy5pbmZpbmlXaW5kb3cgPSBuZXcgSW5maW5pV2luZG93KCk7XHJcbiAgICBcclxuICAgIFxyXG4gICAgXHJcbiAgICBcclxuICAgIFxyXG4gICAgXHJcbiAgICBcclxuICAgIFxyXG4gICAgXHJcbiAgICBcclxuICAgICJdLCJuYW1lcyI6W10sInNvdXJjZVJvb3QiOiIifQ==