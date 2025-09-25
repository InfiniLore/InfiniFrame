/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
var __webpack_exports__ = {};
// This entry needs to be wrapped in an IIFE because it uses a non-standard name for the exports (exports).
(() => {
var exports = __webpack_exports__;
/*!************************************************************!*\
  !*** ./src/InfiniLore.Photino.NET/wwwroot/Source/Index.ts ***!
  \************************************************************/

Object.defineProperty(exports, "__esModule", ({ value: true }));
function sendMessageToHost(message) {
    var _a, _b;
    if ((_a = window.chrome) === null || _a === void 0 ? void 0 : _a.webview) {
        window.chrome.webview.postMessage(message);
    }
    else if ((_b = window.external) === null || _b === void 0 ? void 0 : _b.sendMessage) {
        window.external.sendMessage(message);
    }
    else {
        console.log("Message to host failed:", message);
    }
}
const onFullscreenChange = (event) => {
    if (document.fullscreenElement) {
        sendMessageToHost("fullscreen:enter");
    }
    else {
        sendMessageToHost("fullscreen:exit");
    }
};
document.addEventListener("fullscreenchange", onFullscreenChange);

})();

/******/ })()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiSW5maW5pTG9yZS5QaG90aW5vLmpzIiwibWFwcGluZ3MiOiI7Ozs7Ozs7Ozs7O0FBeUJBLFNBQVMsaUJBQWlCLENBQUMsT0FBZTs7SUFFdEMsSUFBSSxZQUFNLENBQUMsTUFBTSwwQ0FBRSxPQUFPLEVBQUUsQ0FBQztRQUN6QixNQUFNLENBQUMsTUFBTSxDQUFDLE9BQU8sQ0FBQyxXQUFXLENBQUMsT0FBTyxDQUFDLENBQUM7SUFDL0MsQ0FBQztTQUFNLElBQUksWUFBTSxDQUFDLFFBQVEsMENBQUUsV0FBVyxFQUFFLENBQUM7UUFDdEMsTUFBTSxDQUFDLFFBQVEsQ0FBQyxXQUFXLENBQUMsT0FBTyxDQUFDLENBQUM7SUFDekMsQ0FBQztTQUFNLENBQUM7UUFFSixPQUFPLENBQUMsR0FBRyxDQUFDLHlCQUF5QixFQUFFLE9BQU8sQ0FBQyxDQUFDO0lBQ3BELENBQUM7QUFDTCxDQUFDO0FBRUQsTUFBTSxrQkFBa0IsR0FBRyxDQUFDLEtBQVksRUFBRSxFQUFFO0lBQ3hDLElBQUksUUFBUSxDQUFDLGlCQUFpQixFQUFFLENBQUM7UUFDN0IsaUJBQWlCLENBQUMsa0JBQWtCLENBQUMsQ0FBQztJQUMxQyxDQUFDO1NBQU0sQ0FBQztRQUNKLGlCQUFpQixDQUFDLGlCQUFpQixDQUFDLENBQUM7SUFDekMsQ0FBQztBQUNMLENBQUMsQ0FBQztBQUVGLFFBQVEsQ0FBQyxnQkFBZ0IsQ0FBQyxrQkFBa0IsRUFBRSxrQkFBa0IsQ0FBQyxDQUFDIiwic291cmNlcyI6WyJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5ORVQvd3d3cm9vdC9Tb3VyY2UvSW5kZXgudHMiXSwic291cmNlc0NvbnRlbnQiOlsiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IHt9O1xyXG5cclxuZGVjbGFyZSBnbG9iYWwge1xyXG4gICAgLy8gbm9pbnNwZWN0aW9uIEpTVW51c2VkR2xvYmFsU3ltYm9sc1xyXG4gICAgaW50ZXJmYWNlIFdpbmRvdyB7XHJcbiAgICAgICAgY2hyb21lPzoge1xyXG4gICAgICAgICAgICB3ZWJ2aWV3Pzoge1xyXG4gICAgICAgICAgICAgICAgcG9zdE1lc3NhZ2UobWVzc2FnZTogc3RyaW5nKTogdm9pZDtcclxuICAgICAgICAgICAgfTtcclxuICAgICAgICB9O1xyXG4gICAgfVxyXG4gICAgaW50ZXJmYWNlIEV4dGVybmFsIHtcclxuICAgICAgICBzZW5kTWVzc2FnZT8obWVzc2FnZTogc3RyaW5nKTogdm9pZDtcclxuICAgIH1cclxufVxyXG5cclxuXHJcblxyXG5mdW5jdGlvbiBzZW5kTWVzc2FnZVRvSG9zdChtZXNzYWdlOiBzdHJpbmcpIHtcclxuICAgIC8vIFRyeSBkaWZmZXJlbnQgbWVzc2FnaW5nIG1ldGhvZHMgZm9yIFBob3Rpbm8uTkVUXHJcbiAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xyXG4gICAgICAgIHdpbmRvdy5jaHJvbWUud2Vidmlldy5wb3N0TWVzc2FnZShtZXNzYWdlKTtcclxuICAgIH0gZWxzZSBpZiAod2luZG93LmV4dGVybmFsPy5zZW5kTWVzc2FnZSkge1xyXG4gICAgICAgIHdpbmRvdy5leHRlcm5hbC5zZW5kTWVzc2FnZShtZXNzYWdlKTtcclxuICAgIH0gZWxzZSB7XHJcbiAgICAgICAgLy8gRmFsbGJhY2sgZm9yIGRldmVsb3BtZW50XHJcbiAgICAgICAgY29uc29sZS5sb2coXCJNZXNzYWdlIHRvIGhvc3QgZmFpbGVkOlwiLCBtZXNzYWdlKTtcclxuICAgIH1cclxufVxyXG5cclxuY29uc3Qgb25GdWxsc2NyZWVuQ2hhbmdlID0gKGV2ZW50OiBFdmVudCkgPT4ge1xyXG4gICAgaWYgKGRvY3VtZW50LmZ1bGxzY3JlZW5FbGVtZW50KSB7XHJcbiAgICAgICAgc2VuZE1lc3NhZ2VUb0hvc3QoXCJmdWxsc2NyZWVuOmVudGVyXCIpO1xyXG4gICAgfSBlbHNlIHtcclxuICAgICAgICBzZW5kTWVzc2FnZVRvSG9zdChcImZ1bGxzY3JlZW46ZXhpdFwiKTtcclxuICAgIH1cclxufTtcclxuXHJcbmRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJmdWxsc2NyZWVuY2hhbmdlXCIsIG9uRnVsbHNjcmVlbkNoYW5nZSk7XHJcbiJdLCJuYW1lcyI6W10sInNvdXJjZVJvb3QiOiIifQ==