(function() {
	//#region TypeScript/Contracts/InfiniFrameHostMessaging.ts
	var infiniframe = "__infiniframe";
	var SendToHostMessageIds = {
		getRequest: `${infiniframe}:get`,
		titleChange: `${infiniframe}:title:change`,
		fullscreenEnter: `${infiniframe}:fullscreen:enter`,
		fullscreenExit: `${infiniframe}:fullscreen:exit`,
		openExternalLink: `${infiniframe}:open:external`,
		windowClose: `${infiniframe}:window:close`,
		ready: `${infiniframe}:ready`
	};
	var ReceiveFromHostMessageIds = {
		registerOpenExternal: `${infiniframe}:register:open:external`,
		registerFullscreenChange: `${infiniframe}:register:fullscreen:change`,
		registerTitleChange: `${infiniframe}:register:title:change`,
		registerWindowClose: `${infiniframe}:register:window:close`,
		readyAck: `${infiniframe}:ready:ack`,
		getMessageResponse: `${infiniframe}:get:response`
	};
	var InteropMessageMaxSizeBytes = 1024 * 1024;
	var InteropPostCommand = "Post";
	function createEnvelope(id, data, channel, command = InteropPostCommand, requestId) {
		if (!id || id.trim().length === 0) throw new Error("Envelope 'id' is required.");
		return {
			id,
			command,
			requestId,
			data,
			version: 2,
			channel
		};
	}
	function createGetEnvelope(command, args) {
		return createEnvelope(SendToHostMessageIds.getRequest, {
			command,
			args
		}, void 0, "Get");
	}
	function parseIncomingMessage(message) {
		if (!message || message.trim().length === 0) return { error: "Message is empty." };
		if (getUtf8ByteCount(message) > 1048576) return { error: `Message exceeds max size of ${InteropMessageMaxSizeBytes} bytes.` };
		try {
			const parsed = JSON.parse(message);
			if (!isObject$1(parsed)) return { error: "Envelope root must be a JSON object." };
			if (typeof parsed.id !== "string" || parsed.id.trim().length === 0) return { error: "Envelope 'id' is required and must be a string." };
			if (typeof parsed.version !== "number" || !Number.isInteger(parsed.version)) return { error: "Envelope 'version' is required and must be an integer." };
			if (parsed.version !== 2) return { error: `Unsupported envelope version '${parsed.version}'.` };
			const payload = convertDataToPayload(parsed.data);
			if (!isSupportedCommand(parsed.command)) return { error: "Envelope 'command' must be 'Post' or 'Get'." };
			if (parsed.requestId !== void 0 && typeof parsed.requestId !== "string") return { error: "Envelope 'requestId' must be a string." };
			return {
				messageId: parsed.id,
				payload,
				command: parsed.command,
				requestId: parsed.requestId
			};
		} catch {
			return { error: "Envelope JSON is malformed." };
		}
	}
	function convertDataToPayload(data) {
		if (data === null || data === void 0) return;
		if (typeof data === "string") return data;
		return JSON.stringify(data);
	}
	function getUtf8ByteCount(message) {
		return new TextEncoder().encode(message).length;
	}
	function isObject$1(value) {
		return typeof value === "object" && value !== null;
	}
	function isSupportedCommand(command) {
		return command === "Post" || command === "Get";
	}
	//#endregion
	//#region TypeScript/Utils/BlankTargetHandler.ts
	function isExternalLink(url) {
		try {
			return new URL(url, location.href).hostname !== location.hostname;
		} catch {
			return false;
		}
	}
	async function blankTargetHandler(e) {
		let el = e.target;
		while (el && el !== document.body) {
			if (el.tagName?.toLowerCase() !== "a") {
				el = el.parentElement;
				continue;
			}
			const anchor = el;
			if (!anchor.href) {
				el = el.parentElement;
				continue;
			}
			if (!(anchor.getAttribute("target") === "_blank" || anchor.hasAttribute("data-external") || isExternalLink(anchor.href))) {
				el = el.parentElement;
				continue;
			}
			e.preventDefault();
			window.infiniframe.messaging.sendMessageToHost(SendToHostMessageIds.openExternalLink, anchor.href);
			return;
		}
	}
	//#endregion
	//#region TypeScript/Utils/Observers.ts
	function getTitleObserverTarget() {
		return document.querySelector("title");
	}
	function getTitleObserver() {
		return new MutationObserver((mutations, _) => {
			mutations.forEach((mutation) => {
				if (mutation.type !== "childList") return;
				window.infiniframe.messaging.sendMessageToHost(SendToHostMessageIds.titleChange, document.title);
			});
		});
	}
	//#endregion
	//#region TypeScript/InfiniFrameHostMessaging.ts
	var InfiniFrameHostMessaging = class InfiniFrameHostMessaging {
		static {
			this.BlazorWebViewMessagePrefix = "__bwv:";
		}
		get isReady() {
			return this.readyHandshakeAcknowledged;
		}
		constructor() {
			this.messageHandlers = /* @__PURE__ */ new Map();
			this.openExternalRegistered = false;
			this.fullscreenRegistered = false;
			this.titleRegistered = false;
			this.windowCloseRegistered = false;
			this.readyHandshakeAcknowledged = false;
			this.ready = new Promise((resolve) => {
				this.resolveReady = resolve;
			});
			this.assignWebMessageReceiver();
			this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerOpenExternal, (_) => {
				this.registerOpenExternal();
			});
			this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerFullscreenChange, (_) => {
				this.registerFullscreenChange();
			});
			this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerTitleChange, (_) => {
				this.registerTitleChange();
			});
			this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.registerWindowClose, (_) => {
				this.registerWindowClose();
			});
			this.assignMessageReceivedHandler(ReceiveFromHostMessageIds.readyAck, (_) => {
				this.markReadyHandshakeAcknowledged();
			});
			this.sendReadyHandshake();
		}
		sendMessageToHost(id, data) {
			const envelope = createEnvelope(id, data);
			if (window.infiniframe?.host?.postData) window.infiniframe.host.postData(envelope);
			else {
				console.warn("Message to host failed. Host bridge API is not initialized.");
				return;
			}
		}
		async getMessageFromHostAsync(message) {
			const host = window.infiniframe?.host;
			if (!host?.getDataAsync) throw new Error("Message to host failed. Host getDataAsync API is not initialized.");
			const envelope = typeof message === "string" ? createEnvelope(message, void 0, void 0, "Get") : message;
			return await host.getDataAsync(envelope);
		}
		assignMessageReceivedHandler(messageId, callback) {
			this.messageHandlers.set(messageId, callback);
		}
		unregisterMessageReceivedHandler(messageId) {
			this.messageHandlers.delete(messageId);
		}
		assignWebMessageReceiver() {
			if (window.infiniframe?.host?.receiveCallback) window.infiniframe.host.receiveCallback((message) => {
				this.handleInteropMessage(message);
			});
			else {
				console.warn("Web message receiver failed. Host bridge API is not initialized.");
				return;
			}
		}
		handleInteropMessage(message) {
			if (typeof message !== "string") return false;
			if (!message) return false;
			const parsedMessage = parseIncomingMessage(message);
			if ("error" in parsedMessage) return false;
			if (parsedMessage.messageId.startsWith(InfiniFrameHostMessaging.BlazorWebViewMessagePrefix)) return true;
			const handler = this.messageHandlers.get(parsedMessage.messageId);
			if (!handler) {
				console.warn("No handler registered for message:", parsedMessage);
				return false;
			}
			handler(parsedMessage.payload);
			return true;
		}
		registerOpenExternal() {
			if (this.openExternalRegistered) return;
			this.openExternalRegistered = true;
			document.addEventListener("click", blankTargetHandler, { capture: true });
		}
		registerFullscreenChange() {
			if (this.fullscreenRegistered) return;
			this.fullscreenRegistered = true;
			document.addEventListener("fullscreenchange", (_) => {
				if (document.fullscreenElement) this.sendMessageToHost(SendToHostMessageIds.fullscreenEnter);
				else this.sendMessageToHost(SendToHostMessageIds.fullscreenExit);
			});
			document.addEventListener("keydown", async (e) => {
				if (e.key !== "F11") return;
				if (document.fullscreenElement) await document.exitFullscreen();
				else await document.body.requestFullscreen();
			});
		}
		registerTitleChange() {
			if (this.titleRegistered) return;
			this.titleRegistered = true;
			const titleTarget = getTitleObserverTarget();
			if (titleTarget) {
				getTitleObserver().observe(titleTarget, { childList: true });
				return;
			}
			const headTarget = document.head || document.documentElement;
			if (!headTarget) return;
			const headObserver = new MutationObserver(() => {
				const target = getTitleObserverTarget();
				if (!target) return;
				headObserver.disconnect();
				getTitleObserver().observe(target, { childList: true });
			});
			headObserver.observe(headTarget, {
				childList: true,
				subtree: true
			});
		}
		registerWindowClose() {
			if (this.windowCloseRegistered) return;
			this.windowCloseRegistered = true;
			window.close = () => {
				this.sendMessageToHost(SendToHostMessageIds.windowClose);
			};
		}
		sendReadyHandshake() {
			this.sendMessageToHost(SendToHostMessageIds.ready);
		}
		markReadyHandshakeAcknowledged() {
			if (this.readyHandshakeAcknowledged) return;
			this.readyHandshakeAcknowledged = true;
			this.resolveReady();
		}
	};
	//#endregion
	//#region TypeScript/InfiniFrameUtils.ts
	var InfiniFrameUtils = class {
		setPointerCapture(element, pointerId) {
			if (element === null) return;
			if (pointerId === null) return;
			if (element.hasPointerCapture(pointerId)) return;
			element.setPointerCapture(pointerId);
		}
		releasePointerCapture(element, pointerId) {
			if (element === null) return;
			if (pointerId === null) return;
			if (!element.hasPointerCapture(pointerId)) return;
			element.releasePointerCapture(pointerId);
		}
	};
	//#endregion
	//#region TypeScript/InfiniFrameWindow.ts
	var InfiniFrameWindow = class {
		getMessageFromHostAsync(command, args) {
			try {
				return window.infiniframe.messaging.getMessageFromHostAsync(createGetEnvelope(command, args));
			} catch (e) {
				console.error("Failed to get response message from host.", e);
				return Promise.reject(e);
			}
		}
		setTitle(title) {
			window.infiniframe.messaging.sendMessageToHost(SendToHostMessageIds.titleChange, title);
		}
		async getTitleAsync() {
			return this.getMessageFromHostAsync("title");
		}
	};
	//#endregion
	//#region TypeScript/InfiniFrame.ts
	var InfiniFrame = class {
		constructor(existing) {
			this.host = existing?.host;
			this.setup = existing?.setup;
			this.messaging = existing?.messaging ?? new InfiniFrameHostMessaging();
			this.window = existing?.window ?? new InfiniFrameWindow();
			this.utils = existing?.utils ?? new InfiniFrameUtils();
		}
	};
	//#endregion
	//#region TypeScript/Interop/NativeInterop/setupGuard.ts
	function getSetupGuard() {
		window.infiniframe = window.infiniframe ?? {};
		window.infiniframe.setup = window.infiniframe.setup ?? {
			nativeInteropBridgeInitialized: false,
			windowExternalBridgeInitialized: false,
			blazorModulesFetchPatchInitialized: false,
			blazorCustomElementsPatchInitialized: false,
			customElementsInitialized: false
		};
		return window.infiniframe.setup;
	}
	//#endregion
	//#region TypeScript/Interop/NativeInterop/NativeInteropBridge.ts
	var GetMessageResponseId = "__infiniframe:get:response";
	var GetMessageTimeoutMs = 1e4;
	var receiveCallbacks = /* @__PURE__ */ new Set();
	var receiveBridgeAttached = false;
	function installNativeInteropBridge(setup) {
		if (setup.nativeInteropBridgeInitialized) return;
		setup.nativeInteropBridgeInitialized = true;
		window.infiniframe = window.infiniframe ?? {};
		const host = window.infiniframe.host ?? {};
		const existingPostData = host.postData;
		const existingReceiveCallback = host.receiveCallback;
		const existingGetData = host.getDataAsync;
		host.postData = (envelope) => {
			dispatchEnvelopeToHost(envelope, existingPostData);
		};
		host.receiveCallback = (callback) => {
			registerWebMessageReceiver(callback, existingReceiveCallback);
		};
		host.getDataAsync = (message) => {
			return requestMessageFromHost(message, host, existingGetData, existingReceiveCallback);
		};
		window.infiniframe.host = host;
	}
	function dispatchEnvelopeToHost(envelope, existingPostData) {
		if (typeof envelope === "string") {
			const rawMessage = envelope.trim();
			if (rawMessage.length === 0) {
				console.warn("Ignoring empty host bridge payload.");
				return;
			}
			if (existingPostData) try {
				existingPostData(rawMessage);
				return;
			} catch (error) {
				console.warn("Existing InfiniFrame host bridge failed. Falling back to platform adapters.", error);
			}
			postToPlatform(rawMessage);
			return;
		}
		const normalized = normalizeEnvelope(envelope);
		if (!normalized) return;
		const serializedEnvelope = JSON.stringify(normalized);
		if (existingPostData) try {
			existingPostData(serializedEnvelope);
			return;
		} catch (error) {
			try {
				existingPostData(normalized);
				return;
			} catch {
				console.warn("Existing InfiniFrame host bridge failed. Falling back to platform adapters.", error);
			}
		}
		postToPlatform(serializedEnvelope);
	}
	function requestMessageFromHost(message, host, existingGetData, existingReceiveCallback) {
		const normalizedMessage = normalizeGetMessageInput(message);
		if (!normalizedMessage) return Promise.reject(/* @__PURE__ */ new Error("Host getDataAsync payload is invalid."));
		if (existingGetData) try {
			const existingResult = existingGetData(normalizedMessage);
			if (existingResult && typeof existingResult.then === "function") return existingResult;
			return Promise.resolve(String(existingResult ?? ""));
		} catch (error) {
			console.warn("Existing InfiniFrame getDataAsync bridge failed. Falling back to request/response transport.", error);
		}
		const requestId = createRequestId();
		return new Promise((resolve, reject) => {
			const timeout = window.setTimeout(() => {
				unregisterWebMessageReceiver(responseCallback);
				reject(/* @__PURE__ */ new Error("Timed out waiting for getDataAsync response from host."));
			}, GetMessageTimeoutMs);
			const responseCallback = (rawMessage) => {
				const parsed = parseIncomingMessage(rawMessage);
				if ("error" in parsed || parsed.messageId !== GetMessageResponseId || !parsed.payload) return;
				let payload;
				try {
					payload = JSON.parse(parsed.payload);
				} catch {
					return;
				}
				if (!isGetMessageResponsePayload(payload) || payload.requestId !== requestId) return;
				window.clearTimeout(timeout);
				unregisterWebMessageReceiver(responseCallback);
				if (payload.success) {
					resolve(payload.data ?? "");
					return;
				}
				reject(new Error(payload.error ?? "Host getDataAsync failed."));
			};
			registerWebMessageReceiver(responseCallback, existingReceiveCallback);
			const requestEnvelope = createGetRequestEnvelope(normalizedMessage, requestId);
			if (!requestEnvelope) {
				window.clearTimeout(timeout);
				unregisterWebMessageReceiver(responseCallback);
				reject(/* @__PURE__ */ new Error("Host getDataAsync payload is invalid."));
				return;
			}
			host.postData?.(requestEnvelope);
		});
	}
	function createGetRequestEnvelope(normalizedMessage, requestId) {
		if (typeof normalizedMessage !== "string") return normalizeEnvelope(normalizedMessage, "Get", requestId);
		try {
			const parsed = JSON.parse(normalizedMessage);
			if (isObject(parsed)) return normalizeEnvelope(parsed, "Get", requestId);
		} catch {}
		return normalizeEnvelope({
			id: normalizedMessage,
			version: 2
		}, "Get", requestId);
	}
	function normalizeGetMessageInput(message) {
		if (typeof message === "string") {
			const trimmed = message.trim();
			if (trimmed.length === 0) return null;
			return trimmed;
		}
		const normalizedEnvelope = normalizeEnvelope(message);
		if (!normalizedEnvelope) return null;
		return normalizedEnvelope;
	}
	function createRequestId() {
		return `if_req_${Date.now().toString(36)}_${Math.random().toString(36).slice(2, 10)}`;
	}
	function normalizeEnvelope(envelope, command = envelope.command ?? "Post", requestId = envelope.requestId) {
		if (!envelope || typeof envelope !== "object") {
			console.warn("Host bridge payload must be an envelope object.");
			return null;
		}
		if (typeof envelope.id !== "string" || envelope.id.trim().length === 0) {
			console.warn("Host bridge envelope requires a non-empty 'id'.");
			return null;
		}
		const normalized = {
			id: envelope.id,
			command,
			requestId,
			data: envelope.data,
			version: 2
		};
		if (envelope.channel !== void 0 && typeof envelope.channel === "string" && envelope.channel.trim().length > 0) normalized.channel = envelope.channel;
		return normalized;
	}
	function registerWebMessageReceiver(callback, existingReceiveCallback) {
		receiveCallbacks.add(callback);
		attachReceiveBridgeOnce(existingReceiveCallback);
	}
	function unregisterWebMessageReceiver(callback) {
		receiveCallbacks.delete(callback);
	}
	function attachReceiveBridgeOnce(existingReceiveCallback) {
		if (receiveBridgeAttached) return;
		const dispatch = (message) => {
			for (const callback of receiveCallbacks) callback(message);
		};
		if (existingReceiveCallback) try {
			existingReceiveCallback(dispatch);
			receiveBridgeAttached = true;
			return;
		} catch (error) {
			console.warn("Existing InfiniFrame host receive bridge failed. Falling back to platform adapters.", error);
		}
		if (window.chrome?.webview?.addEventListener) {
			window.chrome.webview.addEventListener("message", (event) => dispatch(event.data));
			receiveBridgeAttached = true;
			return;
		}
		if (window.webkit?.messageHandlers?.infiniFrameInterop) {
			window.__dispatchMessageCallback = dispatch;
			receiveBridgeAttached = true;
			return;
		}
		console.warn("Receive message registration failed. No supported host receive transport was found.");
	}
	function postToPlatform(message) {
		if (window.chrome?.webview?.postMessage) {
			window.chrome.webview.postMessage(message);
			return;
		}
		if (window.webkit?.messageHandlers?.infiniFrameInterop?.postMessage) {
			window.webkit.messageHandlers.infiniFrameInterop.postMessage(message);
			return;
		}
		console.warn("[InfiniFrame] No native bridge available:", message);
	}
	function isObject(value) {
		return typeof value === "object" && value !== null;
	}
	function isGetMessageResponsePayload(value) {
		return isObject(value) && typeof value.requestId === "string" && typeof value.success === "boolean" && (value.data === void 0 || typeof value.data === "string") && (value.error === void 0 || typeof value.error === "string");
	}
	//#endregion
	//#region TypeScript/Interop/NativeInterop/blazorExternalBridge.ts
	function initWindowExternalBridge(setup) {
		if (setup.windowExternalBridgeInitialized) return;
		setup.windowExternalBridgeInitialized = true;
		const external = ensureWindowExternal();
		window.__blazorCallbacks = window.__blazorCallbacks ?? [];
		external.receiveMessage = (callback) => {
			window.__blazorCallbacks.push(callback);
		};
		external.receiveCallback = external.receiveMessage;
		external.sendMessage = (message) => {
			if (!window.infiniframe?.host?.postData) {
				console.warn("Message to host failed. Host bridge API is not initialized.");
				return;
			}
			window.infiniframe.host.postData(message);
		};
		external.postMessage = external.sendMessage;
		if (!window.__blazorDispatchHooked) {
			window.__blazorDispatchHooked = true;
			window.infiniframe?.host?.receiveCallback((message) => {
				for (const callback of window.__blazorCallbacks ?? []) try {
					callback(message);
				} catch {}
			});
		}
	}
	function ensureWindowExternal() {
		if (window.external) return window.external;
		const external = {};
		Object.defineProperty(window, "external", {
			configurable: true,
			enumerable: true,
			value: external,
			writable: true
		});
		return external;
	}
	//#endregion
	//#region TypeScript/Interop/NativeInterop/blazorFetchPatch.ts
	var BLAZOR_MODULES_URLS = new Set([
		"http://localhost/_framework/blazor.modules.json",
		"http://localhost/_framework/blazor.modules.json/",
		"https://localhost/_framework/blazor.modules.json",
		"https://localhost/_framework/blazor.modules.json/",
		"app://localhost/_framework/blazor.modules.json",
		"app://localhost/_framework/blazor.modules.json/"
	]);
	function initBlazorModulesFetchPatch(setup) {
		if (setup.blazorModulesFetchPatchInitialized) return;
		setup.blazorModulesFetchPatchInitialized = true;
		const originalFetch = window.fetch;
		window.fetch = function(input, init) {
			try {
				const requestUrl = typeof input === "string" ? input : input instanceof URL ? input.href : input.url ?? "";
				if (requestUrl) {
					const absoluteUrl = new URL(requestUrl, window.location.href).href;
					if (BLAZOR_MODULES_URLS.has(absoluteUrl)) return Promise.resolve(new Response("[]", {
						status: 200,
						statusText: "OK",
						headers: { "Content-Type": "application/json" }
					}));
				}
			} catch {}
			return originalFetch.call(this, input, init);
		};
	}
	//#endregion
	//#region TypeScript/Interop/NativeInterop/customElements.ts
	function toKebabCase(name) {
		return String(name).replace(/([a-z0-9])([A-Z])/g, "$1-$2").replace(/_/g, "-").toLowerCase();
	}
	function toParameterValue(rawValue, typeName) {
		if (typeName === "bool" || typeName === "boolean") {
			if (rawValue === null) return false;
			if (rawValue === "") return true;
			return String(rawValue).toLowerCase() !== "false";
		}
		if (isNumericType(typeName)) {
			const value = Number(rawValue);
			return Number.isNaN(value) ? rawValue : value;
		}
		return rawValue;
	}
	function isNumericType(typeName) {
		return [
			"number",
			"int",
			"float",
			"double",
			"decimal"
		].indexOf(typeName) >= 0;
	}
	var pendingAutoCustomElementRegistrations = [];
	var autoCustomElementRegistrationScheduled = false;
	function scheduleAutoRegisterMissingInitializerCustomElements(defs, initMap) {
		if (!defs) return;
		pendingAutoCustomElementRegistrations.push({
			defs,
			initMap
		});
		if (autoCustomElementRegistrationScheduled) return;
		autoCustomElementRegistrationScheduled = true;
		window.setTimeout(() => {
			autoCustomElementRegistrationScheduled = false;
			flushAutoRegisterMissingInitializerCustomElements();
		}, 0);
	}
	function flushAutoRegisterMissingInitializerCustomElements() {
		if (typeof window.registerBlazorCustomElement !== "function") return;
		while (pendingAutoCustomElementRegistrations.length > 0) {
			const item = pendingAutoCustomElementRegistrations.shift();
			if (!item) return;
			try {
				autoRegisterMissingInitializerCustomElements(item.defs, item.initMap);
			} catch (error) {
				console.error(error);
			}
		}
	}
	function autoRegisterMissingInitializerCustomElements(defs, initMap) {
		const initialized = {};
		const initMapEntries = initMap ?? {};
		for (const key of Object.keys(initMapEntries)) {
			const list = initMapEntries[key];
			if (!Array.isArray(list)) continue;
			for (const id of list) initialized[id] = true;
		}
		const definitions = defs ?? {};
		for (const id of Object.keys(definitions)) {
			if (initialized[id]) continue;
			window.registerBlazorCustomElement(id, definitions[id]);
		}
	}
	function patchAttachWebRendererInteropIfAvailable() {
		const blazor = window.Blazor;
		if (!blazor?._internal || typeof blazor._internal.attachWebRendererInterop !== "function") return false;
		if (blazor._internal.__infiniframeAttachWebRendererInteropPatched) return true;
		const original = blazor._internal.attachWebRendererInterop;
		blazor._internal.attachWebRendererInterop = function(...args) {
			const result = original.apply(this, args);
			scheduleAutoRegisterMissingInitializerCustomElements(args[2], args[3]);
			return result;
		};
		blazor._internal.__infiniframeAttachWebRendererInteropPatched = true;
		return true;
	}
	function initBlazorCustomElementsPatch(setup) {
		if (setup.blazorCustomElementsPatchInitialized) return;
		setup.blazorCustomElementsPatchInitialized = true;
		if (!patchAttachWebRendererInteropIfAvailable()) {
			const descriptor = Object.getOwnPropertyDescriptor(window, "Blazor");
			if (!descriptor || descriptor.configurable) {
				let value = window.Blazor;
				Object.defineProperty(window, "Blazor", {
					configurable: true,
					enumerable: true,
					get: () => value,
					set: (v) => {
						value = v;
						patchAttachWebRendererInteropIfAvailable();
					}
				});
				if (value) patchAttachWebRendererInteropIfAvailable();
			}
		}
	}
	function initCustomElements(setup) {
		if (setup.customElementsInitialized) return;
		setup.customElementsInitialized = true;
		window.registerBlazorCustomElement = function(identifier, parameterDefinitions) {
			if (!window.Blazor?.rootComponents?.add) return;
			if (!window.customElements?.define) return;
			if (window.customElements.get(identifier)) return;
			const defs = Array.isArray(parameterDefinitions) ? parameterDefinitions : [];
			const map = {};
			for (const def of defs) {
				if (!def?.name) continue;
				const type = String(def.type ?? "").toLowerCase();
				if (type === "eventcallback") continue;
				const attr = toKebabCase(def.name);
				map[attr] = {
					name: def.name,
					type
				};
			}
			const observed = Object.keys(map);
			class Host extends HTMLElement {
				constructor(..._args) {
					super(..._args);
					this._component = null;
					this._isDisconnected = false;
				}
				static get observedAttributes() {
					return observed;
				}
				connectedCallback() {
					this._isDisconnected = false;
					window.Blazor.rootComponents.add(this, identifier, this._getParams()).then((c) => {
						this._component = c;
						if (this._isDisconnected && c) {
							this._component = null;
							return c.dispose?.();
						}
					}).catch(console.error);
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
	}
	//#endregion
	//#region TypeScript/Index.ts
	var setup = getSetupGuard();
	installNativeInteropBridge(setup);
	initWindowExternalBridge(setup);
	initBlazorModulesFetchPatch(setup);
	initBlazorCustomElementsPatch(setup);
	initCustomElements(setup);
	if (!window.infiniframe.messaging || !window.infiniframe.window || !window.infiniframe.utils) window.infiniframe = new InfiniFrame(window.infiniframe);
	console.log("InfiniFrame WebView JavaScript bridge initialized.");
	//#endregion
})();

//# sourceMappingURL=InfiniFrame.dev.js.map