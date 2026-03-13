#ifdef __APPLE__
/**
 * @file Dialog.mm (macOS)
 * @brief macOS implementation of InfiniFrameDialog using NSOpenPanel, NSSavePanel, and NSAlert
 */

#import "Models/InfiniFrameDialog.h"

#if defined(VSTGUI_USE_OBJC_UTTYPE)
#import <UniformTypeIdentifiers/UniformTypeIdentifiers.h>
#else
#pragma clang diagnostic ignored "-Wdeprecated-declarations"
#endif

// Base64-encoded PNG icon images bundled directly to avoid a dependency on external icon files
// Each string decodes to a 64×64 PNG used as the NSAlert icon for the corresponding DialogIcon value
NSString* errorBase64 = @"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";
NSString* infoBase64 = @"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";
NSString* questionBase64 = @"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";
NSString* warningBase64 = @"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";

/**
 * @brief Decode a base64-encoded PNG string into an NSImage.
 * @param base64 Base64 string (no line breaks)
 * @return Autoreleased NSImage; caller must retain if stored beyond the current autorelease pool
 */
NSImage* getIcon(NSString* base64) {
  NSData *imageData = [[NSData alloc] initWithBase64EncodedString:base64 options:0];
  NSImage *image = [[NSImage alloc] initWithData:imageData];
  [imageData release];
  return image;
}

InfiniFrameDialog::InfiniFrameDialog() {
  _errorIcon = getIcon(errorBase64);
  _infoIcon = getIcon(infoBase64);
  _questionIcon = getIcon(questionBase64);
  _warningIcon = getIcon(warningBase64);
}

InfiniFrameDialog::~InfiniFrameDialog() {
  [_errorIcon release];
  [_infoIcon release];
  [_questionIcon release];
  [_warningIcon release];
}

AutoString* InfiniFrameDialog::ShowOpenFile(AutoString title, AutoString defaultPath, bool multiSelect, AutoString* filters, int filterCount, int* resultCount) {
  NSOpenPanel* openDlg = [NSOpenPanel openPanel];

  [openDlg setTitle:[NSString stringWithUTF8String:title]];
  [openDlg setCanChooseFiles:YES];
  [openDlg setCanChooseDirectories:NO];
  [openDlg setAllowsMultipleSelection:multiSelect];
  [openDlg setPrompt:[NSString stringWithUTF8String:"Open"]];
  [openDlg setDirectoryURL:[NSURL fileURLWithPath:[NSString stringWithUTF8String:defaultPath]]];

  if (filterCount > 0) {
    NSMutableArray* fileTypes = [[[NSMutableArray alloc] init] autorelease];
    for (int i = 0; i < filterCount; i++) {
      [fileTypes addObject:[NSString stringWithUTF8String:filters[i]]];
    }

#ifdef VSTGUI_USE_OBJC_UTTYPE
		[openDlg setAllowedContentTypes:fileTypes];
#else
		[openDlg setAllowedFileTypes:fileTypes];
#endif
  }

  if ([openDlg runModal] == NSModalResponseOK) {
    NSArray* files = [openDlg URLs];
    *resultCount = static_cast<int>([files count]);
    auto** result = static_cast<char**>(malloc(*resultCount * sizeof(char*)));
    for (int i = 0; i < *resultCount; i++) {
      result[i] = strdup([[[files objectAtIndex:i] path] UTF8String]);
    }
    return result;
  }

  return nullptr;
}

AutoString* InfiniFrameDialog::ShowOpenFolder(AutoString title, AutoString defaultPath, bool multiSelect, int* resultCount) {
  NSOpenPanel* openDlg = [NSOpenPanel openPanel];

  [openDlg setTitle:[NSString stringWithUTF8String:title]];
  [openDlg setCanChooseFiles:NO];
  [openDlg setCanChooseDirectories:YES];
  [openDlg setCanCreateDirectories:YES];
  [openDlg setAllowsMultipleSelection:multiSelect];
  [openDlg setPrompt:[NSString stringWithUTF8String:"Open"]];
  [openDlg setDirectoryURL:[NSURL fileURLWithPath:[NSString stringWithUTF8String:defaultPath]]];

  if ([openDlg runModal] == NSModalResponseOK) {
    NSArray* files = [openDlg URLs];
    *resultCount = static_cast<int>([files count]);
    auto** result = static_cast<char**>(malloc(*resultCount * sizeof(char*)));
    for (int i = 0; i < *resultCount; i++) {
      result[i] = strdup([[[files objectAtIndex:i] path] UTF8String]);
    }
    return result;
  }

  return nullptr;
}

AutoString InfiniFrameDialog::ShowSaveFile(AutoString title, AutoString defaultPath, AutoString* filters, int filterCount, AutoString defaultFileName) {
  NSSavePanel* saveDlg = [NSSavePanel savePanel];

  [saveDlg setTitle:[NSString stringWithUTF8String:title]];
  [saveDlg setPrompt:[NSString stringWithUTF8String:"Save"]];
  [saveDlg setDirectoryURL:[NSURL fileURLWithPath:[NSString stringWithUTF8String:defaultPath]]];
  [saveDlg setNameFieldStringValue:[NSString stringWithUTF8String:defaultFileName]];
  [saveDlg setAllowsOtherFileTypes:NO];
  [saveDlg setCanSelectHiddenExtension:YES];

  if (filterCount > 0) {
    NSMutableArray* fileTypes = [[[NSMutableArray alloc] init] autorelease];
    for (int i = 0; i < filterCount; i++) {
      [fileTypes addObject:[NSString stringWithUTF8String:filters[i]]];
    }

#ifdef VSTGUI_USE_OBJC_UTTYPE
		[saveDlg setAllowedContentTypes:fileTypes];
#else
		[saveDlg setAllowedFileTypes:fileTypes];
#endif
  }

  if ([saveDlg runModal] == NSModalResponseOK) {
    return strdup([[saveDlg URL].path UTF8String]);
  }

  return nullptr;
}

DialogResult InfiniFrameDialog::ShowMessage(AutoString title, AutoString text, DialogButtons buttons, DialogIcon icon) {
  NSAlert* alert = [[NSAlert alloc] init];
  [alert setMessageText:[NSString stringWithUTF8String:title]];
  [alert setInformativeText:[NSString stringWithUTF8String:text]];

  switch (buttons) {
    case DialogButtons::Ok:
      [alert addButtonWithTitle:@"OK"];
      break;
    case DialogButtons::OkCancel:
      [alert addButtonWithTitle:@"OK"];
      [alert addButtonWithTitle:@"Cancel"];
      break;
    case DialogButtons::YesNo:
      [alert addButtonWithTitle:@"Yes"];
      [alert addButtonWithTitle:@"No"];
      break;
    case DialogButtons::YesNoCancel:
      [alert addButtonWithTitle:@"Yes"];
      [alert addButtonWithTitle:@"No"];
      [alert addButtonWithTitle:@"Cancel"];
      break;
    case DialogButtons::RetryCancel:
      [alert addButtonWithTitle:@"Retry"];
      [alert addButtonWithTitle:@"Cancel"];
      break;
    case DialogButtons::AbortRetryIgnore:
      [alert addButtonWithTitle:@"Abort"];
      [alert addButtonWithTitle:@"Retry"];
      [alert addButtonWithTitle:@"Ignore"];
      break;
  }

  switch (icon) {
    case DialogIcon::Error:
      [alert setIcon:_errorIcon];
      break;
    case DialogIcon::Warning:
      [alert setIcon:_warningIcon];
      break;
    case DialogIcon::Info:
      [alert setIcon:_infoIcon];
      break;
    case DialogIcon::Question:
      [alert setIcon:_questionIcon];
      break;
  }

  auto result = [alert runModal];

  if (buttons == DialogButtons::Ok) {
    if (result == NSAlertFirstButtonReturn) return DialogResult::Ok;
    else return DialogResult::Cancel;
  }

  if (buttons == DialogButtons::OkCancel) {
    switch (result) {
      case NSAlertFirstButtonReturn: return DialogResult::Ok;
      case NSAlertSecondButtonReturn:
      default: return DialogResult::Cancel;
    }
  }

  if (buttons == DialogButtons::YesNo) {
    switch (result) {
      case NSAlertFirstButtonReturn: return DialogResult::Yes;
      case NSAlertSecondButtonReturn: return DialogResult::No;
      default: return DialogResult::Cancel;
    }
  }

  if (buttons == DialogButtons::YesNoCancel) {
    switch (result) {
      case NSAlertFirstButtonReturn: return DialogResult::Yes;
      case NSAlertSecondButtonReturn: return DialogResult::No;
      case NSAlertThirdButtonReturn:
      default: return DialogResult::Cancel;
    }
  }

  if (buttons == DialogButtons::RetryCancel) {
    switch (result) {
      case NSAlertFirstButtonReturn: return DialogResult::Retry;
      case NSAlertSecondButtonReturn:
      default: return DialogResult::Cancel;
    }
  }

  if (buttons == DialogButtons::AbortRetryIgnore) {
    switch (result) {
      case NSAlertFirstButtonReturn: return DialogResult::Abort;
      case NSAlertSecondButtonReturn: return DialogResult::Retry;
      case NSAlertThirdButtonReturn:
      default: return DialogResult::Ignore;
    }
  }

  return DialogResult::Cancel;
}
#endif
