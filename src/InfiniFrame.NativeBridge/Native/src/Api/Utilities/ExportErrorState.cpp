#include "ExportErrorState.h"

namespace infiniframe::exports {
    thread_local std::string g_lastErrorMessage;
    thread_local InteropStatus g_lastStatus = InteropStatus::Success;
}
