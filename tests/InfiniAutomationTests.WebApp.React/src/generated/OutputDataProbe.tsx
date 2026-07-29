// @ts-nocheck
"use client";
import * as React from "react";

export interface OutputDataProbeProps {
  title: string;
  buttonText: string;
  data: string;
  onReadRequested: () => void;
  titleId?: string;
  buttonId?: string;
  dataInputId?: string;
  dataLabel?: string;
}

function OutputDataProbe(props: OutputDataProbeProps) {
  return (
    <section className="data-probe">
      <h2 className="output-data-probe-title" id={props.titleId}>
        {props.title}
      </h2>
      <label className="data-probe-field">
        <span>{props.dataLabel || "Output data"}</span>
        <input id={props.dataInputId} value={props.data} readOnly />
      </label>
      <button id={props.buttonId} onClick={(event) => props.onReadRequested()}>
        {props.buttonText}
      </button>
    </section>
  );
}

export default OutputDataProbe;
