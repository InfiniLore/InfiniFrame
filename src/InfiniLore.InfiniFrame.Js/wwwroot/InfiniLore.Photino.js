/******/
(() => { // webpackBootstrap
    /******/
    "use strict";
    /******/
    var __webpack_modules__ = ({

        /***/ "./src/InfiniLore.Photino.Js/TsSource/BlankTargetHandler.ts":
        /*!******************************************************************!*\
          !*** ./src/InfiniLore.Photino.Js/TsSource/BlankTargetHandler.ts ***!
          \******************************************************************/
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
            const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.Photino.Js/TsSource/Contracts/IHostMessaging.ts");

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

        /***/ "./src/InfiniLore.Photino.Js/TsSource/Contracts/IHostMessaging.ts":
        /*!************************************************************************!*\
          !*** ./src/InfiniLore.Photino.Js/TsSource/Contracts/IHostMessaging.ts ***!
          \************************************************************************/
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

        /***/ "./src/InfiniLore.Photino.Js/TsSource/HostMessaging.ts":
        /*!*************************************************************!*\
          !*** ./src/InfiniLore.Photino.Js/TsSource/HostMessaging.ts ***!
          \*************************************************************/
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
            const IHostMessaging_1 = __webpack_require__(/*! ./Contracts/IHostMessaging */ "./src/InfiniLore.Photino.Js/TsSource/Contracts/IHostMessaging.ts");
            const BlankTargetHandler_1 = __webpack_require__(/*! ./BlankTargetHandler */ "./src/InfiniLore.Photino.Js/TsSource/BlankTargetHandler.ts");
            const Observers_1 = __webpack_require__(/*! ./Observers */ "./src/InfiniLore.Photino.Js/TsSource/Observers.ts");

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

        /***/ "./src/InfiniLore.Photino.Js/TsSource/Index.ts":
        /*!*****************************************************!*\
          !*** ./src/InfiniLore.Photino.Js/TsSource/Index.ts ***!
          \*****************************************************/
        /***/ (function (__unused_webpack_module, exports, __webpack_require__) {


            var __importDefault = (this && this.__importDefault) || function (mod) {
                return (mod && mod.__esModule) ? mod : {"default": mod};
            };
            Object.defineProperty(exports, "__esModule", ({value: true}));
            const InfiniWindow_1 = __importDefault(__webpack_require__(/*! ./InfiniWindow */ "./src/InfiniLore.Photino.Js/TsSource/InfiniWindow.ts"));
            window.infiniWindow = new InfiniWindow_1.default();


            /***/
        }),

        /***/ "./src/InfiniLore.Photino.Js/TsSource/InfiniWindow.ts":
        /*!************************************************************!*\
          !*** ./src/InfiniLore.Photino.Js/TsSource/InfiniWindow.ts ***!
          \************************************************************/
        /***/ (function (__unused_webpack_module, exports, __webpack_require__) {


            var __importDefault = (this && this.__importDefault) || function (mod) {
                return (mod && mod.__esModule) ? mod : {"default": mod};
            };
            Object.defineProperty(exports, "__esModule", ({value: true}));
            exports.InfiniWindow = void 0;
            const HostMessaging_1 = __importDefault(__webpack_require__(/*! ./HostMessaging */ "./src/InfiniLore.Photino.Js/TsSource/HostMessaging.ts"));

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

        /***/ "./src/InfiniLore.Photino.Js/TsSource/Observers.ts":
        /*!*********************************************************!*\
          !*** ./src/InfiniLore.Photino.Js/TsSource/Observers.ts ***!
          \*********************************************************/
        /***/ ((__unused_webpack_module, exports, __webpack_require__) => {


            Object.defineProperty(exports, "__esModule", ({value: true}));
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
    var __webpack_exports__ = __webpack_require__("./src/InfiniLore.Photino.Js/TsSource/Index.ts");
    /******/
    /******/
})()
;
//# sourceMappingURL=data:application/json;charset=utf-8;base64,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T0FBTyxDQUFDLHFCQUFxQixDQUFDLFNBQVMsQ0FBQyxDQUFDO0lBQzdDLENBQUM7Q0FDSjtBQWZELG9DQWVDO0FBRUQscUJBQWUsWUFBWTs7Ozs7Ozs7Ozs7Ozs7QUNoQjNCLDRDQU9DO0FBZEQsbUpBQWdFO0FBS25ELDJCQUFtQixHQUE2QixRQUFRLENBQUMsYUFBYSxDQUFDLE9BQU8sQ0FBQyxDQUFDO0FBRTdGLFNBQWdCLGdCQUFnQjtJQUM1QixPQUFPLElBQUksZ0JBQWdCLENBQUMsQ0FBQyxTQUFTLEVBQUUsQ0FBQyxFQUFFLEVBQUU7UUFDekMsU0FBUyxDQUFDLE9BQU8sQ0FBQyxDQUFDLFFBQVEsRUFBRSxFQUFFO1lBQzNCLElBQUksUUFBUSxDQUFDLElBQUksS0FBSyxXQUFXO2dCQUFFLE9BQU87WUFDMUMsTUFBTSxDQUFDLFlBQVksQ0FBQyxhQUFhLENBQUMsaUJBQWlCLENBQUMscUNBQW9CLENBQUMsV0FBVyxFQUFFLFFBQVEsQ0FBQyxLQUFLLENBQUMsQ0FBQztRQUMxRyxDQUFDLENBQUM7SUFDTixDQUFDLENBQUM7QUFDTixDQUFDOzs7Ozs7O1VDakJEO1VBQ0E7O1VBRUE7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7VUFDQTtVQUNBO1VBQ0E7O1VBRUE7VUFDQTs7VUFFQTtVQUNBO1VBQ0E7Ozs7VUV0QkE7VUFDQTtVQUNBO1VBQ0EiLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL0JsYW5rVGFyZ2V0SGFuZGxlci50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL0hvc3RNZXNzYWdpbmcudHMiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vLy4vc3JjL0luZmluaUxvcmUuUGhvdGluby5Kcy9Uc1NvdXJjZS9JbmRleC50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL0luZmluaVdpbmRvdy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vLi9zcmMvSW5maW5pTG9yZS5QaG90aW5vLkpzL1RzU291cmNlL09ic2VydmVycy50cyIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vd2VicGFjay9ib290c3RyYXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svYmVmb3JlLXN0YXJ0dXAiLCJ3ZWJwYWNrOi8vaW5maW5pbG9yZS5waG90aW5vL3dlYnBhY2svc3RhcnR1cCIsIndlYnBhY2s6Ly9pbmZpbmlsb3JlLnBob3Rpbm8vd2VicGFjay9hZnRlci1zdGFydHVwIl0sInNvdXJjZXNDb250ZW50IjpbIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge1NlbmRUb0hvc3RNZXNzYWdlSWRzfSBmcm9tIFwiLi9Db250cmFjdHMvSUhvc3RNZXNzYWdpbmdcIjtcclxuXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5mdW5jdGlvbiBpc0V4dGVybmFsTGluayh1cmw6IHN0cmluZyk6IGJvb2xlYW4ge1xyXG4gICAgdHJ5IHtcclxuICAgICAgICByZXR1cm4gbmV3IFVSTCh1cmwsIGxvY2F0aW9uLmhyZWYpLmhvc3RuYW1lICE9PSBsb2NhdGlvbi5ob3N0bmFtZTtcclxuICAgIH0gY2F0Y2gge1xyXG4gICAgICAgIHJldHVybiBmYWxzZTtcclxuICAgIH1cclxufVxyXG5cclxuZXhwb3J0IGFzeW5jIGZ1bmN0aW9uIGJsYW5rVGFyZ2V0SGFuZGxlcihlOiBNb3VzZUV2ZW50KSB7XHJcbiAgICBsZXQgZWwgPSBlLnRhcmdldCBhcyBIVE1MRWxlbWVudCB8IG51bGw7XHJcbiAgICBcclxuICAgIHdoaWxlIChlbCAmJiBlbCAhPT0gZG9jdW1lbnQuYm9keSkge1xyXG4gICAgICAgIGlmIChlbC50YWdOYW1lPy50b0xvd2VyQ2FzZSgpICE9PSBcImFcIikge1xyXG4gICAgICAgICAgICBlbCA9IGVsLnBhcmVudEVsZW1lbnQ7XHJcbiAgICAgICAgICAgIGNvbnRpbnVlO1xyXG4gICAgICAgIH1cclxuICAgICAgICBcclxuICAgICAgICBjb25zdCBhbmNob3IgPSBlbCBhcyBIVE1MQW5jaG9yRWxlbWVudDtcclxuICAgICAgICBpZiAoIWFuY2hvci5ocmVmKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBjb25zdCB0YXJnZXQgPSBhbmNob3IuZ2V0QXR0cmlidXRlKFwidGFyZ2V0XCIpO1xyXG4gICAgICAgIGlmICghKHRhcmdldCA9PT0gXCJfYmxhbmtcIiB8fCBhbmNob3IuaGFzQXR0cmlidXRlKFwiZGF0YS1leHRlcm5hbFwiKSB8fCBpc0V4dGVybmFsTGluayhhbmNob3IuaHJlZikpKSB7XHJcbiAgICAgICAgICAgIGVsID0gZWwucGFyZW50RWxlbWVudDtcclxuICAgICAgICAgICAgY29udGludWU7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIFxyXG4gICAgICAgIGUucHJldmVudERlZmF1bHQoKTtcclxuICAgICAgICB3aW5kb3cuaW5maW5pV2luZG93Lkhvc3RNZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMub3BlbkV4dGVybmFsTGluaywgYW5jaG9yLmhyZWYpO1xyXG4gICAgICAgIHJldHVybjtcclxuICAgIH1cclxufSIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5cclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIENvZGVcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmV4cG9ydCBjb25zdCBTZW5kVG9Ib3N0TWVzc2FnZUlkcyA9IHtcclxuICAgIHRpdGxlQ2hhbmdlOiBcIl9faW5maW5pV2luZG93OnRpdGxlOmNoYW5nZVwiLFxyXG4gICAgZnVsbHNjcmVlbkVudGVyOiBcIl9faW5maW5pV2luZG93OmZ1bGxzY3JlZW46ZW50ZXJcIixcclxuICAgIGZ1bGxzY3JlZW5FeGl0OiBcIl9faW5maW5pV2luZG93OmZ1bGxzY3JlZW46ZXhpdFwiLFxyXG4gICAgb3BlbkV4dGVybmFsTGluazogXCJfX2luZmluaVdpbmRvdzpvcGVuOmV4dGVybmFsXCIsXHJcbiAgICB3aW5kb3dDbG9zZTogXCJfX2luZmluaVdpbmRvdzp3aW5kb3c6Y2xvc2VcIixcclxufVxyXG5cclxuZXhwb3J0IGNvbnN0IFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMgPSB7XHJcbiAgICByZWdpc3Rlck9wZW5FeHRlcm5hbDogXCJfX2luZmluaVdpbmRvdzpyZWdpc3RlcjpvcGVuOmV4dGVybmFsXCIsXHJcbiAgICByZWdpc3RlckZ1bGxzY3JlZW5DaGFuZ2U6IFwiX19pbmZpbmlXaW5kb3c6cmVnaXN0ZXI6ZnVsbHNjcmVlbjpjaGFuZ2VcIixcclxuICAgIHJlZ2lzdGVyVGl0bGVDaGFuZ2U6IFwiX19pbmZpbmlXaW5kb3c6cmVnaXN0ZXI6dGl0bGU6Y2hhbmdlXCIsXHJcbiAgICByZWdpc3RlcldpbmRvd0Nsb3NlOiBcIl9faW5maW5pV2luZG93OnJlZ2lzdGVyOndpbmRvdzpjbG9zZVwiLFxyXG59XHJcblxyXG5leHBvcnQgdHlwZSBTZW5kVG9Ib3N0TWVzc2FnZUlkID0gdHlwZW9mIFNlbmRUb0hvc3RNZXNzYWdlSWRzW2tleW9mIHR5cGVvZiBTZW5kVG9Ib3N0TWVzc2FnZUlkc107XHJcbmV4cG9ydCB0eXBlIE1lc3NhZ2VDYWxsYmFjayA9IChkYXRhPzogc3RyaW5nKSA9PiB2b2lkO1xyXG5cclxuZXhwb3J0IGludGVyZmFjZSBJSG9zdE1lc3NhZ2luZyB7XHJcbiAgICBzZW5kTWVzc2FnZVRvSG9zdChpZDogU2VuZFRvSG9zdE1lc3NhZ2VJZCB8IHN0cmluZywgZGF0YT86IHN0cmluZyk6IHZvaWQ7XHJcbiAgICBhc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDpzdHJpbmcsIGNhbGxiYWNrOk1lc3NhZ2VDYWxsYmFjayk6IHZvaWQ7XHJcbiAgICB1bnJlZ2lzdGVyTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihtZXNzYWdlSWQ6IHN0cmluZykgOiB2b2lkO1xyXG59IiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCB7XHJcbiAgICBJSG9zdE1lc3NhZ2luZyxcclxuICAgIE1lc3NhZ2VDYWxsYmFjayxcclxuICAgIFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMsXHJcbiAgICBTZW5kVG9Ib3N0TWVzc2FnZUlkLCBTZW5kVG9Ib3N0TWVzc2FnZUlkc1xyXG59IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5pbXBvcnQge2JsYW5rVGFyZ2V0SGFuZGxlcn0gZnJvbSBcIi4vQmxhbmtUYXJnZXRIYW5kbGVyXCI7XHJcbmltcG9ydCB7Z2V0VGl0bGVPYnNlcnZlciwgVGl0bGVPYnNlcnZlclRhcmdldH0gZnJvbSBcIi4vT2JzZXJ2ZXJzXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuY2xhc3MgSG9zdE1lc3NhZ2luZyBpbXBsZW1lbnRzIElIb3N0TWVzc2FnaW5nIHtcclxuICAgIHByaXZhdGUgbWVzc2FnZUhhbmRsZXJzOiBNYXA8c3RyaW5nLCBNZXNzYWdlQ2FsbGJhY2s+ID0gbmV3IE1hcCgpO1xyXG4gICAgXHJcbiAgICBjb25zdHJ1Y3RvcigpIHtcclxuICAgICAgICB0aGlzLmFzc2lnbldlYk1lc3NhZ2VSZWNlaXZlcigpO1xyXG4gICAgICAgIFxyXG4gICAgICAgIHRoaXMuYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLnJlZ2lzdGVyT3BlbkV4dGVybmFsLCBfID0+IHtcclxuICAgICAgICAgICAgZG9jdW1lbnQuYWRkRXZlbnRMaXN0ZW5lciggXCJjbGlja1wiLCBibGFua1RhcmdldEhhbmRsZXIsIHsgY2FwdHVyZTogdHJ1ZSB9KTtcclxuICAgICAgICB9KVxyXG4gICAgICAgIFxyXG4gICAgICAgIHRoaXMuYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLnJlZ2lzdGVyRnVsbHNjcmVlbkNoYW5nZSwgXyA9PiB7XHJcbiAgICAgICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJmdWxsc2NyZWVuY2hhbmdlXCIsIChfOiBFdmVudCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKGRvY3VtZW50LmZ1bGxzY3JlZW5FbGVtZW50KSB0aGlzLnNlbmRNZXNzYWdlVG9Ib3N0KFNlbmRUb0hvc3RNZXNzYWdlSWRzLmZ1bGxzY3JlZW5FbnRlcik7XHJcbiAgICAgICAgICAgICAgICBlbHNlIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMuZnVsbHNjcmVlbkV4aXQpO1xyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIGRvY3VtZW50LmFkZEV2ZW50TGlzdGVuZXIoXCJrZXlkb3duXCIsIGFzeW5jIChlOiBLZXlib2FyZEV2ZW50KSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoZS5rZXkgIT09IFwiRjExXCIpIHJldHVybjtcclxuICAgICAgICAgICAgICAgIGlmIChkb2N1bWVudC5mdWxsc2NyZWVuRWxlbWVudCkgYXdhaXQgZG9jdW1lbnQuZXhpdEZ1bGxzY3JlZW4oKTtcclxuICAgICAgICAgICAgICAgIGVsc2UgYXdhaXQgZG9jdW1lbnQuYm9keS5yZXF1ZXN0RnVsbHNjcmVlbigpO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9KVxyXG4gICAgICAgIFxyXG4gICAgICAgIHRoaXMuYXNzaWduTWVzc2FnZVJlY2VpdmVkSGFuZGxlcihSZWNlaXZlRnJvbUhvc3RNZXNzYWdlSWRzLnJlZ2lzdGVyVGl0bGVDaGFuZ2UsIF8gPT4ge1xyXG4gICAgICAgICAgICBpZiAoVGl0bGVPYnNlcnZlclRhcmdldCkgZ2V0VGl0bGVPYnNlcnZlcigpLm9ic2VydmUoVGl0bGVPYnNlcnZlclRhcmdldCwge2NoaWxkTGlzdDogdHJ1ZX0pO1xyXG4gICAgICAgIH0pXHJcbiAgICAgICAgXHJcbiAgICAgICAgdGhpcy5hc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKFJlY2VpdmVGcm9tSG9zdE1lc3NhZ2VJZHMucmVnaXN0ZXJXaW5kb3dDbG9zZSwgXyA9PiB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5jbG9zZSA9ICgpID0+IHtcclxuICAgICAgICAgICAgICAgIHRoaXMuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMud2luZG93Q2xvc2UpO1xyXG4gICAgICAgICAgICB9IFxyXG4gICAgICAgIH0pXHJcbiAgICB9XHJcbiAgICAgICAgXHJcbiAgICBwdWJsaWMgc2VuZE1lc3NhZ2VUb0hvc3QoaWQ6IFNlbmRUb0hvc3RNZXNzYWdlSWQgfCBzdHJpbmcsIGRhdGE/OiBzdHJpbmcpIHtcclxuICAgICAgICBjb25zdCBtZXNzYWdlID0gZGF0YSA/IGAke2lkfTske2RhdGF9YCA6IGlkO1xyXG5cclxuICAgICAgICAvLyBUT0RPIC0gZGV0ZXJtaW5lIG1lc3NhZ2luZyBtZXRob2RzIGZvciBQaG90aW5vLk5FVCBmb3IgYWxsIHBsYXRmb3Jtc1xyXG4gICAgICAgIGlmICh3aW5kb3cuY2hyb21lPy53ZWJ2aWV3KSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5jaHJvbWUud2Vidmlldy5wb3N0TWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICB9IGVsc2UgaWYgKHdpbmRvdy5leHRlcm5hbD8uc2VuZE1lc3NhZ2UpIHtcclxuICAgICAgICAgICAgd2luZG93LmV4dGVybmFsLnNlbmRNZXNzYWdlKG1lc3NhZ2UpO1xyXG4gICAgICAgIH0gZWxzZSB7XHJcbiAgICAgICAgICAgIGNvbnNvbGUud2FybihcIk1lc3NhZ2UgdG8gaG9zdCBmYWlsZWQ6XCIsIG1lc3NhZ2UpO1xyXG4gICAgICAgIH1cclxuICAgIH1cclxuXHJcbiAgICBwcml2YXRlIGFzc2lnbldlYk1lc3NhZ2VSZWNlaXZlcigpIHtcclxuICAgICAgICAvLyBTdG9yZSB0aGUgb3JpZ2luYWwgcmVjZWl2ZU1lc3NhZ2UgaWYgaXQgZXhpc3RzIChmb3IgQmxhem9yIGNvbXBhdGliaWxpdHkpXHJcbiAgICAgICAgY29uc3Qgb3JpZ2luYWxSZWNlaXZlTWVzc2FnZSA9IHdpbmRvdy5leHRlcm5hbD8ucmVjZWl2ZU1lc3NhZ2U7XHJcblxyXG4gICAgICAgIC8vIEhhbmRsZSBXZWJWaWV3MiBtZXNzYWdlcyAoV2luZG93cylcclxuICAgICAgICBpZiAod2luZG93LmNocm9tZT8ud2Vidmlldykge1xyXG4gICAgICAgICAgICB3aW5kb3cuY2hyb21lLndlYnZpZXcuYWRkRXZlbnRMaXN0ZW5lcignbWVzc2FnZScsIChldmVudCkgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKCF0aGlzLmlzQmxhem9yTWVzc2FnZShldmVudC5kYXRhKSkge1xyXG4gICAgICAgICAgICAgICAgICAgIHRoaXMuaGFuZGxlV2ViTWVzc2FnZShldmVudC5kYXRhKTtcclxuICAgICAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICAvLyBIYW5kbGUgZ2VuZXJhbCBQaG90aW5vIG1lc3NhZ2VzIChjcm9zcy1wbGF0Zm9ybSlcclxuICAgICAgICBpZiAodHlwZW9mIHdpbmRvdyAhPT0gJ3VuZGVmaW5lZCcgJiYgd2luZG93LmV4dGVybmFsKSB7XHJcbiAgICAgICAgICAgIHdpbmRvdy5leHRlcm5hbC5yZWNlaXZlTWVzc2FnZSA9IChtZXNzYWdlOiBhbnkpID0+IHtcclxuICAgICAgICAgICAgICAgIC8vIENoZWNrIGlmIGl0J3MgYSBCbGF6b3IgbWVzc2FnZSBhbmQgcGFzcyBpdCB0byB0aGUgb3JpZ2luYWwgaGFuZGxlclxyXG4gICAgICAgICAgICAgICAgaWYgKHRoaXMuaXNCbGF6b3JNZXNzYWdlKG1lc3NhZ2UpKSB7XHJcbiAgICAgICAgICAgICAgICAgICAgaWYgKG9yaWdpbmFsUmVjZWl2ZU1lc3NhZ2UpIHtcclxuICAgICAgICAgICAgICAgICAgICAgICAgb3JpZ2luYWxSZWNlaXZlTWVzc2FnZShtZXNzYWdlKTtcclxuICAgICAgICAgICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICAgICAgICAgIC8vIEhhbmRsZSBvdXIgY3VzdG9tIG1lc3NhZ2VzXHJcbiAgICAgICAgICAgICAgICB0aGlzLmhhbmRsZVdlYk1lc3NhZ2UobWVzc2FnZSk7XHJcbiAgICAgICAgICAgIH07XHJcbiAgICAgICAgfVxyXG4gICAgfVxyXG5cclxuICAgIHByaXZhdGUgaXNCbGF6b3JNZXNzYWdlKG1lc3NhZ2U6IGFueSk6IGJvb2xlYW4ge1xyXG4gICAgICAgIGlmICh0eXBlb2YgbWVzc2FnZSAhPT0gJ3N0cmluZycpIHJldHVybiB0cnVlOyAvLyBBc3N1bWUgbm9uLXN0cmluZyBtZXNzYWdlcyBhcmUgQmxhem9yXHJcblxyXG4gICAgICAgIC8vIENoZWNrIGZvciBjb21tb24gQmxhem9yIG1lc3NhZ2UgcGF0dGVybnNcclxuICAgICAgICByZXR1cm4gbWVzc2FnZS5zdGFydHNXaXRoKCdfX2J3djonKSBcclxuICAgICAgICAgICAgfHwgbWVzc2FnZS5zdGFydHNXaXRoKCdlPT57JykgXHJcbiAgICAgICAgICAgIHx8IG1lc3NhZ2UuaW5jbHVkZXMoJ0JlZ2luSW52b2tlSlMnKSBcclxuICAgICAgICAgICAgfHwgbWVzc2FnZS5pbmNsdWRlcygnQXR0YWNoVG9Eb2N1bWVudCcpIFxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLmluY2x1ZGVzKCdSZW5kZXJCYXRjaCcpIFxyXG4gICAgICAgICAgICB8fCBtZXNzYWdlLmluY2x1ZGVzKCdCbGF6b3IuJyk7XHJcbiAgICB9XHJcblxyXG4gICAgcHJpdmF0ZSBoYW5kbGVXZWJNZXNzYWdlKG1lc3NhZ2U6IGFueSkge1xyXG4gICAgICAgIC8vIEVuc3VyZSBtZXNzYWdlIGlzIGEgc3RyaW5nXHJcbiAgICAgICAgY29uc3QgbWVzc2FnZVN0ciA9IHR5cGVvZiBtZXNzYWdlID09PSAnc3RyaW5nJyA/IG1lc3NhZ2UgOiBTdHJpbmcobWVzc2FnZSB8fCAnJyk7XHJcblxyXG4gICAgICAgIGlmICghbWVzc2FnZVN0cikge1xyXG4gICAgICAgICAgICBjb25zb2xlLndhcm4oJ1JlY2VpdmVkIGVtcHR5IG9yIGludmFsaWQgbWVzc2FnZScpO1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBsZXQgbWVzc2FnZUlkOiBzdHJpbmc7XHJcbiAgICAgICAgbGV0IGRhdGE6IHN0cmluZyB8IHVuZGVmaW5lZDtcclxuXHJcbiAgICAgICAgLy8gUGFyc2UgbWVzc2FnZSAtIGNoZWNrIGlmIGl0IGNvbnRhaW5zIGRhdGEgc2VwYXJhdGVkIGJ5IHNlbWljb2xvblxyXG4gICAgICAgIGlmIChtZXNzYWdlU3RyLmluY2x1ZGVzKCc7JykpIHtcclxuICAgICAgICAgICAgY29uc3QgcGFydHMgPSBtZXNzYWdlU3RyLnNwbGl0KCc7JywgMik7XHJcbiAgICAgICAgICAgIG1lc3NhZ2VJZCA9IHBhcnRzWzBdO1xyXG4gICAgICAgICAgICBkYXRhID0gcGFydHNbMV07XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgbWVzc2FnZUlkID0gbWVzc2FnZVN0cjtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIC8vIEV4ZWN1dGUgcmVnaXN0ZXJlZCBoYW5kbGVyXHJcbiAgICAgICAgY29uc3QgaGFuZGxlciA9IHRoaXMubWVzc2FnZUhhbmRsZXJzLmdldChtZXNzYWdlSWQpO1xyXG4gICAgICAgIGlmIChoYW5kbGVyKSB7XHJcbiAgICAgICAgICAgIGhhbmRsZXIoZGF0YSk7XHJcbiAgICAgICAgfSBlbHNlIHtcclxuICAgICAgICAgICAgY29uc29sZS53YXJuKCdObyBoYW5kbGVyIHJlZ2lzdGVyZWQgZm9yIG1lc3NhZ2UgSUQ6JywgbWVzc2FnZUlkKTtcclxuICAgICAgICB9XHJcbiAgICB9XHJcbiAgICBcclxuICAgIHB1YmxpYyBhc3NpZ25NZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDpzdHJpbmcsIGNhbGxiYWNrOk1lc3NhZ2VDYWxsYmFjaykge1xyXG4gICAgICAgIHRoaXMubWVzc2FnZUhhbmRsZXJzLnNldChtZXNzYWdlSWQsIGNhbGxiYWNrKTtcclxuICAgIH1cclxuICAgIFxyXG4gICAgcHVibGljIHVucmVnaXN0ZXJNZXNzYWdlUmVjZWl2ZWRIYW5kbGVyKG1lc3NhZ2VJZDogc3RyaW5nKSB7XHJcbiAgICAgICAgdGhpcy5tZXNzYWdlSGFuZGxlcnMuZGVsZXRlKG1lc3NhZ2VJZCk7XHJcbiAgICB9XHJcbn1cclxuXHJcbmV4cG9ydCBkZWZhdWx0IEhvc3RNZXNzYWdpbmdcclxuIiwiLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vIEltcG9ydHNcclxuLy8gLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbmltcG9ydCBJbmZpbmlXaW5kb3cgZnJvbSBcIi4vSW5maW5pV2luZG93XCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQge307XHJcbndpbmRvdy5pbmZpbmlXaW5kb3cgPSBuZXcgSW5maW5pV2luZG93KCk7XHJcbiIsIi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBJbXBvcnRzXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5pbXBvcnQge0lJbmZpbmlXaW5kb3d9IGZyb20gXCIuL0NvbnRyYWN0cy9JSW5maW5pV2luZG93XCI7XHJcbmltcG9ydCB7SUhvc3RNZXNzYWdpbmcsIFNlbmRUb0hvc3RNZXNzYWdlSWR9IGZyb20gXCIuL0NvbnRyYWN0cy9JSG9zdE1lc3NhZ2luZ1wiO1xyXG5pbXBvcnQgSG9zdE1lc3NhZ2luZyBmcm9tIFwiLi9Ib3N0TWVzc2FnaW5nXCI7XHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyBDb2RlXHJcbi8vIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG5leHBvcnQgY2xhc3MgSW5maW5pV2luZG93IGltcGxlbWVudHMgSUluZmluaVdpbmRvdyB7XHJcbiAgICBIb3N0TWVzc2FnaW5nIDogSUhvc3RNZXNzYWdpbmcgPSBuZXcgSG9zdE1lc3NhZ2luZygpO1xyXG5cclxuICAgIC8vIE92ZXJsb2FkIHRvIG1ha2UgYSBkZXYncyBsaWZlIGVhc2llciBpbnN0ZWFkIG9mIGhhdmluZyB0byBnbyB0byB0aGUgSG9zdE1lc3NhZ2luZyBjbGFzc1xyXG4gICAgc2VuZE1lc3NhZ2VUb0hvc3QoaWQ6IFNlbmRUb0hvc3RNZXNzYWdlSWQsIGRhdGE/OiBzdHJpbmcpIHtcclxuICAgICAgICB0aGlzLkhvc3RNZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoaWQsIGRhdGEpO1xyXG4gICAgfVxyXG4gICAgXHJcbiAgICBzZXRQb2ludGVyQ2FwdHVyZShlbGVtZW50OkVsZW1lbnQsIHBvaW50ZXJJZDogbnVtYmVyKSB7XHJcbiAgICAgICAgZWxlbWVudC5zZXRQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG4gICAgfVxyXG5cclxuICAgIHJlbGVhc2VQb2ludGVyQ2FwdHVyZShlbGVtZW50OkVsZW1lbnQsIHBvaW50ZXJJZDpudW1iZXIpIHtcclxuICAgICAgICBlbGVtZW50LnJlbGVhc2VQb2ludGVyQ2FwdHVyZShwb2ludGVySWQpO1xyXG4gICAgfVxyXG59XHJcblxyXG5leHBvcnQgZGVmYXVsdCBJbmZpbmlXaW5kb3ciLCIvLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gSW1wb3J0c1xyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuaW1wb3J0IHtTZW5kVG9Ib3N0TWVzc2FnZUlkc30gZnJvbSBcIi4vQ29udHJhY3RzL0lIb3N0TWVzc2FnaW5nXCI7XHJcblxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gQ29kZVxyXG4vLyAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuZXhwb3J0IGNvbnN0IFRpdGxlT2JzZXJ2ZXJUYXJnZXQgOiBIVE1MVGl0bGVFbGVtZW50IHwgbnVsbCA9IGRvY3VtZW50LnF1ZXJ5U2VsZWN0b3IoJ3RpdGxlJyk7XHJcblxyXG5leHBvcnQgZnVuY3Rpb24gZ2V0VGl0bGVPYnNlcnZlcigpIDogTXV0YXRpb25PYnNlcnZlciB7XHJcbiAgICByZXR1cm4gbmV3IE11dGF0aW9uT2JzZXJ2ZXIoKG11dGF0aW9ucywgXykgPT4ge1xyXG4gICAgICAgIG11dGF0aW9ucy5mb3JFYWNoKChtdXRhdGlvbikgPT4ge1xyXG4gICAgICAgICAgICBpZiAobXV0YXRpb24udHlwZSAhPT0gXCJjaGlsZExpc3RcIikgcmV0dXJuO1xyXG4gICAgICAgICAgICB3aW5kb3cuaW5maW5pV2luZG93Lkhvc3RNZXNzYWdpbmcuc2VuZE1lc3NhZ2VUb0hvc3QoU2VuZFRvSG9zdE1lc3NhZ2VJZHMudGl0bGVDaGFuZ2UsIGRvY3VtZW50LnRpdGxlKTtcclxuICAgICAgICB9KVxyXG4gICAgfSlcclxufSIsIi8vIFRoZSBtb2R1bGUgY2FjaGVcbnZhciBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX18gPSB7fTtcblxuLy8gVGhlIHJlcXVpcmUgZnVuY3Rpb25cbmZ1bmN0aW9uIF9fd2VicGFja19yZXF1aXJlX18obW9kdWxlSWQpIHtcblx0Ly8gQ2hlY2sgaWYgbW9kdWxlIGlzIGluIGNhY2hlXG5cdHZhciBjYWNoZWRNb2R1bGUgPSBfX3dlYnBhY2tfbW9kdWxlX2NhY2hlX19bbW9kdWxlSWRdO1xuXHRpZiAoY2FjaGVkTW9kdWxlICE9PSB1bmRlZmluZWQpIHtcblx0XHRyZXR1cm4gY2FjaGVkTW9kdWxlLmV4cG9ydHM7XG5cdH1cblx0Ly8gQ3JlYXRlIGEgbmV3IG1vZHVsZSAoYW5kIHB1dCBpdCBpbnRvIHRoZSBjYWNoZSlcblx0dmFyIG1vZHVsZSA9IF9fd2VicGFja19tb2R1bGVfY2FjaGVfX1ttb2R1bGVJZF0gPSB7XG5cdFx0Ly8gbm8gbW9kdWxlLmlkIG5lZWRlZFxuXHRcdC8vIG5vIG1vZHVsZS5sb2FkZWQgbmVlZGVkXG5cdFx0ZXhwb3J0czoge31cblx0fTtcblxuXHQvLyBFeGVjdXRlIHRoZSBtb2R1bGUgZnVuY3Rpb25cblx0X193ZWJwYWNrX21vZHVsZXNfX1ttb2R1bGVJZF0uY2FsbChtb2R1bGUuZXhwb3J0cywgbW9kdWxlLCBtb2R1bGUuZXhwb3J0cywgX193ZWJwYWNrX3JlcXVpcmVfXyk7XG5cblx0Ly8gUmV0dXJuIHRoZSBleHBvcnRzIG9mIHRoZSBtb2R1bGVcblx0cmV0dXJuIG1vZHVsZS5leHBvcnRzO1xufVxuXG4iLCIiLCIvLyBzdGFydHVwXG4vLyBMb2FkIGVudHJ5IG1vZHVsZSBhbmQgcmV0dXJuIGV4cG9ydHNcbi8vIFRoaXMgZW50cnkgbW9kdWxlIGlzIHJlZmVyZW5jZWQgYnkgb3RoZXIgbW9kdWxlcyBzbyBpdCBjYW4ndCBiZSBpbmxpbmVkXG52YXIgX193ZWJwYWNrX2V4cG9ydHNfXyA9IF9fd2VicGFja19yZXF1aXJlX18oXCIuL3NyYy9JbmZpbmlMb3JlLlBob3Rpbm8uSnMvVHNTb3VyY2UvSW5kZXgudHNcIik7XG4iLCIiXSwibmFtZXMiOltdLCJzb3VyY2VSb290IjoiIn0=