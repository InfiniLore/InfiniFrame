(function () {
    console.log('InfiniFrame WebView JavaScript bridge initialized.');

    /* ============================================================================================================== */
    /* 1. Messaging bridge (infiniFrame host) */
    /* ============================================================================================================== */
    window.__receiveCallbackCallbacks = window.__receiveCallbackCallbacks || [];

    window.__dispatchMessageCallback = window.__dispatchMessageCallback || function (message) {
        for (let i = 0; i < window.__receiveCallbackCallbacks.length; i++) {
            try {
                window.__receiveCallbackCallbacks[i](message);
            } catch (_) {}
        }
    };

    if (window.chrome && window.chrome.webview && !window.__infiniframeWebviewReceiveAttached) {
        window.chrome.webview.addEventListener('message', function (e) {
            window.__dispatchMessageCallback(e.data);
        });
        window.__infiniframeWebviewReceiveAttached = true;
    }

    window.__infiniframe = window.__infiniframe || {};
    window.__infiniframe.host = window.__infiniframe.host || {};
    
    window.__infiniframe.host.postData = window.__infiniframe.host.postData || function (envelope) {
        const message = (typeof envelope === 'string') ? envelope : JSON.stringify(envelope);
        window.chrome.webview.postMessage(message);
    };

    window.__infiniframe.host.postMessage =
        window.__infiniframe.host.postMessage || window.__infiniframe.host.postData;
    
    window.__infiniframe.host.receiveCallback = window.__infiniframe.host.receiveCallback || function (callback) {
        window.__receiveCallbackCallbacks.push(callback);
    };

    window.__infiniframe.host.receiveMessage =
        window.__infiniframe.host.receiveMessage || window.__infiniframe.host.receiveCallback;
    
    window.__infiniframe.host.getData = window.__infiniframe.host.getData || function (message) {
        const requestId = 'if_req_' + Date.now().toString(36) + '_' + Math.random().toString(36).slice(2);
        const serializedMessage = (typeof message === 'string') ? message : JSON.stringify(message);

        return new Promise(function (resolve, reject) {

            const callback = function (rawMessage) {
                try {
                    const envelope = JSON.parse(rawMessage);
                    if (!envelope || envelope.id !== '__infiniframe:get:response') return;

                    const payload = JSON.parse(envelope.data || '{}');
                    if (!payload || payload.requestId !== requestId) return;

                    // remove callback
                    const idx = window.__receiveCallbackCallbacks.indexOf(callback);
                    if (idx >= 0) window.__receiveCallbackCallbacks.splice(idx, 1);

                    if (payload.success === true) {
                        resolve(payload.data || '');
                    } else {
                        reject(new Error(payload.error || 'Host getData failed.'));
                    }
                } catch (_) {}
            };

            window.__infiniframe.host.receiveCallback(callback);

            window.__infiniframe.host.postData({
                id: '__infiniframe:get:request',
                data: {
                    requestId: requestId,
                    message: serializedMessage
                },
                version: 1
            });
        });
    };

    /* ============================================================================================================== */
    /* 2. window.external bridge (Blazor compatibility) */
    /* ============================================================================================================== */
    window.external = window.external || {};
    window.__blazor_callbacks = window.__blazor_callbacks || [];

    window.external.receiveMessage = function (callback) {
        window.__blazor_callbacks.push(callback);
    };

    window.external.receiveCallback = window.external.receiveMessage;

    window.external.sendMessage = function (message) {
        window.chrome.webview.postMessage(message);
    };

    window.external.postMessage = window.external.sendMessage;

    // hook Blazor callbacks into main dispatcher
    if (!window.__blazor_dispatch_hooked) {
        window.__blazor_dispatch_hooked = true;

        window.__receiveCallbackCallbacks.push(function (message) {
            for (let i = 0; i < window.__blazor_callbacks.length; i++) {
                try {
                    window.__blazor_callbacks[i](message);
                } catch (_) {}
            }
        });
    }

    /* ============================================================================================================== */
    /* 3. Blazor modules fetch patch */
    /* ============================================================================================================== */
    if (!window.__infiniframeBlazorModulesPatched) {
        window.__infiniframeBlazorModulesPatched = true;

        const originalFetch = window.fetch;

        window.fetch = function (input, init) {
            try {
                const requestUrl = typeof input === 'string' ? input : (input && input.url ? input.url : '');
                if (requestUrl) {
                    const absoluteUrl = new URL(requestUrl, window.location.href).href;

                    const isBlazorModulesJson =
                        absoluteUrl === 'http://localhost/_framework/blazor.modules.json' ||
                        absoluteUrl === 'http://localhost/_framework/blazor.modules.json/' ||
                        absoluteUrl === 'app://localhost/_framework/blazor.modules.json' ||
                        absoluteUrl === 'app://localhost/_framework/blazor.modules.json/';

                    if (isBlazorModulesJson) {
                        return Promise.resolve(new Response('[]', {
                            status: 200,
                            statusText: 'OK',
                            headers: { 'Content-Type': 'application/json' }
                        }));
                    }
                }
            } catch (_) {}

            return originalFetch.call(this, input, init);
        };
    }

    /* ============================================================================================================== */
    /* 4. Blazor custom elements + interop patch */
    /* ============================================================================================================== */
    function patchAttachWebRendererInteropIfAvailable() {
        const blazor = window.Blazor;

        if (!blazor || !blazor._internal || typeof blazor._internal.attachWebRendererInterop !== 'function') {
            return false;
        }

        if (blazor._internal.__infiniframeAttachWebRendererInteropPatched) {
            return true;
        }

        const original = blazor._internal.attachWebRendererInterop;

        blazor._internal.attachWebRendererInterop = function () {
            const result = original.apply(this, arguments);
            autoRegisterMissingInitializerCustomElements(arguments[2], arguments[3]);
            return result;
        };

        blazor._internal.__infiniframeAttachWebRendererInteropPatched = true;
        return true;
    }

    if (!patchAttachWebRendererInteropIfAvailable()) {
        const descriptor = Object.getOwnPropertyDescriptor(window, 'Blazor');

        if (!descriptor || descriptor.configurable) {
            let value = window.Blazor;

            Object.defineProperty(window, 'Blazor', {
                configurable: true,
                enumerable: true,
                get: function () { return value; },
                set: function (v) {
                    value = v;
                    patchAttachWebRendererInteropIfAvailable();
                }
            });

            if (value) {
                patchAttachWebRendererInteropIfAvailable();
            }
        }
    }

    /* =================================================================================================== */
    /* 5. Custom elements */
    /* =================================================================================================== */

    if (!window.__infiniframeRegisterBlazorCustomElement) {
        window.__infiniframeRegisterBlazorCustomElement = true;

        function toKebabCase(name) {
            return String(name)
                .replace(/([a-z0-9])([A-Z])/g, '$1-$2')
                .replace(/_/g, '-')
                .toLowerCase();
        }

        function toParameterValue(rawValue, typeName) {
            if (typeName === 'bool' || typeName === 'boolean') {
                if (rawValue === null) return false;
                if (rawValue === '') return true;
                return String(rawValue).toLowerCase() !== 'false';
            }

            if (['number', 'int', 'float', 'double', 'decimal'].includes(typeName)) {
                const n = Number(rawValue);
                return Number.isNaN(n) ? rawValue : n;
            }

            return rawValue;
        }

        window.registerBlazorCustomElement = function (identifier, parameterDefinitions) {
            if (!window.Blazor?.rootComponents?.add) return;
            if (!window.customElements?.define) return;
            if (window.customElements.get(identifier)) return;

            const defs = Array.isArray(parameterDefinitions) ? parameterDefinitions : [];
            const map = {};

            for (const def of defs) {
                if (!def?.name) continue;
                const type = String(def.type || '').toLowerCase();
                if (type === 'eventcallback') continue;

                const attr = toKebabCase(def.name);
                map[attr] = { name: def.name, type };
            }

            const observed = Object.keys(map);

            class Host extends HTMLElement {
                static get observedAttributes() { return observed; }

                constructor() {
                    super();
                    this._component = null;
                    this._isDisconnected = false;
                }

                connectedCallback() {
                    this._isDisconnected = false;

                    window.Blazor.rootComponents.add(this, identifier, this._getParams())
                        .then(c => {
                            this._component = c;
                            if (this._isDisconnected && c) {
                                this._component = null;
                                return c.dispose();
                            }
                        })
                        .catch(console.error);
                }

                disconnectedCallback() {
                    this._isDisconnected = true;
                    const c = this._component;
                    this._component = null;
                    if (c?.dispose) Promise.resolve(c.dispose()).catch(() => {});
                }

                attributeChangedCallback(name, oldValue, newValue) {
                    if (oldValue === newValue) return;
                    if (!this._component?.setParameters) return;

                    const info = map[name];
                    if (!info) return;

                    const p = {};
                    p[info.name] = toParameterValue(newValue, info.type);

                    this._component.setParameters(p).catch(console.error);
                }

                _getParams() {
                    const p = {};
                    for (const attr of observed) {
                        if (!this.hasAttribute(attr)) continue;
                        const info = map[attr];
                        p[info.name] = toParameterValue(this.getAttribute(attr), info.type);
                    }
                    return p;
                }
            }

            window.customElements.define(identifier, Host);
        };

        function autoRegisterMissingInitializerCustomElements(defs, initMap) {
            const initialized = {};

            for (const list of Object.values(initMap || {})) {
                if (!Array.isArray(list)) continue;
                for (const id of list) initialized[id] = true;
            }

            for (const [id, def] of Object.entries(defs || {})) {
                if (initialized[id]) continue;
                window.registerBlazorCustomElement(id, def);
            }
        }
    }

})();