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
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSW5maW5pTG9yZS5QaG90aW5vLmpzIiwibWFwcGluZ3MiOiI7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7OztBQUdBLHlJQUFvRDtBQUNwRCxvSEFBb0U7QUFLcEUsUUFBUSxDQUFDLGdCQUFnQixDQUFDLGtCQUFrQixFQUFFLENBQUMsQ0FBUSxFQUFFLEVBQUU7SUFDdkQsSUFBSSxRQUFRLENBQUMsaUJBQWlCLEVBQUUsQ0FBQztRQUM3Qix1Q0FBaUIsRUFBQyxrQkFBa0IsQ0FBQyxDQUFDO0lBQzFDLENBQUM7U0FBTSxDQUFDO1FBQ0osdUNBQWlCLEVBQUMsaUJBQWlCLENBQUMsQ0FBQztJQUN6QyxDQUFDO0FBQ0wsQ0FBQyxDQUFDLENBQUM7QUFHSCxRQUFRLENBQUMsZ0JBQWdCLENBQUMsU0FBUyxFQUFFLENBQU8sQ0FBZ0IsRUFBRSxFQUFFO0lBQzVELElBQUksQ0FBQyxDQUFDLEdBQUcsS0FBSyxLQUFLLEVBQUUsQ0FBQztRQUNsQixJQUFJLFFBQVEsQ0FBQyxpQkFBaUIsRUFBRSxDQUFDO1lBQzdCLE1BQU0sUUFBUSxDQUFDLGNBQWMsRUFBRSxDQUFDO1FBQ3BDLENBQUM7YUFBTSxDQUFDO1lBQ0osTUFBTSxRQUFRLENBQUMsSUFBSSxDQUFDLGlCQUFpQixFQUFFLENBQUM7UUFDNUMsQ0FBQztJQUNMLENBQUM7QUFDTCxDQUFDLEVBQUMsQ0FBQztBQUVILE1BQU0sQ0FBQyxpQkFBaUIsR0FBRyw0QkFBaUIsQ0FBQztBQUM3QyxNQUFNLENBQUMscUJBQXFCLEdBQUcsZ0NBQXFCLENBQUM7Ozs7Ozs7Ozs7Ozs7QUN0QnJELDhDQVlDO0FBWkQsU0FBZ0IsaUJBQWlCLENBQUMsT0FBZTs7SUFJN0MsSUFBSSxZQUFNLENBQUMsTUFBTSwwQ0FBRSxPQUFPLEVBQUUsQ0FBQztRQUN6QixNQUFNLENBQUMsTUFBTSxDQUFDLE9BQU8sQ0FBQyxXQUFXLENBQUMsT0FBTyxDQUFDLENBQUM7SUFDL0MsQ0FBQztTQUFNLElBQUksWUFBTSxDQUFDLFFBQVEsMENBQUUsV0FBVyxFQUFFLENBQUM7UUFDdEMsTUFBTSxDQUFDLFFBQVEsQ0FBQyxXQUFXLENBQUMsT0FBTyxDQUFDLENBQUM7SUFDekMsQ0FBQztTQUFNLENBQUM7UUFFSixPQUFPLENBQUMsSUFBSSxDQUFDLHlCQUF5QixFQUFFLE9BQU8sQ0FBQyxDQUFDO0lBQ3JELENBQUM7QUFDTCxDQUFDOzs7Ozs7Ozs7Ozs7O0FDWkQsOENBRUM7QUFFRCxzREFFQztBQU5ELFNBQWdCLGlCQUFpQixDQUFDLE9BQWUsRUFBRSxTQUFpQjtJQUNoRSxPQUFPLENBQUMsaUJBQWlCLENBQUMsU0FBUyxDQUFDLENBQUM7QUFDekMsQ0FBQztBQUVELFNBQWdCLHFCQUFxQixDQUFDLE9BQWUsRUFBRSxTQUFnQjtJQUNuRSxPQUFPLENBQUMscUJBQXFCLENBQUMsU0FBUyxDQUFDLENBQUM7QUFDN0MsQ0FBQzs7Ozs7OztVQ2JEO1VBQ0E7O1VBRUE7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7O1VBRUE7VUFDQTs7VUFFQTtVQUNBO1VBQ0E7Ozs7VUV0QkE7VUFDQTtVQUNBO1VBQ0EiLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLk5FVC93d3dyb290L1NvdXJjZS9JbmRleC50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLk5FVC93d3dyb290L1NvdXJjZS9NZXNzYWdpbmdUb0hvc3QudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5ORVQvd3d3cm9vdC9Tb3VyY2UvUG9pbnRlcnMudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svYm9vdHN0cmFwIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL2JlZm9yZS1zdGFydHVwIiwid2VicGFjazovL2luZmluaWxvcmUucGhvdGluby93ZWJwYWNrL3N0YXJ0dXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svYWZ0ZXItc3RhcnR1cCJdLCJzb3VyY2VzQ29udGVudCI6WyIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtzZW5kTWVzc2FnZVRvSG9zdH0gZnJvbSBcIi4vTWVzc2FnaW5nVG9Ib3N0XCI7XHJcbmltcG9ydCB7cmVsZWFzZVBvaW50ZXJDYXB0dXJlLCBzZXRQb2ludGVyQ2FwdHVyZX0gZnJvbSBcIi4vUG9pbnRlcnNcIjtcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCB7fTtcclxuZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImZ1bGxzY3JlZW5jaGFuZ2VcIiwgKF86IEV2ZW50KSA9PiB7XHJcbiAgICBpZiAoZG9jdW1lbnQuZnVsbHNjcmVlbkVsZW1lbnQpIHtcclxuICAgICAgICBzZW5kTWVzc2FnZVRvSG9zdChcImZ1bGxzY3JlZW46ZW50ZXJcIik7XHJcbiAgICB9IGVsc2Uge1xyXG4gICAgICAgIHNlbmRNZXNzYWdlVG9Ib3N0KFwiZnVsbHNjcmVlbjpleGl0XCIpO1xyXG4gICAgfVxyXG59KTtcclxuXHJcbi8vIFRPRE8gYWRkIHRoaXMgaXMgYSBvcHRpb25hbD9cclxuZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lcihcImtleWRvd25cIiwgYXN5bmMgKGU6IEtleWJvYXJkRXZlbnQpID0+IHtcclxuICAgIGlmIChlLmtleSA9PT0gXCJGMTFcIikge1xyXG4gICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkge1xyXG4gICAgICAgICAgICBhd2FpdCBkb2N1bWVudC5leGl0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIGF3YWl0IGRvY3VtZW50LmJvZHkucmVxdWVzdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbn0pO1xyXG5cclxud2luZG93LnNldFBvaW50ZXJDYXB0dXJlID0gc2V0UG9pbnRlckNhcHR1cmU7XHJcbndpbmRvdy5yZWxlYXNlUG9pbnRlckNhcHR1cmUgPSByZWxlYXNlUG9pbnRlckNhcHR1cmU7XHJcbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBmdW5jdGlvbiBzZW5kTWVzc2FnZVRvSG9zdChtZXNzYWdlOiBzdHJpbmcpIHtcclxuICAgIC8vIFRPRE8gLSBkZXRlcm1pbmUgbWVzc2FnaW5nIG1ldGhvZHMgZm9yIFBob3Rpbm8uTkVUIGZvciBhbGwgcGxhdGZvcm1zXHJcbiAgICBcclxuICAgIC8vIFRyeSBkaWZmZXJlbnQgbWVzc2FnaW5nIG1ldGhvZHMgZm9yIFBob3Rpbm8uTkVUXHJcbiAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xyXG4gICAgICAgIHdpbmRvdy5jaHJvbWUud2Vidmlldy5wb3N0TWVzc2FnZShtZXNzYWdlKTtcclxuICAgIH0gZWxzZSBpZiAod2luZG93LmV4dGVybmFsPy5zZW5kTWVzc2FnZSkge1xyXG4gICAgICAgIHdpbmRvdy5leHRlcm5hbC5zZW5kTWVzc2FnZShtZXNzYWdlKTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgICAgLy8gRmFsbGJhY2sgZm9yIGRldmVsb3BtZW50XHJcbiAgICAgICAgY29uc29sZS53YXJuKFwiTWVzc2FnZSB0byBob3N0IGZhaWxlZDpcIiwgbWVzc2FnZSk7XHJcbiAgICB9XHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgZnVuY3Rpb24gc2V0UG9pbnRlckNhcHR1cmUoZWxlbWVudDpFbGVtZW50LCBwb2ludGVySWQ6IG51bWJlcikge1xyXG4gICAgZWxlbWVudC5zZXRQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG59XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gcmVsZWFzZVBvaW50ZXJDYXB0dXJlKGVsZW1lbnQ6RWxlbWVudCwgcG9pbnRlcklkOm51bWJlcikge1xyXG4gICAgZWxlbWVudC5yZWxlYXNlUG9pbnRlckNhcHR1cmUocG9pbnRlcklkKTtcclxufSIsIi8vIFRoZSBtb2R1bGUgY2FjaGVcbnZhciBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX18gPSB7fTtcblxuLy8gVGhlIHJlcXVpcmUgZnVuY3Rpb25cbmZ1bmN0aW9uIF9fd2VicGFja19yZXF1aXJlX18obW9kdWxlSWQpIHtcblx0Ly8gQ2hlY2sgaWYgbW9kdWxlIGlzIGluIGNhY2hlXG5cdHZhciBjYWNoZWRNb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdO1xuXHRpZiAoY2FjaGVkTW9kdWxlICE9PSB1bmRlZmluZWQpIHtcblx0XHRyZXR1cm4gY2FjaGVkTW9kdWxlLmV4cG9ydHM7XG5cdH1cblx0Ly8gQ3JlYXRlIGEgbmV3IG1vZHVsZSAoYW5kIHB1dCBpdCBpbnRvIHRoZSBjYWNoZSlcblx0dmFyIG1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF0gPSB7XG5cdFx0Ly8gbm8gbW9kdWxlLmlkIG5lZWRlZFxuXHRcdC8vIG5vIG1vZHVsZS5sb2FkZWQgbmVlZGVkXG5cdFx0ZXhwb3J0czoge31cblx0fTtcblxuXHQvLyBFeGVjdXRlIHRoZSBtb2R1bGUgZnVuY3Rpb25cblx0X193ZWJwYWNrX21vZHVsZXNfX1ttb2R1bGVJZF0uY2FsbChtb2R1bGUuZXhwb3J0cywgbW9kdWxlLCBtb2R1bGUuZXhwb3J0cywgX193ZWJwYWNrX3JlcXVpcmVfXyk7XG5cblx0Ly8gUmV0dXJuIHRoZSBleHBvcnRzIG9mIHRoZSBtb2R1bGVcblx0cmV0dXJuIG1vZHVsZS5leHBvcnRzO1xufVxuXG4iLCIiLCIvLyBzdGFydHVwXG4vLyBMb2FkIGVudHJ5IG1vZHVsZSBhbmQgcmV0dXJuIGV4cG9ydHNcbi8vIFRoaXMgZW50cnkgbW9kdWxlIGlzIHJlZmVyZW5jZWQgYnkgb3RoZXIgbW9kdWxlcyBzbyBpdCBjYW4ndCBiZSBpbmxpbmVkXG52YXIgX193ZWJwYWNrX2V4cG9ydHNfXyA9IF9fd2VicGFja19yZXF1aXJlX18oXCIuL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uTkVUL3d3d3Jvb3QvU291cmNlL0luZGV4LnRzXCIpO1xuIiwiIl0sIm5hbWVzIjpbXSwic291cmNlUm9vdCI6IiJ9