import { beforeEach, describe, expect, it } from "vitest";
import type { FilePickerDialogsInfiniFrameWindowFeature } from "./FilePickerDialogsInfiniFrameWindowFeature";
import { setupFeature } from "./_testHelpers";

describe("FilePickerDialogsInfiniFrameWindowFeature", () => {
	let feature: FilePickerDialogsInfiniFrameWindowFeature;
	let messaging: ReturnType<
		typeof import("./_testHelpers").createMessagingMock
	>;

	beforeEach(async () => {
		messaging = setupFeature();
		const mod = await import("./FilePickerDialogsInfiniFrameWindowFeature");
		feature = new mod.FilePickerDialogsInfiniFrameWindowFeature();
	});

	it("showOpenFileAsync calls get", async () => {
		messaging.getMessageFromHostAsync.mockResolvedValue(
			JSON.stringify("/selected/file.txt"),
		);
		await feature.showOpenFileAsync("Choose file");
		expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
	});
	it("showOpenFolderAsync calls get", async () => {
		messaging.getMessageFromHostAsync.mockResolvedValue(
			JSON.stringify("/selected/folder"),
		);
		await feature.showOpenFolderAsync("Select folder");
		expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
	});
	it("showSaveFileAsync calls get", async () => {
		messaging.getMessageFromHostAsync.mockResolvedValue(
			JSON.stringify("/save/path.txt"),
		);
		await feature.showSaveFileAsync("Save file");
		expect(messaging.getMessageFromHostAsync).toHaveBeenCalled();
	});
});
