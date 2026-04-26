(function () {
    console.log('InfiniFrame WebView JavaScript bridge initialized.');
    
    /* ============================================================================================================== */
    /* 1. Messaging bridge (infiniframe host) */
    /* ============================================================================================================== */
    window.__receiveCallbackCallbacks = window.__receiveCallbackCallbacks || [];

    window.__dispatchMessageCallback = window.__dispatchMessageCallback || function (message) {
        window.__receiveCallbackCallbacks.forEach(function (callback) {
            callback(message);
        });
    };

    if (window.chrome && window.chrome.webview && !window.__infiniframeWebviewReceiveAttached) {
        window.chrome.webview.addEventListener('message', function (e) {
            window.__dispatchMessageCallback(e.data);
        });
        window.__infiniframeWebviewReceiveAttached = true;
    }

    window.infiniframe = window.infiniframe || {};
    window.infiniframe.host = window.infiniframe.host || {};

    window.infiniframe.host.postData = window.infiniframe.host.postData || function (envelope) {
        const message = (typeof envelope === 'string') ? envelope : JSON.stringify(envelope);
        window.chrome.webview.postMessage(message);
    };

    window.infiniframe.host.receiveCallback = window.infiniframe.host.receiveCallback || function (callback) {
        window.__receiveCallbackCallbacks.push(callback);
    };

    window.infiniframe.host.getData = window.infiniframe.host.getData || function (message) {
        const requestId = 'if_req_' + Date.now().toString(36) + '_' + Math.random().toString(36).slice(2);
        const serializedMessage = (typeof message === 'string') ? message : JSON.stringify(message);

        return new Promise(function (resolve, reject) {

            const callback = function (rawMessage) {
                try {
                    const envelope = JSON.parse(rawMessage);
                    if (!envelope || envelope.id !== '__infiniframe:get:response' || typeof envelope.data !== 'string') {
                        return;
                    }

                    const payload = JSON.parse(envelope.data);
                    if (!payload || payload.requestId !== requestId) {
                        return;
                    }

                    const callbackIndex = window.__receiveCallbackCallbacks.indexOf(callback);
                    if (callbackIndex >= 0) {
                        window.__receiveCallbackCallbacks.splice(callbackIndex, 1);
                    }

                    if (payload.success === true) {
                        resolve(payload.data || '');
                    } else {
                        reject(new Error(payload.error || 'Host getData failed.'));
                    }
                } catch (_) {
                }
            };

            window.infiniframe.host.receiveCallback(callback);

            window.infiniframe.host.postData({
                id: '__infiniframe:get:request',
                data: {requestId: requestId, message: serializedMessage},
                version: 1
            });
        });
    };

    window.external = window.external || {};
    window.__blazor_callbacks = window.__blazor_callbacks || [];

    if (window.chrome && window.chrome.webview && !window.__blazor_bridge_attached) {
        window.chrome.webview.addEventListener('message', function (e) {
            for (let i = 0; i < window.__blazor_callbacks.length; i++) {
                try {
                    window.__blazor_callbacks[i](e.data);
                } catch (_) {}
            }
        });

        window.__blazor_bridge_attached = true;
    }

    window.external.receiveMessage = function (callback) {
        window.__blazor_callbacks.push(callback);
    };

    window.external.receiveCallback = window.external.receiveMessage;

    window.external.sendMessage = function (message) {
        window.chrome.webview.postMessage(message);
    };

    window.external.postMessage = window.external.sendMessage;
    
    /* ============================================================================================================== */
    /* 2. Blazor modules fetch patch */
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
                            headers: {'Content-Type': 'application/json'}
                        }));
                    }
                }
            } catch (_) {
            }

            return originalFetch.call(this, input, init);
        };
    }


    /* ============================================================================================================== */
    /* 3. Blazor custom element + interop patch */
    /* ============================================================================================================== */
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
                if (rawValue === null) {
                    return false;
                }
                if (rawValue === '') {
                    return true;
                }
                return String(rawValue).toLowerCase() !== 'false';
            }
            if (typeName === 'number' || typeName === 'int' || typeName === 'float' || typeName === 'double' || typeName === 'decimal') {
                const numericValue = Number(rawValue);
                return Number.isNaN(numericValue) ? rawValue : numericValue;
            }
            return rawValue;
        }

        window.registerBlazorCustomElement = window.registerBlazorCustomElement || function (identifier, parameterDefinitions) {
            if (!window.Blazor || !window.Blazor.rootComponents || !window.Blazor.rootComponents.add) {
                console.warn('registerBlazorCustomElement skipped: Blazor.rootComponents is unavailable.');
                return;
            }
            if (!window.customElements || typeof window.customElements.define !== 'function') {
                console.warn('registerBlazorCustomElement skipped: customElements API is unavailable.');
                return;
            }
            if (window.customElements.get(identifier)) {
                return;
            }

            const definitions = Array.isArray(parameterDefinitions) ? parameterDefinitions : [];
            const parametersByAttribute = {};

            for (let index = 0; index < definitions.length; index++) {
                const definition = definitions[index];
                if (!definition || !definition.name) {
                    continue;
                }
                const parameterType = String(definition.type || '').toLowerCase();
                if (parameterType === 'eventcallback') {
                    continue;
                }
                const attributeName = toKebabCase(definition.name);
                parametersByAttribute[attributeName] = {
                    name: definition.name,
                    type: parameterType
                };
            }

            const observedAttributes = Object.keys(parametersByAttribute);

            class BlazorCustomElementHost extends HTMLElement {

                constructor() {
                    super();
                    this._component = null;
                    this._isDisconnected = false;
                }

                static get observedAttributes() {
                    return observedAttributes;
                }

                connectedCallback() {
                    this._isDisconnected = false;
                    const parameters = this._getCurrentParameters();

                    window.Blazor.rootComponents.add(this, identifier, parameters)
                        .then((component) => {
                            this._component = component;
                            if (this._isDisconnected && this._component) {
                                var detached = this._component;
                                this._component = null;
                                return detached.dispose();
                            }
                        })
                        .catch((error) => {
                            console.error('Failed to attach custom element component.', error);
                        });
                }

                disconnectedCallback() {
                    this._isDisconnected = true;
                    const component = this._component;
                    this._component = null;

                    if (component && typeof component.dispose === 'function') {
                        Promise.resolve(component.dispose()).catch(function () {
                        });
                    }
                }

                attributeChangedCallback(attributeName, oldValue, newValue) {
                    if (oldValue === newValue) {
                        return;
                    }
                    if (!this._component || typeof this._component.setParameters !== 'function') {
                        return;
                    }

                    const parameterInfo = parametersByAttribute[String(attributeName).toLowerCase()];
                    if (!parameterInfo) {
                        return;
                    }

                    const nextParameters = {};
                    nextParameters[parameterInfo.name] = toParameterValue(newValue, parameterInfo.type);

                    this._component.setParameters(nextParameters).catch(function (error) {
                        console.error('Failed to update custom element parameters.', error);
                    });
                }

                _getCurrentParameters() {
                    const parameters = {};

                    for (let index = 0; index < observedAttributes.length; index++) {
                        const attributeName = observedAttributes[index];
                        if (!this.hasAttribute(attributeName)) {
                            continue;
                        }
                        const parameterInfo = parametersByAttribute[attributeName];
                        parameters[parameterInfo.name] = toParameterValue(
                            this.getAttribute(attributeName),
                            parameterInfo.type
                        );
                    }

                    return parameters;
                }
            }

            window.customElements.define(identifier, BlazorCustomElementHost);

            function autoRegisterMissingInitializerCustomElements(componentDefinitionsByIdentifier, identifiersByInitializer) {
                const initialized = {};

                for (const list of Object.values(identifiersByInitializer || {})) {
                    if (!Array.isArray(list)) continue;
                    for (const id of list) {
                        initialized[id] = true;
                    }
                }

                for (const entry of Object.entries(componentDefinitionsByIdentifier || {})) {
                    const identifier = entry[0];
                    if (initialized[identifier]) continue;
                    window.registerBlazorCustomElement(identifier, entry[1]);
                }
            }

            function patchAttach() {
                const blazor = window.Blazor;
                if (!blazor || !blazor._internal || typeof blazor._internal.attachWebRendererInterop !== 'function') {
                    return false;
                }

                if (blazor._internal.__patched) return true;

                const original = blazor._internal.attachWebRendererInterop;
                blazor._internal.attachWebRendererInterop = function () {
                    const result = original.apply(this, arguments);
                    autoRegisterMissingInitializerCustomElements(arguments[2], arguments[3]);
                    return result;
                };

                blazor._internal.__patched = true;
                return true;
            }

            patchAttach();
        }
    }
})()