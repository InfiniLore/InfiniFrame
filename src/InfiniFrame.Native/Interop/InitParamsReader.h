#pragma once
/**
 * @file InitParamsReader.h
 * @brief Validates InfiniFrameInitParams before platform-specific window setup.
 */

#ifndef INFINIFRAME_INTEROP_INITPARAMSREADER_H
#define INFINIFRAME_INTEROP_INITPARAMSREADER_H

#include "../Core/InfiniFrameInitParams.h"

#include <stdexcept>
#include <string>

namespace InfiniFrame::Native::Interop {
    class InitParamsReader {
        public:
            explicit InitParamsReader(const InfiniFrameInitParams* initParams) :
                _params(initParams) {
                if (_params == nullptr)
                    throw std::invalid_argument("InfiniFrameInitParams pointer must not be null.");

                if (_params->Size != sizeof(InfiniFrameInitParams)) {
                    throw std::invalid_argument(
                        "Initial parameters passed are "
                        + std::to_string(_params->Size)
                        + " bytes, but expected "
                        + std::to_string(sizeof(InfiniFrameInitParams))
                        + " bytes."
                        );
                }
            }

            [[nodiscard]] const InfiniFrameInitParams& Params() const noexcept {
                return *_params;
            }

            [[nodiscard]] bool HasStartContent() const noexcept {
                return _params->StartUrl != nullptr || _params->StartString != nullptr;
            }

            void RequireStartContent() const {
                if (!HasStartContent())
                    throw std::invalid_argument("Either StartUrl or StartString must be specified.");
            }

        private:
            const InfiniFrameInitParams* _params;
    };
}

#endif // INFINIFRAME_INTEROP_INITPARAMSREADER_H
