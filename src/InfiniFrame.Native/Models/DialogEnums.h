#pragma once

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
