export interface Envelope {
    id: string;
    command: string;
    data?: unknown;
    version: number;
    requestId?: string;
    channel?: string;
}

export interface RawEnvelope {
    id?: string;
    command?: string;
    data?: unknown;
    requestId?: string;
    channel?: string;
}

export function normalizeEnvelope(
    envelope: RawEnvelope,
    command?: string,
    requestId?: string
): Envelope | null {
    if (!envelope || typeof envelope !== 'object') return null;
    if (typeof envelope.id !== 'string' || envelope.id.trim().length === 0) return null;

    const normalized: Envelope = {
        id: envelope.id,
        command: command || envelope.command || 'Post',
        data: envelope.data,
        version: 2,
    };

    const resolvedRequestId = requestId || envelope.requestId;
    if (typeof resolvedRequestId === 'string' && resolvedRequestId.length > 0) {
        normalized.requestId = resolvedRequestId;
    }

    if (typeof envelope.channel === 'string' && envelope.channel.trim().length > 0) {
        normalized.channel = envelope.channel;
    }

    return normalized;
}

export function createGetEnvelope(
    message: string | RawEnvelope,
    requestId?: string
): Envelope | null {
    if (typeof message !== 'string') {
        return normalizeEnvelope(message, 'Get', requestId);
    }

    const trimmed = message.trim();
    if (!trimmed) return null;

    try {
        const parsed = JSON.parse(trimmed);
        if (parsed && typeof parsed === 'object') {
            return normalizeEnvelope(parsed as RawEnvelope, 'Get', requestId);
        }
    } catch {
        // Plain strings are treated as get message IDs.
    }

    return normalizeEnvelope({ id: trimmed }, 'Get', requestId);
}
