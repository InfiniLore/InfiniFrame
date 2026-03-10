#pragma once

#include "InfiniFrame.h"

#ifdef __APPLE__
#include <Cocoa/Cocoa.h>
#endif

enum class DialogResult
{
	Cancel = -1,
	Ok,
	Yes,
	No,
	Abort,
	Retry,
	Ignore,
};

enum class DialogButtons
{
	Ok,
	OkCancel,
	YesNo,
	YesNoCancel,
	RetryCancel,
	AbortRetryIgnore,
};

enum class DialogIcon
{
	Info,
	Warning,
	Error,
	Question,
};

class InfiniFrameDialog
{
public:
#ifdef _WIN32
	InfiniFrameDialog(InfiniFrame *window);
#else
	InfiniFrameDialog();
#endif
	~InfiniFrameDialog();

	AutoString *ShowOpenFile(AutoString title, AutoString defaultPath, bool multiSelect, AutoString *filters, int filterCount, int *resultCount);
	AutoString *ShowOpenFolder(AutoString title, AutoString defaultPath, bool multiSelect, int *resultCount);
	AutoString ShowSaveFile(AutoString title, AutoString defaultPath, AutoString* filters, int filterCount, AutoString defaultFileName = nullptr);
	DialogResult ShowMessage(AutoString title, AutoString text, DialogButtons buttons, DialogIcon icon);

protected:
#ifdef __APPLE__
	NSImage *_errorIcon;
	NSImage *_infoIcon;
	NSImage *_questionIcon;
	NSImage *_warningIcon;
#elif _WIN32
	InfiniFrame *_window;
#endif
};
