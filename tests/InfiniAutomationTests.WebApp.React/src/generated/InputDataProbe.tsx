// @ts-nocheck
"use client";
import * as React from "react";

export interface InputDataProbeProps {
  title: string;
  buttonText: string;
  data: string;
  onDataChanged: (value: string) => void;
  onSubmitted: (value: string) => void;
  titleId?: string;
  buttonId?: string;
  dataInputId?: string;
  dataLabel?: string;
}

function InputDataProbe(props: InputDataProbeProps) {
  return (
    <section className="data-probe">
      <h2 className="input-data-probe-title" id={props.titleId}>
        {props.title}
      </h2>
      <label className="data-probe-field">
        <span>{props.dataLabel || "Input data"}</span>
        <input
          id={props.dataInputId}
          value={props.data}
          onInput={(event) => props.onDataChanged(event.target.value)}
        />
      </label>
      <button
        id={props.buttonId}
        onClick={(event) => props.onSubmitted(props.data)}
      >
        {props.buttonText}
      </button>
    </section>
  );
}

export default InputDataProbe;
