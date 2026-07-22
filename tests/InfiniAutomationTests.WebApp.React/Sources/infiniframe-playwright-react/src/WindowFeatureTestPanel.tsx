import {useState} from 'react'

function WindowFeatureTestPanel() {
    const [results, setResults] = useState<Record<string, string>>({})
    const features = () => (window as any).infiniframe.window.features

    const save = (feature: string, value: unknown) =>
        setResults(current => ({...current, [feature]: JSON.stringify(value)}))

    const probeBrowser = async () => {
        const feature = features().browser
        save('browser', {
            contextMenu: await feature.isContextMenuEnabledAsync(),
            mediaAutoplay: await feature.isMediaAutoplayEnabledAsync(),
            userAgent: await feature.getUserAgentAsync(),
            webSecurity: await feature.isWebSecurityEnabledAsync(),
            smoothScrolling: await feature.isSmoothScrollingEnabledAsync()
        })
    }

    const probeDecorations = async () => {
        const feature = features().decorations
        save('decorations', {
            chromeless: await feature.isChromelessAsync(),
            transparent: await feature.isTransparentAsync(),
            title: await feature.getTitleAsync(),
            limitLinuxTitle: await feature.getLimitLinuxWindowTitleLengthAsync()
        })
    }

    const probePosition = async () => {
        const feature = features().position
        save('position', {
            location: await feature.getLocationAsync(),
            top: await feature.getTopAsync(),
            left: await feature.getLeftAsync()
        })
    }

    const probeSize = async () => {
        const feature = features().size
        save('size', {
            size: await feature.getSizeAsync(),
            width: await feature.getWidthAsync(),
            height: await feature.getHeightAsync(),
            resizable: await feature.isResizableAsync()
        })
    }

    const probeState = async () => {
        const feature = features().state
        save('state', {
            fullScreen: await feature.isFullScreenAsync(),
            maximized: await feature.isMaximizedAsync(),
            minimized: await feature.isMinimizedAsync(),
            topMost: await feature.isTopMostAsync(),
            zoomFactor: await feature.getZoomFactorAsync(),
            zoomEnabled: await feature.isZoomEnabledAsync()
        })
    }

    const probeLifecycleAndMonitors = async () => save('lifecycle-monitors', {
        closedOrClosing: await features().lifecycle.isClosedOrClosingAsync(),
        dpi: await features().monitors.getMainMonitorScreenDpiAsync()
    })

    return <section id="window-feature-test-panel">
        <button id="probe-browser-feature" onClick={probeBrowser}>Probe browser</button>
        <output id="browser-feature-result">{results.browser}</output>
        <button id="probe-decorations-feature" onClick={probeDecorations}>Probe decorations</button>
        <output id="decorations-feature-result">{results.decorations}</output>
        <button id="probe-position-feature" onClick={probePosition}>Probe position</button>
        <output id="position-feature-result">{results.position}</output>
        <button id="probe-size-feature" onClick={probeSize}>Probe size</button>
        <output id="size-feature-result">{results.size}</output>
        <button id="probe-state-feature" onClick={probeState}>Probe state</button>
        <output id="state-feature-result">{results.state}</output>
        <button id="probe-lifecycle-monitors-feature" onClick={probeLifecycleAndMonitors}>Probe lifecycle and monitors</button>
        <output id="lifecycle-monitors-feature-result">{results['lifecycle-monitors']}</output>
    </section>
}

export default WindowFeatureTestPanel
