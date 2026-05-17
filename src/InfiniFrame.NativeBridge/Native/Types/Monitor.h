#pragma once

#ifndef INFINIFRAME_TYPES_MONITOR_H
#define INFINIFRAME_TYPES_MONITOR_H

struct Monitor {
    struct MonitorRect {
        int x, y;
        int width, height;
    } monitor,
      work;
    double scale;
};

#endif // INFINIFRAME_TYPES_MONITOR_H
