import { onMount, useMetadata, useStore } from '@builder.io/mitosis'

useMetadata({
  angular: {
    selector: 'window-feature-test-panel',
    nativeAttributes: ['value', 'readOnly']
  }
})

export default function WindowFeatureTestPanel() {
  const state = useStore({
    defaultTitle: '',
    currentTitle: '',
    titleInput: 'New Title',
    fullscreenData: '',
    browserData: '',
    decorationsData: '',
    positionData: '',
    sizeData: '',
    windowStateData: '',
    lifecycleMonitorsData: '',
    features() {
      return (window as any).infiniframe.window.features
    },
    output(id: string, value: string) {
      const element = document.getElementById(id) as HTMLInputElement | null
      if (element) element.value = value
      return value
    },
    reset() {
      state.currentTitle = document.title
      state.titleInput = 'New Title'
      state.fullscreenData = ''
      state.browserData = ''
      state.decorationsData = ''
      state.positionData = ''
      state.sizeData = ''
      state.windowStateData = ''
      state.lifecycleMonitorsData = ''
    },
    applyOrResetTitle(value: string) {
      const target = document.title === value ? state.defaultTitle : value
      document.title = target
      state.currentTitle = target
    },
    async toggleFullscreen() {
      if (document.fullscreenElement) await document.exitFullscreen()
      else await document.body.requestFullscreen()
      state.fullscreenData = String(document.fullscreenElement !== null)
    },
    async readBrowser() {
      const feature = state.features().browser
      state.browserData = state.output('browser-feature-result', JSON.stringify({
        contextMenu: await feature.isContextMenuEnabledAsync(),
        mediaAutoplay: await feature.isMediaAutoplayEnabledAsync(),
        userAgent: await feature.getUserAgentAsync(),
        webSecurity: await feature.isWebSecurityEnabledAsync(),
        smoothScrolling: await feature.isSmoothScrollingEnabledAsync()
      }))
    },
    async readDecorations() {
      const feature = state.features().decorations
      state.decorationsData = state.output('decorations-feature-result', JSON.stringify({
        chromeless: await feature.isChromelessAsync(),
        transparent: await feature.isTransparentAsync(),
        title: await feature.getTitleAsync(),
        limitLinuxTitle: await feature.getLimitLinuxWindowTitleLengthAsync()
      }))
    },
    async readPosition() {
      const feature = state.features().position
      state.positionData = state.output('position-feature-result', JSON.stringify({
        location: await feature.getLocationAsync(),
        top: await feature.getTopAsync(),
        left: await feature.getLeftAsync()
      }))
    },
    async readSize() {
      const feature = state.features().size
      state.sizeData = state.output('size-feature-result', JSON.stringify({
        size: await feature.getSizeAsync(),
        width: await feature.getWidthAsync(),
        height: await feature.getHeightAsync(),
        resizable: await feature.isResizableAsync()
      }))
    },
    async readState() {
      const feature = state.features()['state']
      state.windowStateData = state.output('state-feature-result', JSON.stringify({
        fullScreen: await feature.isFullScreenAsync(),
        maximized: await feature.isMaximizedAsync(),
        minimized: await feature.isMinimizedAsync(),
        topMost: await feature.isTopMostAsync(),
        zoomFactor: await feature.getZoomFactorAsync(),
        zoomEnabled: await feature.isZoomEnabledAsync()
      }))
    },
    async readLifecycleMonitors() {
      state.lifecycleMonitorsData = state.output('lifecycle-monitors-feature-result', JSON.stringify({
        closedOrClosing: await state.features().lifecycle.isClosedOrClosingAsync(),
        dpi: await state.features().monitors.getMainMonitorScreenDpiAsync()
      }))
    }
  })

  onMount(() => {
    state.defaultTitle = document.title
    state.currentTitle = document.title
    window.addEventListener('infiniframe:test-reset', () => state.reset())
  })

  return <main class="test-panel">
    <header>
      <h1>Window data exchange</h1>
      <p>Input probes send entered data to InfiniFrame. Output probes read InfiniFrame data into their fields.</p>
    </header>
    <div class="probe-grid probe-grid-two">
      <section class="data-probe">
        <h2 id="current-window-title">{state.currentTitle}</h2>
        <label class="data-probe-field"><span>Window title</span>
          <input id="title-data-input" value={state.titleInput}
            onInput={(event) => state.titleInput = event.target.value}/>
        </label>
        <button id="title-toggle-button" onClick={() => state.applyOrResetTitle(state.titleInput)}>Apply title / reset</button>
      </section>
      <section class="data-probe">
        <h2 class="output-data-probe-title">Fullscreen</h2>
        <label class="data-probe-field"><span>Current fullscreen state</span>
          <input id="fullscreen-data-result" value={state.fullscreenData} readOnly={true}/>
        </label>
        <button id="fullscreen-toggle-button" onClick={() => state.toggleFullscreen()}>Toggle fullscreen</button>
      </section>
    </div>
    <hr/>
    <h2>Window feature readers</h2>
    <div id="window-feature-test-panel" class="probe-grid probe-grid-three">
      <section class="data-probe"><h2 class="output-data-probe-title">Browser</h2>
        <label class="data-probe-field"><span>Serialized window data</span><input id="browser-feature-result" value={state.browserData} readOnly={true}/></label>
        <button id="probe-browser-feature" onClick={() => state.readBrowser()}>Read browser</button>
      </section>
      <section class="data-probe"><h2 class="output-data-probe-title">Decorations</h2>
        <label class="data-probe-field"><span>Serialized window data</span><input id="decorations-feature-result" value={state.decorationsData} readOnly={true}/></label>
        <button id="probe-decorations-feature" onClick={() => state.readDecorations()}>Read decorations</button>
      </section>
      <section class="data-probe"><h2 class="output-data-probe-title">Position</h2>
        <label class="data-probe-field"><span>Serialized window data</span><input id="position-feature-result" value={state.positionData} readOnly={true}/></label>
        <button id="probe-position-feature" onClick={() => state.readPosition()}>Read position</button>
      </section>
      <section class="data-probe"><h2 class="output-data-probe-title">Size</h2>
        <label class="data-probe-field"><span>Serialized window data</span><input id="size-feature-result" value={state.sizeData} readOnly={true}/></label>
        <button id="probe-size-feature" onClick={() => state.readSize()}>Read size</button>
      </section>
      <section class="data-probe"><h2 class="output-data-probe-title">State</h2>
        <label class="data-probe-field"><span>Serialized window data</span><input id="state-feature-result" value={state.windowStateData} readOnly={true}/></label>
        <button id="probe-state-feature" onClick={() => state.readState()}>Read state</button>
      </section>
      <section class="data-probe"><h2 class="output-data-probe-title">Lifecycle and monitors</h2>
        <label class="data-probe-field"><span>Serialized window data</span><input id="lifecycle-monitors-feature-result" value={state.lifecycleMonitorsData} readOnly={true}/></label>
        <button id="probe-lifecycle-monitors-feature" onClick={() => state.readLifecycleMonitors()}>Read lifecycle and monitors</button>
      </section>
    </div>
  </main>
}
