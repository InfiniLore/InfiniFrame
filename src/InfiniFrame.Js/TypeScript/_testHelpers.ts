import type {
	InfiniFrame,
	InfiniFrameHostBridge,
	InfiniFrameHostMessaging,
} from "./Contracts";

export type TestHostBridge = Partial<InfiniFrameHostBridge>;
export type TestMessaging = Partial<InfiniFrameHostMessaging>;

export type TestInfiniFrameFixture = Omit<
	Partial<InfiniFrame>,
	"host" | "messaging"
> & {
	host?: TestHostBridge;
	messaging?: TestMessaging;
} & {
	custom?: unknown;
	existing?: boolean;
};

export type TestInfiniFrame = TestInfiniFrameFixture;

export type TestBlazor = NonNullable<Window["Blazor"]>;

export type TestWindow = Omit<
	Window,
	"infiniframe" | "__infiniframe_dispatch"
> & {
	infiniframe: TestInfiniFrameFixture;
	__infiniframe?: unknown;
	__infiniframe_dispatch?: (message: string) => void;
	__dispatchMessageCallback?: (message: string) => void;
};

export function getTestWindow(): TestWindow {
	return window as TestWindow;
}

export function requireValue<T>(value: T | null | undefined): T {
	if (value == null) {
		throw new Error("Expected test value to be present.");
	}
	return value;
}

export function deleteTestProperty(
	target: object,
	property: PropertyKey,
): void {
	Reflect.deleteProperty(target, property);
}
