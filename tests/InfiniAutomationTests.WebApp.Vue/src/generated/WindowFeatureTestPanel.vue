<template>
  <main class="test-panel">
    <header>
      <h1>Window data exchange</h1>
      <p>
        Input probes send entered data to InfiniFrame. Output probes read
        InfiniFrame data into their fields.
      </p>
    </header>
    <div class="probe-grid probe-grid-two">
      <section class="data-probe">
        <h2 id="current-window-title">{{ currentTitle }}</h2>
        <label class="data-probe-field"
          ><span>Window title</span
          ><input
            id="title-data-input"
            :value="titleInput"
            @input="
              async (event) => (titleInput = event.target.value)
            " /></label
        ><button
          id="title-toggle-button"
          @click="async (event) => applyOrResetTitle(titleInput)"
        >
          Apply title / reset
        </button>
      </section>
      <section class="data-probe">
        <h2 class="output-data-probe-title">Fullscreen</h2>
        <label class="data-probe-field"
          ><span>Current fullscreen state</span
          ><input
            id="fullscreen-data-result"
            :value="fullscreenData"
            :readOnly="true" /></label
        ><button
          id="fullscreen-toggle-button"
          @click="async (event) => toggleFullscreen()"
        >
          Toggle fullscreen
        </button>
      </section>
    </div>
    <hr />
    <h2>Window feature readers</h2>
    <div id="window-feature-test-panel" class="probe-grid probe-grid-three">
      <section class="data-probe">
        <h2 class="output-data-probe-title">Browser</h2>
        <label class="data-probe-field"
          ><span>Serialized window data</span
          ><input
            id="browser-feature-result"
            :value="browserData"
            :readOnly="true" /></label
        ><button
          id="probe-browser-feature"
          @click="async (event) => readBrowser()"
        >
          Read browser
        </button>
      </section>
      <section class="data-probe">
        <h2 class="output-data-probe-title">Decorations</h2>
        <label class="data-probe-field"
          ><span>Serialized window data</span
          ><input
            id="decorations-feature-result"
            :value="decorationsData"
            :readOnly="true" /></label
        ><button
          id="probe-decorations-feature"
          @click="async (event) => readDecorations()"
        >
          Read decorations
        </button>
      </section>
      <section class="data-probe">
        <h2 class="output-data-probe-title">Position</h2>
        <label class="data-probe-field"
          ><span>Serialized window data</span
          ><input
            id="position-feature-result"
            :value="positionData"
            :readOnly="true" /></label
        ><button
          id="probe-position-feature"
          @click="async (event) => readPosition()"
        >
          Read position
        </button>
      </section>
      <section class="data-probe">
        <h2 class="output-data-probe-title">Size</h2>
        <label class="data-probe-field"
          ><span>Serialized window data</span
          ><input
            id="size-feature-result"
            :value="sizeData"
            :readOnly="true" /></label
        ><button id="probe-size-feature" @click="async (event) => readSize()">
          Read size
        </button>
      </section>
      <section class="data-probe">
        <h2 class="output-data-probe-title">State</h2>
        <label class="data-probe-field"
          ><span>Serialized window data</span
          ><input
            id="state-feature-result"
            :value="windowStateData"
            :readOnly="true" /></label
        ><button id="probe-state-feature" @click="async (event) => readState()">
          Read state
        </button>
      </section>
      <section class="data-probe">
        <h2 class="output-data-probe-title">Lifecycle and monitors</h2>
        <label class="data-probe-field"
          ><span>Serialized window data</span
          ><input
            id="lifecycle-monitors-feature-result"
            :value="lifecycleMonitorsData"
            :readOnly="true" /></label
        ><button
          id="probe-lifecycle-monitors-feature"
          @click="async (event) => readLifecycleMonitors()"
        >
          Read lifecycle and monitors
        </button>
      </section>
    </div>
  </main>
</template>

<script setup lang="ts">
// @ts-nocheck
import { onMounted, ref } from "vue";

const defaultTitle = ref("");
const currentTitle = ref("");
const titleInput = ref("New Title");
const fullscreenData = ref("");
const browserData = ref("");
const decorationsData = ref("");
const positionData = ref("");
const sizeData = ref("");
const windowStateData = ref("");
const lifecycleMonitorsData = ref("");

onMounted(() => {
  defaultTitle.value = document.title;
  currentTitle.value = document.title;
  window.addEventListener("infiniframe:test-reset", () => reset());
});

function features() {
  return (window as any).infiniframe.window.features;
}
function output(id: string, value: string) {
  const element = document.getElementById(id) as HTMLInputElement | null;
  if (element) element.value = value;
  return value;
}
function reset() {
  currentTitle.value = document.title;
  titleInput.value = "New Title";
  fullscreenData.value = "";
  browserData.value = "";
  decorationsData.value = "";
  positionData.value = "";
  sizeData.value = "";
  windowStateData.value = "";
  lifecycleMonitorsData.value = "";
}
function applyOrResetTitle(value: string) {
  const target = document.title === value ? defaultTitle.value : value;
  document.title = target;
  currentTitle.value = target;
}
async function toggleFullscreen() {
  if (document.fullscreenElement) await document.exitFullscreen();
  else await document.body.requestFullscreen();
  fullscreenData.value = String(document.fullscreenElement !== null);
}
async function readBrowser() {
  const feature = features().browser;
  browserData.value = output(
    "browser-feature-result",
    JSON.stringify({
      contextMenu: await feature.isContextMenuEnabledAsync(),
      mediaAutoplay: await feature.isMediaAutoplayEnabledAsync(),
      userAgent: await feature.getUserAgentAsync(),
      webSecurity: await feature.isWebSecurityEnabledAsync(),
      smoothScrolling: await feature.isSmoothScrollingEnabledAsync(),
    })
  );
}
async function readDecorations() {
  const feature = features().decorations;
  decorationsData.value = output(
    "decorations-feature-result",
    JSON.stringify({
      chromeless: await feature.isChromelessAsync(),
      transparent: await feature.isTransparentAsync(),
      title: await feature.getTitleAsync(),
      limitLinuxTitle: await feature.getLimitLinuxWindowTitleLengthAsync(),
    })
  );
}
async function readPosition() {
  const feature = features().position;
  positionData.value = output(
    "position-feature-result",
    JSON.stringify({
      location: await feature.getLocationAsync(),
      top: await feature.getTopAsync(),
      left: await feature.getLeftAsync(),
    })
  );
}
async function readSize() {
  const feature = features().size;
  sizeData.value = output(
    "size-feature-result",
    JSON.stringify({
      size: await feature.getSizeAsync(),
      width: await feature.getWidthAsync(),
      height: await feature.getHeightAsync(),
      resizable: await feature.isResizableAsync(),
    })
  );
}
async function readState() {
  const feature = features()["state"];
  windowStateData.value = output(
    "state-feature-result",
    JSON.stringify({
      fullScreen: await feature.isFullScreenAsync(),
      maximized: await feature.isMaximizedAsync(),
      minimized: await feature.isMinimizedAsync(),
      topMost: await feature.isTopMostAsync(),
      zoomFactor: await feature.getZoomFactorAsync(),
      zoomEnabled: await feature.isZoomEnabledAsync(),
    })
  );
}
async function readLifecycleMonitors() {
  lifecycleMonitorsData.value = output(
    "lifecycle-monitors-feature-result",
    JSON.stringify({
      closedOrClosing: await features().lifecycle.isClosedOrClosingAsync(),
      dpi: await features().monitors.getMainMonitorScreenDpiAsync(),
    })
  );
}
</script>