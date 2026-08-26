// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniFrame;

namespace InfiniTests.InfiniFrame.Shared.Features;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
public class FilePickerDialogsExtensionMethodTests {

    [Test]
    public async Task ShowOpenFile_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IFilePickerDialogsInfiniFrameWindowFeature> dialogs = MockFactory.CreateFilePickerDialogsMock();
        window.Features.Returns(features.Object);
        features.FilePickerDialogs.Returns(dialogs.Object);
        string[] expectedResult = ["/path/to/file.txt"];
        dialogs.ShowOpenFile(Any<string>(), Any<string?>(), Any<bool>(), Any<(string Name, string[] Extensions)[]?>()).Returns(expectedResult);

        // Act
        string?[] result = window.Object.ShowOpenFile("Select File");

        // Assert
        await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task ShowSaveFile_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IFilePickerDialogsInfiniFrameWindowFeature> dialogs = MockFactory.CreateFilePickerDialogsMock();
        window.Features.Returns(features.Object);
        features.FilePickerDialogs.Returns(dialogs.Object);
        dialogs.ShowSaveFile(Any<string>(), Any<string?>(), Any<(string Name, string[] Extensions)[]?>(), Any<string?>()).Returns("/path/to/save.txt");

        // Act
        string? result = window.Object.ShowSaveFile("Save File");

        // Assert
        await Assert.That(result).IsEqualTo("/path/to/save.txt");
    }

    [Test]
    public async Task ShowOpenFolder_CallsFeature(CancellationToken ct = default) {
        // Arrange
        Mock<IInfiniFrameWindow> window = MockFactory.CreateWindowMock();
        Mock<IInfiniFrameWindowFeatures> features = MockFactory.CreateFeaturesMock();
        Mock<IFilePickerDialogsInfiniFrameWindowFeature> dialogs = MockFactory.CreateFilePickerDialogsMock();
        window.Features.Returns(features.Object);
        features.FilePickerDialogs.Returns(dialogs.Object);
        string[] expectedResult = ["/path/to/folder"];
        dialogs.ShowOpenFolder(Any<string>(), Any<string?>(), Any<bool>()).Returns(expectedResult);

        // Act
        string?[] result = window.Object.ShowOpenFolder("Select Folder");

        // Assert
        await Assert.That(result).IsNotNull();
    }
}
