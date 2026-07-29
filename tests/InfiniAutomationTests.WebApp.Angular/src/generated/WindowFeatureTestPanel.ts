import { Component } from "@angular/core";

import { CommonModule } from "@angular/common";

@Component({
  selector: "window-feature-test-panel",
  template: `
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
          <h2 id="current-window-title">{{currentTitle}}</h2>
          <label class="data-probe-field"
            ><span>Window title</span>
            <input
              id="title-data-input"
              [value]="titleInput"
              (input)="titleInput = $event.target.value"
          /></label>
          <button
            id="title-toggle-button"
            (click)="applyOrResetTitle(titleInput)"
          >
            Apply title / reset
          </button>
        </section>
        <section class="data-probe">
          <h2 class="output-data-probe-title">Fullscreen</h2>
          <label class="data-probe-field"
            ><span>Current fullscreen state</span>
            <input
              id="fullscreen-data-result"
              [value]="fullscreenData"
              [readOnly]="true"
          /></label>
          <button id="fullscreen-toggle-button" (click)="toggleFullscreen()">
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
            ><span>Serialized window data</span>
            <input
              id="browser-feature-result"
              [value]="browserData"
              [readOnly]="true"
          /></label>
          <button id="probe-browser-feature" (click)="readBrowser()">
            Read browser
          </button>
        </section>
        <section class="data-probe">
          <h2 class="output-data-probe-title">Decorations</h2>
          <label class="data-probe-field"
            ><span>Serialized window data</span>
            <input
              id="decorations-feature-result"
              [value]="decorationsData"
              [readOnly]="true"
          /></label>
          <button id="probe-decorations-feature" (click)="readDecorations()">
            Read decorations
          </button>
        </section>
        <section class="data-probe">
          <h2 class="output-data-probe-title">Position</h2>
          <label class="data-probe-field"
            ><span>Serialized window data</span>
            <input
              id="position-feature-result"
              [value]="positionData"
              [readOnly]="true"
          /></label>
          <button id="probe-position-feature" (click)="readPosition()">
            Read position
          </button>
        </section>
        <section class="data-probe">
          <h2 class="output-data-probe-title">Size</h2>
          <label class="data-probe-field"
            ><span>Serialized window data</span>
            <input
              id="size-feature-result"
              [value]="sizeData"
              [readOnly]="true"
          /></label>
          <button id="probe-size-feature" (click)="readSize()">
            Read size
          </button>
        </section>
        <section class="data-probe">
          <h2 class="output-data-probe-title">State</h2>
          <label class="data-probe-field"
            ><span>Serialized window data</span>
            <input
              id="state-feature-result"
              [value]="windowStateData"
              [readOnly]="true"
          /></label>
          <button id="probe-state-feature" (click)="readState()">
            Read state
          </button>
        </section>
        <section class="data-probe">
          <h2 class="output-data-probe-title">Lifecycle and monitors</h2>
          <label class="data-probe-field"
            ><span>Serialized window data</span>
            <input
              id="lifecycle-monitors-feature-result"
              [value]="lifecycleMonitorsData"
              [readOnly]="true"
          /></label>
          <button
            id="probe-lifecycle-monitors-feature"
            (click)="readLifecycleMonitors()"
          >
            Read lifecycle and monitors
          </button>
        </section>
      </div>
    </main>
  `,
  styles: [
    `
      :host {
        display: contents;
      }
    `,
  ],
  standalone: true,
  imports: [CommonModule],
})
export default class WindowFeatureTestPanel {
  defaultTitle = "";
  currentTitle = "";
  titleInput = "New Title";
  fullscreenData = "";
  browserData = "";
  decorationsData = "";
  positionData = "";
  sizeData = "";
  windowStateData = "";
  lifecycleMonitorsData = "";
  features() {
    return (window as any).infiniframe.window.features;
  }
  output(id: string, value: string) {
    const element = document.getElementById(id) as HTMLInputElement | null;
    if (element) element.value = value;
    return value;
  }
  reset() {
    this.currentTitle = document.title;
    this.titleInput = "New Title";
    this.fullscreenData = "";
    this.browserData = "";
    this.decorationsData = "";
    this.positionData = "";
    this.sizeData = "";
    this.windowStateData = "";
    this.lifecycleMonitorsData = "";
  }
  applyOrResetTitle(value: string) {
    const target = document.title === value ? this.defaultTitle : value;
    document.title = target;
    this.currentTitle = target;
  }
  toggleFullscreen = async function toggleFullscreen() {
    if (document.fullscreenElement) await document.exitFullscreen();
    else await document.body.requestFullscreen();
    this.fullscreenData = String(document.fullscreenElement !== null);
  };
  readBrowser = async function readBrowser() {
    const feature = this.features().browser;
    this.browserData = this.output(
      "browser-feature-result",
      JSON.stringify({
        contextMenu: await feature.isContextMenuEnabledAsync(),
        mediaAutoplay: await feature.isMediaAutoplayEnabledAsync(),
        userAgent: await feature.getUserAgentAsync(),
        webSecurity: await feature.isWebSecurityEnabledAsync(),
        smoothScrolling: await feature.isSmoothScrollingEnabledAsync(),
      })
    );
  };
  readDecorations = async function readDecorations() {
    const feature = this.features().decorations;
    this.decorationsData = this.output(
      "decorations-feature-result",
      JSON.stringify({
        chromeless: await feature.isChromelessAsync(),
        transparent: await feature.isTransparentAsync(),
        title: await feature.getTitleAsync(),
        limitLinuxTitle: await feature.getLimitLinuxWindowTitleLengthAsync(),
      })
    );
  };
  readPosition = async function readPosition() {
    const feature = this.features().position;
    this.positionData = this.output(
      "position-feature-result",
      JSON.stringify({
        location: await feature.getLocationAsync(),
        top: await feature.getTopAsync(),
        left: await feature.getLeftAsync(),
      })
    );
  };
  readSize = async function readSize() {
    const feature = this.features().size;
    this.sizeData = this.output(
      "size-feature-result",
      JSON.stringify({
        size: await feature.getSizeAsync(),
        width: await feature.getWidthAsync(),
        height: await feature.getHeightAsync(),
        resizable: await feature.isResizableAsync(),
      })
    );
  };
  readState = async function readState() {
    const feature = this.features()["state"];
    this.windowStateData = this.output(
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
  };
  readLifecycleMonitors = async function readLifecycleMonitors() {
    this.lifecycleMonitorsData = this.output(
      "lifecycle-monitors-feature-result",
      JSON.stringify({
        closedOrClosing:
          await this.features().lifecycle.isClosedOrClosingAsync(),
        dpi: await this.features().monitors.getMainMonitorScreenDpiAsync(),
      })
    );
  };

  ngOnInit() {
    if (typeof window !== "undefined") {
      this.defaultTitle = document.title;
      this.currentTitle = document.title;
      window.addEventListener("infiniframe:test-reset", () => this.reset());
    }
  }
}
