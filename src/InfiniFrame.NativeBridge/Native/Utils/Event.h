#pragma once
/**
 * @file Event.h
 * @brief Modern event handling system with thread safety
 */

#ifndef INFINIFRAME_EVENT_H
#define INFINIFRAME_EVENT_H

#include <functional>
#include <map>
#include <mutex>
#include <shared_mutex>
#include <vector>

// ---------------------------------------------------------------------------------------------------------------------
// Event System
// ---------------------------------------------------------------------------------------------------------------------

template <typename... Args> class Event {
public:
    using Handler = std::function<void(Args...)>;
    using Token = size_t;

    Event() = default;
    ~Event() = default;

    Event(const Event&) = delete;
    Event& operator=(const Event&) = delete;
    Event(Event&&) noexcept = default;
    Event& operator=(Event&&) noexcept = default;

    /**
         * @brief Subscribe to event
         * @param handler Callback function to invoke when event is raised
         * @return Token for unsubscribing
         */
    [[nodiscard]] Token Subscribe(Handler handler) {
        std::unique_lock<std::shared_mutex> lock(m_mutex);
        const auto token = m_nextToken++;
        m_handlers.emplace(token, std::move(handler));
        return token;
    }

    /**
         * @brief Unsubscribe from event
         * @param token Token returned from Subscribe
         */
    void Unsubscribe(Token token) {
        std::unique_lock<std::shared_mutex> lock(m_mutex);
        m_handlers.erase(token);
    }

    /**
         * @brief Raise event (invoke all handlers)
         * @param args Arguments to pass to handlers
         */
    void Raise(Args... args) {
        std::shared_lock<std::shared_mutex> lock(m_mutex);
        for (const auto& [_, handler] : m_handlers) {
            if (handler) {
                handler(args...);
            }
        }
    }

    /**
         * @brief Check if event has subscribers
         * @return true if at least one handler is subscribed
         */
    [[nodiscard]] bool HasSubscribers() const {
        std::shared_lock<std::shared_mutex> lock(m_mutex);
        return !m_handlers.empty();
    }

    /**
         * @brief Clear all subscribers
         */
    void Clear() {
        std::unique_lock<std::shared_mutex> lock(m_mutex);
        m_handlers.clear();
    }

private:
    mutable std::shared_mutex m_mutex;
    std::map<Token, Handler> m_handlers;
    Token m_nextToken = 1;
};

// ---------------------------------------------------------------------------------------------------------------------
// Event Subscription Guard
// ---------------------------------------------------------------------------------------------------------------------

template <typename... Args> class EventSubscription {
public:
    using EventType = Event<Args...>;
    using Token = EventType::Token;

    EventSubscription() = default;

    EventSubscription(EventType& event, EventType::Handler handler)
        : m_event(&event)
        , m_token(event.Subscribe(std::move(handler))) {}

    ~EventSubscription() {
        Unsubscribe();
    }

    EventSubscription(const EventSubscription&) = delete;
    EventSubscription& operator=(const EventSubscription&) = delete;

    EventSubscription(EventSubscription&& other) noexcept
        : m_event(other.m_event)
        , m_token(other.m_token) {
        other.m_event = nullptr;
        other.m_token = 0;
    }

    EventSubscription& operator=(EventSubscription&& other) noexcept {
        if (this != &other) {
            Unsubscribe();
            m_event = other.m_event;
            m_token = other.m_token;
            other.m_event = nullptr;
            other.m_token = 0;
        }
        return *this;
    }

    /**
         * @brief Manually unsubscribe from event
         */
    void Unsubscribe() {
        if (m_event && m_token != 0) {
            m_event->Unsubscribe(m_token);
            m_event = nullptr;
            m_token = 0;
        }
    }

    /**
         * @brief Check if subscription is active
         * @return true if still subscribed
         */
    [[nodiscard]] bool IsActive() const noexcept {
        return m_event != nullptr && m_token != 0;
    }

private:
    EventType* m_event = nullptr;
    Token m_token = 0;
};

#endif // INFINIFRAME_EVENT_H
