// @ts-nocheck
"use client";
import * as React from "react";
import { useState, useEffect } from "react";

function WindowFeatureTestPanel(props: any) {
  const [defaultTitle, setDefaultTitle] = useState(() => "");

  const [currentTitle, setCurrentTitle] = useState(() => "");

  const [titleInput, setTitleInput] = useState(() => "New Title");

  const [fullscreenData, setFullscreenData] = useState(() => "");

  const [browserData, setBrowserData] = useState(() => "");

  const [decorationsData, setDecorationsData] = useState(() => "");

  const [positionData, setPositionData] = useState(() => "");

  const [sizeData, setSizeData] = useState(() => "");

  const [windowStateData, setWindowStateData] = useState(() => "");

  const [lifecycleMonitorsData, setLifecycleMonitorsData] = useState(() => "");

  function features() {
    return (window as any).infiniframe.window.features;
  }

  function output(id: string, value: string) {
    const element = document.getElementById(id) as HTMLInputElement | null;
    if (element) element.value = value;
    return value;
  }

  function reset() {
    setCurrentTitle(document.title);
    setTitleInput("New Title");
    setFullscreenData("");
    setBrowserData("");
    setDecorationsData("");
    setPositionData("");
    setSizeData("");
    setWindowStateData("");
    setLifecycleMonitorsData("");
  }

  function applyOrResetTitle(value: string) {
    const target = document.title === value ? defaultTitle : value;
    document.title = target;
    setCurrentTitle(target);
  }

  async function toggleFullscreen() {
    if (document.fullscreenElement) await document.exitFullscreen();
    else await document.body.requestFullscreen();
    setFullscreenData(String(document.fullscreenElement !== null));
  }

  async function readBrowser() {
    const feature = features().browser;
    setBrowserData(
      output(
        "browser-feature-result",
        JSON.stringify({
          contextMenu: await feature.isContextMenuEnabledAsync(),
          mediaAutoplay: await feature.isMediaAutoplayEnabledAsync(),
          userAgent: await feature.getUserAgentAsync(),
          webSecurity: await feature.isWebSecurityEnabledAsync(),
          smoothScrolling: await feature.isSmoothScrollingEnabledAsync(),
        })
      )
    );
  }

  async function readDecorations() {
    const feature = features().decorations;
    setDecorationsData(
      output(
        "decorations-feature-result",
        JSON.stringify({
          chromeless: await feature.isChromelessAsync(),
          transparent: await feature.isTransparentAsync(),
          title: await feature.getTitleAsync(),
          limitLinuxTitle: await feature.getLimitLinuxWindowTitleLengthAsync(),
        })
      )
    );
  }

  async function readPosition() {
    const feature = features().position;
    setPositionData(
      output(
        "position-feature-result",
        JSON.stringify({
          location: await feature.getLocationAsync(),
          top: await feature.getTopAsync(),
          left: await feature.getLeftAsync(),
        })
      )
    );
  }

  async function readSize() {
    const feature = features().size;
    setSizeData(
      output(
        "size-feature-result",
        JSON.stringify({
          size: await feature.getSizeAsync(),
          width: await feature.getWidthAsync(),
          height: await feature.getHeightAsync(),
          resizable: await feature.isResizableAsync(),
        })
      )
    );
  }

  async function readState() {
    const feature = features()["state"];
    setWindowStateData(
      output(
        "state-feature-result",
        JSON.stringify({
          fullScreen: await feature.isFullScreenAsync(),
          maximized: await feature.isMaximizedAsync(),
          minimized: await feature.isMinimizedAsync(),
          topMost: await feature.isTopMostAsync(),
          zoomFactor: await feature.getZoomFactorAsync(),
          zoomEnabled: await feature.isZoomEnabledAsync(),
        })
      )
    );
  }

  async function readLifecycleMonitors() {
    setLifecycleMonitorsData(
      output(
        "lifecycle-monitors-feature-result",
        JSON.stringify({
          closedOrClosing: await features().lifecycle.isClosedOrClosingAsync(),
          dpi: await features().monitors.getMainMonitorScreenDpiAsync(),
        })
      )
    );
  }

  useEffect(() => {
    setDefaultTitle(document.title);
    setCurrentTitle(document.title);
    window.addEventListener("infiniframe:test-reset", () => reset());
  }, []);

  return (
    <main className="test-panel">
      <header>
        <h1>Window data exchange</h1>
        <p>
          Input probes send entered data to InfiniFrame. Output probes read
          InfiniFrame data into their fields.
        </p>
      </header>
      <div className="probe-grid probe-grid-two">
        <section className="data-probe">
          <h2 id="current-window-title">{currentTitle}</h2>
          <label className="data-probe-field">
            <span>Window title</span>
            <input
              id="title-data-input"
              value={titleInput}
              onInput={(event) => setTitleInput(event.target.value)}
            />
          </label>
          <button
            id="title-toggle-button"
            onClick={(event) => applyOrResetTitle(titleInput)}
          >
            Apply title / reset
          </button>
        </section>
        <section className="data-probe">
          <h2 className="output-data-probe-title">Fullscreen</h2>
          <label className="data-probe-field">
            <span>Current fullscreen state</span>
            <input
              id="fullscreen-data-result"
              value={fullscreenData}
              readOnly
            />
          </label>
          <button
            id="fullscreen-toggle-button"
            onClick={(event) => toggleFullscreen()}
          >
            Toggle fullscreen
          </button>
        </section>
      </div>
      <hr />
      <h2>Window feature readers</h2>
      <div
        id="window-feature-test-panel"
        className="probe-grid probe-grid-three"
      >
        <section className="data-probe">
          <h2 className="output-data-probe-title">Browser</h2>
          <label className="data-probe-field">
            <span>Serialized window data</span>
            <input id="browser-feature-result" value={browserData} readOnly />
          </label>
          <button id="probe-browser-feature" onClick={(event) => readBrowser()}>
            Read browser
          </button>
        </section>
        <section className="data-probe">
          <h2 className="output-data-probe-title">Decorations</h2>
          <label className="data-probe-field">
            <span>Serialized window data</span>
            <input
              id="decorations-feature-result"
              value={decorationsData}
              readOnly
            />
          </label>
          <button
            id="probe-decorations-feature"
            onClick={(event) => readDecorations()}
          >
            Read decorations
          </button>
        </section>
        <section className="data-probe">
          <h2 className="output-data-probe-title">Position</h2>
          <label className="data-probe-field">
            <span>Serialized window data</span>
            <input id="position-feature-result" value={positionData} readOnly />
          </label>
          <button
            id="probe-position-feature"
            onClick={(event) => readPosition()}
          >
            Read position
          </button>
        </section>
        <section className="data-probe">
          <h2 className="output-data-probe-title">Size</h2>
          <label className="data-probe-field">
            <span>Serialized window data</span>
            <input id="size-feature-result" value={sizeData} readOnly />
          </label>
          <button id="probe-size-feature" onClick={(event) => readSize()}>
            Read size
          </button>
        </section>
        <section className="data-probe">
          <h2 className="output-data-probe-title">State</h2>
          <label className="data-probe-field">
            <span>Serialized window data</span>
            <input id="state-feature-result" value={windowStateData} readOnly />
          </label>
          <button id="probe-state-feature" onClick={(event) => readState()}>
            Read state
          </button>
        </section>
        <section className="data-probe">
          <h2 className="output-data-probe-title">Lifecycle and monitors</h2>
          <label className="data-probe-field">
            <span>Serialized window data</span>
            <input
              id="lifecycle-monitors-feature-result"
              value={lifecycleMonitorsData}
              readOnly
            />
          </label>
          <button
            id="probe-lifecycle-monitors-feature"
            onClick={(event) => readLifecycleMonitors()}
          >
            Read lifecycle and monitors
          </button>
        </section>
      </div>
    </main>
  );
}

export default WindowFeatureTestPanel;
