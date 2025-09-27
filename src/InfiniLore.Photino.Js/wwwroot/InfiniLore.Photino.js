/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/InfiniLore.Photino.Js/TsSource/Index.ts":
/*!*****************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/Index.ts ***!
  \*****************************************************/
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
const MessagingToHost_1 = __webpack_require__(/*! ./MessagingToHost */ "./src/InfiniLore.Photino.Js/TsSource/MessagingToHost.ts");
const Pointers_1 = __webpack_require__(/*! ./Pointers */ "./src/InfiniLore.Photino.Js/TsSource/Pointers.ts");
const Observers_1 = __webpack_require__(/*! ./Observers */ "./src/InfiniLore.Photino.Js/TsSource/Observers.ts");
document.addEventListener("fullscreenchange", (_) => {
    if (document.fullscreenElement) {
        (0, MessagingToHost_1.sendMessageToHost)("fullscreen:enter");
    }
    else {
        (0, MessagingToHost_1.sendMessageToHost)("fullscreen:exit");
    }
});
document.addEventListener("keydown", (e) => __awaiter(void 0, void 0, void 0, function* () {
    if (e.key === "F11") {
        if (document.fullscreenElement) {
            yield document.exitFullscreen();
        }
        else {
            yield document.body.requestFullscreen();
        }
    }
}));
window.setPointerCapture = Pointers_1.setPointerCapture;
window.releasePointerCapture = Pointers_1.releasePointerCapture;
(0, Observers_1.getTitleObserver)().observe(Observers_1.TitleObserverTarget, { childList: true });


/***/ }),

/***/ "./src/InfiniLore.Photino.Js/TsSource/MessagingToHost.ts":
/*!***************************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/MessagingToHost.ts ***!
  \***************************************************************/
/***/ ((__unused_webpack_module, exports) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.sendMessageToHost = sendMessageToHost;
function sendMessageToHost(message) {
    var _a, _b;
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
                (0, MessagingToHost_1.sendMessageToHost)(`title:change;${document.title}`);
            }
        });
    });
}


/***/ }),

/***/ "./src/InfiniLore.Photino.Js/TsSource/Pointers.ts":
/*!********************************************************!*\
  !*** ./src/InfiniLore.Photino.Js/TsSource/Pointers.ts ***!
  \********************************************************/
/***/ ((__unused_webpack_module, exports) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.setPointerCapture = setPointerCapture;
exports.releasePointerCapture = releasePointerCapture;
function setPointerCapture(element, pointerId) {
    element.setPointerCapture(pointerId);
}
function releasePointerCapture(element, pointerId) {
    element.releasePointerCapture(pointerId);
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
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSW5maW5pTG9yZS5QaG90aW5vLmpzIiwibWFwcGluZ3MiOiI7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7OztBQUdBLGtJQUFvRDtBQUNwRCw2R0FBb0U7QUFDcEUsZ0hBQWtFO0FBS2xFLFFBQVEsQ0FBQyxnQkFBZ0IsQ0FBQyxrQkFBa0IsRUFBRSxDQUFDLENBQVEsRUFBRSxFQUFFO0lBQ3ZELElBQUksUUFBUSxDQUFDLGlCQUFpQixFQUFFLENBQUM7UUFDN0IsdUNBQWlCLEVBQUMsa0JBQWtCLENBQUMsQ0FBQztJQUMxQyxDQUFDO1NBQU0sQ0FBQztRQUNKLHVDQUFpQixFQUFDLGlCQUFpQixDQUFDLENBQUM7SUFDekMsQ0FBQztBQUNMLENBQUMsQ0FBQyxDQUFDO0FBR0gsUUFBUSxDQUFDLGdCQUFnQixDQUFDLFNBQVMsRUFBRSxDQUFPLENBQWdCLEVBQUUsRUFBRTtJQUM1RCxJQUFJLENBQUMsQ0FBQyxHQUFHLEtBQUssS0FBSyxFQUFFLENBQUM7UUFDbEIsSUFBSSxRQUFRLENBQUMsaUJBQWlCLEVBQUUsQ0FBQztZQUM3QixNQUFNLFFBQVEsQ0FBQyxjQUFjLEVBQUUsQ0FBQztRQUNwQyxDQUFDO2FBQU0sQ0FBQztZQUNKLE1BQU0sUUFBUSxDQUFDLElBQUksQ0FBQyxpQkFBaUIsRUFBRSxDQUFDO1FBQzVDLENBQUM7SUFDTCxDQUFDO0FBQ0wsQ0FBQyxFQUFDLENBQUM7QUFFSCxNQUFNLENBQUMsaUJBQWlCLEdBQUcsNEJBQWlCLENBQUM7QUFDN0MsTUFBTSxDQUFDLHFCQUFxQixHQUFHLGdDQUFxQixDQUFDO0FBRXJELGdDQUFnQixHQUFFLENBQUMsT0FBTyxDQUFDLCtCQUFtQixFQUFFLEVBQUMsU0FBUyxFQUFFLElBQUksRUFBQyxDQUFDLENBQUM7Ozs7Ozs7Ozs7Ozs7QUN6Qm5FLDhDQVlDO0FBWkQsU0FBZ0IsaUJBQWlCLENBQUMsT0FBZTs7SUFJN0MsSUFBSSxZQUFNLENBQUMsTUFBTSwwQ0FBRSxPQUFPLEVBQUUsQ0FBQztRQUN6QixNQUFNLENBQUMsTUFBTSxDQUFDLE9BQU8sQ0FBQyxXQUFXLENBQUMsT0FBTyxDQUFDLENBQUM7SUFDL0MsQ0FBQztTQUFNLElBQUksWUFBTSxDQUFDLFFBQVEsMENBQUUsV0FBVyxFQUFFLENBQUM7UUFDdEMsTUFBTSxDQUFDLFFBQVEsQ0FBQyxXQUFXLENBQUMsT0FBTyxDQUFDLENBQUM7SUFDekMsQ0FBQztTQUFNLENBQUM7UUFFSixPQUFPLENBQUMsSUFBSSxDQUFDLHlCQUF5QixFQUFFLE9BQU8sQ0FBQyxDQUFDO0lBQ3JELENBQUM7QUFDTCxDQUFDOzs7Ozs7Ozs7Ozs7OztBQ1JELDRDQVFDO0FBaEJELGtJQUFvRDtBQU12QywyQkFBbUIsR0FBRyxRQUFRLENBQUMsYUFBYSxDQUFDLE9BQU8sQ0FBZ0IsQ0FBQztBQUVsRixTQUFnQixnQkFBZ0I7SUFDNUIsT0FBTyxJQUFJLGdCQUFnQixDQUFDLENBQUMsU0FBUyxFQUFFLENBQUMsRUFBRSxFQUFFO1FBQ3pDLFNBQVMsQ0FBQyxPQUFPLENBQUMsQ0FBQyxRQUFRLEVBQUUsRUFBRTtZQUMzQixJQUFJLFFBQVEsQ0FBQyxJQUFJLEtBQUssV0FBVyxFQUFFLENBQUM7Z0JBQ2hDLHVDQUFpQixFQUFDLGdCQUFnQixRQUFRLENBQUMsS0FBSyxFQUFFLENBQUM7WUFDdkQsQ0FBQztRQUNMLENBQUMsQ0FBQztJQUNOLENBQUMsQ0FBQztBQUNOLENBQUM7Ozs7Ozs7Ozs7Ozs7QUNaRCw4Q0FFQztBQUVELHNEQUVDO0FBTkQsU0FBZ0IsaUJBQWlCLENBQUMsT0FBZSxFQUFFLFNBQWlCO0lBQ2hFLE9BQU8sQ0FBQyxpQkFBaUIsQ0FBQyxTQUFTLENBQUMsQ0FBQztBQUN6QyxDQUFDO0FBRUQsU0FBZ0IscUJBQXFCLENBQUMsT0FBZSxFQUFFLFNBQWdCO0lBQ25FLE9BQU8sQ0FBQyxxQkFBcUIsQ0FBQyxTQUFTLENBQUMsQ0FBQztBQUM3QyxDQUFDOzs7Ozs7O1VDYkQ7VUFDQTs7VUFFQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTs7VUFFQTtVQUNBOztVQUVBO1VBQ0E7VUFDQTs7OztVRXRCQTtVQUNBO1VBQ0E7VUFDQSIsInNvdXJjZXMiOlsid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvSW5kZXgudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9NZXNzYWdpbmdUb0hvc3QudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9PYnNlcnZlcnMudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9Qb2ludGVycy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vd2VicGFjay9ib290c3RyYXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svYmVmb3JlLXN0YXJ0dXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svc3RhcnR1cCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vd2VicGFjay9hZnRlci1zdGFydHVwIl0sInNvdXJjZXNDb250ZW50IjpbIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge3NlbmRNZXNzYWdlVG9Ib3N0fSBmcm9tIFwiLi9NZXNzYWdpbmdUb0hvc3RcIjtcclxuaW1wb3J0IHtyZWxlYXNlUG9pbnRlckNhcHR1cmUsIHNldFBvaW50ZXJDYXB0dXJlfSBmcm9tIFwiLi9Qb2ludGVyc1wiO1xyXG5pbXBvcnQge2dldFRpdGxlT2JzZXJ2ZXIsIFRpdGxlT2JzZXJ2ZXJUYXJnZXR9IGZyb20gXCIuL09ic2VydmVyc1wiO1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IHt9O1xyXG5kb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwiZnVsbHNjcmVlbmNoYW5nZVwiLCAoXzogRXZlbnQpID0+IHtcclxuICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkge1xyXG4gICAgICAgIHNlbmRNZXNzYWdlVG9Ib3N0KFwiZnVsbHNjcmVlbjplbnRlclwiKTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgICAgc2VuZE1lc3NhZ2VUb0hvc3QoXCJmdWxsc2NyZWVuOmV4aXRcIik7XHJcbiAgICB9XHJcbn0pO1xyXG5cclxuLy8gVE9ETyBhZGQgdGhpcyBpcyBhIG9wdGlvbmFsP1xyXG5kb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwia2V5ZG93blwiLCBhc3luYyAoZTogS2V5Ym9hcmRFdmVudCkgPT4ge1xyXG4gICAgaWYgKGUua2V5ID09PSBcIkYxMVwiKSB7XHJcbiAgICAgICAgaWYgKGRvY3VtZW50LmZ1bGxzY3JlZW5FbGVtZW50KSB7XHJcbiAgICAgICAgICAgIGF3YWl0IGRvY3VtZW50LmV4aXRGdWxsc2NyZWVuKCk7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgYXdhaXQgZG9jdW1lbnQuYm9keS5yZXF1ZXN0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxufSk7XHJcblxyXG53aW5kb3cuc2V0UG9pbnRlckNhcHR1cmUgPSBzZXRQb2ludGVyQ2FwdHVyZTtcclxud2luZG93LnJlbGVhc2VQb2ludGVyQ2FwdHVyZSA9IHJlbGVhc2VQb2ludGVyQ2FwdHVyZTtcclxuXHJcbmdldFRpdGxlT2JzZXJ2ZXIoKS5vYnNlcnZlKFRpdGxlT2JzZXJ2ZXJUYXJnZXQsIHtjaGlsZExpc3Q6IHRydWV9KTtcclxuICAgIFxyXG4gICAgXHJcbiAgICBcclxuICAgIFxyXG4gICAgXHJcbiAgICBcclxuICAgIFxyXG4gICAgXHJcbiAgICBcclxuICAgIFxyXG4gICAgIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGZ1bmN0aW9uIHNlbmRNZXNzYWdlVG9Ib3N0KG1lc3NhZ2U6IHN0cmluZykge1xyXG4gICAgLy8gVE9ETyAtIGRldGVybWluZSBtZXNzYWdpbmcgbWV0aG9kcyBmb3IgUGhvdGluby5ORVQgZm9yIGFsbCBwbGF0Zm9ybXNcclxuICAgIFxyXG4gICAgLy8gVHJ5IGRpZmZlcmVudCBtZXNzYWdpbmcgbWV0aG9kcyBmb3IgUGhvdGluby5ORVRcclxuICAgIGlmICh3aW5kb3cuY2hyb21lPy53ZWJ2aWV3KSB7XHJcbiAgICAgICAgd2luZG93LmNocm9tZS53ZWJ2aWV3LnBvc3RNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgfSBlbHNlIGlmICh3aW5kb3cuZXh0ZXJuYWw/LnNlbmRNZXNzYWdlKSB7XHJcbiAgICAgICAgd2luZG93LmV4dGVybmFsLnNlbmRNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgICAvLyBGYWxsYmFjayBmb3IgZGV2ZWxvcG1lbnRcclxuICAgICAgICBjb25zb2xlLndhcm4oXCJNZXNzYWdlIHRvIGhvc3QgZmFpbGVkOlwiLCBtZXNzYWdlKTtcclxuICAgIH1cclxufSIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge3NlbmRNZXNzYWdlVG9Ib3N0fSBmcm9tIFwiLi9NZXNzYWdpbmdUb0hvc3RcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5cclxuZXhwb3J0IGNvbnN0IFRpdGxlT2JzZXJ2ZXJUYXJnZXQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKCd0aXRsZScpIGFzIEhUTUxFbGVtZW50O1xyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIGdldFRpdGxlT2JzZXJ2ZXIoKSA6IE11dGF0aW9uT2JzZXJ2ZXIge1xyXG4gICAgcmV0dXJuIG5ldyBNdXRhdGlvbk9ic2VydmVyKChtdXRhdGlvbnMsIF8pID0+IHtcclxuICAgICAgICBtdXRhdGlvbnMuZm9yRWFjaCgobXV0YXRpb24pID0+IHtcclxuICAgICAgICAgICAgaWYgKG11dGF0aW9uLnR5cGUgPT09IFwiY2hpbGRMaXN0XCIpIHtcclxuICAgICAgICAgICAgICAgIHNlbmRNZXNzYWdlVG9Ib3N0KGB0aXRsZTpjaGFuZ2U7JHtkb2N1bWVudC50aXRsZX1gKVxyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgfSlcclxuICAgIH0pXHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgZnVuY3Rpb24gc2V0UG9pbnRlckNhcHR1cmUoZWxlbWVudDpFbGVtZW50LCBwb2ludGVySWQ6IG51bWJlcikge1xyXG4gICAgZWxlbWVudC5zZXRQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG59XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gcmVsZWFzZVBvaW50ZXJDYXB0dXJlKGVsZW1lbnQ6RWxlbWVudCwgcG9pbnRlcklkOm51bWJlcikge1xyXG4gICAgZWxlbWVudC5yZWxlYXNlUG9pbnRlckNhcHR1cmUocG9pbnRlcklkKTtcclxufSIsIi8vIFRoZSBtb2R1bGUgY2FjaGVcbnZhciBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX18gPSB7fTtcblxuLy8gVGhlIHJlcXVpcmUgZnVuY3Rpb25cbmZ1bmN0aW9uIF9fd2VicGFja19yZXF1aXJlX18obW9kdWxlSWQpIHtcblx0Ly8gQ2hlY2sgaWYgbW9kdWxlIGlzIGluIGNhY2hlXG5cdHZhciBjYWNoZWRNb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdO1xuXHRpZiAoY2FjaGVkTW9kdWxlICE9PSB1bmRlZmluZWQpIHtcblx0XHRyZXR1cm4gY2FjaGVkTW9kdWxlLmV4cG9ydHM7XG5cdH1cblx0Ly8gQ3JlYXRlIGEgbmV3IG1vZHVsZSAoYW5kIHB1dCBpdCBpbnRvIHRoZSBjYWNoZSlcblx0dmFyIG1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF0gPSB7XG5cdFx0Ly8gbm8gbW9kdWxlLmlkIG5lZWRlZFxuXHRcdC8vIG5vIG1vZHVsZS5sb2FkZWQgbmVlZGVkXG5cdFx0ZXhwb3J0czoge31cblx0fTtcblxuXHQvLyBFeGVjdXRlIHRoZSBtb2R1bGUgZnVuY3Rpb25cblx0X193ZWJwYWNrX21vZHVsZXNfX1ttb2R1bGVJZF0uY2FsbChtb2R1bGUuZXhwb3J0cywgbW9kdWxlLCBtb2R1bGUuZXhwb3J0cywgX193ZWJwYWNrX3JlcXVpcmVfXyk7XG5cblx0Ly8gUmV0dXJuIHRoZSBleHBvcnRzIG9mIHRoZSBtb2R1bGVcblx0cmV0dXJuIG1vZHVsZS5leHBvcnRzO1xufVxuXG4iLCIiLCIvLyBzdGFydHVwXG4vLyBMb2FkIGVudHJ5IG1vZHVsZSBhbmQgcmV0dXJuIGV4cG9ydHNcbi8vIFRoaXMgZW50cnkgbW9kdWxlIGlzIHJlZmVyZW5jZWQgYnkgb3RoZXIgbW9kdWxlcyBzbyBpdCBjYW4ndCBiZSBpbmxpbmVkXG52YXIgX193ZWJwYWNrX2V4cG9ydHNfXyA9IF9fd2VicGFja19yZXF1aXJlX18oXCIuL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvSW5kZXgudHNcIik7XG4iLCIiXSwibmFtZXMiOltdLCJzb3VyY2VSb290IjoiIn0=