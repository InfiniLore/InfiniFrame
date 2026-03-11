#pragma once

struct Monitor
{
	struct MonitorRect
	{
		int x, y;
		int width, height;
	} monitor, work;
	double scale;
};
