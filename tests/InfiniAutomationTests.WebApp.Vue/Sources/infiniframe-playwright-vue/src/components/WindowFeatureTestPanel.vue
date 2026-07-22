<script lang="ts" setup>
import {reactive} from 'vue'

const results = reactive<Record<string, string>>({})
const features = () => (window as any).infiniframe.window.features
const save = (feature: string, value: unknown) => results[feature] = JSON.stringify(value)

async function probeBrowser() {
    const feature = features().browser
    save('browser', {
        contextMenu: await feature.isContextMenuEnabledAsync(), mediaAutoplay: await feature.isMediaAutoplayEnabledAsync(),
        userAgent: await feature.getUserAgentAsync(), webSecurity: await feature.isWebSecurityEnabledAsync(),
        smoothScrolling: await feature.isSmoothScrollingEnabledAsync()
    })
}

async function probeDecorations() {
    const feature = features().decorations
    save('decorations', {
        chromeless: await feature.isChromelessAsync(), transparent: await feature.isTransparentAsync(),
        title: await feature.getTitleAsync(), limitLinuxTitle: await feature.getLimitLinuxWindowTitleLengthAsync()
    })
}

async function probePosition() {
    const feature = features().position
    save('position', {location: await feature.getLocationAsync(), top: await feature.getTopAsync(), left: await feature.getLeftAsync()})
}

async function probeSize() {
    const feature = features().size
    save('size', {size: await feature.getSizeAsync(), width: await feature.getWidthAsync(), height: await feature.getHeightAsync(), resizable: await feature.isResizableAsync()})
}

async function probeState() {
    const feature = features().state
    save('state', {
        fullScreen: await feature.isFullScreenAsync(), maximized: await feature.isMaximizedAsync(),
        minimized: await feature.isMinimizedAsync(), topMost: await feature.isTopMostAsync(),
        zoomFactor: await feature.getZoomFactorAsync(), zoomEnabled: await feature.isZoomEnabledAsync()
    })
}

async function probeLifecycleAndMonitors() {
    save('lifecycle-monitors', {
        closedOrClosing: await features().lifecycle.isClosedOrClosingAsync(),
        dpi: await features().monitors.getMainMonitorScreenDpiAsync()
    })
}
</script>

<template>
    <section id="window-feature-test-panel">
        <button id="probe-browser-feature" @click="probeBrowser">Probe browser</button>
        <output id="browser-feature-result">{{ results.browser }}</output>
        <button id="probe-decorations-feature" @click="probeDecorations">Probe decorations</button>
        <output id="decorations-feature-result">{{ results.decorations }}</output>
        <button id="probe-position-feature" @click="probePosition">Probe position</button>
        <output id="position-feature-result">{{ results.position }}</output>
        <button id="probe-size-feature" @click="probeSize">Probe size</button>
        <output id="size-feature-result">{{ results.size }}</output>
        <button id="probe-state-feature" @click="probeState">Probe state</button>
        <output id="state-feature-result">{{ results.state }}</output>
        <button id="probe-lifecycle-monitors-feature" @click="probeLifecycleAndMonitors">Probe lifecycle and monitors</button>
        <output id="lifecycle-monitors-feature-result">{{ results['lifecycle-monitors'] }}</output>
    </section>
</template>
