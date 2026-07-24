<script lang="ts" setup>
import {onBeforeUnmount, onMounted, reactive, ref} from 'vue'
import InputDataProbe from './InputDataProbe.vue'
import OutputDataProbe from './OutputDataProbe.vue'
import {defaultWindowTestState, windowTestResetEvent} from '../windowTestState'

type FeatureProbe = {key: string, title: string, buttonText: string, readData: () => Promise<unknown>}

const title = ref<string>(defaultWindowTestState.title)
const titleInput = ref<string>(defaultWindowTestState.titleInput)
const fullscreenData = ref<string>()
const results = reactive<Record<string, string | undefined>>({})
const features = () => (window as any).infiniframe.window.features

function applyOrResetTitle(value: string) {
    const target = document.title === value ? defaultWindowTestState.title : value
    document.title = target
    title.value = target
}

async function toggleFullscreen() {
    if (document.fullscreenElement) await document.exitFullscreen()
    else await document.body.requestFullscreen()
    fullscreenData.value = String(document.fullscreenElement !== null)
}

const probes: FeatureProbe[] = [
    {key: 'browser', title: 'Browser', buttonText: 'Read browser', readData: async () => {
        const feature = features().browser
        return {contextMenu: await feature.isContextMenuEnabledAsync(), mediaAutoplay: await feature.isMediaAutoplayEnabledAsync(), userAgent: await feature.getUserAgentAsync(), webSecurity: await feature.isWebSecurityEnabledAsync(), smoothScrolling: await feature.isSmoothScrollingEnabledAsync()}
    }},
    {key: 'decorations', title: 'Decorations', buttonText: 'Read decorations', readData: async () => {
        const feature = features().decorations
        return {chromeless: await feature.isChromelessAsync(), transparent: await feature.isTransparentAsync(), title: await feature.getTitleAsync(), limitLinuxTitle: await feature.getLimitLinuxWindowTitleLengthAsync()}
    }},
    {key: 'position', title: 'Position', buttonText: 'Read position', readData: async () => {
        const feature = features().position
        return {location: await feature.getLocationAsync(), top: await feature.getTopAsync(), left: await feature.getLeftAsync()}
    }},
    {key: 'size', title: 'Size', buttonText: 'Read size', readData: async () => {
        const feature = features().size
        return {size: await feature.getSizeAsync(), width: await feature.getWidthAsync(), height: await feature.getHeightAsync(), resizable: await feature.isResizableAsync()}
    }},
    {key: 'state', title: 'State', buttonText: 'Read state', readData: async () => {
        const feature = features().state
        return {fullScreen: await feature.isFullScreenAsync(), maximized: await feature.isMaximizedAsync(), minimized: await feature.isMinimizedAsync(), topMost: await feature.isTopMostAsync(), zoomFactor: await feature.getZoomFactorAsync(), zoomEnabled: await feature.isZoomEnabledAsync()}
    }},
    {key: 'lifecycle-monitors', title: 'Lifecycle and monitors', buttonText: 'Read lifecycle and monitors', readData: async () => ({closedOrClosing: await features().lifecycle.isClosedOrClosingAsync(), dpi: await features().monitors.getMainMonitorScreenDpiAsync()})}
]

async function readFeature(probe: FeatureProbe) {
    results[probe.key] = JSON.stringify(await probe.readData())
}

function reset() {
    title.value = defaultWindowTestState.title
    titleInput.value = defaultWindowTestState.titleInput
    fullscreenData.value = undefined
    for (const key of Object.keys(results)) delete results[key]
}

onMounted(() => window.addEventListener(windowTestResetEvent, reset))
onBeforeUnmount(() => window.removeEventListener(windowTestResetEvent, reset))
</script>

<template>
    <v-container fluid class="pa-6">
        <v-row>
            <v-col cols="12"><v-sheet class="text-h4">Window data exchange</v-sheet></v-col>
            <v-col cols="12"><v-sheet>Input probes send entered data to InfiniFrame. Output probes read InfiniFrame data into their fields.</v-sheet></v-col>
            <v-col cols="12" md="6">
                <InputDataProbe v-model="titleInput" :title="title" title-id="current-window-title" button-text="Apply title / reset" button-id="title-toggle-button" data-input-id="title-data-input" data-label="Window title" @submitted="applyOrResetTitle"/>
            </v-col>
            <v-col cols="12" md="6">
                <OutputDataProbe title="Fullscreen" button-text="Toggle fullscreen" button-id="fullscreen-toggle-button" data-input-id="fullscreen-data-result" data-label="Current fullscreen state" :data="fullscreenData" @read-requested="toggleFullscreen"/>
            </v-col>
            <v-col cols="12"><v-divider/></v-col>
            <v-col cols="12"><v-sheet class="text-h5">Window feature readers</v-sheet></v-col>
        </v-row>
        <v-row id="window-feature-test-panel">
            <v-col v-for="probe in probes" :key="probe.key" cols="12" md="6" lg="4">
                <OutputDataProbe :title="probe.title" :button-text="probe.buttonText" :button-id="`probe-${probe.key}-feature`" :data-input-id="`${probe.key}-feature-result`" data-label="Serialized window data" :data="results[probe.key]" @read-requested="readFeature(probe)"/>
            </v-col>
        </v-row>
    </v-container>
</template>
