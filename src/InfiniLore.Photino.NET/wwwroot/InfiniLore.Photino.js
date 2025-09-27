/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/InfiniLore.Photino.NET/wwwroot/Source/Index.ts":
/*!************************************************************!*\
  !*** ./src/InfiniLore.Photino.NET/wwwroot/Source/Index.ts ***!
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
const MessagingToHost_1 = __webpack_require__(/*! ./MessagingToHost */ "./src/InfiniLore.Photino.NET/wwwroot/Source/MessagingToHost.ts");
const Pointers_1 = __webpack_require__(/*! ./Pointers */ "./src/InfiniLore.Photino.NET/wwwroot/Source/Pointers.ts");
const Observers_1 = __webpack_require__(/*! ./Observers */ "./src/InfiniLore.Photino.NET/wwwroot/Source/Observers.ts");
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

/***/ "./src/InfiniLore.Photino.NET/wwwroot/Source/MessagingToHost.ts":
/*!**********************************************************************!*\
  !*** ./src/InfiniLore.Photino.NET/wwwroot/Source/MessagingToHost.ts ***!
  \**********************************************************************/
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

/***/ "./src/InfiniLore.Photino.NET/wwwroot/Source/Observers.ts":
/*!****************************************************************!*\
  !*** ./src/InfiniLore.Photino.NET/wwwroot/Source/Observers.ts ***!
  \****************************************************************/
/***/ ((__unused_webpack_module, exports, __webpack_require__) => {


Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.TitleObserverTarget = void 0;
exports.getTitleObserver = getTitleObserver;
const MessagingToHost_1 = __webpack_require__(/*! ./MessagingToHost */ "./src/InfiniLore.Photino.NET/wwwroot/Source/MessagingToHost.ts");
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

/***/ "./src/InfiniLore.Photino.NET/wwwroot/Source/Pointers.ts":
/*!***************************************************************!*\
  !*** ./src/InfiniLore.Photino.NET/wwwroot/Source/Pointers.ts ***!
  \***************************************************************/
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
/******/ 	var __webpack_exports__ = __webpack_require__("./src/InfiniLore.Photino.NET/wwwroot/Source/Index.ts");
/******/ 	
/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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Q0E7VUFDQTs7VUFFQTtVQUNBOztVQUVBO1VBQ0E7VUFDQTs7OztVRXRCQTtVQUNBO1VBQ0E7VUFDQSIsInNvdXJjZXMiOlsid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uTkVUL3d3d3Jvb3QvU291cmNlL0luZGV4LnRzIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby8uL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uTkVUL3d3d3Jvb3QvU291cmNlL01lc3NhZ2luZ1RvSG9zdC50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLk5FVC93d3dyb290L1NvdXJjZS9PYnNlcnZlcnMudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5ORVQvd3d3cm9vdC9Tb3VyY2UvUG9pbnRlcnMudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svYm9vdHN0cmFwIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL2JlZm9yZS1zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL3N0YXJ0dXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svYWZ0ZXItc3RhcnR1cCJdLCJzb3VyY2VzQ29udGVudCI6WyIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtzZW5kTWVzc2FnZVRvSG9zdH0gZnJvbSBcIi4vTWVzc2FnaW5nVG9Ib3N0XCI7XHJcbmltcG9ydCB7cmVsZWFzZVBvaW50ZXJDYXB0dXJlLCBzZXRQb2ludGVyQ2FwdHVyZX0gZnJvbSBcIi4vUG9pbnRlcnNcIjtcclxuaW1wb3J0IHtnZXRUaXRsZU9ic2VydmVyLCBUaXRsZU9ic2VydmVyVGFyZ2V0fSBmcm9tIFwiLi9PYnNlcnZlcnNcIjtcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCB7fTtcclxuZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImZ1bGxzY3JlZW5jaGFuZ2VcIiwgKF86IEV2ZW50KSA9PiB7XHJcbiAgICBpZiAoZG9jdW1lbnQuZnVsbHNjcmVlbkVsZW1lbnQpIHtcclxuICAgICAgICBzZW5kTWVzc2FnZVRvSG9zdChcImZ1bGxzY3JlZW46ZW50ZXJcIik7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICAgIHNlbmRNZXNzYWdlVG9Ib3N0KFwiZnVsbHNjcmVlbjpleGl0XCIpO1xyXG4gICAgfVxyXG59KTtcclxuXHJcbi8vIFRPRE8gYWRkIHRoaXMgaXMgYSBvcHRpb25hbD9cclxuZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImtleWRvd25cIiwgYXN5bmMgKGU6IEtleWJvYXJkRXZlbnQpID0+IHtcclxuICAgIGlmIChlLmtleSA9PT0gXCJGMTFcIikge1xyXG4gICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkge1xyXG4gICAgICAgICAgICBhd2FpdCBkb2N1bWVudC5leGl0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIGF3YWl0IGRvY3VtZW50LmJvZHkucmVxdWVzdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0pO1xyXG5cclxud2luZG93LnNldFBvaW50ZXJDYXB0dXJlID0gc2V0UG9pbnRlckNhcHR1cmU7XHJcbndpbmRvdy5yZWxlYXNlUG9pbnRlckNhcHR1cmUgPSByZWxlYXNlUG9pbnRlckNhcHR1cmU7XHJcblxyXG5nZXRUaXRsZU9ic2VydmVyKCkub2JzZXJ2ZShUaXRsZU9ic2VydmVyVGFyZ2V0LCB7Y2hpbGRMaXN0OiB0cnVlfSk7XHJcbiAgICBcclxuICAgIFxyXG4gICAgXHJcbiAgICBcclxuICAgIFxyXG4gICAgXHJcbiAgICBcclxuICAgIFxyXG4gICAgXHJcbiAgICBcclxuICAgICIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBmdW5jdGlvbiBzZW5kTWVzc2FnZVRvSG9zdChtZXNzYWdlOiBzdHJpbmcpIHtcclxuICAgIC8vIFRPRE8gLSBkZXRlcm1pbmUgbWVzc2FnaW5nIG1ldGhvZHMgZm9yIFBob3Rpbm8uTkVUIGZvciBhbGwgcGxhdGZvcm1zXHJcbiAgICBcclxuICAgIC8vIFRyeSBkaWZmZXJlbnQgbWVzc2FnaW5nIG1ldGhvZHMgZm9yIFBob3Rpbm8uTkVUXHJcbiAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xyXG4gICAgICAgIHdpbmRvdy5jaHJvbWUud2Vidmlldy5wb3N0TWVzc2FnZShtZXNzYWdlKTtcclxuICAgIH0gZWxzZSBpZiAod2luZG93LmV4dGVybmFsPy5zZW5kTWVzc2FnZSkge1xyXG4gICAgICAgIHdpbmRvdy5leHRlcm5hbC5zZW5kTWVzc2FnZShtZXNzYWdlKTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgICAgLy8gRmFsbGJhY2sgZm9yIGRldmVsb3BtZW50XHJcbiAgICAgICAgY29uc29sZS53YXJuKFwiTWVzc2FnZSB0byBob3N0IGZhaWxlZDpcIiwgbWVzc2FnZSk7XHJcbiAgICB9XHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtzZW5kTWVzc2FnZVRvSG9zdH0gZnJvbSBcIi4vTWVzc2FnaW5nVG9Ib3N0XCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuXHJcbmV4cG9ydCBjb25zdCBUaXRsZU9ic2VydmVyVGFyZ2V0ID0gZG9jdW1lbnQucXVlcnlTZWxlY3RvcigndGl0bGUnKSBhcyBIVE1MRWxlbWVudDtcclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBnZXRUaXRsZU9ic2VydmVyKCkgOiBNdXRhdGlvbk9ic2VydmVyIHtcclxuICAgIHJldHVybiBuZXcgTXV0YXRpb25PYnNlcnZlcigobXV0YXRpb25zLCBfKSA9PiB7XHJcbiAgICAgICAgbXV0YXRpb25zLmZvckVhY2goKG11dGF0aW9uKSA9PiB7XHJcbiAgICAgICAgICAgIGlmIChtdXRhdGlvbi50eXBlID09PSBcImNoaWxkTGlzdFwiKSB7XHJcbiAgICAgICAgICAgICAgICBzZW5kTWVzc2FnZVRvSG9zdChgdGl0bGU6Y2hhbmdlOyR7ZG9jdW1lbnQudGl0bGV9YClcclxuICAgICAgICAgICAgfVxyXG4gICAgICAgIH0pXHJcbiAgICB9KVxyXG59IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGZ1bmN0aW9uIHNldFBvaW50ZXJDYXB0dXJlKGVsZW1lbnQ6RWxlbWVudCwgcG9pbnRlcklkOiBudW1iZXIpIHtcclxuICAgIGVsZW1lbnQuc2V0UG9pbnRlckNhcHR1cmUocG9pbnRlcklkKTtcclxufVxyXG5cclxuZXhwb3J0IGZ1bmN0aW9uIHJlbGVhc2VQb2ludGVyQ2FwdHVyZShlbGVtZW50OkVsZW1lbnQsIHBvaW50ZXJJZDpudW1iZXIpIHtcclxuICAgIGVsZW1lbnQucmVsZWFzZVBvaW50ZXJDYXB0dXJlKHBvaW50ZXJJZCk7XHJcbn0iLCIvLyBUaGUgbW9kdWxlIGNhY2hlXG52YXIgX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fID0ge307XG5cbi8vIFRoZSByZXF1aXJlIGZ1bmN0aW9uXG5mdW5jdGlvbiBfX3dlYnBhY2tfcmVxdWlyZV9fKG1vZHVsZUlkKSB7XG5cdC8vIENoZWNrIGlmIG1vZHVsZSBpcyBpbiBjYWNoZVxuXHR2YXIgY2FjaGVkTW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXTtcblx0aWYgKGNhY2hlZE1vZHVsZSAhPT0gdW5kZWZpbmVkKSB7XG5cdFx0cmV0dXJuIGNhY2hlZE1vZHVsZS5leHBvcnRzO1xuXHR9XG5cdC8vIENyZWF0ZSBhIG5ldyBtb2R1bGUgKGFuZCBwdXQgaXQgaW50byB0aGUgY2FjaGUpXG5cdHZhciBtb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdID0ge1xuXHRcdC8vIG5vIG1vZHVsZS5pZCBuZWVkZWRcblx0XHQvLyBubyBtb2R1bGUubG9hZGVkIG5lZWRlZFxuXHRcdGV4cG9ydHM6IHt9XG5cdH07XG5cblx0Ly8gRXhlY3V0ZSB0aGUgbW9kdWxlIGZ1bmN0aW9uXG5cdF9fd2VicGFja19tb2R1bGVzX19bbW9kdWxlSWRdLmNhbGwobW9kdWxlLmV4cG9ydHMsIG1vZHVsZSwgbW9kdWxlLmV4cG9ydHMsIF9fd2VicGFja19yZXF1aXJlX18pO1xuXG5cdC8vIFJldHVybiB0aGUgZXhwb3J0cyBvZiB0aGUgbW9kdWxlXG5cdHJldHVybiBtb2R1bGUuZXhwb3J0cztcbn1cblxuIiwiIiwiLy8gc3RhcnR1cFxuLy8gTG9hZCBlbnRyeSBtb2R1bGUgYW5kIHJldHVybiBleHBvcnRzXG4vLyBUaGlzIGVudHJ5IG1vZHVsZSBpcyByZWZlcmVuY2VkIGJ5IG90aGVyIG1vZHVsZXMgc28gaXQgY2FuJ3QgYmUgaW5saW5lZFxudmFyIF9fd2VicGFja19leHBvcnRzX18gPSBfX3dlYnBhY2tfcmVxdWlyZV9fKFwiLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLk5FVC93d3dyb290L1NvdXJjZS9JbmRleC50c1wiKTtcbiIsIiJdLCJuYW1lcyI6W10sInNvdXJjZVJvb3QiOiIifQ==