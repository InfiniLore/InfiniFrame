/******/
(() => { // webpackBootstrap
    /******/
    "use strict";
    /******/
    var __webpack_modules__ = ({

        /***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/BlankTargetHandler.ts":
        /*!**********************************************************************!*\
          !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/BlankTargetHandler.ts ***!
          \**********************************************************************/
        /***/ (function (__unused_webpack_module, exports, __webpack_require__) {


            var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
                function adopt(value) {
                    return value instanceof P ? value : new P(function (resolve) {
                        resolve(value);
                    });
                }

                return new (P || (P = Promise))(function (resolve, reject) {
                    function fulfilled(value) {
                        try {
                            step(generator.next(value));
                        } catch (e) {
                            reject(e);
                        }
                    }

                    function rejected(value) {
                        try {
                            step(generator["throw"](value));
                        } catch (e) {
                            reject(e);
                        }
                    }

                    function step(result) {
                        result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected);
                    }

                    step((generator = generator.apply(thisArg, _arguments || [])).next());
                });
            };
            Object.defineProperty(exports, "__esModule", ({value: true}));
            exports.blankTargetHandler = blankTargetHandler;
            const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts");

            function isExternalLink(url) {
                try {
                    return new URL(url, location.href).hostname !== location.hostname;
                } catch (_a) {
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


            /***/
        }),

        /***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts":
        /*!****************************************************************************!*\
          !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts ***!
          \****************************************************************************/
        /***/ ((__unused_webpack_module, exports) => {


            Object.defineProperty(exports, "__esModule", ({value: true}));
            exports.ReceiveFromHostMessageIds = exports.SendToHostMessageIds = void 0;
            exports.SendToHostMessageIds = {
                titleChange: "__infiniWindow:title:change",
                fullscreenEnter: "__infiniWindow:fullscreen:enter",
                fullscreenExit: "__infiniWindow:fullscreen:exit",
                openExternalLink: "__infiniWindow:open:external",
                windowClose: "__infiniWindow:window:close",
            };
            exports.ReceiveFromHostMessageIds = {
                registerOpenExternal: "__infiniWindow:register:open:external",
                registerFullscreenChange: "__infiniWindow:register:fullscreen:change",
                registerTitleChange: "__infiniWindow:register:title:change",
                registerWindowClose: "__infiniWindow:register:window:close",
            };


            /***/
        }),

        /***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/HostMessaging.ts":
        /*!*****************************************************************!*\
          !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/HostMessaging.ts ***!
          \*****************************************************************/
        /***/ (function (__unused_webpack_module, exports, __webpack_require__) {


            var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
                function adopt(value) {
                    return value instanceof P ? value : new P(function (resolve) {
                        resolve(value);
                    });
                }

                return new (P || (P = Promise))(function (resolve, reject) {
                    function fulfilled(value) {
                        try {
                            step(generator.next(value));
                        } catch (e) {
                            reject(e);
                        }
                    }

                    function rejected(value) {
                        try {
                            step(generator["throw"](value));
                        } catch (e) {
                            reject(e);
                        }
                    }

                    function step(result) {
                        result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected);
                    }

                    step((generator = generator.apply(thisArg, _arguments || [])).next());
                });
            };
            Object.defineProperty(exports, "__esModule", ({value: true}));
            const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts");
            const BlankTargetHandler_1 = __webpack_require__(/*! ./BlankTargetHandler */ "./src/InfiniLore.InfiniFrame.Js/TsSource/BlankTargetHandler.ts");
            const Observers_1 = __webpack_require__(/*! ./Observers */ "./src/InfiniLore.InfiniFrame.Js/TsSource/Observers.ts");

            class HostMessaging {
                constructor() {
                    this.messageHandlers = new Map();
                    this.assignWebMessageReceiver();
                    this.assignMessageReceivedHandler(IHostMessaging_1.ReceiveFromHostMessageIds.registerOpenExternal, _ => {
                        document.addEventListener("click", BlankTargetHandler_1.blankTargetHandler, {capture: true});
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
                            (0, Observers_1.getTitleObserver)().observe(Observers_1.TitleObserverTarget, {childList: true});
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
                    } else if ((_b = window.external) === null || _b === void 0 ? void 0 : _b.sendMessage) {
                        window.external.sendMessage(message);
                    } else {
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
                    } else {
                        messageId = messageStr;
                    }
                    const handler = this.messageHandlers.get(messageId);
                    if (handler) {
                        handler(data);
                    } else {
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


            /***/
        }),

        /***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/Index.ts":
        /*!*********************************************************!*\
          !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/Index.ts ***!
          \*********************************************************/
        /***/ (function (__unused_webpack_module, exports, __webpack_require__) {


            var __importDefault = (this && this.__importDefault) || function (mod) {
                return (mod && mod.__esModule) ? mod : {"default": mod};
            };
            Object.defineProperty(exports, "__esModule", ({value: true}));
            const InfiniWindow_1 = __importDefault(__webpack_require__(/*! ./InfiniWindow */ "./src/InfiniLore.InfiniFrame.Js/TsSource/InfiniWindow.ts"));
            window.infiniWindow = new InfiniWindow_1.default();


            /***/
        }),

        /***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/InfiniWindow.ts":
        /*!****************************************************************!*\
          !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/InfiniWindow.ts ***!
          \****************************************************************/
        /***/ (function (__unused_webpack_module, exports, __webpack_require__) {


            var __importDefault = (this && this.__importDefault) || function (mod) {
                return (mod && mod.__esModule) ? mod : {"default": mod};
            };
            Object.defineProperty(exports, "__esModule", ({value: true}));
            exports.InfiniWindow = void 0;
            const HostMessaging_1 = __importDefault(__webpack_require__(/*! ./HostMessaging */ "./src/InfiniLore.InfiniFrame.Js/TsSource/HostMessaging.ts"));

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


            /***/
        }),

        /***/ "./src/InfiniLore.InfiniFrame.Js/TsSource/Observers.ts":
        /*!*************************************************************!*\
          !*** ./src/InfiniLore.InfiniFrame.Js/TsSource/Observers.ts ***!
          \*************************************************************/
        /***/ ((__unused_webpack_module, exports, __webpack_require__) => {


            Object.defineProperty(exports, "__esModule", ({value: true}));
            exports.TitleObserverTarget = void 0;
            exports.getTitleObserver = getTitleObserver;
            const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.InfiniFrame.Js/TsSource/Contracts/IHostMessaging.ts");
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


            /***/
        })

        /******/
    });
    /************************************************************************/
    /******/ 	// The module cache
    /******/
    var __webpack_module_cache__ = {};
    /******/
    /******/ 	// The require function
    /******/
    function __webpack_require__(moduleId) {
        /******/ 		// Check if module is in cache
        /******/
        var cachedModule = __webpack_module_cache__[moduleId];
        /******/
        if (cachedModule !== undefined) {
            /******/
            return cachedModule.exports;
            /******/
        }
        /******/ 		// Create a new module (and put it into the cache)
        /******/
        var module = __webpack_module_cache__[moduleId] = {
            /******/ 			// no module.id needed
            /******/ 			// no module.loaded needed
            /******/            exports: {}
            /******/
        };
        /******/
        /******/ 		// Execute the module function
        /******/
        __webpack_modules__[moduleId].call(module.exports, module, module.exports, __webpack_require__);
        /******/
        /******/ 		// Return the exports of the module
        /******/
        return module.exports;
        /******/
    }

    /******/
    /************************************************************************/
    /******/
    /******/ 	// startup
    /******/ 	// Load entry module and return exports
    /******/ 	// This entry module is referenced by other modules so it can't be inlined
    /******/
    var __webpack_exports__ = __webpack_require__("./src/InfiniLore.InfiniFrame.Js/TsSource/Index.ts");
    /******/
    /******/
})()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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UUFDckQsT0FBTyxDQUFDLHFCQUFxQixDQUFDLFNBQVMsQ0FBQyxDQUFDO0lBQzdDLENBQUM7Q0FDSjtBQWZELG9DQWVDO0FBRUQscUJBQWUsWUFBWTs7Ozs7Ozs7Ozs7Ozs7QUNoQjNCLDRDQU9DO0FBZEQsdUpBQWdFO0FBS25ELDJCQUFtQixHQUE0QixRQUFRLENBQUMsYUFBYSxDQUFDLE9BQU8sQ0FBQyxDQUFDO0FBRTVGLFNBQWdCLGdCQUFnQjtJQUM1QixPQUFPLElBQUksZ0JBQWdCLENBQUMsQ0FBQyxTQUFTLEVBQUUsQ0FBQyxFQUFFLEVBQUU7UUFDekMsU0FBUyxDQUFDLE9BQU8sQ0FBQyxDQUFDLFFBQVEsRUFBRSxFQUFFO1lBQzNCLElBQUksUUFBUSxDQUFDLElBQUksS0FBSyxXQUFXO2dCQUFFLE9BQU87WUFDMUMsTUFBTSxDQUFDLFlBQVksQ0FBQyxhQUFhLENBQUMsaUJBQWlCLENBQUMscUNBQW9CLENBQUMsV0FBVyxFQUFFLFFBQVEsQ0FBQyxLQUFLLENBQUMsQ0FBQztRQUMxRyxDQUFDLENBQUM7SUFDTixDQUFDLENBQUM7QUFDTixDQUFDOzs7Ozs7O1VDakJEO1VBQ0E7O1VBRUE7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7O1VBRUE7VUFDQTs7VUFFQTtVQUNBO1VBQ0E7Ozs7VUV0QkE7VUFDQTtVQUNBO1VBQ0EiLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lLy4vc3JjL0luZmluaUxvcmUuSW5maW5pRnJhbWUuSnMvVHNTb3VyY2UvQmxhbmtUYXJnZXRIYW5kbGVyLnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvLi9zcmMvSW5maW5pTG9yZS5JbmZpbmlGcmFtZS5Kcy9Uc1NvdXJjZS9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS8uL3NyYy9JbmZpbmlMb3JlLkluZmluaUZyYW1lLkpzL1RzU291cmNlL0hvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS8uL3NyYy9JbmZpbmlMb3JlLkluZmluaUZyYW1lLkpzL1RzU291cmNlL0luZGV4LnRzIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvLi9zcmMvSW5maW5pTG9yZS5JbmZpbmlGcmFtZS5Kcy9Uc1NvdXJjZS9JbmZpbmlXaW5kb3cudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5pbmZpbmlmcmFtZS8uL3NyYy9JbmZpbmlMb3JlLkluZmluaUZyYW1lLkpzL1RzU291cmNlL09ic2VydmVycy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lL3dlYnBhY2svYm9vdHN0cmFwIiwid2VicGFjazovL2luZmluaWxvcmUuaW5maW5pZnJhbWUvd2VicGFjay9iZWZvcmUtc3RhcnR1cCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lL3dlYnBhY2svc3RhcnR1cCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLmluZmluaWZyYW1lL3dlYnBhY2svYWZ0ZXItc3RhcnR1cCJdLCJzb3VyY2VzQ29udGVudCI6WyIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtTZW5kVG9Ib3N0TWVzc2FnZUlkc30gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZnVuY3Rpb24gaXNFeHRlcm5hbExpbmsodXJsOiBzdHJpbmcpOiBib29sZWFuIHtcclxuICAgIHRyeSB7XHJcbiAgICAgICAgcmV0dXJuIG5ldyBVUkwodXJsLCBsb2NhdGlvbi5ocmVmKS5ob3N0bmFtZSAhPT0gbG9jYXRpb24uaG9zdG5hbWU7XHJcbiAgICB9IGNhdGNoIHtcclxuICAgICAgICByZXR1cm4gZmFsc2U7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBhc3luYyBmdW5jdGlvbiBibGFua1RhcmdldEhhbmRsZXIoZTogTW91c2VFdmVudCkge1xyXG4gICAgbGV0IGVsID0gZS50YXJnZXQgYXMgSFRNTEVsZW1lbnQgfCBudWxsO1xyXG5cclxuICAgIHdoaWxlIChlbCAmJiBlbCAhPT0gZG9jdW1lbnQuYm9keSkge1xyXG4gICAgICAgIGlmIChlbC50YWdOYW1lPy50b0xvd2VyQ2FzZSgpICE9PSBcImFcIikge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgYW5jaG9yID0gZWwgYXMgSFRNTEFuY2hvckVsZW1lbnQ7XHJcbiAgICAgICAgaWYgKCFhbmNob3IuaHJlZikge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgY29uc3QgdGFyZ2V0ID0gYW5jaG9yLmdldEF0dHJpYnV0ZShcInRhcmdldFwiKTtcclxuICAgICAgICBpZiAoISh0YXJnZXQgPT09IFwiX2JsYW5rXCIgfHwgYW5jaG9yLmhhc0F0dHJpYnV0ZShcImRhdGEtZXh0ZXJuYWxcIikgfHwgaXNFeHRlcm5hbExpbmsoYW5jaG9yLmhyZWYpKSkge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgZS5wcmV2ZW50RGVmYXVsdCgpO1xyXG4gICAgICAgIHdpbmRvdy5pbmZpbmlXaW5kb3cuSG9zdE1lc3NhZ2luZy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy5vcGVuRXh0ZXJuYWxMaW5rLCBhbmNob3IuaHJlZik7XHJcbiAgICAgICAgcmV0dXJuO1xyXG4gICAgfVxyXG59IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGNvbnN0IFNlbmRUb0hvc3RNZXNzYWdlSWRzID0ge1xyXG4gICAgdGl0bGVDaGFuZ2U6IFwiX19pbmZpbmlXaW5kb3c6dGl0bGU6Y2hhbmdlXCIsXHJcbiAgICBmdWxsc2NyZWVuRW50ZXI6IFwiX19pbmZpbmlXaW5kb3c6ZnVsbHNjcmVlbjplbnRlclwiLFxyXG4gICAgZnVsbHNjcmVlbkV4aXQ6IFwiX19pbmZpbmlXaW5kb3c6ZnVsbHNjcmVlbjpleGl0XCIsXHJcbiAgICBvcGVuRXh0ZXJuYWxMaW5rOiBcIl9faW5maW5pV2luZG93Om9wZW46ZXh0ZXJuYWxcIixcclxuICAgIHdpbmRvd0Nsb3NlOiBcIl9faW5maW5pV2luZG93OndpbmRvdzpjbG9zZVwiLFxyXG59XHJcblxyXG5leHBvcnQgY29uc3QgUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcyA9IHtcclxuICAgIHJlZ2lzdGVyT3BlbkV4dGVybmFsOiBcIl9faW5maW5pV2luZG93OnJlZ2lzdGVyOm9wZW46ZXh0ZXJuYWxcIixcclxuICAgIHJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZTogXCJfX2luZmluaVdpbmRvdzpyZWdpc3RlcjpmdWxsc2NyZWVuOmNoYW5nZVwiLFxyXG4gICAgcmVnaXN0ZXJUaXRsZUNoYW5nZTogXCJfX2luZmluaVdpbmRvdzpyZWdpc3Rlcjp0aXRsZTpjaGFuZ2VcIixcclxuICAgIHJlZ2lzdGVyV2luZG93Q2xvc2U6IFwiX19pbmZpbmlXaW5kb3c6cmVnaXN0ZXI6d2luZG93OmNsb3NlXCIsXHJcbn1cclxuXHJcbmV4cG9ydCB0eXBlIFNlbmRUb0hvc3RNZXNzYWdlSWQgPSB0eXBlb2YgU2VuZFRvSG9zdE1lc3NhZ2VJZHNba2V5b2YgdHlwZW9mIFNlbmRUb0hvc3RNZXNzYWdlSWRzXTtcclxuZXhwb3J0IHR5cGUgTWVzc2FnZUNhbGxiYWNrID0gKGRhdGE/OiBzdHJpbmcpID0+IHZvaWQ7XHJcblxyXG5leHBvcnQgaW50ZXJmYWNlIElIb3N0TWVzc2FnaW5nIHtcclxuICAgIHNlbmRNZXNzYWdlVG9Ib3N0KGlkOiBTZW5kVG9Ib3N0TWVzc2FnZUlkIHwgc3RyaW5nLCBkYXRhPzogc3RyaW5nKTogdm9pZDtcclxuXHJcbiAgICBhc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nLCBjYWxsYmFjazogTWVzc2FnZUNhbGxiYWNrKTogdm9pZDtcclxuXHJcbiAgICB1bnJlZ2lzdGVyTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZyk6IHZvaWQ7XHJcbn0iLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtcclxuICAgIElIb3N0TWVzc2FnaW5nLFxyXG4gICAgTWVzc2FnZUNhbGxiYWNrLFxyXG4gICAgUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcyxcclxuICAgIFNlbmRUb0hvc3RNZXNzYWdlSWQsIFNlbmRUb0hvc3RNZXNzYWdlSWRzXHJcbn0gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcbmltcG9ydCB7YmxhbmtUYXJnZXRIYW5kbGVyfSBmcm9tIFwiLi9CbGFua1RhcmdldEhhbmRsZXJcIjtcclxuaW1wb3J0IHtnZXRUaXRsZU9ic2VydmVyLCBUaXRsZU9ic2VydmVyVGFyZ2V0fSBmcm9tIFwiLi9PYnNlcnZlcnNcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5jbGFzcyBIb3N0TWVzc2FnaW5nIGltcGxlbWVudHMgSUhvc3RNZXNzYWdpbmcge1xyXG4gICAgcHJpdmF0ZSBtZXNzYWdlSGFuZGxlcnM6IE1hcDxzdHJpbmcsIE1lc3NhZ2VDYWxsYmFjaz4gPSBuZXcgTWFwKCk7XHJcblxyXG4gICAgY29uc3RydWN0b3IoKSB7XHJcbiAgICAgICAgdGhpcy5hc3NpZ25XZWJNZXNzYWdlUmVjZWl2ZXIoKTtcclxuXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJPcGVuRXh0ZXJuYWwsIF8gPT4ge1xyXG4gICAgICAgICAgICBkb2N1bWVudC5hZGRFdmVudExpc3RlbmVyKFwiY2xpY2tcIiwgYmxhbmtUYXJnZXRIYW5kbGVyLCB7Y2FwdHVyZTogdHJ1ZX0pO1xyXG4gICAgICAgIH0pXHJcblxyXG4gICAgICAgIHRoaXMuYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLnJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZSwgXyA9PiB7XHJcbiAgICAgICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJmdWxsc2NyZWVuY2hhbmdlXCIsIChfOiBFdmVudCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKGRvY3VtZW50LmZ1bGxzY3JlZW5FbGVtZW50KSB0aGlzLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLmZ1bGxzY3JlZW5FbnRlcik7XHJcbiAgICAgICAgICAgICAgICBlbHNlIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMuZnVsbHNjcmVlbkV4aXQpO1xyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJrZXlkb3duXCIsIGFzeW5jIChlOiBLZXlib2FyZEV2ZW50KSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoZS5rZXkgIT09IFwiRjExXCIpIHJldHVybjtcclxuICAgICAgICAgICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkgYXdhaXQgZG9jdW1lbnQuZXhpdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICAgICAgICAgIGVsc2UgYXdhaXQgZG9jdW1lbnQuYm9keS5yZXF1ZXN0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9KVxyXG5cclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3RlclRpdGxlQ2hhbmdlLCBfID0+IHtcclxuICAgICAgICAgICAgaWYgKFRpdGxlT2JzZXJ2ZXJUYXJnZXQpIGdldFRpdGxlT2JzZXJ2ZXIoKS5vYnNlcnZlKFRpdGxlT2JzZXJ2ZXJUYXJnZXQsIHtjaGlsZExpc3Q6IHRydWV9KTtcclxuICAgICAgICB9KVxyXG5cclxuICAgICAgICB0aGlzLmFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIoUmVjZWl2ZUZyb21Ib3N0TWVzc2FnZUlkcy5yZWdpc3RlcldpbmRvd0Nsb3NlLCBfID0+IHtcclxuICAgICAgICAgICAgd2luZG93LmNsb3NlID0gKCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgdGhpcy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy53aW5kb3dDbG9zZSk7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KVxyXG4gICAgfVxyXG5cclxuICAgIHB1YmxpYyBzZW5kTWVzc2FnZVRvSG9zdChpZDogU2VuZFRvSG9zdE1lc3NhZ2VJZCB8IHN0cmluZywgZGF0YT86IHN0cmluZykge1xyXG4gICAgICAgIGNvbnN0IG1lc3NhZ2UgPSBkYXRhID8gYCR7aWR9OyR7ZGF0YX1gIDogaWQ7XHJcblxyXG4gICAgICAgIC8vIFRPRE8gLSBkZXRlcm1pbmUgbWVzc2FnaW5nIG1ldGhvZHMgZm9yIFBob3Rpbm8uTkVUIGZvciBhbGwgcGxhdGZvcm1zXHJcbiAgICAgICAgaWYgKHdpbmRvdy5jaHJvbWU/LndlYnZpZXcpIHtcclxuICAgICAgICAgICAgd2luZG93LmNocm9tZS53ZWJ2aWV3LnBvc3RNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgIH0gZWxzZSBpZiAod2luZG93LmV4dGVybmFsPy5zZW5kTWVzc2FnZSkge1xyXG4gICAgICAgICAgICB3aW5kb3cuZXh0ZXJuYWwuc2VuZE1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgY29uc29sZS53YXJuKFwiTWVzc2FnZSB0byBob3N0IGZhaWxlZDpcIiwgbWVzc2FnZSk7XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgYXNzaWduV2ViTWVzc2FnZVJlY2VpdmVyKCkge1xyXG4gICAgICAgIC8vIFN0b3JlIHRoZSBvcmlnaW5hbCByZWNlaXZlTWVzc2FnZSBpZiBpdCBleGlzdHMgKGZvciBCbGF6b3IgY29tcGF0aWJpbGl0eSlcclxuICAgICAgICBjb25zdCBvcmlnaW5hbFJlY2VpdmVNZXNzYWdlID0gd2luZG93LmV4dGVybmFsPy5yZWNlaXZlTWVzc2FnZTtcclxuXHJcbiAgICAgICAgLy8gSGFuZGxlIFdlYlZpZXcyIG1lc3NhZ2VzIChXaW5kb3dzKVxyXG4gICAgICAgIGlmICh3aW5kb3cuY2hyb21lPy53ZWJ2aWV3KSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5jaHJvbWUud2Vidmlldy5hZGRFdmVudExpc3RlbmVyKCdtZXNzYWdlJywgKGV2ZW50KSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoIXRoaXMuaXNCbGF6b3JNZXNzYWdlKGV2ZW50LmRhdGEpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgdGhpcy5oYW5kbGVXZWJNZXNzYWdlKGV2ZW50LmRhdGEpO1xyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIEhhbmRsZSBnZW5lcmFsIFBob3Rpbm8gbWVzc2FnZXMgKGNyb3NzLXBsYXRmb3JtKVxyXG4gICAgICAgIGlmICh0eXBlb2Ygd2luZG93ICE9PSAndW5kZWZpbmVkJyAmJiB3aW5kb3cuZXh0ZXJuYWwpIHtcclxuICAgICAgICAgICAgd2luZG93LmV4dGVybmFsLnJlY2VpdmVNZXNzYWdlID0gKG1lc3NhZ2U6IGFueSkgPT4ge1xyXG4gICAgICAgICAgICAgICAgLy8gQ2hlY2sgaWYgaXQncyBhIEJsYXpvciBtZXNzYWdlIGFuZCBwYXNzIGl0IHRvIHRoZSBvcmlnaW5hbCBoYW5kbGVyXHJcbiAgICAgICAgICAgICAgICBpZiAodGhpcy5pc0JsYXpvck1lc3NhZ2UobWVzc2FnZSkpIHtcclxuICAgICAgICAgICAgICAgICAgICBpZiAob3JpZ2luYWxSZWNlaXZlTWVzc2FnZSkge1xyXG4gICAgICAgICAgICAgICAgICAgICAgICBvcmlnaW5hbFJlY2VpdmVNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgICAgLy8gSGFuZGxlIG91ciBjdXN0b20gbWVzc2FnZXNcclxuICAgICAgICAgICAgICAgIHRoaXMuaGFuZGxlV2ViTWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICAgICAgfTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBpc0JsYXpvck1lc3NhZ2UobWVzc2FnZTogYW55KTogYm9vbGVhbiB7XHJcbiAgICAgICAgaWYgKHR5cGVvZiBtZXNzYWdlICE9PSAnc3RyaW5nJykgcmV0dXJuIHRydWU7IC8vIEFzc3VtZSBub24tc3RyaW5nIG1lc3NhZ2VzIGFyZSBCbGF6b3JcclxuXHJcbiAgICAgICAgLy8gQ2hlY2sgZm9yIGNvbW1vbiBCbGF6b3IgbWVzc2FnZSBwYXR0ZXJuc1xyXG4gICAgICAgIHJldHVybiBtZXNzYWdlLnN0YXJ0c1dpdGgoJ19fYnd2OicpXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2Uuc3RhcnRzV2l0aCgnZT0+eycpXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2UuaW5jbHVkZXMoJ0JlZ2luSW52b2tlSlMnKVxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLmluY2x1ZGVzKCdBdHRhY2hUb0RvY3VtZW50JylcclxuICAgICAgICAgICAgfHwgbWVzc2FnZS5pbmNsdWRlcygnUmVuZGVyQmF0Y2gnKVxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLmluY2x1ZGVzKCdCbGF6b3IuJyk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBoYW5kbGVXZWJNZXNzYWdlKG1lc3NhZ2U6IGFueSkge1xyXG4gICAgICAgIC8vIEVuc3VyZSBtZXNzYWdlIGlzIGEgc3RyaW5nXHJcbiAgICAgICAgY29uc3QgbWVzc2FnZVN0ciA9IHR5cGVvZiBtZXNzYWdlID09PSAnc3RyaW5nJyA/IG1lc3NhZ2UgOiBTdHJpbmcobWVzc2FnZSB8fCAnJyk7XHJcblxyXG4gICAgICAgIGlmICghbWVzc2FnZVN0cikge1xyXG4gICAgICAgICAgICBjb25zb2xlLndhcm4oJ1JlY2VpdmVkIGVtcHR5IG9yIGludmFsaWQgbWVzc2FnZScpO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBsZXQgbWVzc2FnZUlkOiBzdHJpbmc7XHJcbiAgICAgICAgbGV0IGRhdGE6IHN0cmluZyB8IHVuZGVmaW5lZDtcclxuXHJcbiAgICAgICAgLy8gUGFyc2UgbWVzc2FnZSAtIGNoZWNrIGlmIGl0IGNvbnRhaW5zIGRhdGEgc2VwYXJhdGVkIGJ5IHNlbWljb2xvblxyXG4gICAgICAgIGlmIChtZXNzYWdlU3RyLmluY2x1ZGVzKCc7JykpIHtcclxuICAgICAgICAgICAgY29uc3QgcGFydHMgPSBtZXNzYWdlU3RyLnNwbGl0KCc7JywgMik7XHJcbiAgICAgICAgICAgIG1lc3NhZ2VJZCA9IHBhcnRzWzBdO1xyXG4gICAgICAgICAgICBkYXRhID0gcGFydHNbMV07XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgbWVzc2FnZUlkID0gbWVzc2FnZVN0cjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIEV4ZWN1dGUgcmVnaXN0ZXJlZCBoYW5kbGVyXHJcbiAgICAgICAgY29uc3QgaGFuZGxlciA9IHRoaXMubWVzc2FnZUhhbmRsZXJzLmdldChtZXNzYWdlSWQpO1xyXG4gICAgICAgIGlmIChoYW5kbGVyKSB7XHJcbiAgICAgICAgICAgIGhhbmRsZXIoZGF0YSk7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgY29uc29sZS53YXJuKCdObyBoYW5kbGVyIHJlZ2lzdGVyZWQgZm9yIG1lc3NhZ2UgSUQ6JywgbWVzc2FnZUlkKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIGFzc2lnbk1lc3NhZ2VSZWNlaXZlZEhhbmRsZXIobWVzc2FnZUlkOiBzdHJpbmcsIGNhbGxiYWNrOiBNZXNzYWdlQ2FsbGJhY2spIHtcclxuICAgICAgICB0aGlzLm1lc3NhZ2VIYW5kbGVycy5zZXQobWVzc2FnZUlkLCBjYWxsYmFjayk7XHJcbiAgICB9XHJcblxyXG4gICAgcHVibGljIHVucmVnaXN0ZXJNZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nKSB7XHJcbiAgICAgICAgdGhpcy5tZXNzYWdlSGFuZGxlcnMuZGVsZXRlKG1lc3NhZ2VJZCk7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBkZWZhdWx0IEhvc3RNZXNzYWdpbmdcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCBJbmZpbmlXaW5kb3cgZnJvbSBcIi4vSW5maW5pV2luZG93XCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQge307XHJcbndpbmRvdy5pbmZpbmlXaW5kb3cgPSBuZXcgSW5maW5pV2luZG93KCk7XHJcbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge0lJbmZpbmlXaW5kb3d9IGZyb20gXCIuL0NvbnRyYWN0cy9JSW5maW5pV2luZG93XCI7XHJcbmltcG9ydCB7SUhvc3RNZXNzYWdpbmcsIFNlbmRUb0hvc3RNZXNzYWdlSWR9IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5pbXBvcnQgSG9zdE1lc3NhZ2luZyBmcm9tIFwiLi9Ib3N0TWVzc2FnaW5nXCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY2xhc3MgSW5maW5pV2luZG93IGltcGxlbWVudHMgSUluZmluaVdpbmRvdyB7XHJcbiAgICBIb3N0TWVzc2FnaW5nOiBJSG9zdE1lc3NhZ2luZyA9IG5ldyBIb3N0TWVzc2FnaW5nKCk7XHJcblxyXG4gICAgLy8gT3ZlcmxvYWQgdG8gbWFrZSBhIGRldidzIGxpZmUgZWFzaWVyIGluc3RlYWQgb2YgaGF2aW5nIHRvIGdvIHRvIHRoZSBIb3N0TWVzc2FnaW5nIGNsYXNzXHJcbiAgICBzZW5kTWVzc2FnZVRvSG9zdChpZDogU2VuZFRvSG9zdE1lc3NhZ2VJZCwgZGF0YT86IHN0cmluZykge1xyXG4gICAgICAgIHRoaXMuSG9zdE1lc3NhZ2luZy5zZW5kTWVzc2FnZVRvSG9zdChpZCwgZGF0YSk7XHJcbiAgICB9XHJcblxyXG4gICAgc2V0UG9pbnRlckNhcHR1cmUoZWxlbWVudDogRWxlbWVudCwgcG9pbnRlcklkOiBudW1iZXIpIHtcclxuICAgICAgICBlbGVtZW50LnNldFBvaW50ZXJDYXB0dXJlKHBvaW50ZXJJZCk7XHJcbiAgICB9XHJcblxyXG4gICAgcmVsZWFzZVBvaW50ZXJDYXB0dXJlKGVsZW1lbnQ6IEVsZW1lbnQsIHBvaW50ZXJJZDogbnVtYmVyKSB7XHJcbiAgICAgICAgZWxlbWVudC5yZWxlYXNlUG9pbnRlckNhcHR1cmUocG9pbnRlcklkKTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGRlZmF1bHQgSW5maW5pV2luZG93IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7U2VuZFRvSG9zdE1lc3NhZ2VJZHN9IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBjb25zdCBUaXRsZU9ic2VydmVyVGFyZ2V0OiBIVE1MVGl0bGVFbGVtZW50IHwgbnVsbCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IoJ3RpdGxlJyk7XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gZ2V0VGl0bGVPYnNlcnZlcigpOiBNdXRhdGlvbk9ic2VydmVyIHtcclxuICAgIHJldHVybiBuZXcgTXV0YXRpb25PYnNlcnZlcigobXV0YXRpb25zLCBfKSA9PiB7XHJcbiAgICAgICAgbXV0YXRpb25zLmZvckVhY2goKG11dGF0aW9uKSA9PiB7XHJcbiAgICAgICAgICAgIGlmIChtdXRhdGlvbi50eXBlICE9PSBcImNoaWxkTGlzdFwiKSByZXR1cm47XHJcbiAgICAgICAgICAgIHdpbmRvdy5pbmZpbmlXaW5kb3cuSG9zdE1lc3NhZ2luZy5zZW5kTWVzc2FnZVRvSG9zdChTZW5kVG9Ib3N0TWVzc2FnZUlkcy50aXRsZUNoYW5nZSwgZG9jdW1lbnQudGl0bGUpO1xyXG4gICAgICAgIH0pXHJcbiAgICB9KVxyXG59IiwiLy8gVGhlIG1vZHVsZSBjYWNoZVxudmFyIF9fd2VicGFja19tb2R1bGVfY2FjaGVfXyA9IHt9O1xuXG4vLyBUaGUgcmVxdWlyZSBmdW5jdGlvblxuZnVuY3Rpb24gX193ZWJwYWNrX3JlcXVpcmVfXyhtb2R1bGVJZCkge1xuXHQvLyBDaGVjayBpZiBtb2R1bGUgaXMgaW4gY2FjaGVcblx0dmFyIGNhY2hlZE1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF07XG5cdGlmIChjYWNoZWRNb2R1bGUgIT09IHVuZGVmaW5lZCkge1xuXHRcdHJldHVybiBjYWNoZWRNb2R1bGUuZXhwb3J0cztcblx0fVxuXHQvLyBDcmVhdGUgYSBuZXcgbW9kdWxlIChhbmQgcHV0IGl0IGludG8gdGhlIGNhY2hlKVxuXHR2YXIgbW9kdWxlID0gX193ZWJwYWNrX21vZHVsZV9jYWNoZV9fW21vZHVsZUlkXSA9IHtcblx0XHQvLyBubyBtb2R1bGUuaWQgbmVlZGVkXG5cdFx0Ly8gbm8gbW9kdWxlLmxvYWRlZCBuZWVkZWRcblx0XHRleHBvcnRzOiB7fVxuXHR9O1xuXG5cdC8vIEV4ZWN1dGUgdGhlIG1vZHVsZSBmdW5jdGlvblxuXHRfX3dlYnBhY2tfbW9kdWxlc19fW21vZHVsZUlkXS5jYWxsKG1vZHVsZS5leHBvcnRzLCBtb2R1bGUsIG1vZHVsZS5leHBvcnRzLCBfX3dlYnBhY2tfcmVxdWlyZV9fKTtcblxuXHQvLyBSZXR1cm4gdGhlIGV4cG9ydHMgb2YgdGhlIG1vZHVsZVxuXHRyZXR1cm4gbW9kdWxlLmV4cG9ydHM7XG59XG5cbiIsIiIsIi8vIHN0YXJ0dXBcbi8vIExvYWQgZW50cnkgbW9kdWxlIGFuZCByZXR1cm4gZXhwb3J0c1xuLy8gVGhpcyBlbnRyeSBtb2R1bGUgaXMgcmVmZXJlbmNlZCBieSBvdGhlciBtb2R1bGVzIHNvIGl0IGNhbid0IGJlIGlubGluZWRcbnZhciBfX3dlYnBhY2tfZXhwb3J0c19fID0gX193ZWJwYWNrX3JlcXVpcmVfXyhcIi4vc3JjL0luZmluaUxvcmUuSW5maW5pRnJhbWUuSnMvVHNTb3VyY2UvSW5kZXgudHNcIik7XG4iLCIiXSwibmFtZXMiOltdLCJzb3VyY2VSb290IjoiIn0=