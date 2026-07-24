import {useEffect, useState} from 'react'
import Box from '@mui/material/Box'
import Container from '@mui/material/Container'
import Divider from '@mui/material/Divider'
import Stack from '@mui/material/Stack'
import Typography from '@mui/material/Typography'
import InputDataProbe from './InputDataProbe'
import OutputDataProbe from './OutputDataProbe'
import {defaultWindowTestState, windowTestResetEvent} from './windowTestState'

type Results = Record<string, string | undefined>
type FeatureProbe = {key: string, title: string, buttonText: string, readData: () => Promise<unknown>}

export default function WindowFeatureTestPanel() {
    const [title, setTitle] = useState<string>(defaultWindowTestState.title)
    const [titleInput, setTitleInput] = useState<string>(defaultWindowTestState.titleInput)
    const [fullscreenData, setFullscreenData] = useState<string>()
    const [results, setResults] = useState<Results>({})
    const features = () => (window as any).infiniframe.window.features

    useEffect(() => {
        const reset = () => {
            setTitle(defaultWindowTestState.title)
            setTitleInput(defaultWindowTestState.titleInput)
            setFullscreenData(undefined)
            setResults({})
        }
        window.addEventListener(windowTestResetEvent, reset)
        return () => window.removeEventListener(windowTestResetEvent, reset)
    }, [])

    const applyOrResetTitle = (value: string) => {
        const target = document.title === value ? defaultWindowTestState.title : value
        document.title = target
        setTitle(target)
    }

    const toggleFullscreen = async () => {
        if (document.fullscreenElement) await document.exitFullscreen()
        else await document.body.requestFullscreen()
        setFullscreenData(String(document.fullscreenElement !== null))
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

    const readFeature = async (probe: FeatureProbe) => {
        const data = JSON.stringify(await probe.readData())
        setResults(current => ({...current, [probe.key]: data}))
    }

    return <Container maxWidth={false} sx={{py: 4}}>
        <Stack spacing={3}>
            <Typography variant="h4">Window data exchange</Typography>
            <Typography>Input probes send entered data to InfiniFrame. Output probes read InfiniFrame data into their fields.</Typography>
            <Box sx={{display: 'grid', gridTemplateColumns: {xs: '1fr', md: 'repeat(2, 1fr)'}, gap: 3}}>
                <InputDataProbe title={title} titleId="current-window-title" buttonText="Apply title / reset" buttonId="title-toggle-button" dataInputId="title-data-input" dataLabel="Window title" data={titleInput} onDataChanged={setTitleInput} onSubmitted={applyOrResetTitle}/>
                <OutputDataProbe title="Fullscreen" buttonText="Toggle fullscreen" buttonId="fullscreen-toggle-button" dataInputId="fullscreen-data-result" dataLabel="Current fullscreen state" data={fullscreenData} onReadRequested={toggleFullscreen}/>
            </Box>
            <Divider/>
            <Typography variant="h5">Window feature readers</Typography>
            <Box id="window-feature-test-panel" sx={{display: 'grid', gridTemplateColumns: {xs: '1fr', md: 'repeat(2, 1fr)', lg: 'repeat(3, 1fr)'}, gap: 3}}>
                {probes.map(probe => <OutputDataProbe key={probe.key} title={probe.title} buttonText={probe.buttonText} buttonId={`probe-${probe.key}-feature`} dataInputId={`${probe.key}-feature-result`} dataLabel="Serialized window data" data={results[probe.key]} onReadRequested={() => readFeature(probe)}/>) }
            </Box>
        </Stack>
    </Container>
}
